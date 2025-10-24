using System;

namespace MiUI.MVVM
{
    /// <summary>
    /// Возможный базовый класс для ViewModel (просто реализующий IDisposable)
    /// </summary>
    public abstract class BaseViewModel : IViewModel, IDisposable
    {
        public virtual void Dispose()
        {
        }
    }
}