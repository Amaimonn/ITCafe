using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.World
{
    public class OrderCloudWorldUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _worldDocument;
        [SerializeField] private Transform _uiHolder;

        private Transform _lookAtTarget;
        private VisualElement _imagesContainer;
        private VisualElement _root;

        private List<(int hash, VisualElement image)> _images = new();
        
        public void Init()
        {
            _lookAtTarget = Camera.main.transform;
            _root = _worldDocument.rootVisualElement;
            _imagesContainer = _root.Q<VisualElement>(name: "ImagesContainer");
            _imagesContainer.Clear();
            Hide();
        }

#region MonoBehaviour
        private void Update()
        {
            _uiHolder.transform.LookAt(_lookAtTarget);
        }
#endregion

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }

        public void AddImage(Sprite sprite, int hash)
        {
            var image = new VisualElement
            {
                style = { backgroundImage = new StyleBackground(sprite) },
                name = hash.ToString()
            };
            image.AddToClassList("order-cloud__item-image");
            _imagesContainer.Add(image);
            _images.Add((hash, image));
        }

        public void RemoveImage(int hash)
        {
            var imageToRemove = _images.FirstOrDefault(x => x.hash == hash);
            if (imageToRemove != default)
            {
                _imagesContainer.Remove(imageToRemove.image);
                _images.Remove(imageToRemove);
            }
            else
                Debug.LogWarning($"Image {hash} not found");
        }
    }
}