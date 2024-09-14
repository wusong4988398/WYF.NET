using WYF.DataEntity.Entity;
using WYF.Bos.Entity.validate;
using WYF.Bos.fulltext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    public  class TextProp: FieldProp, IScopeCheck
    {
        private string _dataScopeMessage;
        public int MaxLenth { get; set; }

        public int MinLenth { get; set; }

        public override  string ClientType { 
            get
            {
                return "text";
            }
        }
        [SimpleProperty(Name = "Password")]
        public bool IsPassword { get; set; }
  

        public override Type PropertyType { get { return typeof(string); } }

        public override int DbType { get { return -9; } }

        public bool CheckScope(object value)
        {
            int valueLength = 0;
            if (value is string)
            valueLength = ((string)value).Trim().Length;
            if (valueLength < this.MinLenth || (this.MaxLenth > 0 && valueLength > this.MaxLenth))
                return false;
            return true;
        }

        public  string GetDataScopeMessage(object fldValue)
        {
            _dataScopeMessage = $"字段“{(string.IsNullOrEmpty(this.DisplayName)?this.Name:this.DisplayName)}”输入长度超出限定范围[{this.MinLenth},{this.MaxLenth}]";

            return this._dataScopeMessage;
        }
    }
}
