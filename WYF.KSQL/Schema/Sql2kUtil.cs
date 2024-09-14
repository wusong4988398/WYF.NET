using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Formater;
using WYF.KSQL.Parser;


namespace WYF.KSQL.Schema
{
    // Token: 0x020000CF RID: 207
    public class Sql2kUtil
    {
        // Token: 0x060005E9 RID: 1513 RVA: 0x00050544 File Offset: 0x0004E744
        public static string generateSchemaSql(SqlConnection conn, SQLFormater formater)
        {
            IList stmtList = Sql2kUtil.generateCreateTableSqlDom(conn, false);
            formater.AppendString("\n/* create talbe */\n");
            formater.Format(stmtList);
            stmtList = Sql2kUtil.generatePkSqlDom(conn, false);
            formater.AppendString("\n/* create primary key */\n");
            formater.Format(stmtList);
            stmtList = Sql2kUtil.generateUniqueSqlDom(conn, false);
            formater.AppendString("\n/* create unique */\n");
            formater.Format(stmtList);
            stmtList = Sql2kUtil.generateFkSqlDom(conn, false);
            formater.AppendString("\n/* create foreign key */\n");
            formater.Format(stmtList);
            stmtList = Sql2kUtil.generateCheckSqlDom(conn, false);
            formater.AppendString("\n/* create check */\n");
            formater.Format(stmtList);
            stmtList = Sql2kUtil.generateCreateIndexSqlDom(conn, false);
            formater.AppendString("\n/* create index */\n");
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        // Token: 0x060005EA RID: 1514 RVA: 0x000505FC File Offset: 0x0004E7FC
        public static string generateTableDDL(SqlConnection conn, SqlTable table, SQLFormater formater)
        {
            formater.SetBuffer("\n\n\n\n/* create talbe */\n\n");
            SqlStmt stmt = Sql2kUtil.buildCreateTableStmt(table);
            formater.FormatStmt(stmt);
            formater.AppendString("\n\n\n\n/* create primary key */\n\n");
            IList stmtList = Sql2kUtil.buildPKStmt(conn, table, false);
            formater.Format(stmtList);
            formater.AppendString("\n\n\n\n/* create unique */\n\n");
            stmtList = Sql2kUtil.buildUniqueStmt(conn, table, false);
            formater.Format(stmtList);
            formater.AppendString("\n\n\n\n/* create foreign key */\n\n");
            stmtList = Sql2kUtil.buildFkStmt(conn, table, false);
            formater.Format(stmtList);
            formater.AppendString("\n\n\n\n/* create check */\n\n");
            stmtList = Sql2kUtil.buildCheckStmt(conn, table, false);
            formater.Format(stmtList);
            formater.AppendString("\n\n\n\n/* create index */\n\n");
            stmtList = Sql2kUtil.buildCreateIndexStmt(conn, table, false, null);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        // Token: 0x060005EB RID: 1515 RVA: 0x000506B8 File Offset: 0x0004E8B8
        public static string generateDataSql(SqlConnection conn, SQLFormater formater, bool checkExists)
        {
            if (formater == null)
            {
                formater = new MSTransactSQLFormater();
            }
            formater.AppendString("\n\n\n\n/* insert data */\n\n");
            IList stmtList = Sql2kUtil.buildDataStmtSql(conn, checkExists);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        // Token: 0x060005EC RID: 1516 RVA: 0x000506F0 File Offset: 0x0004E8F0
        public static IList generatePkSqlDom(SqlConnection conn, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            ArrayList userTables = Sql2kUtil.getUserTables(conn);
            foreach (object obj in userTables)
            {
                SqlTable table = (SqlTable)obj;
                IList c = Sql2kUtil.buildPKStmt(conn, table, checkExists);
                arrayList.AddRange(c);
            }
            return arrayList;
        }

        // Token: 0x060005ED RID: 1517 RVA: 0x0005073C File Offset: 0x0004E93C
        public static IList generateUniqueSqlDom(SqlConnection conn, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            ArrayList userTables = Sql2kUtil.getUserTables(conn);
            foreach (object obj in userTables)
            {
                SqlTable table = (SqlTable)obj;
                IList c = Sql2kUtil.buildUniqueStmt(conn, table, checkExists);
                arrayList.AddRange(c);
            }
            return arrayList;
        }

        // Token: 0x060005EE RID: 1518 RVA: 0x00050788 File Offset: 0x0004E988
        public static IList generateFkSqlDom(SqlConnection conn, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            ArrayList userTables = Sql2kUtil.getUserTables(conn);
            foreach (object obj in userTables)
            {
                SqlTable table = (SqlTable)obj;
                IList c = Sql2kUtil.buildFkStmt(conn, table, checkExists);
                arrayList.AddRange(c);
            }
            return arrayList;
        }

        // Token: 0x060005EF RID: 1519 RVA: 0x000507D4 File Offset: 0x0004E9D4
        public static IList generateCheckSqlDom(SqlConnection conn, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            ArrayList userTables = Sql2kUtil.getUserTables(conn);
            foreach (object obj in userTables)
            {
                SqlTable table = (SqlTable)obj;
                IList c = Sql2kUtil.buildCheckStmt(conn, table, checkExists);
                arrayList.AddRange(c);
            }
            return arrayList;
        }

        // Token: 0x060005F0 RID: 1520 RVA: 0x00050820 File Offset: 0x0004EA20
        public static IList generateCreateIndexSqlDom(SqlConnection conn, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            ArrayList userTables = Sql2kUtil.getUserTables(conn);
            foreach (object obj in userTables)
            {
                SqlTable table = (SqlTable)obj;
                IList c = Sql2kUtil.buildCreateIndexStmt(conn, table, checkExists, null);
                arrayList.AddRange(c);
            }
            return arrayList;
        }

        // Token: 0x060005F1 RID: 1521 RVA: 0x0005086C File Offset: 0x0004EA6C
        public static IList buildDataStmtSql(SqlConnection conn, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            ArrayList userTables = Sql2kUtil.getUserTables(conn);
            foreach (object obj in userTables)
            {
                SqlTable table = (SqlTable)obj;
                IList c = Sql2kUtil.buildDataStmtSql(conn, table, checkExists);
                arrayList.AddRange(c);
            }
            return arrayList;
        }

        // Token: 0x060005F2 RID: 1522 RVA: 0x000508B8 File Offset: 0x0004EAB8
        public static IList buildDataStmtSql(SqlConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            string commandText = "select * from " + table.name;
            SqlCommand sqlCommand = conn.CreateCommand();
            SqlDataReader sqlDataReader = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                sqlCommand.CommandText = commandText;
                sqlDataReader = sqlCommand.ExecuteReader();
                arrayList.AddRange(KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.AllData, table.name, table.name));
                while (sqlDataReader.Read())
                {
                    IEnumerator enumerator = table.columns.GetEnumerator();
                    SqlInsert sqlInsert = new SqlInsert(table.name);
                    int num = 0;
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        SqlColumn sqlColumn = (SqlColumn)obj;
                        string name = sqlColumn.name;
                        sqlInsert.columnList.Add(new SqlIdentifierExpr(name));
                        if (sqlColumn.dataType.EqualsIgnoreCase("bigint"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                long value = Convert.ToInt64(sqlDataReader[num]);
                                sqlInsert.valueList.Add(SqlExpr.toExpr(value));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("binary"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string value2 = sqlDataReader[num].ToString();
                                sqlInsert.valueList.Add(SqlExpr.toCharExpr(value2));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("bit"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                short value3 = Convert.ToInt16(sqlDataReader[num]);
                                sqlInsert.valueList.Add(SqlExpr.toExpr((int)value3));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("char"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string text = (string)sqlDataReader[num];
                                text = text.Replace("'", "''");
                                sqlInsert.valueList.Add(SqlExpr.toCharExpr(text));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("datetime"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string value4 = sqlDataReader[num].ToString();
                                sqlInsert.valueList.Add(SqlExpr.toDateTimeExpr(value4));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("decimal"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                decimal value5 = (decimal)sqlDataReader[num];
                                sqlInsert.valueList.Add(SqlExpr.toExpr(value5));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("float"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                double value6 = (double)sqlDataReader[num];
                                sqlInsert.valueList.Add(SqlExpr.toExpr(value6));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("image"))
                        {
                            if (sqlDataReader[num] != DBNull.Value)
                            {
                                string value7 = sqlDataReader[num].ToString();
                                sqlInsert.valueList.Add(SqlExpr.toCharExpr(value7));
                            }
                            else
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("int"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                int value8 = (int)sqlDataReader[num];
                                sqlInsert.valueList.Add(SqlExpr.toExpr(value8));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("money"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                decimal value9 = (decimal)sqlDataReader[num];
                                sqlInsert.valueList.Add(SqlExpr.toExpr(value9));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("nchar"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string text2 = (string)sqlDataReader[num];
                                text2 = text2.Replace("'", "''");
                                sqlInsert.valueList.Add(SqlExpr.toNCharExpr(text2));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("ntext"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string text3 = (string)sqlDataReader[num];
                                text3 = text3.Replace("'", "''");
                                sqlInsert.valueList.Add(SqlExpr.toNCharExpr(text3));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("numeric"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                decimal value10 = (decimal)sqlDataReader[num];
                                sqlInsert.valueList.Add(SqlExpr.toExpr(value10));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("nvarchar"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string text4 = (string)sqlDataReader[num];
                                text4 = text4.Replace("'", "''");
                                sqlInsert.valueList.Add(SqlExpr.toNCharExpr(text4));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("real"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                decimal value11 = (decimal)sqlDataReader[num];
                                sqlInsert.valueList.Add(SqlExpr.toExpr(value11));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("smalldatetime"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string value12 = sqlDataReader[num].ToString();
                                sqlInsert.valueList.Add(SqlExpr.toDateTimeExpr(value12));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("smallint"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                short value13 = (short)sqlDataReader[num];
                                sqlInsert.valueList.Add(SqlExpr.toExpr((int)value13));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("smallmoney"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                decimal value14 = (decimal)sqlDataReader[num];
                                sqlInsert.valueList.Add(SqlExpr.toExpr(value14));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("sql_variant"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string text5 = sqlDataReader[num].ToString();
                                text5 = text5.Replace("'", "''");
                                sqlInsert.valueList.Add(SqlExpr.toCharExpr(text5));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("text"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string text6 = sqlDataReader[num].ToString();
                                text6 = text6.Replace("'", "''");
                                sqlInsert.valueList.Add(SqlExpr.toCharExpr(text6));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("timestamp"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string value15 = "1900-01-01";
                                sqlInsert.valueList.Add(SqlExpr.toCharExpr(value15));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("tinyint"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                short value16 = Convert.ToInt16(sqlDataReader[num]);
                                sqlInsert.valueList.Add(SqlExpr.toExpr((int)value16));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("uniqueidentifier"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string value17 = sqlDataReader[num].ToString();
                                sqlInsert.valueList.Add(SqlExpr.toCharExpr(value17));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("varbinary"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string value18 = sqlDataReader[num].ToString();
                                sqlInsert.valueList.Add(SqlExpr.toCharExpr(value18));
                            }
                        }
                        else if (sqlColumn.dataType.EqualsIgnoreCase("varchar"))
                        {
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string text7 = (string)sqlDataReader[num];
                                text7 = text7.Replace("'", "''");
                                sqlInsert.valueList.Add(SqlExpr.toCharExpr(text7));
                            }
                        }
                        else
                        {
                            if (!sqlColumn.dataType.EqualsIgnoreCase("XMLTYPE"))
                            {
                                throw new System.Exception("TODO");
                            }
                            if (sqlDataReader[num] == DBNull.Value)
                            {
                                sqlInsert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string text8 = (string)sqlDataReader[num];
                                text8 = text8.Replace("'", "''");
                                sqlInsert.valueList.Add(SqlExpr.toXmlExpr(text8));
                            }
                        }
                        num++;
                    }
                    arrayList.Add(new SqlInsertStmt(sqlInsert));
                }
            }
            finally
            {
                sqlDataReader.Close();
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
            return arrayList;
        }

        
        public static IList buildPKStmt(SqlConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            string commandText = "exec sp_MStablekeys N'" + table.name + "', null, 6";
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = commandText;
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    int num = Convert.ToInt32(sqlDataReader["cType"]);
                    if (num == 1)
                    {
                        SqlTablePrimaryKey sqlTablePrimaryKey = new SqlTablePrimaryKey();
                        sqlTablePrimaryKey.name = (string)sqlDataReader["cName"];
                        int num2 = (int)sqlDataReader["cColCount"];
                        for (int i = 7; i < 7 + num2; i++)
                        {
                            string text = (string)sqlDataReader[i];
                            if (text == null || text.Length == 0)
                            {
                                break;
                            }
                            sqlTablePrimaryKey.columnList.Add(text);
                        }
                        SqlAlterTableStmt sqlAlterTableStmt = new SqlAlterTableStmt(table.name);
                        sqlAlterTableStmt.item = new SqlAlterTableAddItem(sqlTablePrimaryKey);
                        if (checkExists)
                        {
                            IList objectCheckStmt = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.PrimaryKey, sqlTablePrimaryKey.name, table.name);
                            arrayList.AddRange(objectCheckStmt);
                        }
                        arrayList.Add(sqlAlterTableStmt);
                    }
                }
            }
            finally
            {
                sqlDataReader.Close();
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
            return arrayList;
        }

        // Token: 0x060005F4 RID: 1524 RVA: 0x0005158C File Offset: 0x0004F78C
        public static IList buildUniqueStmt(SqlConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            string commandText = "exec sp_MStablekeys N'" + table.name + "', null, 6";
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = commandText;
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    int num = Convert.ToInt32(sqlDataReader["cType"]);
                    if (num == 2)
                    {
                        SqlTableUnique sqlTableUnique = new SqlTableUnique();
                        sqlTableUnique.name = (string)sqlDataReader["cName"];
                        int num2 = (int)sqlDataReader["cColCount"];
                        for (int i = 7; i < 7 + num2; i++)
                        {
                            string text = (string)sqlDataReader[i];
                            if (text == null || text.Length == 0)
                            {
                                break;
                            }
                            sqlTableUnique.columnList.Add(text);
                        }
                        SqlAlterTableStmt sqlAlterTableStmt = new SqlAlterTableStmt(table.name);
                        sqlAlterTableStmt.item = new SqlAlterTableAddItem(sqlTableUnique);
                        if (checkExists)
                        {
                            IList objectCheckStmt = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Unique, sqlTableUnique.name, table.name);
                            arrayList.AddRange(objectCheckStmt);
                        }
                        arrayList.Add(sqlAlterTableStmt);
                    }
                }
            }
            finally
            {
                sqlDataReader.Close();
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
            return arrayList;
        }

        // Token: 0x060005F5 RID: 1525 RVA: 0x000516E8 File Offset: 0x0004F8E8
        public static IList buildCheckStmt(SqlConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            string commandText = "exec sp_MStablechecks N'" + table.name + "'";
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = commandText;
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    SqlTableCheck sqlTableCheck = new SqlTableCheck();
                    sqlTableCheck.name = (string)sqlDataReader[0];
                    string text = (string)sqlDataReader[1];
                    SqlExprParser sqlExprParser = new SqlExprParser(text);
                    sqlTableCheck.expr = sqlExprParser.expr();
                    SqlAlterTableStmt sqlAlterTableStmt = new SqlAlterTableStmt(table.name);
                    sqlAlterTableStmt.item = new SqlAlterTableAddItem(sqlTableCheck);
                    if (checkExists)
                    {
                        IList objectCheckStmt = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.PrimaryKey, sqlTableCheck.name, table.name);
                        arrayList.AddRange(objectCheckStmt);
                    }
                    arrayList.Add(sqlAlterTableStmt);
                }
            }
            finally
            {
                sqlDataReader.Close();
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
            return arrayList;
        }

        // Token: 0x060005F6 RID: 1526 RVA: 0x000517EC File Offset: 0x0004F9EC
        public static IList buildFkStmt(SqlConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            string commandText = "exec sp_MStablekeys N'" + table.name + "', null, 8";
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = commandText;
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    int num = int.Parse(sqlDataReader["cType"].ToString());
                    if (num == 3)
                    {
                        SqlTableForeignKey sqlTableForeignKey = new SqlTableForeignKey();
                        sqlTableForeignKey.name = (string)sqlDataReader["cName"];
                        int num2 = (int)sqlDataReader["cColCount"];
                        sqlTableForeignKey.refTableName = (string)sqlDataReader["cRefTable"];
                        commandText = "select object_name(object_id('" + sqlTableForeignKey.refTableName + "'))";
                        conn.Open();
                        SqlCommand sqlCommand2 = conn.CreateCommand();
                        sqlCommand2.CommandText = commandText;
                        SqlDataReader sqlDataReader2 = sqlCommand2.ExecuteReader();
                        if (!sqlDataReader2.Read())
                        {
                            throw new System.Exception("FATAL ERROR.");
                        }
                        sqlTableForeignKey.refTableName = (string)sqlDataReader2[0];
                        for (int i = 7; i < 7 + num2; i++)
                        {
                            string text = (string)sqlDataReader[i];
                            if (text == null || text.Length == 0)
                            {
                                break;
                            }
                            sqlTableForeignKey.columnList.Add(text);
                        }
                        for (int j = 23; j < 23 + num2; j++)
                        {
                            string text2 = (string)sqlDataReader[j];
                            if (text2 == null || text2.Length == 0)
                            {
                                break;
                            }
                            sqlTableForeignKey.refColumnList.Add(text2);
                        }
                        SqlAlterTableStmt sqlAlterTableStmt = new SqlAlterTableStmt(table.name);
                        sqlAlterTableStmt.item = new SqlAlterTableAddItem(sqlTableForeignKey);
                        if (checkExists)
                        {
                            IList objectCheckStmt = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.ForeignKey, sqlTableForeignKey.name, table.name);
                            arrayList.AddRange(objectCheckStmt);
                        }
                        arrayList.Add(sqlAlterTableStmt);
                    }
                }
            }
            finally
            {
                sqlDataReader.Close();
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
            return arrayList;
        }

        // Token: 0x060005F7 RID: 1527 RVA: 0x00051A04 File Offset: 0x0004FC04
        public static IList buildCreateIndexStmt(SqlConnection conn, SqlTable table, bool checkExists = false, string strconnectstring = null)
        {
            ArrayList arrayList = new ArrayList();
            string text = "exec sp_MShelpindex N'" + table.name + "'";
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = strconnectstring;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                DataSet dataSet = new DataSet();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(text, conn);
                sqlDataAdapter.Fill(dataSet);
                sqlCommand = sqlConnection.CreateCommand();
                foreach (object obj in dataSet.Tables[0].Rows)
                {
                    DataRow dataRow = (DataRow)obj;
                    string text2 = (string)dataRow["name"];
                    if (text2 != null && !text2.StartsWith("_WA_Sys"))
                    {
                        SqlCreateIndexStmt sqlCreateIndexStmt = new SqlCreateIndexStmt();
                        text = "select objectproperty(object_id('" + text2 + "'),'IsConstraint')";
                        sqlCommand.CommandText = text;
                        if (sqlConnection.State == ConnectionState.Closed)
                        {
                            sqlConnection.Open();
                        }
                        if (sqlDataReader != null && !sqlDataReader.IsClosed)
                        {
                            sqlDataReader.Close();
                        }
                        sqlDataReader = sqlCommand.ExecuteReader();
                        if (!sqlDataReader.Read())
                        {
                            throw new System.Exception("FATAL ERROR.");
                        }
                        int num = (int)((sqlDataReader[0] == DBNull.Value) ? 0 : sqlDataReader[0]);
                        if (num != 1)
                        {
                            sqlCreateIndexStmt.indexName = text2;
                            sqlCreateIndexStmt.tableName = table.name;
                            int num2 = (int)dataRow["Descending"];
                            for (int i = 4; i < 20; i++)
                            {
                                string text3 = (string)((dataRow[i] == DBNull.Value) ? "" : dataRow[i]);
                                if (text3 == null || text3.Length == 0)
                                {
                                    break;
                                }
                                int mode = 0;
                                if (num2 > 0 && (num2 & 1 << i - 5) == 1 << i - 5)
                                {
                                    mode = 1;
                                }
                                SqlOrderByItem value = new SqlOrderByItem(new SqlIdentifierExpr(text3), mode, -1);
                                sqlCreateIndexStmt.itemList.Add(value);
                            }
                            sqlDataReader.Close();
                            if (checkExists)
                            {
                                IList objectCheckStmt = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Index, text2, table.name);
                                arrayList.AddRange(objectCheckStmt);
                            }
                            arrayList.Add(sqlCreateIndexStmt);
                        }
                    }
                }
            }
            finally
            {
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
            return arrayList;
        }

        // Token: 0x060005F8 RID: 1528 RVA: 0x00051C90 File Offset: 0x0004FE90
        public static IList generateCreateTableSqlDom(SqlConnection conn, bool checkExists = false)
        {
            ArrayList arrayList = new ArrayList();
            ArrayList userTables = Sql2kUtil.getUserTables(conn);
            foreach (object obj in userTables)
            {
                SqlTable sqlTable = (SqlTable)obj;
                if (checkExists)
                {
                    arrayList.AddRange(KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Table, sqlTable.name, sqlTable.name));
                }
                SqlCreateTableStmt value = Sql2kUtil.buildCreateTableStmt(sqlTable);
                arrayList.Add(value);
            }
            return arrayList;
        }

        // Token: 0x060005F9 RID: 1529 RVA: 0x00051CF4 File Offset: 0x0004FEF4
        public static SqlCreateTableStmt buildCreateTableStmt(SqlTable table)
        {
            SqlCreateTableStmt sqlCreateTableStmt = new SqlCreateTableStmt(table.name);
            foreach (object obj in table.columns)
            {
                SqlColumn sqlColumn = (SqlColumn)obj;
                SqlColumnDef sqlColumnDef = new SqlColumnDef();
                sqlColumnDef.name = sqlColumn.name;
                sqlColumnDef.dataType = sqlColumn.dataType.ToUpper();
                sqlColumnDef.length = sqlColumn.length;
                sqlColumnDef.precision = sqlColumn.precision;
                sqlColumnDef.scale = sqlColumn.scale;
                sqlColumnDef.allowNull = sqlColumn.isNullable;
                sqlColumnDef.autoIncrement = (bool)sqlColumn.extendedAttributes["col_identity"];
                if (sqlColumn.defaultExpr != null && sqlColumn.defaultExpr != "")
                {
                    sqlColumnDef.defaultValueExpr = new SqlDefaultExpr(sqlColumn.defaultExpr);
                }
                Sql2kUtil.convertColumnDataType(sqlColumnDef);
                sqlCreateTableStmt.columnList.Add(sqlColumnDef);
            }
            return sqlCreateTableStmt;
        }

        // Token: 0x060005FA RID: 1530 RVA: 0x00051DE2 File Offset: 0x0004FFE2
        public static void validateTable(SqlCreateTableStmt table)
        {
            if (table.name == null)
            {
                throw new System.Exception("table name is null");
            }
            int length = table.name.Length;
        }

        // Token: 0x060005FB RID: 1531 RVA: 0x00051E08 File Offset: 0x00050008
        public static void convertColumnDataType(SqlColumnDef colDef)
        {
            string dataType = colDef.dataType;
            if (dataType.EqualsIgnoreCase("BIGINT"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("BIT"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("BINARY"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("CHAR"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("DATETIME"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("DECIMAL"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("FLOAT"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("IMAGE"))
            {
                colDef.dataType = "BLOB";
                return;
            }
            if (dataType.EqualsIgnoreCase("INT"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("MONEY"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("NCHAR"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("NTEXT"))
            {
                colDef.dataType = "NCLOB";
                return;
            }
            if (dataType.EqualsIgnoreCase("NUMERIC"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("NVARCHAR"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("REAL"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("SMALLDATETIME"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("SMALLINT"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("SMALLMONEY"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("SQL_VARIANT"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("TEXT"))
            {
                colDef.dataType = "CLOB";
                return;
            }
            if (dataType.EqualsIgnoreCase("TIMESTAMP"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("TINYINT"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("UNIQUEIDENTIFIER"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("VARBINARY"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("VARCHAR"))
            {
                return;
            }
            if (dataType.EqualsIgnoreCase("XMLTYPE"))
            {
                colDef.dataType = "XML";
            }
        }

        // Token: 0x060005FC RID: 1532 RVA: 0x00051FB4 File Offset: 0x000501B4
        public static string convertDataType(string dataType)
        {
            if (dataType.EqualsIgnoreCase("BIGINT"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("BINARY"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("CHAR"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("DATETIME"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("DECIMAL"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("FLOAT"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("IMAGE"))
            {
                return "BLOB";
            }
            if (dataType.EqualsIgnoreCase("INT"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("MONEY"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("NCHAR"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("NTEXT"))
            {
                return "NCLOB";
            }
            if (dataType.EqualsIgnoreCase("NUMERIC"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("NVARCHAR"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("REAL"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("SMALLDATETIME"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("SMALLINT"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("SMALLMONEY"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("SQL_VARIANT"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("TEXT"))
            {
                return "CLOB";
            }
            if (dataType.EqualsIgnoreCase("TIMESTAMP"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("TINYINT"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("UNIQUEIDENTIFIER"))
            {
                throw new System.Exception("incompatible datatype : '" + dataType + "'");
            }
            if (dataType.EqualsIgnoreCase("VARBINARY"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("VARCHAR"))
            {
                return dataType;
            }
            if (dataType.EqualsIgnoreCase("XMLTYPE"))
            {
                return "XML";
            }
            throw new System.Exception("incompatible datatype : '" + dataType + "'");
        }

        // Token: 0x060005FD RID: 1533 RVA: 0x0005223C File Offset: 0x0005043C
        public static ArrayList getUserTables(SqlConnection conn)
        {
            ArrayList arrayList = new ArrayList();
            SqlCommand sqlCommand = null;
            string commandText = "select name, id, crdate, owner = user_name(uid) from sysobjects where xtype = 'u' and name != 'dtproperties'";
            SqlDataReader sqlDataReader = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = commandText;
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    arrayList.Add(new SqlTable
                    {
                        name = (string)sqlDataReader[0],
                        extendedAttributes =
                        {
                            {
                                "id",
                                sqlDataReader[1]
                            },
                            {
                                "crdate",
                                sqlDataReader[2]
                            },
                            {
                                "owner",
                                sqlDataReader[3]
                            }
                        }
                    });
                }
                sqlDataReader.Close();
                for (int i = 0; i < arrayList.Count; i++)
                {
                    SqlTable sqlTable = (SqlTable)arrayList[i];
                    Sql2kUtil.getTableDetail(conn, sqlTable);
                    arrayList[i] = sqlTable;
                }
            }
            finally
            {
                sqlDataReader.Close();
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
            return arrayList;
        }

        // Token: 0x060005FE RID: 1534 RVA: 0x00052358 File Offset: 0x00050558
        public static ArrayList getUserTableNames(SqlConnection conn)
        {
            ArrayList arrayList = new ArrayList();
            SqlCommand sqlCommand = null;
            string commandText = "select name from sysobjects where xtype = 'u' and name != 'dtproperties'";
            SqlDataReader sqlDataReader = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = commandText;
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    arrayList.Add(sqlDataReader[0]);
                }
                sqlDataReader.Close();
            }
            finally
            {
                sqlDataReader.Close();
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
            return arrayList;
        }

        // Token: 0x060005FF RID: 1535 RVA: 0x000523E0 File Offset: 0x000505E0
        public static SqlTable getUserTable(SqlConnection conn, string tableName)
        {
            SqlTable result = null;
            SqlCommand sqlCommand = conn.CreateCommand();
            string commandText = "select name, id, crdate, owner = user_name(uid) from sysobjects where xtype = 'u' and name = '" + tableName + "'";
            SqlDataReader sqlDataReader = null;
            try
            {
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = commandText;
                sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.Read())
                {
                    SqlTable sqlTable = new SqlTable();
                    sqlTable.name = (string)sqlDataReader[0];
                    sqlTable.extendedAttributes.Add("id", sqlDataReader[1]);
                    sqlTable.extendedAttributes.Add("crdate", sqlDataReader[2]);
                    sqlTable.extendedAttributes.Add("owner", sqlDataReader[3]);
                    sqlDataReader.Close();
                    Sql2kUtil.getTableDetail(conn, sqlTable);
                    result = sqlTable;
                }
            }
            finally
            {
                sqlDataReader.Close();
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
            return result;
        }

        // Token: 0x06000600 RID: 1536 RVA: 0x000524BC File Offset: 0x000506BC
        public static void getTableDetail(SqlConnection conn, SqlTable table)
        {
            string commandText = "sp_MShelpcolumns N'" + table.name + "', null, 'id', 1";
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                sqlCommand = conn.CreateCommand();
                sqlCommand.CommandText = commandText;
                sqlCommand.CommandType = CommandType.Text;
                sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    SqlColumn sqlColumn = new SqlColumn();
                    sqlColumn.name = (string)sqlDataReader["col_name"];
                    sqlColumn.extendedAttributes.Add("id", sqlDataReader["col_id"]);
                    sqlColumn.dataType = (string)sqlDataReader["col_typename"];
                    sqlColumn.length = (int)sqlDataReader["col_len"];
                    if (sqlDataReader["col_prec"] == DBNull.Value)
                    {
                        sqlColumn.precision = -1;
                    }
                    else
                    {
                        sqlColumn.precision = (int)sqlDataReader["col_prec"];
                    }
                    if (sqlDataReader["col_scale"] == DBNull.Value)
                    {
                        sqlColumn.scale = -1;
                    }
                    else
                    {
                        sqlColumn.scale = (int)sqlDataReader["col_scale"];
                    }
                    sqlColumn.extendedAttributes.Add("col_basetypename", sqlDataReader["col_basetypename"]);
                    sqlColumn.extendedAttributes.Add("col_defname", (sqlDataReader["col_defname"] == DBNull.Value) ? "" : sqlDataReader["col_defname"]);
                    sqlColumn.extendedAttributes.Add("col_rulname", (sqlDataReader["col_rulname"] == DBNull.Value) ? "" : sqlDataReader["col_rulname"]);
                    sqlColumn.isNullable = (bool)sqlDataReader["col_null"];
                    sqlColumn.extendedAttributes.Add("col_identity", sqlDataReader["col_identity"]);
                    sqlColumn.extendedAttributes.Add("col_flags", sqlDataReader["col_flags"]);
                    sqlColumn.extendedAttributes.Add("col_seed", (sqlDataReader["col_seed"] == DBNull.Value) ? "" : sqlDataReader["col_seed"]);
                    sqlColumn.extendedAttributes.Add("col_increment", (sqlDataReader["col_increment"] == DBNull.Value) ? "" : sqlDataReader["col_increment"]);
                    sqlColumn.extendedAttributes.Add("col_dridefname", (sqlDataReader["col_dridefname"] == DBNull.Value) ? "" : sqlDataReader["col_dridefname"]);
                    sqlColumn.extendedAttributes.Add("text", (sqlDataReader[17] == DBNull.Value) ? "" : sqlDataReader[17]);
                    sqlColumn.defaultExpr = (string)((sqlDataReader[15] == DBNull.Value) ? "" : sqlDataReader[15]);
                    sqlColumn.extendedAttributes.Add("col_iscomputed", sqlDataReader["col_iscomputed"]);
                    table.columns.Add(sqlColumn);
                }
            }
            finally
            {
                sqlDataReader.Close();
                KSqlUtil.cleanUp(sqlCommand, sqlDataReader);
            }
        }

        // Token: 0x06000601 RID: 1537 RVA: 0x000527F8 File Offset: 0x000509F8
        public static string GenerateTableSQL(SqlConnection conn, string tableName, SQLFormater formater, bool multiUse)
        {
            string result = "";
            IList list = new ArrayList();
            tableName = tableName.Trim();
            if (tableName != "")
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SqlTable userTable = Sql2kUtil.getUserTable(conn, tableName);
                if (userTable == null || userTable.name.Trim() == "")
                {
                    return "";
                }
                if (multiUse)
                {
                    list = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Table, tableName, tableName);
                }
                SqlStmt value = Sql2kUtil.buildCreateTableStmt(userTable);
                list.Add(value);
                formater.Format(list);
                result = formater.GetBuffer();
                formater.SetBuffer("");
            }
            return result;
        }

        // Token: 0x06000602 RID: 1538 RVA: 0x00052894 File Offset: 0x00050A94
        public static string GeneratePKSQL(SqlConnection conn, string tableName, SQLFormater formater, bool multiUse)
        {
            IList stmtList = new ArrayList();
            tableName = tableName.Trim();
            if (!(tableName != ""))
            {
                return "";
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlTable userTable = Sql2kUtil.getUserTable(conn, tableName);
            if (userTable == null || userTable.name.Trim() == "")
            {
                return "";
            }
            stmtList = Sql2kUtil.buildPKStmt(conn, userTable, multiUse);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        // Token: 0x06000603 RID: 1539 RVA: 0x00052910 File Offset: 0x00050B10
        public static string GenerateUniqueSQL(SqlConnection conn, string tableName, SQLFormater formater, bool multiUse)
        {
            IList stmtList = new ArrayList();
            tableName = tableName.Trim();
            if (!(tableName != ""))
            {
                return "";
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlTable userTable = Sql2kUtil.getUserTable(conn, tableName);
            if (userTable == null || userTable.name.Trim() == "")
            {
                return "";
            }
            stmtList = Sql2kUtil.buildUniqueStmt(conn, userTable, multiUse);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        // Token: 0x06000604 RID: 1540 RVA: 0x0005298C File Offset: 0x00050B8C
        public static string GenerateFKSQL(SqlConnection conn, string tableName, SQLFormater formater, bool multiUse)
        {
            IList stmtList = new ArrayList();
            tableName = tableName.Trim();
            if (!(tableName != ""))
            {
                return "";
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlTable userTable = Sql2kUtil.getUserTable(conn, tableName);
            if (userTable == null || userTable.name.Trim() == "")
            {
                return "";
            }
            stmtList = Sql2kUtil.buildFkStmt(conn, userTable, multiUse);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        // Token: 0x06000605 RID: 1541 RVA: 0x00052A08 File Offset: 0x00050C08
        public static string GenerateCheckSQL(SqlConnection conn, string tableName, SQLFormater formater, bool multiUse)
        {
            IList stmtList = new ArrayList();
            tableName = tableName.Trim();
            if (!(tableName != ""))
            {
                return "";
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlTable userTable = Sql2kUtil.getUserTable(conn, tableName);
            if (userTable == null || userTable.name.Trim() == "")
            {
                return "";
            }
            stmtList = Sql2kUtil.buildCheckStmt(conn, userTable, multiUse);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        // Token: 0x06000606 RID: 1542 RVA: 0x00052A84 File Offset: 0x00050C84
        public static string GenerateIndexSQL(SqlConnection conn, string tableName, SQLFormater formater, bool multiUse)
        {
            IList stmtList = new ArrayList();
            tableName = tableName.Trim();
            if (!(tableName != ""))
            {
                return "";
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlTable userTable = Sql2kUtil.getUserTable(conn, tableName);
            if (userTable == null || userTable.name.Trim() == "")
            {
                return "";
            }
            stmtList = Sql2kUtil.buildCreateIndexStmt(conn, userTable, multiUse, null);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        // Token: 0x06000607 RID: 1543 RVA: 0x00052B00 File Offset: 0x00050D00
        public static string GenerateDataSQL(SqlConnection conn, string tableName, SQLFormater formater, bool multiUse, TextWriter fWriter)
        {
            IList list = new ArrayList();
            tableName = tableName.Trim();
            if (!(tableName != ""))
            {
                return "";
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlTable userTable = Sql2kUtil.getUserTable(conn, tableName);
            if (userTable == null || userTable.name.Trim() == "")
            {
                return "";
            }
            list = Sql2kUtil.buildDataStmtSql(conn, userTable, multiUse);
            int num = 0;
            int num2 = list.Count - 1;
            int num3 = -1;
            ArrayList arrayList = new ArrayList();
            foreach (object obj in list)
            {
                SqlStmt value = (SqlStmt)obj;
                if (num3 < 0)
                {
                    if (arrayList.Count > 0)
                    {
                        formater.Format(arrayList);
                        fWriter.WriteLine(formater.GetBuffer());
                        formater.SetBuffer("");
                    }
                    int num4 = (num2 - num > 50) ? 50 : (num2 - num);
                    arrayList = new ArrayList();
                    num3 = num4;
                }
                arrayList.Add(value);
                num3--;
                num++;
            }
            if (arrayList.Count > 0)
            {
                formater.Format(arrayList);
            }
            fWriter.WriteLine(formater.GetBuffer());
            formater.SetBuffer("");
            return "";
        }

        // Token: 0x06000608 RID: 1544 RVA: 0x00052C5C File Offset: 0x00050E5C
        public static string GenerateTableFullSQL(SqlConnection conn, string tableName, SQLFormater formater, KSqlExportOption expOpt)
        {
            IList list = new ArrayList();
            tableName = tableName.Trim();
            if (!(tableName != ""))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (expOpt.ExpTableSql)
                {
                    formater.SetBuffer("\n\n\n\n/* create talbe */\n\n");
                    list = Sql2kUtil.generateCreateTableSqlDom(conn, expOpt.CheckTable);
                    formater.Format(list);
                }
                if (expOpt.ExpPrimaryKeySql)
                {
                    formater.AppendString("\n\n\n\n/* create primary key */\n\n");
                    list = Sql2kUtil.generatePkSqlDom(conn, expOpt.CheckPrimaryKey);
                    formater.Format(list);
                }
                if (expOpt.ExpUniqueSql)
                {
                    formater.AppendString("\n\n\n\n/* create unique */\n\n");
                    list = Sql2kUtil.generateUniqueSqlDom(conn, expOpt.CheckUnique);
                    formater.Format(list);
                }
                if (expOpt.ExpForeignKeySql)
                {
                    formater.AppendString("\n\n\n\n/* create foreign key */\n\n");
                    list = Sql2kUtil.generateFkSqlDom(conn, expOpt.CheckForeignKey);
                    formater.Format(list);
                }
                if (expOpt.ExpCheckSql)
                {
                    formater.AppendString("\n\n\n\n/* create check */\n\n");
                    list = Sql2kUtil.generateCheckSqlDom(conn, expOpt.CheckCheckRule);
                    formater.Format(list);
                }
                if (expOpt.ExpIndexSql)
                {
                    formater.AppendString("\n\n\n\n/* create index */\n\n");
                    list = Sql2kUtil.generateCreateIndexSqlDom(conn, expOpt.CheckIndex);
                    formater.Format(list);
                }
                if (expOpt.ExpDataSql)
                {
                    formater.AppendString("\n\n\n\n/* create Data */\n\n");
                    list = Sql2kUtil.buildDataStmtSql(conn, expOpt.CheckData);
                    formater.Format(list);
                }
                return formater.GetBuffer();
            }
            formater.SetBuffer("\n\n\n\n/* create talbe */\n\n");
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlTable userTable = Sql2kUtil.getUserTable(conn, tableName);
            if (userTable == null || userTable.name.Trim() == "")
            {
                return "";
            }
            if (expOpt.ExpTableSql)
            {
                if (expOpt.CheckTable)
                {
                    list = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Table, tableName, tableName);
                }
                SqlStmt value = Sql2kUtil.buildCreateTableStmt(userTable);
                list.Add(value);
                formater.Format(list);
            }
            if (expOpt.ExpPrimaryKeySql)
            {
                formater.AppendString("\n\n\n\n/* create primary key */\n\n");
                list = Sql2kUtil.buildPKStmt(conn, userTable, expOpt.CheckUnique);
                formater.Format(list);
            }
            if (expOpt.ExpUniqueSql)
            {
                formater.AppendString("\n\n\n\n/* create unique */\n\n");
                list = Sql2kUtil.buildUniqueStmt(conn, userTable, expOpt.CheckUnique);
                formater.Format(list);
            }
            if (expOpt.ExpForeignKeySql)
            {
                formater.AppendString("\n\n\n\n/* create foreign key */\n\n");
                list = Sql2kUtil.buildFkStmt(conn, userTable, expOpt.CheckForeignKey);
                formater.Format(list);
            }
            if (expOpt.ExpCheckSql)
            {
                formater.AppendString("\n\n\n\n/* create check */\n\n");
                list = Sql2kUtil.buildCheckStmt(conn, userTable, expOpt.CheckCheckRule);
                formater.Format(list);
            }
            if (expOpt.ExpIndexSql)
            {
                formater.AppendString("\n\n\n\n/* create index */\n\n");
                list = Sql2kUtil.buildCreateIndexStmt(conn, userTable, expOpt.CheckIndex, null);
                formater.Format(list);
            }
            if (expOpt.ExpDataSql)
            {
                formater.AppendString("\n\n\n\n/* create Data */\n\n");
                list = Sql2kUtil.buildDataStmtSql(conn, userTable, expOpt.CheckData);
                formater.Format(list);
            }
            return formater.GetBuffer();
        }
    }
}
