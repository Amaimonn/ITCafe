using DevKit.Locator;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.Shared;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace ITCafe.UI.MVVM
{
    public class ConfirmPopUpView : AttachableToolkitWindow<ConfirmPopUpViewModel>
    {
        [SerializeField] private string _titleLabelName = "TitleLabel";
        [SerializeField] private string _messageLabelName = "MessageLabel";
        [SerializeField] private string _confirmButtonName = "ConfirmButton";
        [SerializeField] private string _cancelButtonName = "CancelButton";
        
        [Header("SFX"), Space(4)]
        [SerializeField] private SfxData _confirmClickSfx;
        [SerializeField] private SfxData _cancelClickSfx;
        [SerializeField] private SfxData _closedSfx;

        private Label _titleLabel;
        private Label _messageLabel;
        private Button _confirmButton;
        private Button _cancelButton;
        
        private ILocalizer _localizer;

        private CompositeDisposable _disposables;

        [Inject] private readonly AudioPlayer _audioPlayer;

        protected override void OnInit()
        {
            base.OnInit();
            
            _localizer = ServiceLocator.Current.Get<ILocalizer>();
            
            _titleLabel = Root.Q<Label>(name: _titleLabelName);
            _messageLabel = Root.Q<Label>(name: _messageLabelName);
            _confirmButton = Root.Q<Button>(name: _confirmButtonName);
            _cancelButton = Root.Q<Button>(name: _cancelButtonName);
        }

        protected override void OnBind(ConfirmPopUpViewModel viewModel)
        {
            base.OnBind(viewModel);

            _disposables = new CompositeDisposable();
            
            viewModel.OnTitleChanged.Subscribe(x => SetLocalizedText(_titleLabel, x))
                .AddTo(_disposables);
            viewModel.OnMessageChanged.Subscribe(x => SetLocalizedText(_messageLabel, x))
                .AddTo(_disposables);
            viewModel.OnConfirmTextChanged.Subscribe(x => SetLocalizedText(_confirmButton, x))
                .AddTo(_disposables);
            viewModel.OnCancelTextChanged.Subscribe(x => SetLocalizedText(_cancelButton, x))
                .AddTo(_disposables);
            viewModel.OnConfirmEnabledChanged.Subscribe(_confirmButton.SetEnabled)
                .AddTo(_disposables);

            _confirmButton.SubscribeCallbackOnce<ClickEvent>(OnConfirmClicked)
                .AddTo(_disposables);
            _cancelButton.SubscribeCallbackOnce<ClickEvent>(OnCancelClicked)
                .AddTo(_disposables);
        }

        private void SetLocalizedText(TextElement textElement, string text)
        {
            textElement.text = text;
            if (!string.IsNullOrEmpty(textElement.text))
                _localizer.Localize(textElement, Constants.SHARED_TABLE);
        }
        
        private void OnConfirmClicked(ClickEvent _)
        {
            PlaySfx(_confirmClickSfx);
            ViewModel.Confirm();
        }

        private void OnCancelClicked(ClickEvent _)
        {
            PlaySfx(_cancelClickSfx);
            ViewModel.Cancel();
        }

        private void PlaySfx(SfxData sfxData)
        {
            if (sfxData.IsValid)
                _audioPlayer.GetSfxBuilder().Play(sfxData);
        }

        public override void Dispose()
        {
            Disposes.ClearDispose(ref _disposables);
            base.Dispose();
        }
    }
}
