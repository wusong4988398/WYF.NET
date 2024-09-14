using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;

namespace WYF.OrmEngine.Impl
{
    public interface IORMEntityInvoker
    {
        DynamicObjectType GetDataEntityType(string entityName);

        string GetBaseDataEntityName(IComplexProperty baseDataProp);
        IDataEntityType GetMulBasedataPropDataEntityType(IDataEntityProperty dataEntityProperty);
    }
}
