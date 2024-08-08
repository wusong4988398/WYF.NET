using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WYF.Bos.DataEntity.Entity
{
    [Serializable]
    public class DataEntityCollection<T> : Collection<T>, ISupportInitialize
    {
        protected Object parent;

        private  bool initializing;


        public object Parent { get { return parent; } }

        public DataEntityCollection() { }

        public DataEntityCollection(Object parent)
        {
            this.parent = parent;
        }

    

        public DataEntityCollection(Object parent, IList<T> list):base(list) 
        {
           
            this.parent = parent;
        }
        public bool isInitialized()
        {
            return !this.initializing;
        }
        public void BeginInit()
        {
            this.initializing = true;
        }

        public void EndInit()
        {
            if (this.initializing)
            {
                ResetAllParent();
                this.initializing = false;
            }
        }


        private void ResetAllParent()
        {
            if (this.parent != null)
            {
                foreach (T item in this)
                {
                    IObjectWithParent op = item as IObjectWithParent;
                    if (op != null)
                    {
                        op.Parent = this.parent;
                     
                    }
                }
            }
        }
        public void Add(int index, T item)
        {
            if (item == null)
                throw new ArgumentException("item");
            base.Insert(index, item);
            if (!this.initializing && this.parent != null)
            {
                IObjectWithParent op = (item is IObjectWithParent) ? (IObjectWithParent)item : null;
                if (op != null)
                    op.Parent = this.parent;
            }
        }

        public T Set(int index, T item)
        {
            T oldItem = this[index];
            this[index] = item;
            IObjectWithParent op = (oldItem as IObjectWithParent);
            if (op != null) { op.Parent = null; }
                
            op = (item as IObjectWithParent);
            if (op != null)
                op.Parent= this.parent;
            return oldItem;
        }

        public T Remove(int index)
        {
            T item = this[index];
            base.Remove(item);

            IObjectWithParent op = (item is IObjectWithParent) ? (IObjectWithParent)item : null;
            if (op != null)
                op.Parent = null;
            return item;
        }

        public new void Clear()
        {
            foreach (T item in this)
            {
                IObjectWithParent op = (item is IObjectWithParent) ? (IObjectWithParent)item : null;
                if (op != null)
                    op.Parent = null;
            }
            base.Clear();
        }

    }
}
