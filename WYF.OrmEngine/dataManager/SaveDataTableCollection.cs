using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Collections;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.dataManager
{
    public sealed class SaveDataTableCollection : KeyedCollectionBase<string, ISaveDataTable>
    {
        protected override string GetKeyForItem(ISaveDataTable item)
        {
            return item.Schema.Name;
        }
    }
}
