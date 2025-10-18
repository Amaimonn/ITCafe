using System.Linq;
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
        public Observable<Unit> OnLeft => _onLeft;
        public Observable<Unit> OnOrdered => _onOrdered;
        public ClientState CurrentState { get; private set; } = ClientState.WaitingForOrder;

        [field: SerializeField] public OrderCloudWorldUI OrderUI { get; private set; }
        [SerializeField] private NavMeshAgent _agent;

        private Subject<Unit> _onLeft = new();
        private Subject<Unit> _onOrdered = new();

        public enum ClientState
        {
            WaitingForOrder,
            OrderDelay,
            MovingToTable,
            WaitingForFood,
            Leaving
        }

        private bool IsCompleted => _order.IsCompleted;
        private IOrder _order;
        private Transform _targetTable;
        private TableService _tableService;
        private const int AfterOrderDelayMs = 1333;

        public void Init(IOrder order, TableService tableService)
        {
            _order = order;
            _tableService = tableService;
            _agent = GetComponent<NavMeshAgent>();
            _targetTable = _tableService.GetFreeTable();

            CurrentState = ClientState.WaitingForOrder;
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

            await UniTask.Delay(AfterOrderDelayMs);

            CurrentState = ClientState.MovingToTable;
            MoveToTable();
            _onOrdered.OnNext(Unit.Default);
        }

        private void ConsumeItem(IItem item)
        {
            Destroy(item.transform.gameObject);

            if (IsCompleted)
                LeaveCafe();
        }

#region IInteractable
        public override bool CanInteract(PlayerContext context)
        {
            if (CurrentState == ClientState.WaitingForOrder)
                return true;

            if (IsCompleted)
                return false;

            var item = context.CurrentItem.CurrentValue;
            if (item != null)
                return item.CanHandle(this, context);

            return false;
        }

        public override void Interact(PlayerContext context)
        {
            if (CurrentState == ClientState.WaitingForOrder)
            {
                MakeOrderAsync().Forget();
                return;
            }

            var item = context.CurrentItem.CurrentValue;
            item.Handle(this, context);
        }
#endregion

#region IItemHandler
        public bool CanHandle(IItem item, PlayerContext context)
        {
            if (CurrentState != ClientState.WaitingForFood)
                return false;

            if (item is IEquatableItem equatableItem)
            {
                var code = equatableItem.GetItemHash();
                return _order.IsCorresponds(code);
            }

            return false;
        }

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            if (CurrentState != ClientState.WaitingForFood)
                return false;

            foreach (var item in container.Items)
            {
                if (_order.IsCorresponds(item.GetItemHash()))
                    return true;
            }

            return false;
        }

        public void Handle(IItem item, PlayerContext context)
        {
            if (item is IEquatableItem equatableItem)
            {
                var hash = equatableItem.GetItemHash();
                if (_order.TryHandOver(hash))
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
                if (_order.IsCompleted)
                    break;

                var hash = it.GetItemHash();
                if (_order.IsCorresponds(hash))
                {
                    var item = container.ExtractItem(hash);
                    if (item != null && _order.TryHandOver(hash))
                        ConsumeItem(item);
                }
            }
        }
#endregion
    }
}