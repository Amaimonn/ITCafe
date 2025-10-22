using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            try
            {
                await UniTask.Delay(TimeSpan.FromMinutes(3), cancellationToken: linkedTokenSource.Token);
                _workProgressService.CompleteDay();
                var report = _workProgressService.GetDailyReport();
                Debug.Log(report);
                
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                Debug.Log($"[{nameof(GameSessionRunner)}]: Operation cancelled");
            }
            finally
            {
                Debug.Log($"[{nameof(GameSessionRunner)}]: Clients lifecycle stopped");
            }
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