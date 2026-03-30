using DevKit.UI.MVVM.Bases;
using ITCafe.CafeBusiness;
using R3;
using VContainer;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class ResultsViewModel : ScreenViewModel
    {
        public Observable<ProgressReport> OnProgressReported => _reported;

        private readonly ReactiveProperty<ProgressReport> _reported;
        private readonly Subject<Unit> _exitGameplaySignal;
        private readonly Subject<Unit> _restartGameplaySignal;

        public ResultsViewModel(GameStatsService gameStatsService,
            [Key(Constants.GAMEPLAY_EXIT_SIGNAL)] Subject<Unit> exitGameplaySignal,
            [Key(Constants.RESTART_GAMEPLAY_SIGNAL)] Subject<Unit> restartGameplaySignal)
        {
            _exitGameplaySignal = exitGameplaySignal;
            _restartGameplaySignal = restartGameplaySignal;
            
            var report = gameStatsService.GetDailyReport();
            _reported = new ReactiveProperty<ProgressReport>(report);
        }

        public void ExitToMainMenu()
        {
            _exitGameplaySignal.OnNext(Unit.Default);
        }

        public void GoNextMission()
        {
        }

        public void Restart()
        {
            _restartGameplaySignal.OnNext(Unit.Default);
        }
    }
}