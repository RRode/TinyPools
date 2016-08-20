using NUnit.Framework;
using System;

namespace TinyPools.UnitTests.ObjectPool_specs
{
    [TestFixture]
    public class A_new_pool
    {
        [Test]
        public void Is_empty()
        {
            var objectPool = new ObjectPool<object>(() => null);

            Assert.That(objectPool.StoredObjects, Is.Zero);
        }
    }

    [TestFixture]
    public class A_pool_filled_to_maximum_capacity
    {
        [Test]
        public void Does_not_store_more_than_a_maximum_capacity_of_objects()
        {
            Func<object> factory = () => new object();
            var objectPool = new ObjectPool<object>(factory, 1);

            var firstObject = objectPool.GetObject();
            var secondObject = objectPool.GetObject();
            firstObject.Dispose();
            secondObject.Dispose();

            Assert.That(objectPool.StoredObjects, Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class A_pool_with_stored_objects
    {
        [Test]
        public void Returns_a_stored_object_when_object_is_requested()
        {
            Func<object> factory = () => new object();
            var objectPool = new ObjectPool<object>(factory);
            var pooledObject = objectPool.GetObject();
            var expected = pooledObject.Object;
            pooledObject.Dispose();

            pooledObject = objectPool.GetObject();

            Assert.That(pooledObject.Object, Is.SameAs(expected));
            Assert.That(objectPool.StoredObjects, Is.Zero);
        }
    }

    [TestFixture]
    public class An_empty_pool
    {
        [Test]
        public void Creates_a_new_object_when_object_is_requested()
        {
            var expected = new object();
            Func<object> factory = () => expected;
            var objectPool = new ObjectPool<object>(factory);

            var pooledObject = objectPool.GetObject();

            Assert.That(pooledObject.Object, Is.SameAs(expected));
        }
    }

    [TestFixture]
    public class Any_pool
    {
        private Func<object> _factory;
        private ObjectPool<object> _objectPool;

        [SetUp]
        public void _Setup()
        {
            _factory = () => new object();
            _objectPool = new ObjectPool<object>(_factory);
        }

        [Test]
        public void Accepts_released_object_into_pool()
        {
            var pooledObject = _objectPool.GetObject();
            pooledObject.Dispose();

            Assert.That(_objectPool.StoredObjects, Is.EqualTo(1));
        }

        [Test]
        public void Rejects_access_to_an_object_returned_to_pool()
        {
            var pooledObject = _objectPool.GetObject();
            pooledObject.Dispose();

            Assert.That(() => pooledObject.Object,
                Throws.TypeOf<ObjectDisposedException>().With.Message.Contains("PooledObject<System.Object>"));
        }

        [Test]
        public void Rejects_capacity_of_less_than_1()
        {
            Assert.That(() => new ObjectPool<object>(_factory, 0),
                Throws.ArgumentException.With.Message.Contains("capacity"));
        }

        [Test]
        public void Rejects_null_factory_method()
        {
            Assert.That(() => new ObjectPool<object>(null),
                Throws.ArgumentNullException.With.Message.Contains("factory"));
        }

        [Test]
        public void Rejects_returns_of_an_object_already_in_the_pool()
        {
            var pooledObject = _objectPool.GetObject();
            pooledObject.Dispose();
            pooledObject.Dispose();

            Assert.That(_objectPool.StoredObjects, Is.EqualTo(1));
        }
    }
}