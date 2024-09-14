using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.Entity.property;
using WYF.Bos.Orm.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.query
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
