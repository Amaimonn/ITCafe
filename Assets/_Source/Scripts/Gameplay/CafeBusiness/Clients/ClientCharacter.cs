using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ITCafe.Environment;
using ITCafe.Shared;
using ITCafe.Gameplay.WorldUI;
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
        public Observable<Unit> OnSucceed => _onSucceed;
        public Observable<Unit> OnFailed => _onFailed;

        public IOrder CurrentOrder { get; private set; }

        public bool IsCompleted => CurrentOrder.IsCompleted;
        public ClientState CurrentState { get; private set; } = ClientState.WaitingForOrder;

        [field: SerializeField] public OrderCloudWorldUI OrderUI { get; private set; }
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private SfxData _onOrderTakenSfx;
        [SerializeField] private SfxData _onItemAcceptedSfx;
        [SerializeField] private SfxData _onSuccessSfx;

        private readonly Subject<Unit> _onLeft = new();
        private readonly Subject<Unit> _onOrdered = new();
        private readonly Subject<Unit> _onRegistrationLeft = new();
        private readonly Subject<Unit> _onSucceed = new();
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
            
            if (_onOrderTakenSfx.IsValid)
                AudioPlayer.GetSfxBuilder().WithPosition(transform.position).Play(_onOrderTakenSfx);
            
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

            if (_onItemAcceptedSfx.IsValid)
                AudioPlayer.GetSfxBuilder().WithPosition(transform.position).Play(_onItemAcceptedSfx);

            if (IsCompleted)
            {
                if (_onSuccessSfx.IsValid)
                    AudioPlayer.GetSfxBuilder().WithPosition(transform.position).Play(_onSuccessSfx);
                
                _onSucceed.OnNext(Unit.Default);
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

            if (item.TryGetCachedComponent<IMenuAspect>(out var menuAspect))
            {
                var code = menuAspect.GetItemHash();
                return CurrentOrder.IsCorresponds(code);
            }

            return false;
        }

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            if (!CheckCanTakeFood())
                return false;

            return container.Items.Any(item =>
                item.TryGetCachedComponent<IMenuAspect>(out var menuAspect) &&
                CurrentOrder.IsCorresponds(menuAspect.GetItemHash()));
        }

        public void Handle(IItem item, PlayerContext context)
        {
            if (item.TryGetCachedComponent<IMenuAspect>(out var menuAspect))
                TakeMenuItem(item, menuAspect, context);
        }

        public void HandleContainer(IItemsContainer container, PlayerContext context)
        {
            if (container.TryGetCachedComponent<IMenuAspect>(out var containerMenuAspect))
                TakeMenuItem(container, containerMenuAspect, context);
            
            var items = container.Items.ToArray();
            foreach (var item in items)
            {
                if (CurrentOrder.IsCompleted)
                    break;
                
                if (!item.TryGetCachedComponent<IMenuAspect>(out var menuAspect))
                    continue;
                
                var hash = menuAspect.GetItemHash();
                if (CurrentOrder.IsCorresponds(hash))
                {
                    var extractedItem = container.ExtractItem(hash);
                    if (extractedItem != null && CurrentOrder.TryHandOver(hash))
                        ConsumeItem(extractedItem);
                }
            }
        }
#endregion

        private void TakeMenuItem(IItem item, IMenuAspect menuAspect, PlayerContext context)
        {
            var hash = menuAspect.GetItemHash();
            if (CurrentOrder.TryHandOver(hash))
            {
                context.ItemPicker.Release();
                ConsumeItem(item);
            }
        }

        private bool CheckCanTakeFood() => ClientState.CanTakeFood.HasFlag(CurrentState);
    }
}