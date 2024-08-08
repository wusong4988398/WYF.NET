using WYF.Bos.DataEntity.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    public class QuickDataTableCollection : KeyedCollectionBase<string, QuickDataTable>
    {
    

        protected override string GetKeyForItem(QuickDataTable item)
        {
            return item.Schema.Name;
        }
    }
}
