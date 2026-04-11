using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.Data;
using R3;

namespace ITCafe.UI.MVVM
{
    public class ConfirmPopUpViewModel : ScreenViewModel
    {
        public Observable<string> OnTitleChanged => _title;
        public Observable<string> OnMessageChanged => _message;
        public Observable<string> OnConfirmTextChanged => _confirmText;
        public Observable<string> OnCancelTextChanged => _cancelText;
        public Observable<bool> OnConfirmEnabledChanged => _isConfirmEnabled;
        public Observable<Unit> OnConfirmed => _confirmed;
        public Observable<Unit> OnCancelled => _cancelled;

        private readonly ReactiveProperty<string> _title = new();
        private readonly ReactiveProperty<string> _message = new();
        private readonly ReactiveProperty<string> _confirmText = new();
        private readonly ReactiveProperty<string> _cancelText = new();
        private readonly ReactiveProperty<bool> _isConfirmEnabled = new(true);
        private readonly Subject<Unit> _confirmed = new();
        private readonly Subject<Unit> _cancelled = new();

        public void Setup(ConfirmationSetup confirmationSetup)
        {
            if (_confirmText == null)
            {
                FLogger.LogWarning<ConfirmPopUpViewModel>("Confirm text not set");
                return;
            }
            
            _title.Value = confirmationSetup.TitleLid;
            _message.Value = confirmationSetup.MessageLid;
            _confirmText.Value = confirmationSetup.ConfirmTextLid;
            _cancelText.Value = confirmationSetup.CancelTextLid;
        }

        public void Setup(string title, string message, string confirmText = "CONFIRM", string cancelText = "CANCEL")
        {
            _title.Value = title;
            _message.Value = message;
            _confirmText.Value = confirmText;
            _cancelText.Value = cancelText;
        }

        public void SetConfirmEnabled(bool isEnabled)
        {
            _isConfirmEnabled.Value = isEnabled;
        }

        public void Confirm()
        {
            _confirmed.OnNext(Unit.Default);
            StartClosing();
        }

        public void Cancel()
        {
            _cancelled.OnNext(Unit.Default);
            StartClosing();
        }
    }
}