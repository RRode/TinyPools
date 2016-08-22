using System;

namespace TinyPools
{
    /// <summary>
    /// Defines a segment in the memory pool.
    /// </summary>
    public sealed class SegmentDefinition
    {
        private readonly int _capacity;

        /// <summary>
        /// Initializes a new instance of segment definition with unlimited capacity 
        /// of stored arrays.
        /// </summary>
        /// <param name="arraySize">Size of arrays contained in this segment.</param>
        /// <exception cref="ArgumentException">Array size is smaller than 1.</exception>
        public SegmentDefinition(long arraySize)
        {
            if (arraySize < 1)
            {
                throw new ArgumentException("Segment array size can not be smaller than 1");
            }

            ArraySize = arraySize;
        }

        /// <summary>
        /// Initializes a new instance of segment definition with defined maximum capacity 
        /// of stored arrays.
        /// </summary>
        /// <param name="arraySize">Size of arrays contained in this segment.</param>
        /// <param name="capacity">Maximum number of stored arrays for this segment.</param>
        /// <exception cref="ArgumentException">
        /// Array size is smaller than 1. -or- 
        /// Segment capacity is smaller than 1.
        /// </exception>
        public SegmentDefinition(long arraySize, int capacity)
            : this(arraySize)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("Segment capacity can not be smaller than 1");
            }

            _capacity = capacity;
        }

        /// <summary>
        /// Size of arrays defined for this segment.
        /// </summary>
        public long ArraySize { get; }

        internal Segment<T> CreateSegment<T>()
        {
            return IsCapacityDefined() ? new Segment<T>(ArraySize, _capacity) : new Segment<T>(ArraySize);
        }

        private bool IsCapacityDefined()
        {
            return _capacity > 0;
        }
    }
}