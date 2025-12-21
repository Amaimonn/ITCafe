using System;
using DevKit.UI.MVVM.Bases;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class MainMenuView : ScreenToolkitAttach<MainMenuViewModel>
    {
        [SerializeField] private string _playButtonName = "PlayButton";
        [SerializeField] private string _exitButtonName = "ExitButton";
        [SerializeField] private string _settingsButtonName = "SettingsButton";

        [Header("Animations")]
        [SerializeField] private float _typingSpeed = 0.05f; // Задержка между символами
        [SerializeField] private float _lineDelay = 0.3f; // Задержка между строками
        [SerializeField] private float _initialDelay = 0.5f; // Начальная задержка перед стартом
        [SerializeField] private float _errorFlashDelay = 0.15f; // Задержка мигания ошибки
        [SerializeField] private int _errorFlashCount = 3; // Количество миганий ошибки

        [SerializeField] private string _cursorChar = "_";
        [SerializeField] private float _cursorBlinkSpeed = 0.5f;

        private Button _playButton;
        private Button _exitButton;
        private Button _settingsButton;
        private readonly List<Label> _terminalLabels = new();
        private VisualElement _logsContainer;
        private readonly Dictionary<Label, string> _originalTexts = new();
        private Coroutine _typingCoroutine;

        protected override void OnInit()
        {
            _playButton = Root.Q<Button>(name: _playButtonName);
            _exitButton = Root.Q<Button>(name: _exitButtonName);
            _settingsButton = Root.Q<Button>(name: _settingsButtonName);

            _logsContainer = Root.Q<VisualElement>("Logs");
            _terminalLabels.Clear();
            _originalTexts.Clear();

            for (var i = 1; i <= 12; i++)
            {
                var labelName = $"TerminalLog{i}";
                var label = _logsContainer.Q<Label>(labelName);

                if (label != null)
                {
                    _terminalLabels.Add(label);
                    _originalTexts[label] = label.text;
                    label.text = string.Empty;
                    label.style.visibility = Visibility.Hidden;
                }
            }
        }

        public override void Show()
        {
            base.Show();
            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(TypeTerminalLogs());
        }

        protected override void OnBind(MainMenuViewModel viewModel)
        {
            base.OnBind(viewModel);
            _playButton.RegisterCallbackOnce<ClickEvent>(StartGameplay);
            _settingsButton.RegisterCallback<ClickEvent>(OpenSettings);
            _exitButton.RegisterCallbackOnce<ClickEvent>(Quit);
        }
        
        private void StartGameplay(ClickEvent clickEvent)
        {
            ViewModel.StartGameplay();
        }
        
        private void OpenSettings(ClickEvent clickEvent)
        {
            ViewModel.OpenSettings();
        }
        
        private void Quit(ClickEvent clickEvent)
        {
            ViewModel.Quit();
        }

        private IEnumerator TypeTerminalLogs()
        {
            yield return new WaitForSeconds(_initialDelay);

            foreach (var label in _terminalLabels)
            {
                if (!_originalTexts.TryGetValue(label, out var originalText))
                    continue;

                label.style.visibility = Visibility.Visible;

                yield return label.name switch
                {
                    "TerminalLog7" => StartCoroutine(TypeWithErrorEffect(label, originalText)), // error log
                    "TerminalLog12" => StartCoroutine(TypeWithBlinkingCursor(label, originalText)), // last log
                    _ => StartCoroutine(TypeText(label, originalText)) // common log
                };

                if (label != _terminalLabels[^1]) // new line delay
                    yield return new WaitForSeconds(_lineDelay);
            }
        }

        private IEnumerator TypeText(Label label, string text)
        {
            var currentText = string.Empty;
            label.text = currentText;

            foreach (char c in text)
            {
                currentText += c;
                label.text = currentText;

                if (c is '=' or '-' or '║' or '▒' or '▓') // special symbols delay
                    yield return new WaitForSeconds(_typingSpeed * 0.3f);
                else
                    yield return new WaitForSeconds(_typingSpeed);
            }
        }

        private IEnumerator TypeWithErrorEffect(Label label, string text)
        {
            var currentText = string.Empty;
            label.text = currentText;

            var errorStartIndex = text.IndexOf("**", StringComparison.Ordinal);
            if (errorStartIndex == -1) errorStartIndex = text.Length;

            for (var i = 0; i < errorStartIndex; i++)
            {
                currentText += text[i];
                label.text = currentText;
                yield return new WaitForSeconds(_typingSpeed);
            }

            // Flickering error
            var errorText = text[errorStartIndex..];
            var textBeforeError = currentText;

            for (var i = 0; i < _errorFlashCount; i++)
            {
                label.text = textBeforeError + errorText;
                yield return new WaitForSeconds(_errorFlashDelay);

                label.text = textBeforeError;
                yield return new WaitForSeconds(_errorFlashDelay);
            }

            label.text = text;
        }

        private IEnumerator TypeWithBlinkingCursor(Label label, string text)
        {
            yield return StartCoroutine(TypeText(label, text));

            var originalText = label.text;
            var cursorVisible = true;
            var timer = 0f;

            while (true)
            {
                timer += Time.deltaTime;
                if (timer >= _cursorBlinkSpeed)
                {
                    timer = 0f;
                    cursorVisible = !cursorVisible;
                    label.text = originalText + (cursorVisible ? _cursorChar : "");
                }
                yield return null;
            }
        }

        private void SetButtonsVisibility(bool visible)
        {
            var opacity = visible ? 1f : 0f;

            _playButton.style.opacity = opacity;
            _playButton.SetEnabled(visible);

            _settingsButton.style.opacity = opacity;
            _settingsButton.SetEnabled(visible);

            _exitButton.style.opacity = opacity;
            _exitButton.SetEnabled(visible);

            if (visible)
            {
                _playButton.schedule.Execute(() =>
                {
                    _playButton.experimental.animation.Start(opacity, 1f, 500, (e, v) => e.style.opacity = v);
                    _settingsButton.experimental.animation.Start(opacity, 1f, 500, (e, v) => e.style.opacity = v);
                    _exitButton.experimental.animation.Start(opacity, 1f, 500, (e, v) => e.style.opacity = v);
                }).StartingIn(100);
            }
        }

        public void ResetTerminalAnimation()
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
                _typingCoroutine = null;
            }

            foreach (var label in _terminalLabels)
            {
                if (_originalTexts.ContainsKey(label))
                {
                    label.text = "";
                    label.style.visibility = Visibility.Hidden;
                }
            }

            SetButtonsVisibility(false);
            _typingCoroutine = StartCoroutine(TypeTerminalLogs());
        }

        public override void Dispose()
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
                _typingCoroutine = null;
            }
            base.Dispose();
        }
    }
}