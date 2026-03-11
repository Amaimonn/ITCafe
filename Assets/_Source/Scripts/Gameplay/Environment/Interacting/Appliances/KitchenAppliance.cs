using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ITCafe.Gameplay.UI.World;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment.Appliances
{
    public abstract class KitchenAppliance<T> : BaseInteractable, IItemHandler, IDisposable 
        where T : IProcessableAspect
    {
        [SerializeField] private Transform _placedTransform;
        [SerializeField] private ProcessingProgressWorldUI _progressUI;

        protected bool IsBusy => _holdingItem != null;
        protected IItem _holdingItem;
        protected bool _isReadyResult = false;
        protected CancellationTokenSource _cts;

#region IInteractable
        public override void Focus()
        {
            base.Focus();
            _holdingItem?.Focus();
        }

        public override void UnFocus()
        {
            base.UnFocus();
            _holdingItem?.UnFocus();
        }

        public override bool CanInteract(PlayerContext context)
        {
            var item = context.OnItemChanged.CurrentValue;
            var emptyHands = item == null;

            if (emptyHands)
                return _isReadyResult && IsBusy; // can be taken with empty hands by default
            else
                return item.CanBeHandled(this, context);
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.OnItemChanged.CurrentValue;
            if (item == null)
                HandOver(context);
            else
                context.OnItemChanged.CurrentValue.BecomeHandled(this, context);
        }
#endregion

#region IItemHandler
        public virtual bool CanHandle(IItem item, PlayerContext context)
        {
            return !IsBusy &&
                   item.TryGetCachedComponent<T>(out var processable) &&
                   processable.IsProcessable ||
                   _isReadyResult && context.ItemPicker.CanTake(_holdingItem);
            
            // TODO: if IsBusy: mb try to craft:
            //       check for processable (higher priority than Craft with Take) ->
            //       search for recipes ->
            //       take item, set 'NotReady' and increase cooking time).
            //       (Not for all appliances. Ex.: ok for pot, but not for grill)
        }

        public virtual bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            return false;
        }

        public virtual void Handle(IItem item, PlayerContext context)
        {
            if (!IsBusy)
                Place(item, context);
            else
                HandOver(context);
        }

        public virtual void HandleContainer(IItemsContainer container, PlayerContext context)
        {
        }
#endregion

        protected virtual void Place(IItem item, PlayerContext context)
        {
            var itemPicker = context.ItemPicker;

            itemPicker.Release();
            item.transform.SetParent(_placedTransform, worldPositionStays: true);
            item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            _holdingItem = item;
            _holdingItem.Focus();

            Process(context);
        }

        protected virtual void Process(PlayerContext context)
        {
            if (!_holdingItem.TryGetCachedComponent<T>(out var processable) ||
                !processable.IsProcessable)
            {
                return;
            }

            ProcessAsync(processable, context).Forget();
        }

        protected virtual async UniTaskVoid ProcessAsync(IProcessableAspect processable, PlayerContext context)
        {
            try
            {
                ClearProcessing();
                
                _cts = new CancellationTokenSource();
                
                _progressUI.Show();
                _progressUI.SetProgress(0f);
                
                var startTime = Time.time;
                var currentTime = startTime;
                var finishTime = startTime + processable.ProcessingTime;
                
                while (!_cts.IsCancellationRequested && currentTime < finishTime)
                {
                    await UniTask.WaitForEndOfFrame(cancellationToken:  _cts.Token);
                    
                    currentTime = Time.time;
                    _progressUI.SetProgress((currentTime - startTime) / processable.ProcessingTime);
                }
                
                _progressUI.SetProgress(1f);

                SetProcessingResult(processable, context);
                _holdingItem.SetPhysicsEnabled(false);
                
                _isReadyResult = true;
            }
            catch
            {
                // ignored
            }
        }

        protected virtual void SetProcessingResult(IProcessableAspect processable, PlayerContext context)
        {
            _holdingItem = processable.GetResult(_holdingItem, context);
        }

        protected virtual void HandOver(PlayerContext context)
        {
            _progressUI.Hide();
            
            context.ItemPicker.Take(_holdingItem);
            _holdingItem.UnFocus();
            _holdingItem = null;
            _isReadyResult = false;
        }

        protected void ClearProcessing()
        {
            Disposes.ClearDispose(ref _cts);
        }
        
        protected void OnDestroy()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            ClearProcessing();
        }
    }
}