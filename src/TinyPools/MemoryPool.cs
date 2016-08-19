using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TinyPools
{
    public class MemoryPool<T>
    {
        public MemoryPool(SegmentDefinition definition, params SegmentDefinition[] definitions)
        {
            if (_ContainsNulls(definition, definitions))
            {
                throw new ArgumentNullException("Segment definition can not be null", (Exception)null);
            }

            var list = new List<SegmentDefinition>(definitions) { definition };

            var orderedSegments = list.Select(sd => sd.CreateSegment<T>())
                .OrderBy(s => s.Size)
                .ToList();
            Segments = new ReadOnlyCollection<Segment<T>>(orderedSegments);
            MaxArraySize = Segments.Last().Size;
        }

        public long MaxArraySize { get; }

        public ReadOnlyCollection<Segment<T>> Segments { get; }

        //TODO: document a throw of exception
        public PooledObject<T[]> GetArray(long size)
        {
            if (size > MaxArraySize)
            {
                throw new ArgumentException($"Requested array size of {size} exceeds maximum array size");
            }

            var segment = Segments.First(s => s.Size >= size);
            return segment.GetArray();
        }

        private static bool _ContainsNulls(SegmentDefinition definition, SegmentDefinition[] definitions)
        {
            return definition == null || definitions == null || definitions.Any(sd => sd == null);
        }
    }
}