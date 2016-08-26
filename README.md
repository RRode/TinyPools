# TinyPools
Yet another thread safe object pool implementation in .NET.

## License
TinyPools is licensed under [MIT license](LICENSE).

## How to
### Create an object pool
Object pool is created by providing a factory method for the object that will be stored in the pool.
```C#
var objectPool = new ObjectPool<ExpensiveObject>(() => new ExpensiveObject());
```
By default this creates an object pool with unlimited storage capacity. Optionally you can specify maximum capacity of stored items in the pool. 
```C#
var objectPool = new ObjectPool<ExpensiveObject>(() => new ExpensiveObject(), 3);
```
Objects returned while the pool is full will not be stored and thus be available for garbage collection.

### Using an object pool
```C#
//Get a wrapper for the pooled object from the pool
using (var pooledObject = objectPool.GetObject())
{
  //Get and use the pooled object
  var expensiveObject = pooledObject.Object;
  expensiveObject.DoSomeWork();
}
//Dispose of the wrapper to return the object into pool
```
Note that the pool always returns an object.

### Create a memory pool
First define segments that will be used by the memory pool by setting the size of arrays returned by that segment. A segment in a nutshell acts as an object pool for arrays of the specified size.
```C#
var smallSegment = new SegmentDefinition(700);
```
Optionally as with the object pool, you can set the maximum storage capacity for the segment.
```C#
//Limit medium and largest array segment to store only 2 arrays at a time
var mediumSegment = new SegmentDefinition(1400, 2);
var largeSegment = new SegmentDefinition(2000, 2);
```
Create a memory pool of type <T> with defined segments.
```C#
var memoryPool = new MemoryPool<int>(smallSegment, mediumSegment, largeSegment);
```
### Using a memory pool
Get an array wrapper from memory pool. Note that returned array size will be equal to nearest larger of equal segment size. Requesting an array larger than the largest defined segment will throw an exception.
```C#
var requestedSize = 800;

using (var pooledArray = memoryPool.GetArray(requestedSize))
{
  //Get and use the array from the pool
  var array = pooledArray.Object; //Note that array.Length == 1400
}
//Dispose of the wrapper to return the array into pool
```
