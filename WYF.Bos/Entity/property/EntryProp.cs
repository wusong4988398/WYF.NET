using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.Entity.validate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    /// <summary>
    /// 单据体
    /// </summary>
    public class EntryProp : DynamicCollectionProperty , IValidatorHanlder
    {

        public EntryProp() { }

        public EntryProp(string name, DynamicObjectType dynamicItemPropertyType):base(name, dynamicItemPropertyType) 
        {
            
        }
        /// <summary>
        /// 分录在新建时缺省的行数
        /// </summary>
        [SimpleProperty]
        [DefaultValue("1")]
        public int DefaultRows { get; set; } = 1;
        /// <summary>
        /// 分录的关键字段，用于表明分录行录入的有效性
        /// </summary>
        [SimpleProperty]
        public string KeyFieldId { get; set; }

        public override bool IsDbIgnore => this.ItemType.IsDbIgnore;
        /// <summary>
        /// 分录是否必录
        /// </summary>
        [SimpleProperty(Name = "EntryMustInput")]
        public bool IsEntryMustInput { get; set; }
    }
}
