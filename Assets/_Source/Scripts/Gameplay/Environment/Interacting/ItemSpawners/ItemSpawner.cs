using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class ItemSpawner : BaseInteractable
    {
        [SerializeField] private ItemTag _spawningItemTag;

        private IItem _coreItem; // util object

        public override bool CanInteract(PlayerContext context)
        {
            if (_coreItem == null)
            {
                _coreItem = context.ItemsCreator.Get(_spawningItemTag);
                _coreItem.transform.gameObject.SetActive(false);
                _coreItem.transform.SetParent(transform);
            }
            
            var picker = context.ItemPicker;
            
            return picker.CanTake(_coreItem);
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.ItemsCreator.Get(_spawningItemTag);
            item.SetPhysicsEnabled(false);

            context.ItemPicker.Take(item);
#if UNITY_EDITOR
            if (item is IEquatableItem eqItem) // TODO: Remove
                Debug.Log($"Spawner: {eqItem.GetItemHash()} item");
#endif
        }
    }
}