using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal sealed class SaveDataSet : ISaveDataSet
    {
    
        public readonly KeyedCollection<string, ISaveDataTable> _tables = new SaveDataTableCollection();


        public void AnalyseRows(PkSnapshotSet pkSnapshotSet)
        {
            if (pkSnapshotSet == null)
            {
                foreach (ISaveDataTable table in this.Tables)
                {
                    (table as SaveDataTable).AnalyseRows(null);
                }
            }
            else
            {
                foreach (PkSnapshot snapshot in pkSnapshotSet.Snapshots)
                {
                    if ((snapshot != null) && this.Tables.Contains(snapshot.TableName))
                    {
                        ((SaveDataTable)this.Tables[snapshot.TableName]).AnalyseRows(snapshot);
                    }
                }
            }
        }

 
        public KeyedCollection<string, ISaveDataTable> Tables
        {
            get
            {
                return this._tables;
            }
        }
    }

}
