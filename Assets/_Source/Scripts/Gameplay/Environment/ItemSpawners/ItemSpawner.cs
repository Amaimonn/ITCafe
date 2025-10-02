using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class ItemSpawner : BaseInteractable
    {
        [SerializeField] private GameObject _itemPrefab;
        // [SerializeField] private int _maxSpawnedItemsAmount;


        public override bool CanInteract(PlayerContext context)
        {
            return context.ItemPicker.CanTake();
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