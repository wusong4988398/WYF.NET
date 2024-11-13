using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using WYF.Form.DataEntity;
using WYF.OrmEngine.Drivers;

namespace WYF.OrmEngine.dataManager
{
    public interface IDataManager
    {
        IDataEntityType DataEntityType { get; set; }
        DataEntityTypeMap DataEntityTypeMap { get;  }

        bool IsSelectHeadOnly {  get; set; }

        ISaveDataSet GetSaveDataSet(object[] dataEntities, bool includeDefaultValue);
        object[] Read(object[] ids);
        object Read(object pk);

        object[] Read(ReadWhere where);

        void Save(object dataEntity, IOrmTransaction ormTransaction = null, OperateOption option = null);


    }
    public interface IDataManager<DataT, OidT>
    {

    }
}
