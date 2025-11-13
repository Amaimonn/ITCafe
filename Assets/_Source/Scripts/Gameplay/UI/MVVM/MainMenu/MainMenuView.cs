using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class MainMenuView : ScreenToolkitAttach<MainMenuViewModel>
    {
        [SerializeField] private string _playButtonName = "PlayButton";
        [SerializeField] private string _exitButtonName = "ExitButton";
        
        private Button _playButton;
        private Button _exitButton;

        protected override void OnInit()
        {
            _playButton = Root.Q<Button>(name: _playButtonName);
            _exitButton = Root.Q<Button>(name: _exitButtonName);
        }

        protected override void OnBind(MainMenuViewModel viewModel)
        {
            _playButton.RegisterCallbackOnce<ClickEvent>(StartGameplay);
            _exitButton.RegisterCallbackOnce<ClickEvent>(_ => Application.Quit());
        }

        public void StartGameplay(ClickEvent clickEvent)
        {
            ViewModel.StartGameplay();
        }
    }
}