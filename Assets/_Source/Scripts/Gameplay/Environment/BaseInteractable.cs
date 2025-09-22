using Flopin.Utils;
using UnityEngine;

namespace ITCafe
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] protected Outline _outline;

        #region MonoBehaviour

        protected virtual void OnValidate()
        {
            if (_outline == null)
                _outline = gameObject.GetOrAddComponent<Outline>();
        }

        protected virtual void Awake()
        {
            _outline.enabled = false;
        }

        #endregion

        public virtual void Focus()
        {
            _outline.enabled = true;
            Debug.Log($"Focus: {name}");
        }

        public virtual void UnFocus()
        {
            _outline.enabled = false;
            Debug.Log($"Unfocus: {name}");
        }

        public abstract bool CanInteract(PlayerContext context);

        public abstract void Interact(PlayerContext context);
    }
}