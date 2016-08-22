using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TinyPools
{
    /// <summary>
    /// Represents a thread safe memory pool implementation.
    /// </summary>
    /// <typeparam name="T">Type of object arrays stored in the pool.</typeparam>
    public class MemoryPool<T>
    {
        /// <summary>
        /// Initializes a new instance of memory pool class that is empty.
        /// </summary>
        /// <param name="definition">Definition of an array segment in memory pool.</param>
        /// <param name="definitions">Definitions of an array segments in memory pool.</param>
        /// <exception cref="ArgumentNullException">Segment definition is null.</exception>
        public MemoryPool(SegmentDefinition definition, params SegmentDefinition[] definitions)
        {
            if (_ContainsNulls(definition, definitions))
            {
                throw new ArgumentNullException("Segment definition can not be null", (Exception)null);
            }

            var list = new List<SegmentDefinition>(definitions) { definition };

            var orderedSegments = list.Select(sd => sd.CreateSegment<T>())
                .OrderBy(s => s.ArraySize)
                .ToList();
            Segments = new ReadOnlyCollection<Segment<T>>(orderedSegments);
            MaxArraySize = Segments.Last().ArraySize;
        }

        /// <summary>
        /// Maximum array size that can be returned from the pool segments.
        /// </summary>
        public long MaxArraySize { get; }

        /// <summary>
        /// Array segments contained in the pool.
        /// </summary>
        public ReadOnlyCollection<Segment<T>> Segments { get; }

        /// <summary>
        /// Returns a stored array from a pool segment that contains equal or nearest larger size 
        /// of arrays. If no arrays are available for that pool segment, a new instance is created.
        /// </summary>
        /// <param name="size">Minimum required size of an array.</param>
        /// <returns>An array larger or equal to requested size.</returns>
        /// <exception cref="ArgumentException">
        /// Requested size is larger than maximum size that can be returned from the pool segments.
        /// </exception>
        public PooledObject<T[]> GetArray(long size)
        {
            if (size > MaxArraySize)
            {
                throw new ArgumentException($"Requested array size of {size} exceeds maximum array size");
            }

            var segment = Segments.First(s => s.ArraySize >= size);
            return segment.GetArray();
        }

        private static bool _ContainsNulls(SegmentDefinition definition, SegmentDefinition[] definitions)
        {
            return definition == null || definitions == null || definitions.Any(sd => sd == null);
        }
    }
}