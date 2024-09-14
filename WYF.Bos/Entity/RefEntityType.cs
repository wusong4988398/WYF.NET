using WYF.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity
{
    /// <summary>
    /// 引用实体
    /// </summary>
    public class RefEntityType : BasedataEntityType
    {
        public void FillRefEntityTypes(Dictionary<string, DynamicObjectType> types)
        {
            FillRefType(this, types);
            this.RefPropTypes = null;
        }
    }
}
