using System;
using System.Threading.Tasks;

namespace TinyPools.Samples
{
    public static class ObjectPoolSample
    {
        public static void Run()
        {
            Console.WriteLine("Starting object pool sample");

            //Object pool is created by providing factory method for the object that will be stored in the pool
            var objectPool = new ObjectPool<ExpensiveObject>(() => new ExpensiveObject());

            var task1 = Task.Run(() => UseObjectPool(objectPool));
            var task2 = Task.Run(() => UseObjectPool(objectPool));
            var task3 = Task.Run(() => UseObjectPool(objectPool));

            Task.WaitAll(task1, task2, task3);
        }

        private static void UseObjectPool(ObjectPool<ExpensiveObject> objectPool)
        {
            for (var i = 0; i < 5; i++)
            {
                //Get a wrapper for the pooled object from the pool
                using (var pooledObject = objectPool.GetObject())
                {
                    //Get and use the pooled object
                    var expensiveObject = pooledObject.Object;
                    expensiveObject.DoSomeWork();
                }
                //Dispose of the wrapper to return the object into pool
            }
        }
    }
}