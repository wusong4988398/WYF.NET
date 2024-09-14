using WYF.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    /// <summary>
    /// 多类别基础资料字段
    /// </summary>
    public class ItemClassProp: BasedataProp
    {
        public static readonly string NumberPropName = "number";
  
        public static readonly string NamePropName = "name";
        /// <summary>
        /// 基础资料类型名称
        /// </summary>
        [SimpleProperty]
        public string TypePropName {  get; set; }

        public override Type PropertyType => typeof(DynamicObject);
         
    }
}
