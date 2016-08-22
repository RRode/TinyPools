using NUnit.Framework;

namespace TinyPools.UnitTests.MemoryPool_specs
{
    [TestFixture]
    public class A_new_pool
    {
        [Test]
        public void Is_empty()
        {
            var segment = new SegmentDefinition(1);
            var pool = new MemoryPool<int>(segment);

            Assert.That(pool.Segments[0].StoredArrays, Is.Zero);
        }
    }

    [TestFixture]
    public class A_pool_with_a_segment_filled_to_maximum_capacity
    {
        [Test]
        public void Does_not_store_more_than_a_maximum_capacity_of_arrays_for_that_segment()
        {
            const int capacity = 1;
            var segment = new SegmentDefinition(2, capacity);
            var pool = new MemoryPool<int>(segment);

            var firstArray = pool.GetArray(segment.ArraySize);
            var secondArray = pool.GetArray(segment.ArraySize);
            firstArray.Dispose();
            secondArray.Dispose();

            Assert.That(pool.Segments[0].StoredArrays, Is.EqualTo(capacity));
        }
    }

    [TestFixture]
    public class A_pool_with_stored_arrays
    {
        private MemoryPool<int> _pool;
        private SegmentDefinition _smallSegment, _mediumSegment, _largeSegment;
        private int[] _storedArray;

        [SetUp]
        public void _Setup()
        {
            _smallSegment = new SegmentDefinition(2);
            _mediumSegment = new SegmentDefinition(5);
            _largeSegment = new SegmentDefinition(15);
            _pool = new MemoryPool<int>(_smallSegment, _largeSegment, _mediumSegment);
            var pooledArray = _pool.GetArray(5);
            _storedArray = pooledArray.Object;
            pooledArray.Dispose();
        }

        [Test]
        public void Returns_a_stored_array_of_nearest_largest_segment_array_size_when_different_to_requested_size()
        {
            const long requestedSize = 3;

            var pooledArray = _pool.GetArray(requestedSize);

            Assert.That(pooledArray.Object, Is.SameAs(_storedArray));
            Assert.That(pooledArray.Object.LongLength, Is.EqualTo(_mediumSegment.ArraySize));
        }

        [Test]
        public void Returns_a_stored_array_of_requested_size_when_equal_to_segment_array_size()
        {
            var pooledArray = _pool.GetArray(_mediumSegment.ArraySize);

            Assert.That(pooledArray.Object, Is.SameAs(_storedArray));
            Assert.That(pooledArray.Object.LongLength, Is.EqualTo(_mediumSegment.ArraySize));
        }
    }

    [TestFixture]
    public class An_empty_pool_with_segment_definitions_not_sorted_by_size
    {
        private MemoryPool<int> _pool;
        private SegmentDefinition _smallSegment, _mediumSegment, _largeSegment;

        [SetUp]
        public void _Setup()
        {
            _smallSegment = new SegmentDefinition(2);
            _mediumSegment = new SegmentDefinition(5);
            _largeSegment = new SegmentDefinition(15);
            _pool = new MemoryPool<int>(_smallSegment, _largeSegment, _mediumSegment);
        }

        [Test]
        public void Allocates_a_new_array_of_nearest_larger_segment_array_size_when_different_to_requested_size()
        {
            const long requestedSize = 3;

            var pooledArray = _pool.GetArray(requestedSize);

            Assert.That(pooledArray.Object.LongLength, Is.EqualTo(_mediumSegment.ArraySize));
        }

        [Test]
        public void Allocates_a_new_array_of_requested_size_when_equal_to_segment_array_size()
        {
            var pooledArray = _pool.GetArray(_mediumSegment.ArraySize);

            Assert.That(pooledArray.Object.LongLength, Is.EqualTo(_mediumSegment.ArraySize));
        }
    }

    [TestFixture]
    public class Any_pool
    {
        private MemoryPool<int> _pool;
        private SegmentDefinition _smallSegment, _mediumSegment, _largeSegment;

        [SetUp]
        public void _Setup()
        {
            _smallSegment = new SegmentDefinition(2);
            _mediumSegment = new SegmentDefinition(5);
            _largeSegment = new SegmentDefinition(15);
            _pool = new MemoryPool<int>(_smallSegment, _mediumSegment, _largeSegment);
        }

        [Test]
        public void Accepts_released_array_into_originating_segment()
        {
            const long requestedSize = 3;
            var pooledArray = _pool.GetArray(requestedSize);

            pooledArray.Dispose();

            Assert.That(_pool.Segments[1].StoredArrays, Is.EqualTo(1));
        }

        [Test]
        public void Has_maximum_array_size_equal_to_largest_segment_array_size()
        {
            Assert.That(_pool.MaxArraySize, Is.EqualTo(_largeSegment.ArraySize));
        }

        [Test]
        public void Rejects_allocation_of_arrays_larger_than_largest_defined_array_size_in_a_segment()
        {
            const long requestedSize = 16;

            Assert.That(() => _pool.GetArray(requestedSize),
                Throws.ArgumentException.With.
                Message.EqualTo($"Requested array size of {requestedSize} exceeds maximum array size"));
        }

        [Test]
        public void Rejects_null_segment_definitions()
        {
            Assert.That(() => new MemoryPool<int>(null),
                Throws.ArgumentNullException.With.Message.EqualTo("Segment definition can not be null"),
                "First parameter");

            Assert.That(() => new MemoryPool<int>(_smallSegment, null),
                Throws.ArgumentNullException.With.Message.EqualTo("Segment definition can not be null"),
                "Params array");

            Assert.That(() => new MemoryPool<int>(_smallSegment, _mediumSegment, null),
                Throws.ArgumentNullException.With.Message.EqualTo("Segment definition can not be null"),
                "Params item");
        }

        [Test]
        public void Rejects_segments_of_array_size_smaller_than_1()
        {
            Assert.That(() => new SegmentDefinition(0),
                Throws.ArgumentException.With.Message.EqualTo("Segment array size can not be smaller than 1"));
        }

        [Test]
        public void Rejects_segments_with_maximum_capacity_smaller_than_1()
        {
            Assert.That(() => new SegmentDefinition(1, 0),
                Throws.ArgumentException.With.Message.EqualTo("Segment capacity can not be smaller than 1"));
        }
    }
}