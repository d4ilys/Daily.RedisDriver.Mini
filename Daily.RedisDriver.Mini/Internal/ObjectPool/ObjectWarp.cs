using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daily.RedisDriver.Mini.Internal.ObjectPool
{
    public class ObjectWarp<T>
    {
        /// <summary>
        /// 资源对象
        /// </summary>
        public T Value { get; internal set; }

        internal bool _isReturned = false;

        /// <summary>
        /// 所属对象池
        /// </summary>
        public IObjectPool<T> Pool { get; internal set; }


        public void ResetValue()
        {
            T value = default;
            try
            {
                value = Pool.Policy.OnCreate();
            }
            catch
            {
            }

            Value = value;
        }
    }
}