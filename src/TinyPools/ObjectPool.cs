using System;
using System.Collections.Generic;

namespace TinyPools
{
    /// <summary>
    /// Represents a thread safe object pool implementation.
    /// </summary>
    /// <typeparam name="T">Type of objects stored in the pool.</typeparam>
    public class ObjectPool<T> where T : class
    {
        private const int UNLIMITED_CAPACITY = -1;
        private readonly int _capacity = UNLIMITED_CAPACITY;
        private readonly Func<T> _factory;
        private readonly Queue<T> _objectQueue = new Queue<T>();

        /// <summary>
        /// Initializes a new instance of object pool class that is empty and has unlimited capacity.
        /// </summary>
        /// <param name="factory">The delegate used to create instances of pooled objects.</param>
        /// <exception cref="ArgumentNullException">The factory argument is null.</exception>
        public ObjectPool(Func<T> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _factory = factory;
        }

        /// <summary>
        /// Initializes a new instance of object pool class that is empty and has limited storage capacity.
        /// Objects returned after object pool is full will be released for garbage collection.
        /// </summary>
        /// <param name="factory">The delegate used to create instances of pooled objects.</param>
        /// <param name="capacity">Maximum number of stored objects in the pool.</param>
        /// <exception cref="ArgumentNullException">Factory argument is null.</exception>
        /// <exception cref="ArgumentException">Capacity is less than 1.</exception>
        public ObjectPool(Func<T> factory, int capacity)
            : this(factory)
        {
            if (capacity < 1)
            {
                throw new ArgumentException(nameof(capacity));
            }

            _capacity = capacity;
        }

        /// <summary>
        /// Total count of currently available objects in the pool.
        /// </summary>
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

        /// <summary>
        /// Returns a stored object from the pool or creates a new instance if none are available.
        /// </summary>
        /// <returns>Wrapper containing pooled object.</returns>
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