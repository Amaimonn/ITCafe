using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.Data;
using ITCafe.Shared;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace ITCafe.UI.MVVM
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
        
        [Header("PopUps"), Space(4)]
        [SerializeField] private ConfirmationSetup _restartPopUpSetup;
        [SerializeField] private ConfirmationSetup _exitPopUpSetup;
        
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
            
            viewModel.SetupExitPopUp(_exitPopUpSetup);
            viewModel.SetupRestartPopUp(_restartPopUpSetup);
            
            _disposables  = new CompositeDisposable();
            
            _resumeButton.SubscribeCallback<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);
            
            _closeButton.SubscribeCallback<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);

            _restartButton.SubscribeCallback<ClickEvent>(OnRestartClicked)
                .AddTo(_disposables);
            
            _settingsButton.SubscribeCallback<ClickEvent>(OnSettingsClicked)
                .AddTo(_disposables);

            _exitButton.SubscribeCallback<ClickEvent>(OnExitClicked)
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