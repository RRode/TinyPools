using System;
using System.Threading.Tasks;

namespace TinyPools.Samples
{
    public static class MemoryPoolSample
    {
        public static void Run()
        {
            Console.WriteLine("Starting memory pool sample");

            var random = new Random();

            //Define array sizes returned from memory pool segments
            var smallSegment = new SegmentDefinition(700);

            //Limit medium and largest array segment to store only 2 arrays at a time
            var mediumSegment = new SegmentDefinition(1400, 2);
            var largeSegment = new SegmentDefinition(2000, 2);

            //Create a memory pool with defined segments
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

                //Get an array wrapper from memory pool. Note that returned array size will 
                //be equal to nearest larger of equal segment size. Requesting an array larger
                //than the largest defined segment will throw an exception. 
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