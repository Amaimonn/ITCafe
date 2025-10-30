using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ITCafe.Gameplay.UI.MVVM;
using UnityEngine;

namespace ITCafe.CafeBusiness
{
    public class GameSessionRunner : IDisposable
    {
        private readonly WorkProgressService _workProgressService;
        private readonly HUDViewModel _hudViewModel;
        private readonly ClientsRunner _clientsRunner;

        private CancellationTokenSource _cts;
        private readonly TimeSpan _sessionDuration = TimeSpan.FromMinutes(3);

        public GameSessionRunner(WorkProgressService workProgressService, HUDViewModel hudViewModel, ClientsRunner clientsRunner)
        {
            _workProgressService = workProgressService;
            _hudViewModel = hudViewModel;
            _clientsRunner = clientsRunner;
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
            var report = _workProgressService.GetDailyReport();
            Debug.Log(report);
        }

        public void Dispose()
        {
            Disposes.ClearCts(ref _cts);
        }
    }
}