using UnityEngine;

namespace ITCafe
{
    public class ItemSpawner : BaseInteractable
    {
        [SerializeField] private GameObject _itemPrefab;
        // [SerializeField] private int _maxSpawnedItemsAmount;


        public override bool CanInteract(PlayerContext context)
        {
            // Debug.Log("CanInteract: " + context.ItemPicker.CanTake());
            return context.ItemPicker.CanTake();
        }

        public override void Interact(PlayerContext context)
        {
            var itemObject = Instantiate(_itemPrefab);
            var item = itemObject.GetComponent<IItem>();
            item.SetPhysicsEnabled(false);
            context.ItemPicker.Take(item);
            Debug.Log("Spawner: item was taken");
        }
    }
}