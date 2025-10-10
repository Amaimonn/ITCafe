using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private ItemInfoSO[] _itemInfoSO;
        [SerializeField] private Transform _uiHolder;

        private bool IsCompleted => _order.IsCompleted;

        private Camera _camera;
        private IOrder _order;
        private VisualElement _imagesContainer;
        private List<(int hash, VisualElement image)> _images = new();

        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
            InitOrder();
        }

        private void Update()
        {
            _uiHolder.transform.LookAt(_camera.transform);
        }

        private void InitOrder()
        {
            var root = _worldDocument.rootVisualElement;
            _imagesContainer = root.Q<VisualElement>(name: "ImagesContainer");
            _imagesContainer.Clear();

            var orderCount = _itemInfoSO.Length;
            if (orderCount > 0)
            {
                if (orderCount == 1)
                {
                    var itemSO = _itemInfoSO[0];
                    var itemHash = itemSO.ItemInfo.GetItemHash();

                    _order = new OrderItem(itemHash);
                    AddImage(itemSO.Image, itemHash);
                }
                else
                {
                    Dictionary<int, int> orderedItemsMap = new();

                    foreach (var so in _itemInfoSO)
                    {
                        var hash = so.ItemInfo.GetItemHash();
                        if (orderedItemsMap.ContainsKey(hash))
                            orderedItemsMap[hash] += 1;
                        else
                            orderedItemsMap[hash] = 1;
                        AddImage(so.Image, hash);
                    }
                    foreach (var item in orderedItemsMap)
                    {
                        Debug.Log($"Item {item.Key} count {item.Value}");
                    }
                    _order = new OrderMap(orderedItemsMap);
                }
            }
            else
            {
                Debug.LogError($"No items in order {gameObject.name}");
                _order = new OrderItem(-1);
            }

            void AddImage(Sprite sprite, int hash)
            {
                var image = new VisualElement()
                {
                    style = { backgroundImage = new StyleBackground(sprite) },
                    name = hash.ToString()
                };
                Debug.Log($"Add image {hash}");
                image.AddToClassList("order-cloud__item-image");
                _imagesContainer.Add(image);
                _images.Add((hash, image));
            }
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
                    ConsumeItem(item, hash);
                }
            }
        }

        public void HandleContainer(IItemsContainer container, PlayerContext context)
        {
            var orderHashes = _order.OrderHashes.ToArray();
            foreach (var hash in orderHashes)
            {
                var item = container.ExtractItem(hash);
                Debug.Log($"Extract {hash}");
                if (item != null && _order.TryHandOver(hash))
                    ConsumeItem(item, hash);
            }
        }
        #endregion

        private void ConsumeItem(IItem item, int hash)
        {
            Destroy(item.transform.gameObject);
            RemoveImage(hash);

            if (IsCompleted)
                Destroy(gameObject);
        }

        private void RemoveImage(int hash)
        {
            var imageToRemove = _images.FirstOrDefault(x => x.hash == hash);
            if (imageToRemove != default)
            {
                _imagesContainer.Remove(imageToRemove.image);
                _images.Remove(imageToRemove);
            }
            // imageToRemove.RemoveFromHierarchy();
            else
                Debug.LogError($"Image {hash} not found");
        }
    }
}