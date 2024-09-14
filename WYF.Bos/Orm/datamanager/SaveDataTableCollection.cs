using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal sealed class SaveDataTableCollection : KeyedCollection<string, ISaveDataTable>
    {
        protected override string GetKeyForItem(ISaveDataTable item)
        {
            return (item as SaveDataTable).Schema.Name;
        }
    }
}
