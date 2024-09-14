using WYF.Bos.DataEntity;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal sealed class SaveDataTable : ISaveDataTable
    {
        public SaveDataTable(DbMetadataTable schema, int rowCount)
        {
            this.Schema = schema;
            this.SaveRows = new SaveRow[rowCount];
        }

        public void AnalyseRows(PkSnapshot pkSnapshot)
        {
            int rightNowIndex = 0;
            object OidLeft = null;
            object OidRight = null;
            object OidTemp = null;
            bool isFound = false;
            bool isNullSnapshot = (pkSnapshot == null) || (pkSnapshot.Oids == null);
            bool oidTypeChecked = false;
            for (int i = 0; i < this.SaveRows.Length; i++)
            {
                SaveRow row = this.SaveRows[i] as SaveRow;
                if (!isNullSnapshot)
                {
                    OidLeft = row.Oid.Value;
                    isFound = false;
                    if (OidLeft != null)
                    {
                        for (int j = rightNowIndex; j < pkSnapshot.Oids.Length; j++)
                        {
                            OidRight = pkSnapshot.Oids[j];
                            if (!oidTypeChecked && (OidRight != null))
                            {
                                oidTypeChecked = true;
                                if (OidLeft.GetType() != OidRight.GetType())
                                {
                                    throw new ORMDesignException("??????", string.Format("侦测到表{0}所对应的实体与数据库设计不一致的问题，此表主键类型为{1},而实体中类型为{2}。请修正保持一致", this.Schema.Name, OidRight.GetType().Name, OidLeft.GetType().Name));
                                }

                            }
                            if (object.Equals(OidLeft, OidRight))
                            {
                                OidTemp = pkSnapshot.Oids[rightNowIndex];
                                pkSnapshot.Oids[rightNowIndex] = OidRight;
                                pkSnapshot.Oids[j] = OidTemp;
                                rightNowIndex++;
                                isFound = true;
                                break;
                            }
                        }
                    }
                }
                if (isFound)
                {
                    if (row.DirtyValues.Count > 0)
                    {
                        row.Operate = RowOperateType.Update;
                        row.RebuildOutputValues();
                    }
                    else
                    {
                        row.Operate = RowOperateType.None;
                    }
                }
                else
                {
                    row.Operate = RowOperateType.Insert;
                    if (row.DirtyValues.Count == 0)
                    {
                        row.DirtyValues.Add(row.Oid);
                    }
                    if (this.Schema.ParentRelation != null)
                    {
                        row.DirtyValues.Add(new ParentOidColumnValuePair(this.Schema.ParentRelation.ChildColumn, null, null, row.ParentOid));
                    }
                    row.RebuildOutputValues();
                }
            }
            if (!isNullSnapshot)
            {
                int num4 = pkSnapshot.Oids.Length - rightNowIndex;
                if (num4 > 0)
                {
                    this.DeleteRows = new DeleteRow[num4];
                    int num5 = 0;
                    for (int k = rightNowIndex; k < pkSnapshot.Oids.Length; k++)
                    {
                        DeleteRow row2 = new DeleteRow
                        {
                            Oid = pkSnapshot.Oids[k]
                        };
                        this.DeleteRows[num5] = row2;
                        num5++;
                    }
                }
            }
        }




        public IDeleteMetaRow[] DeleteRows { get; private set; }

        public ISaveMetaRow[] SaveRows { get; private set; }

        public DbMetadataTable Schema { get; private set; }
    }
}
