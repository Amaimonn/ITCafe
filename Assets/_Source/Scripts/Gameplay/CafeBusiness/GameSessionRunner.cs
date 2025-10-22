using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace ITCafe.CafeBusiness
{
    public class GameSessionRunner : IDisposable
    {
        private readonly WorkProgressService _workProgressService;
        private CancellationTokenSource _cts;

        public GameSessionRunner(WorkProgressService workProgressService)
        {
            _workProgressService = workProgressService;
        }

        public async UniTaskVoid RunSessionAsync(CancellationToken token)
        {
            _cts = new();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, token);
            
            try
            {
                Debug.Log($"[{nameof(GameSessionRunner)}]: Running game session");
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
            }
        }

        public void CompleteSession()
        {
            _workProgressService.CompleteDay();
            var report = _workProgressService.GetDailyReport();
            Debug.Log(report);
        }

        public void Dispose()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}