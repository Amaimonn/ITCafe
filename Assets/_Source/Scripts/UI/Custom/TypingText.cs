using System.Collections;
using System.Collections.Generic;
using DevKit.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.UI.Custom
{
    [UxmlElement]
    public abstract partial class AnimatedText : Label
    {
        [UxmlAttribute] private float InitialDelay { get; set; } = 0f;

        public bool IsRunning { get; protected set; }

        public IEnumerator ShowCoroutine()
        {
            IsRunning = true;

            if (InitialDelay > 0)
                yield return new WaitForSeconds(InitialDelay);

            style.visibility = Visibility.Visible;
            yield return OnShow();

            IsRunning = false;
        }

        protected abstract IEnumerator OnShow();
    }

    [UxmlElement]
    public partial class FlashingText : AnimatedText
    {
        [UxmlAttribute] private float FlashInterval { get; set; } = 0.15f;
        [UxmlAttribute] private bool IsInfinite { get; set; } = false;
        [UxmlAttribute] private int FlashCount { get; set; } = 3;

        protected override IEnumerator OnShow()
        {
            if (IsInfinite)
            {
                while (IsRunning)
                    yield return Flash();
            }
            else
            {
                for (var i = 0; i < FlashCount; i++)
                    yield return Flash();
            }

            style.visibility = Visibility.Visible;

            IEnumerator Flash()
            {
                style.visibility = Visibility.Visible;
                yield return new WaitForSeconds(FlashInterval);

                style.visibility = Visibility.Hidden;
                yield return new WaitForSeconds(FlashInterval);
            }
        }
    }

    [UxmlElement]
    public partial class ImmediateText : AnimatedText
    {
        protected override IEnumerator OnShow()
        {
            yield return null;
        }
    }

    [UxmlElement]
    public partial class TypingText : AnimatedText
    {
        [UxmlAttribute] private float CharInterval { get; set; } = 0.05f;

        protected override IEnumerator OnShow()
        {
            this.RegisterValueChangedCallback(OnSourceTextChanged);

            var originalText = text;

            INotifyValueChanged<string> notifyLabel = this;
            notifyLabel.SetValueWithoutNotify(string.Empty);


            foreach (var c in originalText)
            {
                if (!IsRunning)
                    break;

                notifyLabel.SetValueWithoutNotify(text + c);
                yield return new WaitForSeconds(CharInterval);
            }

            this.UnregisterValueChangedCallback(OnSourceTextChanged);
        }

        private void OnSourceTextChanged(ChangeEvent<string> _)
        {
            IsRunning = false;
        }
    }

    [UxmlElement]
    public partial class AnimatedTextContainer : VisualElement
    {
        [UxmlAttribute] private float _lineDelay = 0.3f;

        private readonly List<AnimatedText> _animatedTextEntries = new();
        private readonly List<Label> _labels = new();

        public AnimatedTextContainer()
        {
            RegisterCallback<AttachToPanelEvent>(HandleAttachedToPanel);
        }

        private void HandleAttachedToPanel(AttachToPanelEvent evt)
        {
            GatherChildren();
        }

        private void GatherChildren()
        {
            _animatedTextEntries.Clear();
            this.Query<AnimatedText>().ForEach(x => _animatedTextEntries.Add(x));

            FLogger.Log<AnimatedTextContainer>($"Labels: {_animatedTextEntries.Count}");
        }

        public IEnumerator RunCoroutine()
        {
            _labels.Clear();
            this.Query<Label>().ForEach(label =>
            {
                label.style.visibility = Visibility.Hidden;
                _labels.Add(label);
            });

            for (var i = 0; i < _animatedTextEntries.Count && i < _labels.Count; i++)
            {
                var textEntry = _animatedTextEntries[i];

                yield return textEntry.ShowCoroutine();
                yield return new WaitForSeconds(_lineDelay);
            }
        }
    }
}