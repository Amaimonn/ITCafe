using System;
using UnityEngine;
using R3;

namespace MiUI
{
    /// <summary>
    /// Базовая реализация представления через MonoBehaviour.
    /// От этого класса может отталкиваться (через наследование) дальнейшая реализация через Canvas или UI Toolkit.
    /// </summary>
    public abstract class BaseView : MonoBehaviour, IView, IDisposable
    {
        public Observable<Unit> OnDisposed => _onDisposed;
        public Subject<Unit> _onDisposed = new();

        public abstract void Show();
        public abstract void Hide();

        public virtual void Dispose()
        {
            _onDisposed.OnNext(Unit.Default);
        }
    }
}