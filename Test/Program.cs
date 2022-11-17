using Daily.RedisDriver.Mini;
using Daily.RedisDriver.Mini.Internal;
using Daily.RedisDriver.Mini.Internal.ObjectPool;
using Daily.RedisDriver.Mini.Internal.RedisClientPool;
using System.Net.Sockets;

namespace Test
{
    internal class Program
    {
        static RedisClient redis = new RedisClient("192.168.0.34:6379,password=123456,min poolsize=1,max poolsize=1");

        static void Main(string[] args)
        {
            #region MyRegion

            //var objectPool = new ObjectPool<Person>(3, () => new Person() { Id = 1 });
            //Parallel.For(0, 1000, i =>
            //{
            //    var objectWarp = objectPool.Get();

            //    objectPool.Return(objectWarp);
            //});

            #endregion

            ////多线程写数据
            //Parallel.For(0, 1000, i =>
            //{
            //    var res = redis.Set($"mini-{i}", i.ToString());
            //    Console.WriteLine($"{i}-{res}");
            //});

            ////多线程读数据
            //Parallel.For(0, 1000, i =>
            //{
            //    var res = redis.Get($"mini-{i}");
            //    Console.WriteLine($"{i}-{res}");
            //});

            //多线程删除数据
            //Parallel.For(0, 1000, i =>
            //{
            //    var res = redis.Del($"mini-{i}");
            //    Console.WriteLine(res);
            //});
        }
    }

    public class Person
    {
        public Person()
        {
        }

        public int Id { get; set; }
    }
}