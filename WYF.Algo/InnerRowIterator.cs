using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronPython.SQLite.PythonSQLite;


namespace WYF.Algo
{
    public abstract class InnerRowIterator : IEnumerator<IRow>
    {
        // 定义一个空的迭代器实例
        public static readonly InnerRowIterator Empty = new EmptyInnerRowIterator();

        // 包装一个普通的迭代器为InnerRowIterator
        public static InnerRowIterator Wrapper(IEnumerator<IRow> iter)
        {
            if (iter is InnerRowIterator innerIter)
                return innerIter;

            return new InnerRowIteratorWrapper(iter);
        }

        private bool? hasNexted;
        private bool hasItered;
        private bool? isEmpty;

        // 实现IEnumerator<Row>接口的方法
        public IRow Current {

            get
            {
                if (this.hasNexted == null)
                    MoveNext();
                if (!this.hasNexted.Value)
                    throw new Exception();
                IRow next = _Next();
                this.hasNexted = null;
                this.hasItered = true;
                return next;
            }
            
           
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // 释放资源
        }
         
        public bool MoveNext()
        {
            if (hasNexted.HasValue)
                return hasNexted.Value;

            bool hasNext = _HasNext();
            hasNexted = hasNext;
            //if (!hasNexted.Value)
            //    throw new InvalidOperationException("No more elements to iterate.");
            this.hasNexted = hasNext;
            if (this.isEmpty == null)
                this.isEmpty = !hasNexted;
            return hasNext;
            //Current = _Next();
            //hasNexted = null;
            //hasItered = true;
            //return true;
        }

        public void Reset()
        {
            // 重置迭代器
            hasNexted = null;
            hasItered = false;
            isEmpty = null;
        }

        // 判断是否已经迭代过
        public bool HasItered()
        {
            return hasItered;
        }

        // 判断迭代器是否为空
        public bool IsEmpty()
        {
            return !hasNexted.HasValue ? !_HasNext() : !hasNexted.Value;
        }

        // 抽象方法：判断是否有下一个元素
        protected abstract bool _HasNext();

        // 抽象方法：获取下一个元素
        protected abstract IRow _Next();
    }

    // 空的InnerRowIterator实现
    public class EmptyInnerRowIterator : InnerRowIterator
    {
        protected override bool _HasNext()
        {
            return false;
        }

        protected override IRow _Next()
        {
            throw new InvalidOperationException("The iterator is empty.");
        }
    }

    // 包装普通迭代器的InnerRowIterator实现
    public class InnerRowIteratorWrapper : InnerRowIterator
    {
        private readonly IEnumerator<IRow> innerIterator;

        public InnerRowIteratorWrapper(IEnumerator<IRow> innerIterator)
        {
            this.innerIterator = innerIterator ?? throw new ArgumentNullException(nameof(innerIterator));
        }

        protected override bool _HasNext()
        {
            return innerIterator.MoveNext();
        }

        protected override IRow _Next()
        {
            return innerIterator.Current;
        }
    }
}
