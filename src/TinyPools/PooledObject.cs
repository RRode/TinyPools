using System;

namespace TinyPools
{
    /// <summary>
    /// Wrapper for pooled object returned by object pool allowing thread
    /// safe access and release of pooled object.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the object that is stored in the object pool
    /// </typeparam>
    public sealed class PooledObject<T> : IDisposable where T : class
    {
        private readonly object _disposeLock = new object();
        private bool _isDisposed;
        private T _obj;
        private ObjectPool<T> _sourcePool;

        internal PooledObject(T obj, ObjectPool<T> sourcePool)
        {
            _obj = obj;
            _sourcePool = sourcePool;
        }

        /// <summary>
        /// Gets the object returned from object pool.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Pooled object was returned to pool
        /// </exception>
        public T Object
        {
            get
            {
                lock (_disposeLock)
                {
                    if (_isDisposed)
                    {
                        throw new ObjectDisposedException($"PooledObject<{typeof(T)}>");
                    }

                    return _obj;
                }
            }
        }

        /// <summary>
        /// Returns the object back to object pool. Must be called explicitly
        ///  since dispose is not called during object finalization.
        /// </summary>
        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
                _sourcePool.ReturnObject(_obj);
                _obj = null;
                _sourcePool = null;
            }
        }
    }
}