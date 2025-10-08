using ITCafe.Data.Items;
using ITCafe.Environment;
using ITCafe.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.CafeBusiness
{
    public class ClientCharacter : BaseInteractable, IItemHandler
    {
        [SerializeField] private UIDocument _worldDocument;
        [SerializeField] private ItemInfoSO _itemInfoSO;
        [SerializeField] private Transform _uiHolder;

        private bool IsCompleted => _order.IsCompleted;

        private Camera _camera;
        private IOrder _order;

        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
            _order = new OrderItem(_itemInfoSO.ItemInfo.GetItemHash());
            Debug.Log($"OrderHash: {_order.OrderHashes}");
        }

        private void Start()
        {
            var root = _worldDocument.rootVisualElement;
            var imagesContainer = root.Q<VisualElement>(name: "ImagesContainer");
            imagesContainer.Clear();
            var image = new VisualElement()
            {
                style = { backgroundImage = new StyleBackground(_itemInfoSO.Image) }
            };
            image.AddToClassList("order-cloud__item-image");
            imagesContainer.Add(image);
        }

        private void Update()
        {
            _uiHolder.transform.LookAt(_camera.transform);
        }

#region IInteractable
        public override bool CanInteract(PlayerContext context)
        {
            if (IsCompleted)
                return false;

            var item = context.CurrentItem.CurrentValue;
            if (item != null)
                return item.CanHandle(this, context);

            return false;
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.CurrentItem.CurrentValue;
            item.Handle(this, context);
        }
#endregion

#region IItemHandler
        public bool CanHandle(IItem item, PlayerContext context)
        {
            if (item is IEquatableItem equatableItem)
            {
                var code = equatableItem.GetItemHash();

                if (_order.IsCorresponds(code))
                    return true;
            }

            return false;
        }

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            foreach (var hash in _order.OrderHashes)
            {
                if (container.ContainsHash(hash))
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
            foreach (var hash in _order.OrderHashes)
            {
                var item = container.ExtractItem(hash);
                if (item != null)
                {
                    if (_order.TryHandOver(item.GetItemHash()))
                        ConsumeItem(item);
                }
            }
        }
#endregion

        private void ConsumeItem(IItem item)
        {
            Destroy(item.transform.gameObject);

            if (IsCompleted)
                Destroy(gameObject);
        }
    }
}