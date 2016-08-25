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

### Use an object pool
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
