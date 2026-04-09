using System;
using System.Collections.Generic;

namespace ITCafe.Shared
{
    public class NotNullPool<T>
    {
        private readonly Queue<T> _pool = new();
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Action<T> _onClear;
        private readonly Predicate<T> _checkNotNull;

        public NotNullPool(Func<T> createFunc, Action<T> onGet, Action<T> onRelease, Action<T> onClear, Predicate<T> checkNotNull)
        {
            _createFunc = createFunc;
            _onGet = onGet;
            _onRelease = onRelease;
            _onClear = onClear;
            _checkNotNull = checkNotNull;
        }

        public T Get()
        {
            if (_pool.Count > 0)
            {
                T item = _pool.Dequeue();
                if (_checkNotNull(item))
                {
                    _onGet(item);
                    return item;
                }
                else
                {
                    return Get();
                }
            }
            else
            {
                T item = _createFunc();
                _onGet(item);
                return item;
            }
        }

        public void Release(T item)
        {
            if (_checkNotNull(item))
            {
                _onRelease(item); 
                _pool.Enqueue(item);
            }
        }

        public void Clear()
        {
            foreach (var item in _pool)
            {
                if (_checkNotNull(item))
                {
                    _onClear(item);
                }
            }
            _pool.Clear();
        }
    }
}