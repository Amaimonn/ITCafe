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
            _outline.outlineMode = Outline.Mode.OutlineAndSilhouette;
        }

        public static Color ComposeHdrColor(Color32 baseLinearColor, float exposure)
        {
            if (exposure == 0f)
            {
                return new Color(
                    baseLinearColor.r / 255f,
                    baseLinearColor.g / 255f,
                    baseLinearColor.b / 255f
                );
            }
            else
            {
                var scaleFactor = 255f / Mathf.Pow(2f, exposure);
                
                var r = baseLinearColor.r / scaleFactor;
                var g = baseLinearColor.g / scaleFactor;
                var b = baseLinearColor.b / scaleFactor;
                
                return new Color(r, g, b);
            }
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
            // Debug.Log($"Focus: {name}");
        }

        public virtual void UnFocus()
        {
            if (_outline != null)
                _outline.enabled = false;
            // Debug.Log($"Unfocus: {name}");
        }

        public abstract bool CanInteract(PlayerContext context);

        public abstract void Interact(PlayerContext context);
    }
}