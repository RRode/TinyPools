namespace TinyPools
{
    /// <summary>
    /// Represents a memory pool segment that contains arrays of defined size.
    /// </summary>
    /// <typeparam name="T">Type of object arrays stored in the segment.</typeparam>
    public class Segment<T>
    {
        private readonly ObjectPool<T[]> _arrayPool;

        internal Segment(long arraySize)
        {
            ArraySize = arraySize;
            _arrayPool = new ObjectPool<T[]>(() => new T[arraySize]);
        }

        internal Segment(long arraySize, int capacity)
        {
            ArraySize = arraySize;
            _arrayPool = new ObjectPool<T[]>(() => new T[arraySize], capacity);
        }

        /// <summary>
        /// Size of arrays stored in this segment.
        /// </summary>
        public long ArraySize { get; }

        /// <summary>
        /// Count of currently available arrays in the pool segment.
        /// </summary>
        public long StoredArrays
        {
            get
            {
                lock (_arrayPool)
                {
                    return _arrayPool.StoredObjects;
                }
            }
        }

        internal PooledObject<T[]> GetArray()
        {
            lock (_arrayPool)
            {
                return _arrayPool.GetObject();
            }
        }
    }
}