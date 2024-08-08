using WYF.Bos.Context;
using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.Entity.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    public class CreaterProp:UserProp
    {
        public override void ApplyDefaultValue(IDataModel model, DynamicObject dataEntity, int rowIndex)
        {
            DynamicObject obj = model.LoadReferenceData((DynamicObjectType)this.ComplexType, RequestContext.Get().UserId);
            if (obj != null)
                model.SetValue((IDataEntityProperty)this, dataEntity, obj);

        }
    }
}
