using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.Gameplay.Shared;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class PauseView : AttachableToolkitScreen<PauseViewModel>
    {
        [SerializeField] private string _resumeButtonName = "ResumeButton";
        [SerializeField] private string _closeButtonName = "CloseButton";
        [SerializeField] private string _restartButtonName = "RestartButton";
        [SerializeField] private string _settingsButtonName = "SettingsButton";
        [SerializeField] private string _exitButtonName = "ExitButton";
        
        [Header("SFX"), Space(4)]
        [SerializeField] private SfxData _buttonClickSfx;
        [SerializeField] private SfxData _closeClickSfx;
        
        private Button _resumeButton;
        private Button _closeButton;
        private Button _restartButton;
        private Button _settingsButton;
        private Button _exitButton;
        
        private CompositeDisposable _disposables;
        
        [Inject] private readonly AudioPlayer _audioPlayer;

        protected override void OnInit()
        {
            _resumeButton = Root.Q<Button>(name: _resumeButtonName);
            _closeButton = Root.Q<Button>(name: _closeButtonName);
            _restartButton = Root.Q<Button>(name: _restartButtonName);
            _settingsButton = Root.Q<Button>(name: _settingsButtonName);
            _exitButton = Root.Q<Button>(name: _exitButtonName);
        }

        protected override void OnBind(PauseViewModel viewModel)
        {
            base.OnBind(viewModel);
            
            _disposables  = new CompositeDisposable();
            
            _resumeButton.SubscribeCallbackOnce<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);
            
            _closeButton.SubscribeCallbackOnce<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);

            _restartButton.SubscribeCallbackOnce<ClickEvent>(OnRestartClicked)
                .AddTo(_disposables);
            
            _settingsButton.SubscribeCallback<ClickEvent>(OnSettingsClicked)
                .AddTo(_disposables);

            _exitButton.SubscribeCallbackOnce<ClickEvent>(OnExitClicked)
                .AddTo(_disposables);
        }

        private void OnCloseClicked(ClickEvent _)
        {
            if (_closeClickSfx.IsValid)
                _audioPlayer.GetSfxBuilder().Play(_closeClickSfx);
            
            ViewModel.StartClosing();
        }

        private void OnExitClicked(ClickEvent _)
        {
            PlayButtonSfx();
            ViewModel.ExitToMenu();
        }

        private void OnSettingsClicked(ClickEvent _)
        {
            PlayButtonSfx();
            ViewModel.OpenSettings();
        }
        
        private void OnRestartClicked(ClickEvent _)
        {
            PlayButtonSfx();
            ViewModel.Restart();
        }
        
        private void PlayButtonSfx()
        {
            if (_buttonClickSfx.IsValid)
                _audioPlayer.GetSfxBuilder().Play(_buttonClickSfx);
        }
        
        public override void Dispose()
        {
            Disposes.ClearDispose(ref _disposables);
            base.Dispose();
        }
    }
}