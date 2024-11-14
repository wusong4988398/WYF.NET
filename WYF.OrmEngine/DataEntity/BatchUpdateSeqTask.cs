using System.Data;
using WYF.DataEntity.Metadata.database;
using WYF.DbEngine;
using WYF.DbEngine.db;
using WYF.OrmEngine.Drivers;

namespace WYF.OrmEngine.DataEntity
{
    public class BatchUpdateSeqTask : IDatabaseTask
    {
        private DBRoute dbRoute;
        private DbMetadataTable table;
        private List<Tuple<object, object, int>> changeRows;

        public BatchUpdateSeqTask(DBRoute dbRoute, DbMetadataTable table, List<Tuple<object, object, int>> changeRows)
        {
            this.dbRoute = dbRoute;
            this.table = table;
            this.changeRows = changeRows;
        }

        public int Execute()
        {
            DbMetadataColumn pcol = this.table.ParentRelation.ChildColumn;
            DbMetadataColumn seqCol = this.table.Seq;
            string sql = $"UPDATE {this.table.Name} SET {seqCol.Name} = {seqCol.Name} + @delta WHERE {pcol.Name} = @parentId AND {seqCol.Name} > @minSeq";

            List<object[]> listParas = new List<object[]>(this.changeRows.Count);
            foreach (Tuple<object, object, int> row in this.changeRows)
            {
                SqlParam[] parameters = new SqlParam[3];
                parameters[0] = new SqlParam("@" + seqCol.Name, (KDbType)seqCol.DbType, row.Item3);
                parameters[1] = new SqlParam("@" + pcol.Name, (KDbType)pcol.DbType, row.Item1);
                parameters[2] = new SqlParam("@"+ seqCol.Name, (KDbType)seqCol.DbType, row.Item2);
                listParas.Add(parameters);
            }

            DB.ExecuteBatch(this.dbRoute, "/*ORM*/ " + sql, listParas);
            return 0;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Execute(IDbConnection con, IDbTransaction tran)
        {
            throw new NotImplementedException();
        }

        public int Level => -1;
        

    }
}
