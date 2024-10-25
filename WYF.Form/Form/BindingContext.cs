using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Form
{
    /// <summary>
    /// 绑定上下文
    /// </summary>
    public class BindingContext
    {
        /// <summary>
        /// 当前处理的数据包
        /// </summary>
        public Object DataEntity { get; set; }

        /// <summary>
        /// 当前处理的单据体分录行索引
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 当前处理的字段父实体
        /// </summary>
        public IDataEntityType EntityEntityType { get; set; }


        public BindingContext(Object dataEntity):this(dataEntity, 0)
        {
            
        }

        public BindingContext(Object dataEntity, int rowIndex)
        {
            this.DataEntity = dataEntity;
            this.RowIndex = rowIndex;
        }

        public BindingContext(IDataEntityType entityType, DynamicObject dynamicObject, int rowIndex2) : this(dynamicObject, rowIndex2)
        {
            this.EntityEntityType = entityType;
     
        }

    }
}
