using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DevKit.UI.MVVM;
using ITCafe.Gameplay.UI.MVVM;
using R3;
using UnityEngine;

namespace ITCafe.CafeBusiness
{
    public class GameSessionRunner : IDisposable
    {
        public Observable<Unit> OnCompleted => _onCompleted;
        
        private readonly WorkProgressService _workProgressService;
        private readonly HUDViewModel _hudViewModel;
        private readonly ClientsRunner _clientsRunner;
        private readonly IViewBinder<ResultsView> _resultsBinder;
        private readonly InputService _inputService;

        private CancellationTokenSource _cts;
        private readonly TimeSpan _sessionDuration = TimeSpan.FromMinutes(3);
        private readonly Subject<Unit> _onCompleted = new();

        public GameSessionRunner(WorkProgressService workProgressService,
            HUDViewModel hudViewModel,
            ClientsRunner clientsRunner,
            IViewBinder<ResultsView> resultsBinder,
            InputService inputService)
        {
            _workProgressService = workProgressService;
            _hudViewModel = hudViewModel;
            _clientsRunner = clientsRunner;
            _resultsBinder = resultsBinder;
            _inputService = inputService;
        }

        public async UniTaskVoid RunSessionAsync(CancellationToken token)
        {
            _cts = new();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, token);

            try
            {
                Debug.Log($"[{nameof(GameSessionRunner)}]: Running game session");
                _clientsRunner.RunClientsLifeCycleAsync(linkedTokenSource.Token).Forget();
                _hudViewModel.StartSessionTimer(_sessionDuration);

                await UniTask.Delay(TimeSpan.FromMinutes(3), cancellationToken: linkedTokenSource.Token);

                CompleteSession();
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                Debug.Log($"[{nameof(GameSessionRunner)}]: Operation cancelled");
            }
            finally
            {
                Debug.Log($"[{nameof(GameSessionRunner)}]: Game session stopped");
                _hudViewModel.StopSessionTimer();
            }
        }

        public void CompleteSession()
        {
            Dispose();
            _clientsRunner.Dispose();
            _workProgressService.CompleteDay();
            
            _resultsBinder.Open();
            _inputService.SetInputEnabled(false);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _onCompleted.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            Disposes.ClearCts(ref _cts);
        }
    }
}