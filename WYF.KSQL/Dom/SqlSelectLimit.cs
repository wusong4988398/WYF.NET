using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlSelectLimit : SqlObject
    {
        // Fields
        public int offset;
        public int type;
        public int value;

        // Methods
        public SqlSelectLimit(int value)
        {
            this.value = -1;
            this.value = value;
            this.type = 0;
        }

        public SqlSelectLimit(int value, int type)
        {
            this.value = -1;
            this.value = value;
            this.type = type;
        }

        public SqlSelectLimit(int value, int type, int offset)
        {
            this.value = -1;
            this.value = value;
            this.type = type;
            this.offset = offset;
        }

        public object clone()
        {
            return new SqlSelectLimit(this.value, this.type, this.offset);
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }






}
