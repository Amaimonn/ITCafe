using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class PauseView : ScreenToolkitAttach<PauseViewModel>
    {
        [SerializeField] private string _resumeButtonName = "ResumeButton";
        [SerializeField] private string _closeButtonName = "CloseButton";
        [SerializeField] private string _restartButtonName = "RestartButton";
        [SerializeField] private string _exitButtonName = "ExitButton";
        
        private Button _resumeButton;
        private Button _closeButton;
        private Button _restartButton;
        private Button _exitButton;

        protected override void OnInit()
        {
            _resumeButton = Root.Q<Button>(name: _resumeButtonName);
            _closeButton = Root.Q<Button>(name: _closeButtonName);
            _restartButton = Root.Q<Button>(name: _restartButtonName);
            _exitButton = Root.Q<Button>(name: _exitButtonName);
        }

        protected override void OnBind(PauseViewModel viewModel)
        {
            base.OnBind(viewModel);
            
            _resumeButton.SubscribeCallback<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);
            
            _closeButton.SubscribeCallback<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);

            _restartButton.SubscribeCallbackOnce<ClickEvent>(OnRestartClicked)
                .AddTo(_disposables);

            _exitButton.SubscribeCallbackOnce<ClickEvent>(OnExitClicked)
                .AddTo(_disposables);
        }

        public void OnCloseClicked(ClickEvent _)
        {
            ViewModel.StartClosing();
        }

        public void OnExitClicked(ClickEvent _)
        {
            ViewModel.ExitToMenu();
        }

        public void OnRestartClicked(ClickEvent _)
        {
            ViewModel.Restart();
        }
    }
}