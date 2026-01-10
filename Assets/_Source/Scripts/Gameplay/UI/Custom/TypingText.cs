using System.Collections;
using System.Collections.Generic;
using DevKit.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.Custom
{
    [UxmlObject]
    public abstract partial class AnimatedText
    {
        protected Label Label;

        public bool IsRunning { get; protected set; }

        public void Bind(Label label)
        {
            Label = label;
        }

        public IEnumerator ShowCoroutine()
        {
            IsRunning = true;
            
            if (Label != null)
                yield return OnShow();
            else
                FLogger.Log<AnimatedText>("No Label binded");
            
            IsRunning = false;
        }
        
        protected abstract IEnumerator OnShow();
    }
    
    [UxmlObject]
    public partial class FlashingText : AnimatedText
    {
        [UxmlAttribute] private float FlashInterval { get; set; } = 0.15f;
        [UxmlAttribute] private int FlashCount { get; set; } = 3;

        protected override IEnumerator OnShow()
        {
            IsRunning = true;

            for (var i = 0; i < FlashCount; i++)
            {
                Label.style.visibility = Visibility.Visible;
                yield return new WaitForSeconds(FlashInterval);

                Label.style.visibility = Visibility.Hidden;
                yield return new WaitForSeconds(FlashInterval);
            }
            
            Label.style.visibility = Visibility.Visible;

            IsRunning = false;
        }
    }

    [UxmlObject]
    public partial class NotAnimatedText : AnimatedText
    {
        protected override IEnumerator OnShow()
        {
            IsRunning = true;
            yield return null;
            IsRunning = false;
        }
    }

    [UxmlObject]
    public partial class TypingText : AnimatedText
    {
        [UxmlAttribute] private float InitialDelay { get; set; } = 0f;
        [UxmlAttribute] private float CharInterval { get; set; } = 0.05f;

        protected override IEnumerator OnShow()
        {
            IsRunning = true;
            Label.RegisterValueChangedCallback(OnSourceTextChanged);
            
            var originalText = Label.text;
            
            INotifyValueChanged<string> notifyLabel = Label;
            notifyLabel.SetValueWithoutNotify(string.Empty);
            
            if (InitialDelay > 0)
                yield return new WaitForSeconds(InitialDelay);
            
            foreach (var c in originalText)
            {
                if (!IsRunning)
                    break;
                
                notifyLabel.SetValueWithoutNotify(Label.text + c);
                yield return new WaitForSeconds(CharInterval);
            }
            Label.UnregisterValueChangedCallback(OnSourceTextChanged);
            IsRunning = false;
        }

        private void OnSourceTextChanged(ChangeEvent<string> _)
        {
            IsRunning = false;
        }
    }
    
    [UxmlElement]
    public partial class AnimatedTextContainer : VisualElement
    {
        [UxmlObjectReference] private AnimatedText[] AnimatedTextEntries { get; set; }

        [UxmlAttribute] private float _lineDelay = 0.3f;

        private readonly List<Label> _labels = new();

        public IEnumerator RunCoroutine()
        {
            _labels.Clear();
            this.Query<Label>().ForEach(label =>
            {
                label.style.visibility = Visibility.Hidden;
                _labels.Add(label);
            });

            for (var i = 0; i < AnimatedTextEntries.Length && i < _labels.Count; i++)
            {
                var label = _labels[i];
                var textEntry = AnimatedTextEntries[i];
                textEntry.Bind(label);
                label.style.visibility = Visibility.Visible;
                yield return textEntry.ShowCoroutine();
                yield return new WaitForSeconds(_lineDelay);
            }
        }
    }
}