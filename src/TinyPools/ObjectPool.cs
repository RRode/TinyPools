using System;
using System.Collections.Generic;

namespace TinyPools
{
    public class ObjectPool<T> where T : class
    {
        private const int UNLIMITED_CAPACITY = -1;
        private readonly int _capacity = UNLIMITED_CAPACITY;
        private readonly Func<T> _factory;
        private readonly Queue<T> _objectQueue = new Queue<T>();

        public ObjectPool(Func<T> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _factory = factory;
        }

        public ObjectPool(Func<T> factory, int capacity)
            : this(factory)
        {
            if (capacity < 1)
            {
                throw new ArgumentException(nameof(capacity));
            }

            _capacity = capacity;
        }

        public int StoredObjects
        {
            get
            {
                lock (_objectQueue)
                {
                    return _objectQueue.Count;
                }
            }
        }

        public PooledObject<T> GetObject()
        {
            lock (_objectQueue)
            {
                var pooledObject = _objectQueue.Count == 0 ? _factory() : _objectQueue.Dequeue();

                return new PooledObject<T>(pooledObject, this);
            }
        }

        internal void ReturnObject(T pooledObject)
        {
            lock (_objectQueue)
            {
                if (_objectQueue.Count == _capacity)
                {
                    return;
                }

                _objectQueue.Enqueue(pooledObject);
            }
        }
    }
}