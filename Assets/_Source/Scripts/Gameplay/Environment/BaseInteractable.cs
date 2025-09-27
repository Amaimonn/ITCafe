using Flopin.Utils;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] protected Outline _outline;

        #region MonoBehaviour

        protected virtual void OnValidate()
        {
            if (_outline == null)
                _outline = gameObject.GetOrAddComponent<Outline>();

            _outline.outlineColor = new Color(1f, 10.6283679f, 0.45f, 1);
            _outline.outlineWidth = 7f;
        }

        protected virtual void Awake()
        {
            _outline.enabled = false;
        }

        #endregion

        public virtual void Focus()
        {
            if (_outline != null)
                _outline.enabled = true;
            Debug.Log($"Focus: {name}");
        }

        public virtual void UnFocus()
        {
            if (_outline != null)
                _outline.enabled = false;
            Debug.Log($"Unfocus: {name}");
        }

        public abstract bool CanInteract(PlayerContext context);

        public abstract void Interact(PlayerContext context);
    }
}