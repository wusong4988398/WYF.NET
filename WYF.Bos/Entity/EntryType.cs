using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity
{
    /// <summary>
    /// 分录实体类型
    /// </summary>
    public class EntryType : EntityType, IEntryType
    {
        /// <summary>
        /// 关键字段
        /// </summary>
        [SimpleProperty]
        public string KeyField {  get; set; }
        /// <summary>
        /// 是否关键实体
        /// </summary>
        [SimpleProperty(Name = "KeyEntry")]
        public bool IsKeyEntry { get; set; }
        /// <summary>
        /// 是否必须
        /// </summary>
        [SimpleProperty(Name = "MustIuput")]
        public bool IsMustIuput { get; set; }
        /// <summary>
        /// 是否在运行期以来父表单的属性
        /// </summary>
        public bool RefParentProperty { get; set; }

        /// <summary>
        /// Seq属性
        /// </summary>
        public IDataEntityProperty SeqProperty => this.Properties["seq"];

        public override void EndInit()
        {
            base.EndInit();

        }


    }
}
