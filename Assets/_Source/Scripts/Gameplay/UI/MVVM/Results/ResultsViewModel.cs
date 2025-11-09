using DevKit.UI.MVVM.Bases;
using ITCafe.CafeBusiness;
using R3;
using VContainer;

namespace ITCafe.Gameplay.UI.MVVM.Results
{
    public class ResultsViewModel : ScreenViewModel
    {
        public Observable<ProgressReport> OnProgressReported => _reported;

        private readonly ReactiveProperty<ProgressReport> _reported;
        private readonly Subject<Unit> _exitGameplaySignal;

        public ResultsViewModel(WorkProgressService workProgressService,
            [Key(Constants.GAMEPLAY_EXIT_SIGNAL)] Subject<Unit> exitGameplaySignal)
        {
            _exitGameplaySignal = exitGameplaySignal;
            var report = workProgressService.GetDailyReport();
            _reported = new ReactiveProperty<ProgressReport>(report);
        }

        public void ExitToMainMenu()
        {
            _exitGameplaySignal.OnNext(Unit.Default);
        }

        public void GoNextDay()
        {
        }

        public void Restart()
        {
        }
    }
}