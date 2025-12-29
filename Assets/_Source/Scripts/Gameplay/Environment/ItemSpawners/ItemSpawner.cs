using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class ItemSpawner : BaseInteractable
    {
        [SerializeField] private ItemTag _spawningItemTag;
        [SerializeField] private GameObject _itemPrefab;

        private IItem _coreItem; // util object

        protected override void Awake()
        {
            base.Awake();

            var itemObject = Instantiate(_itemPrefab);
            _coreItem = itemObject.GetComponent<IItem>();
            itemObject.SetActive(false);
        }

        public override bool CanInteract(PlayerContext context)
        {
            if (_coreItem == null)
            {
                _coreItem = context.ItemsCreator.Get(_spawningItemTag);
                _coreItem.transform.gameObject.SetActive(false);
            }
            
            var picker = context.ItemPicker;
            
            return picker.CanTake(_coreItem);
        }

        public override void Interact(PlayerContext context)
        {
            var itemObject = Instantiate(_itemPrefab);
            var item = itemObject.GetComponent<IItem>();
            item.SetPhysicsEnabled(false);

            context.ItemPicker.Take(item);
#if UNITY_EDITOR
            if (item is IEquatableItem eqItem) // TODO: Remove
                Debug.Log($"Spawner: {eqItem.GetItemHash()} item");
#endif
        }
    }
}