using UnityEngine;

namespace ITCafe.Environment.ItemsProcessing
{
    public class MaterialChanger : MonoBehaviour
    {
        [SerializeField] private Material _newMaterial;
        [SerializeField] private Renderer _targetRenderer;

        private void Change()
        {
            _targetRenderer.material = _newMaterial;
        }
    }
}