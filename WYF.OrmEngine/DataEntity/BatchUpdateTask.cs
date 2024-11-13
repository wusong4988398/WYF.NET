using System.Text;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine;
using WYF.DbEngine.db;

namespace WYF.OrmEngine.DataEntity
{
    // 批量更新任务类
    public class BatchUpdateTask : BatchInsertTask
    {
        private DbMetadataColumn PrimaryKey;
        private List<object> Ids;
        private List<ISaveMetaRow> SaveRows;

        public BatchUpdateTask(DBRoute dbRoute, string tableNameWithGroup, DbMetadataColumn[] columns, List<object[]> rows, DbMetadataColumn primaryKey, List<object> ids, List<ISaveMetaRow> saveRows, int level)
            : base(dbRoute, tableNameWithGroup, columns, level, rows)
        {
            PrimaryKey = primaryKey;
            Ids = ids;
            SaveRows = saveRows;
        }

        public new int Execute()
        {
            StringBuilder fields = new StringBuilder();
            foreach (DbMetadataColumn column in Columns)
                fields.Append(column.Name).Append("=@").Append(column.Name).Append(",");
            fields.Remove(fields.Length - 1, 1); // 删除最后一个逗号
            string sql = $"UPDATE {TableName} SET {fields} WHERE {PrimaryKey.Name}=@{PrimaryKey.Name}";

            List<object[]> listParas = new List<object[]>(Rows.Count);
            int j = 0;
            foreach (object[] row in Rows)
            {
                SqlParameter[] parameters = new SqlParameter[Columns.Length + 1];
                int i = 0;
                foreach (DbMetadataColumn column in Columns)
                {
                    parameters[i] = new SqlParameter($"@{column.Name}", column.DbType, row[column.ColumnIndex]);
                    i++;
                }
                parameters[i] = new SqlParameter($"@{PrimaryKey.Name}", PrimaryKey.DbType, Ids[j]);
                listParas.Add(parameters);
                j++;
            }

             DB.ExecuteBatch(DbRoute, "/*ORM*/ " + sql, listParas);


            return Rows.Count;
        }


        public int Level { get; private set; } = 0;
    }

}