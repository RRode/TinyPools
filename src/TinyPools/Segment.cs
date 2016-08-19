namespace TinyPools
{
    public class Segment<T>
    {
        private readonly ObjectPool<T[]> _arrayPool;

        internal Segment(long size)
        {
            Size = size;
            _arrayPool = new ObjectPool<T[]>(() => new T[size]);
        }

        internal Segment(long size, int capacity)
        {
            Size = size;
            _arrayPool = new ObjectPool<T[]>(() => new T[size], capacity);
        }

        public long Size { get; }

        public long TotalStoredArrays
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