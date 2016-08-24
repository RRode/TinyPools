using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinyPools.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //ObjectPoolSample();
            MemoryPoolSample();
            Console.WriteLine("Sample completed. Press any key to exit.");
            Console.ReadKey();
        }

        private static void ObjectPoolSample()
        {
            //Object pool is created by providing factory method for the object that will be stored in the pool
            var objectPool = new ObjectPool<ExpensiveClass>(() => new ExpensiveClass());

            var task1 = Task.Run(() => UseObjectPool(objectPool));
            var task2 = Task.Run(() => UseObjectPool(objectPool));
            var task3 = Task.Run(() => UseObjectPool(objectPool));

            Task.WaitAll(task1, task2, task3);
        }

        private static void UseObjectPool(ObjectPool<ExpensiveClass> objectPool)
        {
            for (int i = 0; i < 5; i++)
            {
                //Get a wrapper for the pooled object from the pool
                using (var pooledObject = objectPool.GetObject())
                {
                    //Get and use the pooled object
                    var expensiveClass = pooledObject.Object;
                    expensiveClass.DoSomeWork();
                }
                //Dispose of the wrapper to return the object into pool
            }
        }

        private static void MemoryPoolSample()
        {
            var random = new Random();

            //Define sizes returned from memory pool segments
            var smallSegment = new SegmentDefinition(700);
            
            //Limit medium and largest array segment to store only 2 arrays at a time
            var mediumSegment = new SegmentDefinition(1400, 2);
            var largeSegment = new SegmentDefinition(2000, 2);

            //Create a memory pool with defined segements
            var memoryPool = new MemoryPool<int>(smallSegment, mediumSegment, largeSegment);

            var task1 = Task.Run(() => UseMemoryPool(random, memoryPool));
            var task2 = Task.Run(() => UseMemoryPool(random, memoryPool));
            var task3 = Task.Run(() => UseMemoryPool(random, memoryPool));

            Task.WaitAll(task1, task2, task3);
        }

        private static void UseMemoryPool(Random random, MemoryPool<int> memoryPool)
        {
            for (var i = 0; i < 5; i++)
            {
                var requestedSize = random.Next(1, 2000);

                //Get an array wrapper from memory pool. Note that array size will be equal 
                //or larger than requested size, depending on your array segments definition.
                using (var pooledArray = memoryPool.GetArray(requestedSize))
                {
                    //Get and use the array from the pool
                    var array = pooledArray.Object;

                    //Use the array
                    for (var j = 0; j < requestedSize; j++)
                    {
                        array[j] = Task.CurrentId.HasValue ? Task.CurrentId.Value : -1;
                    }

                    Task.Delay(20).Wait();
                    Console.WriteLine($"Task with ID [{Task.CurrentId}] requested an array of size {requestedSize} and got an array of size {array.Length}.");
                }
                //Dispose of the wrapper to return the array into pool
            }
        }
    }
}
