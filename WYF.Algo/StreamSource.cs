using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WYF.Algo
{
    public abstract class StreamSource : IDisposable
    {
        private string type;
        private int refCount;  // 使用int替代AtomicInteger，

        protected StreamSource()
        {
            this.refCount = 0;
        }

        // 获取或设置类型
        public string Type
        {
            get
            {
                return this.type ?? "StreamSource";
            }
            set
            {
                this.type = value;
            }
        }

        // 获取行元数据
        public virtual RowMeta RowMeta { get; }

        // 获取行迭代器
        public abstract InnerRowIterator RowIterator();

        // 实现IDisposable接口的方法
        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        // 抽象方法，用于关闭资源
        public abstract void Close();

        // 提供一个方法来增加引用计数
        protected void IncrementRefCount()
        {
            Interlocked.Increment(ref refCount);
        }

        // 提供一个方法来减少引用计数
        protected void DecrementRefCount()
        {
            Interlocked.Decrement(ref refCount);
        }

        // 提供一个方法来获取当前引用计数
        protected int GetRefCount()
        {
            return refCount;
        }
    }
}
