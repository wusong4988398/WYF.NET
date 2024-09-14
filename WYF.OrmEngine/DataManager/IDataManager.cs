using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.DataManager
{
    public interface IDataManager
    {
        object[] Read(object[] ids);

        object[] Read(ReadWhere where);

        void Save(Object paramObject);

        void SetDataEntityType(IDataEntityType dataEntityType);
    }
    public interface IDataManager<DataT, OidT>
    {

    }
}
