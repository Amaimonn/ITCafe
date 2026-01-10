using DevKit.UI.MVVM.Bases;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using ITCafe.Gameplay.UI.Custom;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class MainMenuView : AttachableToolkitScreen<MainMenuViewModel>
    {
        [SerializeField] private string _playButtonName = "PlayButton";
        [SerializeField] private string _exitButtonName = "ExitButton";
        [SerializeField] private string _settingsButtonName = "SettingsButton";

        private Button _playButton;
        private Button _exitButton;
        private Button _settingsButton;
        private AnimatedTextContainer _animatedTextContainer; 
        
        private Coroutine _typingCoroutine;

        protected override void OnInit()
        {
            _playButton = Root.Q<Button>(name: _playButtonName);
            _exitButton = Root.Q<Button>(name: _exitButtonName);
            _settingsButton = Root.Q<Button>(name: _settingsButtonName);

            _animatedTextContainer = Root.Q<AnimatedTextContainer>(string.Empty);
        }

        public override void Show()
        {
            base.Show();
            if (_typingCoroutine != null)
                StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(TypeTextCoroutine());

            IEnumerator TypeTextCoroutine()
            {
                yield return _animatedTextContainer.RunCoroutine();
                _typingCoroutine = null;
            }
        }

        protected override void OnBind(MainMenuViewModel viewModel)
        {
            base.OnBind(viewModel);
            _playButton.RegisterCallback<ClickEvent>(StartGameplay);
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