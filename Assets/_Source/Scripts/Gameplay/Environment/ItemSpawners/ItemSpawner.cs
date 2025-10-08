using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class ItemSpawner : BaseInteractable
    {
        [SerializeField] private GameObject _itemPrefab;

        // [SerializeField] private int _maxSpawnedItemsAmount;
        private IItem _coreItem; // вспомогательный объект

        protected override void Awake()
        {
            base.Awake();
            
            var itemObject = Instantiate(_itemPrefab);
            _coreItem = itemObject.GetComponent<IItem>();
            itemObject.SetActive(false);
        }

        public override bool CanInteract(PlayerContext context)
        {
            var picker = context.ItemPicker;
            return picker.CanTake(_coreItem);
            //     ||
            // (picker.CurrentItem.CurrentValue is IItemsContainer itemsContainer &&
            //  itemsContainer.CanTake(_coreItem));
        }

        public override void Interact(PlayerContext context)
        {
            var itemObject = Instantiate(_itemPrefab);
            var item = itemObject.GetComponent<IItem>();
            item.SetPhysicsEnabled(false);

            context.ItemPicker.Take(item);

            Debug.Log("Spawner: item has been taken");
        }
    }
}