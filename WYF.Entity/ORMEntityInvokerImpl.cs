using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Entity.Property;
using WYF.OrmEngine.Impl;

namespace WYF.Entity
{
    public class ORMEntityInvokerImpl : IORMEntityInvoker
    {
        public string GetBaseDataEntityName(IComplexProperty baseDataProp)
        {
            string name = ((BasedataProp)baseDataProp).BaseEntityId;
            if (name == null)
                name = ((BasedataProp)baseDataProp).ComplexType.Name;
            return name;
        }

        public DynamicObjectType GetDataEntityType(string entityName)
        {
            return EntityMetadataCache.GetDataEntityType(entityName);
        }

        public IDataEntityType GetMulBasedataPropDataEntityType(IDataEntityProperty dataEntityProperty)
        {
            throw new NotImplementedException();
        }
    }
}
