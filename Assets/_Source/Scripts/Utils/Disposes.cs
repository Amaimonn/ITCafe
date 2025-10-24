using System;
using System.Threading;

public static class Disposes
{
    public static void ResetCts(ref CancellationTokenSource cts)
    {
        CancelAndDispose(cts);
        cts = new();
    }

    public static void ClearCts(ref CancellationTokenSource cts)
    {
        CancelAndDispose(cts);
        cts = null;
    }

    public static void ClearDispose<T>(ref T disposable) where T : IDisposable
    {
        if (disposable != null)
        {
            disposable.Dispose();
            disposable = default;
        }
    }

    public static IDisposable Subscribe<T>(Action<T> source, Action<T> handler)
    {
        source += handler;
        return new UnSubscribe(() => source -= handler);
    }

    private class UnSubscribe : IDisposable
    {
        private readonly Action _unsubscribe;
        public UnSubscribe(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            _unsubscribe?.Invoke();
        }
    }

    private static void CancelAndDispose(CancellationTokenSource cts)
    {
        if (cts == null)
            return;

        cts.Cancel();
        cts.Dispose();
    }
}