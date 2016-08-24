using System;
using System.Threading;
using System.Threading.Tasks;

namespace TinyPools.Samples
{
    internal class ExpensiveObject
    {
        private static int _instanceCount = -1;
        private readonly int _instanceNumber;

        public ExpensiveObject()
        {
            Task.Delay(200).Wait();
            _instanceNumber = Interlocked.Increment(ref _instanceCount);
        }

        public void DoSomeWork()
        {
            Console.WriteLine($"Doing work on instance {_instanceNumber} on task with ID {Task.CurrentId}");
            Task.Delay(20).Wait();
        }
    }
}