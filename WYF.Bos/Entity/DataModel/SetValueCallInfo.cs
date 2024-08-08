using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using JNPF.Form.DataEntity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel
{
    internal struct SetValueCallInfo
    {
        public DynamicObject Row;
        public DynamicProperty Field;
        private int? _hashCode;
        public SetValueCallInfo(DynamicObject row, DynamicProperty field)
        {
            this.Row = row;
            this.Field = field;
            this._hashCode = 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SetValueCallInfo))
            {
                return false;
            }
            SetValueCallInfo info = (SetValueCallInfo)obj;
            return (object.ReferenceEquals(this.Row, info.Row) && object.ReferenceEquals(this.Field, info.Field));
        }

        public override int GetHashCode()
        {
            if (!this._hashCode.HasValue)
            {
                this._hashCode = new int?(this.CreateHashCode());
            }
            return this._hashCode.Value;
        }

        private int CreateHashCode()
        {
            if ((this.Row != null) && (this.Field != null))
            {
                return (RuntimeHelpers.GetHashCode(this.Row) ^ RuntimeHelpers.GetHashCode(this.Field));
            }
            return RuntimeHelpers.GetHashCode(this);
        }

        public override string ToString()
        {
            string str;
            if ((this.Row == null) || (this.Field == null))
            {
                return string.Empty;
            }
            object primaryKeyValue = this.Row.GetPrimaryKeyValue(false);
            if (primaryKeyValue == null)
            {
                str = RuntimeHelpers.GetHashCode(this.Row).ToString();
            }
            else
            {
                str = primaryKeyValue.ToString();
                if (str == "0")
                {
                    str = RuntimeHelpers.GetHashCode(this.Row).ToString();
                }
            }
            return string.Format("Row:{0},Field:{1}", str, this.Field.Name);
        }
    }
}
