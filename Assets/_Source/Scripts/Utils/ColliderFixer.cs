using UnityEngine;

namespace Flopin.Utils
{
    [ExecuteInEditMode]
    public class ColliderFixer : MonoBehaviour
    {
        private void Start()
        {
            var boxColliders = GetComponentsInChildren<BoxCollider>();
            foreach (var boxCollider in boxColliders)
            {
                var fixCenter = boxCollider.center;
                if (fixCenter.x < 0)
                    fixCenter.x *= -1;
                if (fixCenter.y < 0)
                    fixCenter.y *= -1;
                if (fixCenter.z < 0)
                    fixCenter.z *= -1;

                boxCollider.center = fixCenter;

                var fixSize = boxCollider.size;
                if (fixSize.x < 0)
                    fixSize.x *= -1;
                if (fixSize.y < 0)
                    fixSize.y *= -1;
                if (fixSize.z < 0)
                    fixSize.z *= -1;

                boxCollider.size = fixSize;
            }
        }
    }
}