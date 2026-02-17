using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ITCafe.Environment;
using ITCafe.Gameplay.UI.World;
using ITCafe.Player;
using R3;
using UnityEngine;
using UnityEngine.AI;

namespace ITCafe.CafeBusiness
{
    public class ClientCharacter : BaseInteractable, IItemHandler
    {
        public Observable<float> WaitingTimeNormalized => _waitingTimeNormalized;
        public Observable<Unit> OnLeft => _onLeft;
        public Observable<Unit> OnOrdered => _onOrdered;
        public Observable<Unit> OnRegistrationLeft => _onRegistrationLeft;
        public Observable<Unit> OnCompleted => _onCompleted;
        public Observable<Unit> OnFailed => _onFailed;

        public IOrder CurrentOrder { get; private set; }

        public bool IsCompleted => CurrentOrder.IsCompleted;
        public ClientState CurrentState { get; private set; } = ClientState.WaitingForOrder;

        [field: SerializeField] public OrderCloudWorldUI OrderUI { get; private set; }
        [SerializeField] private NavMeshAgent _agent;

        private readonly Subject<Unit> _onLeft = new();
        private readonly Subject<Unit> _onOrdered = new();
        private readonly Subject<Unit> _onRegistrationLeft = new();
        private readonly Subject<Unit> _onCompleted = new();
        private readonly Subject<Unit> _onFailed = new();
        private CancellationToken _destroyToken;
        private readonly ReactiveProperty<float> _waitingTimeNormalized = new();
        private float _remainingWaitingTime;
        private const float WAITING_FOR_ORDER_TIME = 90f;
        
        [Flags]
        public enum ClientState
        {
            WaitingForOrder,
            OrderDelay,
            MovingToTable,
            WaitingForFood,
            CanTakeFood = WaitingForFood | OrderDelay | MovingToTable,
            Leaving
        }

        private Transform _targetTable;
        private TableService _tableService;
        private const int AfterOrderDelayMs = 1333;

        public void Init(IOrder order, TableService tableService)
        {
            CurrentOrder = order;
            _waitingTimeNormalized.Value = 1f;
            _remainingWaitingTime = WAITING_FOR_ORDER_TIME;
            _tableService = tableService;
            _targetTable = _tableService.GetFreeTable();

            CurrentState = ClientState.WaitingForOrder;
        }

#region MonoBehaviour
        protected override void Awake()
        {
            base.Awake();
            _destroyToken = destroyCancellationToken;
        }

        private void Update()
        {
            if (CurrentState == ClientState.MovingToTable && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.pathPending)
                {
                    CurrentState = ClientState.WaitingForFood;
                    _agent.enabled = false;
                    transform.SetPositionAndRotation(_targetTable.position, _targetTable.rotation);
                }
            }
            // else if (CurrentState == ClientState.Leaving && _agent.remainingDistance <= _agent.stoppingDistance)
            // {
            //     LeaveCafe();
            // }

            TickOrder();
        }
#endregion

        private void TickOrder()
        {
            if (CurrentOrder == null)
                return;

            if (CurrentState == ClientState.WaitingForOrder)
            {
                _remainingWaitingTime = Mathf.Max(0f, _remainingWaitingTime - Time.deltaTime);
                _waitingTimeNormalized.Value = _remainingWaitingTime / WAITING_FOR_ORDER_TIME;

                if (_remainingWaitingTime == 0f)
                {
                    _onFailed.OnNext(Unit.Default);
                    LeaveCafe();
                }
                return;
            }

            CurrentOrder.TickTime(Time.deltaTime);
            if (CurrentOrder.RemainingTime.CurrentValue == 0f)
            {
                _onFailed.OnNext(Unit.Default);
                LeaveCafe();
            }
        }

        private void MoveToTable()
        {
            if (_targetTable != null)
                _agent.SetDestination(_targetTable.position);
        }

        private void LeaveCafe()
        {
            if (_targetTable != null)
                _tableService.FreeTable(_targetTable);

            _onLeft.OnNext(Unit.Default);
            Destroy(gameObject);
        }

        private async UniTaskVoid MakeOrderAsync()
        {
            OrderUI.Show();
            CurrentState = ClientState.OrderDelay;
            _onOrdered.OnNext(Unit.Default);

            if (!IsCompleted)
                await UniTask.Delay(AfterOrderDelayMs, cancellationToken: _destroyToken);

            _onRegistrationLeft.OnNext(Unit.Default);
            CurrentState = ClientState.MovingToTable;
            MoveToTable();
        }

        private void ConsumeItem(IItem item)
        {
            Destroy(item.transform.gameObject);

            if (IsCompleted)
            {
                _onCompleted.OnNext(Unit.Default);
                LeaveCafe();
            }
        }

#region IInteractable
        public override bool CanInteract(PlayerContext context)
        {
            if (CurrentState == ClientState.WaitingForOrder)
                return true;

            if (IsCompleted)
                return false;

            var item = context.OnItemChanged.CurrentValue;
            if (item != null)
                return item.CanBeHandled(this, context);

            return false;
        }

        public override void Interact(PlayerContext context)
        {
            if (CurrentState == ClientState.WaitingForOrder)
            {
                MakeOrderAsync().Forget();
                return;
            }

            var item = context.OnItemChanged.CurrentValue;
            item.BecomeHandled(this, context);
        }
#endregion

#region IItemHandler
        public bool CanHandle(IItem item, PlayerContext context)
        {
            if (!CheckCanTakeFood())
                return false;

            if (item is IEquatableItem equatableItem)
            {
                var code = equatableItem.GetItemHash();
                return CurrentOrder.IsCorresponds(code);
            }

            return false;
        }

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            if (!CheckCanTakeFood())
                return false;

            return container.Items.Any(item => CurrentOrder.IsCorresponds(item.GetItemHash()));
        }

        public void Handle(IItem item, PlayerContext context)
        {
            if (item is IEquatableItem equatableItem)
            {
                var hash = equatableItem.GetItemHash();
                if (CurrentOrder.TryHandOver(hash))
                {
                    context.ItemPicker.Release();
                    ConsumeItem(item);
                }
            }
        }

        public void HandleContainer(IItemsContainer container, PlayerContext context)
        {
            var items = container.Items.ToArray();
            foreach (var it in items)
            {
                if (CurrentOrder.IsCompleted)
                    break;

                var hash = it.GetItemHash();
                if (CurrentOrder.IsCorresponds(hash))
                {
                    var item = container.ExtractItem(hash);
                    if (item != null && CurrentOrder.TryHandOver(hash))
                        ConsumeItem(item);
                }
            }
        }
#endregion

        private bool CheckCanTakeFood() => ClientState.CanTakeFood.HasFlag(CurrentState);
    }
}