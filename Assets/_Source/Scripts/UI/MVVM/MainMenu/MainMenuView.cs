using DevKit.UI.MVVM.Bases;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using ITCafe.Shared;
using ITCafe.UI.Custom;
using VContainer;

namespace ITCafe.UI.MVVM
{
    public class MainMenuView : AttachableToolkitScreen<MainMenuViewModel>
    {
        [SerializeField] private string _playButtonName = "PlayButton";
        [SerializeField] private string _exitButtonName = "ExitButton";
        [SerializeField] private string _settingsButtonName = "SettingsButton";
        [SerializeField] private string _creditsButtonName = "CreditsButton";
        [SerializeField] private string _animatedTerminalName = "AnimatedTerminal";

        [Header("SFX"), Space(4)]
        [SerializeField] private SfxData _buttonClickSfx;

        private Button _playButton;
        private Button _exitButton;
        private Button _settingsButton;
        private Button _creditsButton;
        private AnimatedTextContainer _animatedTextContainer;

        private Coroutine _typingCoroutine;
        
        [Inject] private readonly AudioPlayer _audioPlayer;

        protected override void OnInit()
        {
            _playButton = Root.Q<Button>(name: _playButtonName);
            _settingsButton = Root.Q<Button>(name: _settingsButtonName);
            _creditsButton =  Root.Q<Button>(name: _creditsButtonName);
            _exitButton = Root.Q<Button>(name: _exitButtonName);

            _animatedTextContainer = Root.Q<AnimatedTextContainer>(name: _animatedTerminalName);
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
            
            _playButton.RegisterCallback<ClickEvent>(OnStartClicked);
            _settingsButton.RegisterCallback<ClickEvent>(OnSettingsClicked);
            _creditsButton.RegisterCallback<ClickEvent>(OnCreditsClicked);
            _exitButton.RegisterCallbackOnce<ClickEvent>(OnQuitClicked);
        }

        private void OnStartClicked(ClickEvent clickEvent)
        {
            PlayButtonSfx();
            ViewModel.StartGameplay();
        }

        private void OnSettingsClicked(ClickEvent clickEvent)
        {
            PlayButtonSfx();
            ViewModel.OpenSettings();
        }
        
        private void OnCreditsClicked(ClickEvent clickEvent)
        {
            PlayButtonSfx();
            ViewModel.OpenCredits();
        }

        private void OnQuitClicked(ClickEvent clickEvent)
        {
            PlayButtonSfx();
            ViewModel.Quit();
        }

        private void PlayButtonSfx()
        {
            if (_buttonClickSfx.IsValid)
                _audioPlayer.GetSfxBuilder().Play(_buttonClickSfx);
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