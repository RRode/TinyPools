using System;

namespace TinyPools
{
    public sealed class SegmentDefinition
    {
        private readonly int _capacity;

        public SegmentDefinition(long arraySize)
        {
            if (arraySize < 1)
            {
                throw new ArgumentException("Segment array size can not be smaller than 1");
            }

            ArraySize = arraySize;
        }

        public SegmentDefinition(long arraySize, int capacity)
            : this(arraySize)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("Segment capacity can not be smaller than 1");
            }

            _capacity = capacity;
        }

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