using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Dynamicobject;

namespace WYF.Entity.DataModel
{
    public interface IDataProvider
    {
        /// <summary>
        /// 载入关联数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="pkValue"></param>
        /// <returns></returns>
        DynamicObject LoadReferenceData(DynamicObjectType dt, object pkValue);
        /// <summary>
        /// 载入批量的关联数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        Dictionary<object, DynamicObject> LoadReferenceDataBatch(DynamicObjectType dt, object[] pkValues);
    }
}
