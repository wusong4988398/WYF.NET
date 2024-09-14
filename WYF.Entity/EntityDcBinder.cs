using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.DataEntity.Serialization;
using WYF.DataEntity.Utils;

namespace WYF.Entity
{
    public class EntityDcBinder : DcBinder
    {
        public override void ReadCustomJsonProperty(KeyValuePair<string, object> pair, object entity)
        {
            if (entity is DynamicProperty)
            {
                ((DynamicProperty)entity).CustomProperties[pair.Key] = pair.Value;
            }
        }

        public override IDataEntityType TryBindToType(string elementName, Dictionary<string, string> attributes)
        {
            IDataEntityType ret = EntityItemTypes.GetDataEntityType(elementName);
            if (ret == null && elementName.Contains("."))
            {
                Type type = TypesContainer.GetOrRegister(elementName);
                if (type != null)
                {
                    ret = OrmUtils.GetDataEntityType(type);
                }
            }

            return ret;
        }
    }
}
