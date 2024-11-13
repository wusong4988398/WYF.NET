using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;

namespace WYF.OrmEngine.dataManager
{
    public class SaveDataTable : ISaveDataTable
    {
        private Dictionary<object, EntryInfo> mapEntryInfo;
        private Dictionary<object, int> changeRowCount = new Dictionary<object, int>();
        public ISaveMetaRow[] SaveRows { get; set; }
        public IDeleteMetaRow[] DeleteRows { get; set; }

        public DbMetadataTable Schema { get; set; }

        public List<Tuple<object, object, int>> ChangeRows
        {
            get
            {
                if (changeRowCount.Count == 0) return null;
                List<Tuple<Object, Object, int>> retChangeRows = new List<Tuple<object, object, int>>(changeRowCount.Count);
                foreach (KeyValuePair<object, int> crow in changeRowCount)
                {
                    EntryInfo info = mapEntryInfo.ContainsKey(crow.Key) ? mapEntryInfo[crow.Key] : null;
                    if (info != null &&
                        info.StartRowIndex + info.PageSize - crow.Value <= info.RowCount - crow.Value &&
                        crow.Value != 0)
                    {
                        retChangeRows.Add(new Tuple<object, object, int>(
                            crow.Key,
                            info.StartRowIndex + info.PageSize - crow.Value,
                            crow.Value));
                    }
                }

                return retChangeRows;



            }
        }

        private IColumnValuePair parentOid;

        public SaveDataTable(DbMetadataTable schema, int rowCount)
        {
            this.Schema = schema;
            this.SaveRows = new ISaveMetaRow[rowCount];

        }

        public void SetEntryInfo(Dictionary<object, EntryInfo> mapEntryInfo)
        {
            this.mapEntryInfo = mapEntryInfo;
        }

        public void AnalyseRows(PkSnapshot pkSnapshot)
        {
            int rightNowIndex = 0;
            object OidLeft = null;
            object OidRight = null;
            object OidTemp = null;
            bool isFound = false;
            bool isNullSnapshot = pkSnapshot == null || pkSnapshot.Oids == null;
            bool oidTypeChecked = false;

            foreach (ISaveMetaRow row in SaveRows)
            {
                SaveRow curRow = (SaveRow)row;
                if (!isNullSnapshot)
                {
                    OidLeft = curRow.Oid;
                    isFound = false;
                    if (OidLeft != null)
                    {
                        for (int j = rightNowIndex; j < pkSnapshot.Oids.Length; j++)
                        {
                            OidRight = pkSnapshot.Oids[j];
                            if (!oidTypeChecked && OidRight != null)
                            {
                                oidTypeChecked = true;
                                if (OidLeft.GetType() != OidRight.GetType())
                                {
                                    throw new Exception($"类型不匹配：{Schema.Name} 应该对应数据库中的一列，当前类型为 {OidRight.GetType().Name}, 实际类型为 {OidLeft.GetType().Name}，类型不一致");
                                }
                            }
                            if (OidLeft.Equals(OidRight))
                            {
                                OidTemp = pkSnapshot.Oids[rightNowIndex];
                                pkSnapshot.Oids[rightNowIndex] = OidRight;
                                pkSnapshot.Oids[j] = OidTemp;
                                if (pkSnapshot.Opids != null)
                                {
                                    object oprId = pkSnapshot.Opids[j];
                                    object OpidTemp = pkSnapshot.Opids[rightNowIndex];
                                    pkSnapshot.Opids[rightNowIndex] = oprId;
                                    pkSnapshot.Opids[j] = OpidTemp;
                                }
                                rightNowIndex++;
                                isFound = true;
                                break;
                            }
                        }
                    }
                }

                if (isFound)
                {
                    if (curRow.DirtyValues.Count > 0)
                    {
                        curRow.Operate = RowOperateType.Update;
                        curRow.RebuildOutputValues();
                    }
                    else
                    {
                        curRow.Operate = RowOperateType.None;
                    }
                }
                else
                {
                    curRow.Operate = RowOperateType.Insert;
                    if (curRow.DirtyValues.Count == 0)
                    {
                        curRow.DirtyValues.Add(curRow.Oid);
                    }
                    if (Schema.ParentRelation != null)
                    {
                        curRow.DirtyValues.Add(new ParentOidColumnValuePair(Schema.ParentRelation.ChildColumn, null, null, curRow.ParentOid));
                        if (mapEntryInfo != null)
                        {
                            object parentPk = curRow.ParentOid;
                            UpdateChangeRows(parentPk, 1);
                        }
                    }
                    curRow.RebuildOutputValues();
                }
            }

            if (!isNullSnapshot)
            {
                int delRowCount = pkSnapshot.Oids.Length - rightNowIndex;
                if (delRowCount > 0)
                {
                    this.DeleteRows=new DeleteRow[delRowCount];
                    int k = 0;
                    for (int i = rightNowIndex; i < pkSnapshot.Oids.Length; i++)
                    {
                        DeleteRow tempVar2 = new DeleteRow();
                        tempVar2.Oid = pkSnapshot.Oids[i];
                        this.DeleteRows[k] = tempVar2;
                        k++;
                    }
                    if (mapEntryInfo != null)
                    {
                        if (pkSnapshot.Opids != null)
                        {
                            for (int i = rightNowIndex; i < pkSnapshot.Opids.Length; i++)
                            {
                                object parentPk = pkSnapshot.Opids[i];
                                UpdateChangeRows(parentPk, -1);
                            }
                        }
                        else if (parentOid != null)
                        {
                            int len = rightNowIndex - pkSnapshot.Oids.Length;
                            if (len < 0)
                            {
                                UpdateChangeRows(parentOid.Value, len);
                            }
                        }
                    }
                }
            }
        }

        public List<Tuple<object, object, int>> GetChangeRows()
        {
            if (changeRowCount.Count == 0)
            {
                return null;
            }
            List<Tuple<object, object, int>> retChangeRows = new List<Tuple<object, object, int>>(changeRowCount.Count);
            foreach (KeyValuePair<object, int> crow in changeRowCount)
            {
                EntryInfo info = mapEntryInfo?.GetValueOrDefault(crow.Key);
                if (info != null &&
                    info.StartRowIndex + info.PageSize - crow.Value <= info.RowCount - crow.Value &&
                    crow.Value != 0)
                {
                    retChangeRows.Add(new Tuple<object, object, int>(crow.Key,
                        info.StartRowIndex + info.PageSize - crow.Value, crow.Value));
                }
            }
            return retChangeRows;
        }

        private void UpdateChangeRows(object parentPk, int len)
        {
            if (changeRowCount.TryGetValue(parentPk, out int changeRows))
            {
                changeRows += len;
                changeRowCount[parentPk] = changeRows;
            }
            else
            {
                changeRowCount[parentPk] = len;
            }
        }

   

   

  

        public void SetParentOid(IColumnValuePair parentOid)
        {
            this.parentOid = parentOid;
        }

        public override string ToString()
        {
            return Schema.Name;
        }
    }
}
