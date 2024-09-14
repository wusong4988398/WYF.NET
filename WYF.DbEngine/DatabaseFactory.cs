

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DbEngine;
using WYF.KSQL;

namespace WYF.DbEngine

{
    public class DatabaseFactory
    {
        private static Dictionary<string, string> dictConnectionString = new Dictionary<string, string>();
        private static readonly object sync = new object();
        public static void ClearConnectionStrings()
        {
            dictConnectionString.Clear();
        }

        public static IDatabase CreateDataBase(Context ctx)
        {
            TransUtil.OleDBDriver = false;
            AbstractDatabase database = null;
            if (ctx.DatabaseType == DatabaseType.MS_SQL_Server)
            {
                database = new SqlDatabase();
            }
            else if (ctx.DatabaseType == DatabaseType.MySQL)
            {
                //database = new MySqlDatabase();
            }
            else if (ctx.DatabaseType == DatabaseType.PostgreSQL)
            {
                //database = new PostgreSqlDatabase();
            }
            else
            {
                //database = new OracleDatabase();
            }
            database.DbType = (int)ctx.DatabaseType;
            database.ConnectionString = GetConnectionString(ctx);
            database.CurContext = ctx;
            return database;
        }

        private static string GetConnectionString(Context ctx)
        {
            if (string.IsNullOrEmpty(ctx.DBId))//我自己加的，因为测试的时候没有DBId.by wusong 0215
            {
                ctx.DBId = "999";
            }
            //if (ctx.DBId== "5bf4565e7edf54")
            //{
            //    ctx.DBId = "999";
            //}
            string databaseConnectionString = null;
            if (dictConnectionString.TryGetValue(ctx.DBId, out databaseConnectionString))
            {
                return databaseConnectionString;
            }
            lock (sync)
            {
                if (!string.IsNullOrEmpty(ctx.SqlConnectionString))
                {
                    dictConnectionString.Add(ctx.DBId, ctx.SqlConnectionString);
                }
                else
                {
                    if (!dictConnectionString.ContainsKey(ctx.DBId))
                    {

                        databaseConnectionString = "Server=.;Database=WYF;User ID=sa;Password=Wsl4988398;max pool size=10000;";
                        return databaseConnectionString;
                    }
                }

                return dictConnectionString[ctx.DBId];
            }
        }
    }
}
