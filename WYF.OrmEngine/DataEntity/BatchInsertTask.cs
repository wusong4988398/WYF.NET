using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine;
using WYF.DbEngine.db;
using WYF.OrmEngine.Drivers;

namespace WYF.OrmEngine.DataEntity
{
    // 批量插入任务类
    public class BatchInsertTask : IDatabaseTask
    {
        protected string TableName;
        protected List<object[]> Rows;

        protected DbMetadataColumn[] Columns;
        protected DBRoute DbRoute;

        public BatchInsertTask(DBRoute dbRoute, string tableName, DbMetadataColumn[] columns, int level, List<object[]> rows)
        {
            DbRoute = dbRoute;
            TableName = tableName;
            Rows = rows;
            Level = level;
            Columns = columns;
        }

        public int Execute()
        {
            int cols = Columns.Length;
            StringBuilder fields = new StringBuilder(cols * 10);
            StringBuilder parList = new StringBuilder(cols * 2);

            // 构建字段和参数列表
            foreach (DbMetadataColumn column in Columns)
            {
                fields.Append(column.Name).Append(',');
                parList.Append("?,");
            }
            fields.Length--;
            parList.Length--;
            // 构建SQL语句
            string sql = "/*ORM*/ INSERT INTO " + TableName + "(" + fields + ") VALUES (" + parList + ")";
            List<object[]> psList = new List<object[]>(Rows.Count);

            // 构建参数列表
            foreach (object[] row in Rows)
            {
                SqlParameter[] parameters = new SqlParameter[cols];
                int i = 0;
                foreach (DbMetadataColumn column in Columns)
                {
                    parameters[i++] = new SqlParameter(column.Name, column.DbType, row[column.ColumnIndex]);
                }
                psList.Add(parameters);
            }

            // 执行批量插入
            DB.ExecuteBatch(DbRoute, sql, psList);
            //using (DtsThreadContext dtsContext = DtsThreadContext.Create())
            //{

            //}

            // 处理数据同步
            //if (DtsAccountPower.IsAccountDtsEnable() && DataSyncAgent.CheckTable(TableName))
            //{
            //    DbMetadataTable table = Columns[0].Table;
            //    //bool isLocal = table.IsLocale;
            //    IDataEntityType det = table.DataEntityTypeMap.DataEntityType;
            //    string entityNumber = det.Name;
            //    string pk = det.PrimaryKey.Alias;
            //    string mainPkName = det.PrimaryKey.Name;
            //    string parentPk = null;

            //    //if (isLocal && det.Parent != null)
            //    //{
            //    //    entityNumber = det.Parent.Name;
            //    //    mainPkName = det.Parent.PrimaryKey.Name;
            //    //    parentPk = table.ParentRelation.ChildColumn.Name;
            //    //}

            //    InsertDataSyncValue value = new InsertDataSyncValue(TableName, entityNumber, isLocal);
            //    value.SetColumnsLs(Columns, pk, parentPk);
            //    value.SetRows(Rows);
            //    value.SetMainPKName(mainPkName);
            //    DataSyncAgent.Instance.Send(DbRoute, OperationType.INSERT, value);
            //}

            return Rows.Count;
        }

   

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Execute(IDbConnection con, IDbTransaction tran)
        {
            throw new NotImplementedException();
        }

        public int Level { get;  set; }


    }

}
