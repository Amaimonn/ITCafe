using ITCafe.Data.Items;
using ITCafe.Environment;
using ITCafe.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.CafeBusiness
{
    public class ClientCharacter : BaseInteractable
    {
        [SerializeField] private UIDocument _worldDocument;
        [SerializeField] private ItemInfoSO _itemInfoSO;
        [SerializeField] private Transform _uiHolder;
        
        private Camera _camera;
        private IOrder _order;
        private bool _isCompleted = false;

        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
            _order = new OrderItem(_itemInfoSO.ItemInfo.GetItemHash());
            Debug.Log($"OrderHash: {_order.OrderHash}");
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

        public override bool CanInteract(PlayerContext context)
        {
            if (_isCompleted)
                return false;

            if (context.CurrentItem.CurrentValue is IEquatableItem equatableItem)
            {
                var code = equatableItem.GetItemHash();
                Debug.Log($"item: {code}, order: {_order.OrderHash} ");

                if (_order.IsCorresponds(code))
                    return true;
                else if (context.CurrentItem.CurrentValue is IItemsContainer container)
                    return container.ContainsHash(_order.OrderHash);
            }

            return false;
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.CurrentItem.CurrentValue;

            if (item is IItemsContainer container)
                item = container.ExtractItem(_order.OrderHash);
            else
                context.ItemPicker.Release();

            Destroy(item.transform.gameObject);
            _isCompleted = true;
            Destroy(gameObject);
        }
    }
}