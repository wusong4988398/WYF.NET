using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.Entity.DataModel;

namespace WYF.Entity.Property
{
    public class CreateDateProp : DateTimeProp
    {
        /// <summary>
        /// 获取字段默认值，将默认值填充到字段中
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataEntity"></param>
        /// <param name="rowIndex"></param>
        public override void ApplyDefaultValue(IDataModel model, DynamicObject dataEntity, int rowIndex)
        {
            DateTime now = DateTime.Now;
            model.SetValue((IDataEntityProperty)this, dataEntity, now);
        }
    }
}
