using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Entity.DataModel;

namespace WYF.Entity.Property
{
    public class CreaterProp : UserProp
    {
        public override void ApplyDefaultValue(IDataModel model, DynamicObject dataEntity, int rowIndex)
        {
            DynamicObject obj = model.LoadReferenceData((DynamicObjectType)this.ComplexType, RequestContext.Get().UserId);
            if (obj != null)
                model.SetValue((IDataEntityProperty)this, dataEntity, obj);

        }
    }
}
