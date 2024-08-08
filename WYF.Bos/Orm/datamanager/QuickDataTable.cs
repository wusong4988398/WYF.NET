using WYF.Bos.DataEntity;
using WYF.Bos.DataEntity.Metadata.database;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    public class QuickDataTable
    {
        public QuickRow[] Rows;

        public DbMetadataTable Schema;

        private Dictionary<object, int> _keyIndexs;

        private Dictionary<object, int> _entryRowCount;
        public QuickDataTable(DbMetadataTable schema)
        {
            this.Schema = schema;
        }
        public Dictionary<object, int> EntryRowCount
        {
            get { return _entryRowCount; }
            set { _entryRowCount = value; }
        }

        public int? GetRowIndexByParmaryKey(object key)
        {
            if (this.Rows.Length == 1)
                return 0;

            if (this._keyIndexs == null)
            {
                lock (this)
                {
                    if (this._keyIndexs == null)
                    {
                        Dictionary<object, int> keyIndexs = new Dictionary<object, int>(this.Rows.Length / 2);
                        int pkColumnIndex = this.Schema.PrimaryKey.ColumnIndex;
                        object pkOid = null;
                        try
                        {
                            for (int i = 0; i < this.Rows.Length; i++)
                            {
                                pkOid = this.Rows[i].Values[pkColumnIndex];
                                keyIndexs.Add(pkOid, i);
                            }
                            this._keyIndexs = keyIndexs;
                        }
                        catch (ArgumentException e)
                        {
                            if (pkOid == null)
                                throw new ORMDesignException("100001", string.Format("表{0}中读取出的数据，出现不正确的主键({1})值:{2}，请检查此表是否设置了主键或主键是否是{1}。", this.Schema.Name, this.Schema.PrimaryKey.Name, pkOid), e);
                            throw new ORMDesignException("100001", string.Format("表{0}中读取出的数据，出现重复的主键({1})数据:{2}，请检查此表是否设置了主键或主键是否是{1}。", this.Schema.Name, this.Schema.PrimaryKey.Name, pkOid), e);
                        }
                    }
                }
            }
            return this._keyIndexs.ContainsKey(key) ? this._keyIndexs[key] : (int?)null;
        }

    }
}
