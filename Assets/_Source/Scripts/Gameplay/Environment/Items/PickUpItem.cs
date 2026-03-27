using ITCafe.Gameplay.Shared;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class PickUpItem : BaseItem
    {
        [SerializeField] protected SfxData _onTakenSfx;
        
        public override bool CanInteract(PlayerContext context)
        {
            return context.ItemPicker.CanTake(this);
        }

        public override void Interact(PlayerContext context)
        {
            SetPhysicsEnabled(false);
            
            context.ItemPicker.Take(this);
        }

        public override void OnTaken()
        {
            if (_onTakenSfx.IsValid)
                AudioPlayer.GetSfxBuilder().WithPosition(transform.position).Play(_onTakenSfx);
        }
    }
}
