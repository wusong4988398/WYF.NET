using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    /// <summary>
    /// 数据主键字段  表单运行时元数据-实体的主键属性对象
    /// </summary>
    public class PKFieldProp:FieldProp
    {
        public PKFieldProp():this(false)
        {
             
        }

        public PKFieldProp(bool refId)
        {
            this.IsRefId = refId;
            this.Name = "id";
            this.Alias = "FID";
            base.IsPrimaryKey = true;
            this.FilterControlType = "text";
        }

        public bool IsRefId { private get; set; }

        /// <summary>
        /// 主键字段标识
        /// </summary>
        [SimpleProperty]
        [DefaultValue("id")]
        public override string Name 
        { 
            get
            {
                return base.Name;
            } 
        }

        /// <summary>
        /// 物理字段名
        /// </summary>
        [SimpleProperty]
        [DefaultValue("FID")]
        public override string Alias
        {
            get
            {
                return base.Alias;
            }
        }

        /// <summary>
        /// 是否主键字段
        /// </summary>
        [SimpleProperty]
        [DefaultValue("true")]
        public new  bool IsPrimaryKey
        {
            get
            {
                return base.IsPrimaryKey;
            }
        }
    }
}
