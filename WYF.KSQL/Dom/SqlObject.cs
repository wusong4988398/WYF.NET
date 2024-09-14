using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public abstract class SqlObject : ICloneable
    {
        // Fields
        public int col;
        private Hashtable extAttr;
        public int line;

        // Methods
        public SqlObject()
        {
        }

        public SqlObject(int line, int col)
        {
            this.line = line;
            this.col = col;
        }

        public void addExtAttr(object key, object value)
        {
            if (this.extAttr == null)
            {
                lock (this)
                {
                    if (this.extAttr == null)
                    {
                        this.extAttr = new Hashtable();
                    }
                }
            }
            this.extAttr.Add(key, value);
        }

        public abstract object Clone();
        public Hashtable extendedAttributes()
        {
            if (this.extAttr == null)
            {
                lock (this)
                {
                    if (this.extAttr == null)
                    {
                        this.extAttr = new Hashtable();
                    }
                }
            }
            return this.extAttr;
        }

        public virtual void output(StringBuilder buffer, string prefix)
        {
        }
    }





}
