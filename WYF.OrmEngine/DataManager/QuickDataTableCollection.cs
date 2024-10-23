using WYF.DataEntity.Collections;

namespace WYF.OrmEngine.dataManager
{
    public class QuickDataTableCollection : KeyedCollectionBase<string, QuickDataTable>
    {


        protected override string GetKeyForItem(QuickDataTable item)
        {
            return item.Schema.Name;
        }
    }
}