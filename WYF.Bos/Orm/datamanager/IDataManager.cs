using WYF.Bos.DataEntity.Metadata.database;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.Orm.Drivers;
using JNPF.Form.DataEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.DataManager
{
    public interface IDataManager
    {
        object[] Read(object[] ids);

        object[] Read(ReadWhere where);

        void SetDataEntityType(IDataEntityType dataEntityType);
    }

    public interface IDataManager<DataT, OidT>
    {

    }
}
