using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Entity.Property
{
    /// <summary>
    /// 行政区划控件
    /// </summary>
    public class AdminDivisionProp: FieldProp
    {
        public override int DbType { get => 12; set => base.DbType = value; }
        public override Type PropertyType => typeof(string);


    }
}
