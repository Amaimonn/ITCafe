using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.UI.Custom
{
    [UxmlElement]
    public partial class GuideIngredient : VisualElement
    {
        [UxmlAttribute]
        public Sprite Image
        {
            get => _sprite;
            set
            {
                _sprite = value;
                if (_image != null)
                    _image.style.backgroundImage = value != null ? Background.FromSprite(value) : null;
            }
        }

        [UxmlAttribute]
        private string Prefix
        {
            get => _prefixText;
            set
            {
                _prefixText = value;
                if (_prefix != null)
                    _prefix.text = value;
            }
        }

        [UxmlAttribute, CreateProperty]
        private string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        [UxmlAttribute]
        private bool IsResult
        {
            get => _isResult;
            set
            {
                _isResult = value;
                
                _image?.EnableInClassList("guide__ingredient-icon--result", _isResult);
                _text?.EnableInClassList("guide__ingredient-label--result", _isResult);
                _prefix?.EnableInClassList("guide__ingredient-label--result", _isResult);
            }
        }

        private readonly Label _text;
        private readonly Label _prefix;
        private readonly VisualElement _image;

        private Sprite _sprite;
        private bool _isResult = false;
        private string _prefixText;

        public GuideIngredient()
        {
            AddToClassList("guide__ingredient");

            _image = new VisualElement
            {
                style =
                {
                    backgroundImage = Background.FromSprite(_sprite)
                }
            };
            _image.AddToClassList("guide__ingredient-icon");
            Add(_image);

            var labelContainer = new VisualElement();
            labelContainer.AddToClassList("guide__ingredient-labels-container");
            Add(labelContainer);

            _prefix = new Label
            {
                text = _prefixText
            };
            _prefix.AddToClassList("guide__ingredient-label");
            labelContainer.Add(_prefix);
            
            _text = new Label();
            _text.AddToClassList("guide__ingredient-label");
            labelContainer.Add(_text);
        }
    }
}