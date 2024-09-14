using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Entity.Property
{
    /// <summary>
    /// 文本型主键字段
    /// </summary>
    public class VarcharProp : PKFieldProp
    {
        public VarcharProp()
        {
            this.DefaultValue = "";
            this.CompareGroupID = "0,1,2,3";
            this.DefaultCompareTypeId = "67";
            this.DefaultMultiCompareTypeId = "17";
        }
        public VarcharProp(bool refId) : base(refId)
        {

            this.DefaultValue = "";
        }
        public override int DbType { get => 12; }

        public override Type PropertyType => typeof(string);
    }
}
