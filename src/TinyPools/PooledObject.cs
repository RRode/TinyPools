using System;

namespace TinyPools
{
    public sealed class PooledObject<T> : IDisposable where T : class
    {
        private readonly object _disposeLock = new object();
        private T _obj;
        private ObjectPool<T> _sourcePool;

        internal PooledObject(T obj, ObjectPool<T> sourcePool)
        {
            _obj = obj;
            _sourcePool = sourcePool;
        }

        public T Object
        {
            get
            {
                lock (_disposeLock)
                {
                    return _obj;
                }
            }
        }

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (_obj == null)
                {
                    return;
                }

                _sourcePool.ReturnObject(_obj);
                _obj = null;
                _sourcePool = null;
            }
        }
    }
}