using System;
using System.Threading.Tasks;

namespace TinyPools.Samples
{
    public static class WrongPollUsageSample
    {
        public static void Run()
        {
            Console.WriteLine("Starting wrong pool usage sample");

            var objectPool = new ObjectPool<ExpensiveObject>(() => new ExpensiveObject());

            var task1 = Task.Run(() => MisuseObjectPool(objectPool));
            var task2 = Task.Run(() => MisuseObjectPool(objectPool));
            var task3 = Task.Run(() => MisuseObjectPool(objectPool));

            Task.WaitAll(task1, task2, task3);
        }

        private static void MisuseObjectPool(ObjectPool<ExpensiveObject> objectPool)
        {
            for (var i = 0; i < 5; i++)
            {
                ExpensiveObject expensiveObject;

                using (var pooledObject = objectPool.GetObject())
                {
                    expensiveObject = pooledObject.Object;
                }

                //Do not use an object pool instance after it was returned to the pool
                //In this sample it can result in two threads using the same object at the same time
                expensiveObject.DoSomeWork();
            }
        }
    }
}