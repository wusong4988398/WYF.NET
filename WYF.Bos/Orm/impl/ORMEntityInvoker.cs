using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.impl
{
    public interface IORMEntityInvoker
    {
        DynamicObjectType GetDataEntityType(string entityName);

        string GetBaseDataEntityName(IComplexProperty baseDataProp);
        IDataEntityType GetMulBasedataPropDataEntityType(IDataEntityProperty dataEntityProperty);
    }
}
