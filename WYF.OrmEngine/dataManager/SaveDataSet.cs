using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Collections;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.dataManager
{
    public sealed class SaveDataSet : ISaveDataSet
    {
        public readonly KeyedCollectionBase<string, ISaveDataTable> _tables = new SaveDataTableCollection();

        public KeyedCollectionBase<string, ISaveDataTable> Tables => _tables;

        public void AnalyseRows(PkSnapshotSet pkSnapshotSet)
        {
            if (pkSnapshotSet == null)
            {
                foreach (ISaveDataTable table in Tables)
                {
                    ((SaveDataTable)table).AnalyseRows(null);
                }
            }
            else
            {
                foreach (PkSnapshot pkSnapshot in pkSnapshotSet.Snapshots)
                {
                    if (pkSnapshot != null)
                    {
                        ISaveDataTable table = null;
                        Tables.TryGetValue(pkSnapshot.TableName,out table);
                        if (table != null)
                        {
                            ((SaveDataTable)table).AnalyseRows(pkSnapshot);
                        }
                    }
                }
            }
        }

       
    }
}
