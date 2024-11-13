using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;
using WYF.Form.DataEntity;
using WYF.OrmEngine.dataManager;

namespace WYF.OrmEngine.Drivers
{
    public abstract class DbDriverBase : IDbDriver
    {
        // Fields
        private static ObjectCache<DbMetadataTable, string> _selectSqlCache = ObjectCache<DbMetadataTable, string>.Create();
        private const char QuotationMarks = '"';
        private const char SingleQuotationMarks = '\'';
        private const string sNoDataFilter = " 0=1 ";

        // Methods
        protected DbDriverBase()
        {
        }

        public abstract IOrmTransaction BeginTransaction(IDbTransaction dbTransaction = null);
        protected abstract void ExecuteReader(SelectCallback callback, IEnumerable<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, IList<StringBuilder> selectSqls, ReadWhere where, OperateOption option);
        private static string GetIdsWhereSql(object[] ids, DbMetadataTable rootTable)
        {
            if ((ids == null) || (ids.Length == 0))
            {
                return " 0=1 ";
            }
            int index = 0;
            while (index < ids.Length)
            {
                if (ids[index] != null)
                {
                    break;
                }
                index++;
            }
            if (index == ids.Length)
            {
                return " 0=1 ";
            }
            string str = "2";
            object obj1 = ids[index];
            switch ((DbType)rootTable.PrimaryKey.DbType)
            {
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    str = "1";
                    break;

                case DbType.String:
                    str = "3";
                    break;

                case DbType.AnsiString:
                    str = "2";
                    break;
            }
            string str2 = "[" + rootTable.Name + "].[" + rootTable.PrimaryKey.Name + "] ";
            return string.Format(" {0} in (select  fid from  table(fn_StrSplit(@PKValue, ','," + str + ")) )", str2).ToString();
        }

        private static string GetSafeString(string str)
        {
            if (str == null)
            {
                return str;
            }
            char[] chArray = str.ToCharArray();
            bool flag = true;
            for (int i = 0; i < chArray.Length; i++)
            {
                char ch = chArray[i];
                if (ch == '\'')
                {
                    chArray[i] = '"';
                    flag = false;
                }
            }
            if (flag)
            {
                return str;
            }
            return new string(chArray);
        }

        protected virtual string GetSelectSQL(DbMetadataTable tableSchema)
        {
            
            return tableSchema.GetSelectSQL(false);
        }

        public void Select(SelectCallback callback, IEnumerable<DbMetadataTable> tablesSchema, DbMetadataTable rootTable, ReadWhere where, OperateOption option = null)
        {
            int count;
            Func<DbMetadataTable, string> valueFactory = null;
            ICollection<DbMetadataTable> is2 = tablesSchema as ICollection<DbMetadataTable>;
            if (is2 != null)
            {
                count = is2.Count;
            }
            else
            {
                count = 0x10;
            }
            List<StringBuilder> selectSqls = new List<StringBuilder>(count);
            StringBuilder builder2 = new StringBuilder();
            if (where.IsSingleValue)
            {
                builder2.Append(DriverHelper.GetColumnNameSql(rootTable, rootTable.PrimaryKey) + "= @PKValue");
            }
            else if (where.ReadOids != null)
            {
                builder2.Append(GetIdsWhereSql(where.ReadOids, rootTable));
            }
            if (!string.IsNullOrWhiteSpace(where.WhereSql))
            {
                if (builder2.Length > 0)
                {
                    builder2.Append(" AND ");
                }
                builder2.Append(where.WhereSql);
            }
            bool cacheMetadata = option.GetCacheMetadata();
            foreach (DbMetadataTable table in tablesSchema)
            {
                StringBuilder builder;
                if (cacheMetadata)
                {
                    if (valueFactory == null)
                    {
                        valueFactory = key => this.GetSelectSQL(key);
                    }
                    builder = new StringBuilder(_selectSqlCache.GetOrAdd(table, valueFactory));
                }
                else
                {
                    builder = new StringBuilder(this.GetSelectSQL(table));
                }
                if (builder2.Length > 0)
                {
                    builder.Append(" WHERE ");
                    builder.Append(builder2);
                }
                selectSqls.Add(builder);
            }
            this.ExecuteReader(callback, tablesSchema, rootTable, selectSqls, where, option);
        }

        public virtual void UpdateMetadata(DbMetadataDatabase dbMetadata, OperateOption option = null)
        {
            throw new NotImplementedException();
        }

  
        public virtual string ConnectionString { get; set; }
    }
}
