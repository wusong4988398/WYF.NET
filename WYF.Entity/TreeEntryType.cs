using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Entity.Property;

namespace WYF.Entity
{
    public class TreeEntryType: EntryType
    {
        public override void EndInit()
        {
            base.EndInit();
            if (this.Properties["isgroupnode"] == null)
            {
                BooleanProp groupProp = new BooleanProp();
                groupProp.Name = "isgroupnode";
                groupProp.IsDbIgnore = true;
                RegisterSimpleProperty((DynamicSimpleProperty)groupProp);
            }
        }
    }
}
