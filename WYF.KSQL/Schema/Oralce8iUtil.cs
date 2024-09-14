using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Formater;


namespace WYF.KSQL.Schema
{
    public class Oralce8iUtil
    {
        // Methods
        public static IList buildCreateIndexStmt(IDbConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            string str = "SELECT * FROM USER_INDEXES WHERE TABLE_NAME ='" + table.name.ToLower() + "' AND UNIQUENESS ='NONUNIQUE'";
            IDbCommand command = null;
            IDataReader rs = null;
            try
            {
                command = conn.CreateCommand();
                command.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = command.ExecuteReader();
                while (rs.Read())
                {
                    SqlCreateIndexStmt stmt = new SqlCreateIndexStmt
                    {
                        tableName = table.name,
                        indexName = (string)rs[0]
                    };
                    stmt.addExtAttr("INDEX_TYPE", (string)rs[1]);
                    stmt.addExtAttr("TABLE_OWNER", (string)rs[2]);
                    stmt.addExtAttr("TABLE_NAME", (string)rs[3]);
                    stmt.addExtAttr("TABLE_TYPE", (string)rs[4]);
                    string str2 = (string)rs[5];
                    stmt.addExtAttr("COMPRESSION", (rs[6] == DBNull.Value) ? "" : ((string)rs[6]));
                    stmt.addExtAttr("PREFIX_LENGTH", (rs[7] == DBNull.Value) ? "" : ((string)rs[7]));
                    stmt.addExtAttr("TABLESPACE_NAME", (rs[8] == DBNull.Value) ? "" : ((string)rs[8]));
                    stmt.addExtAttr("INI_TRANS", rs[9].ToString());
                    stmt.addExtAttr("MAX_TRANS", rs[10].ToString());
                    stmt.addExtAttr("INITIAL_EXTENT", rs[11].ToString());
                    if (str2.EqualsIgnoreCase("UNIQUE"))
                    {
                        stmt.isUnique = true;
                    }
                    list2.Add(stmt);
                }
                conn.Close();
                foreach (SqlCreateIndexStmt stmt2 in list2)
                {
                    buildCreateIndexStmt_buildColumns(conn, stmt2);
                    if (checkExists)
                    {
                        IList c = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Index, stmt2.indexName, table.name);
                        list.AddRange(c);
                    }
                    list.Add(stmt2);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(command, rs);
            }
            return list;
        }

        private static void buildCreateIndexStmt_buildColumns(IDbConnection conn, SqlCreateIndexStmt createIndexStmt)
        {
            string str = "SELECT t2.COLUMN_EXPRESSION,t1.* FROM USER_IND_COLUMNS t1 LEFT JOIN USER_IND_EXPRESSIONS t2 ON t1.INDEX_NAME=t2.INDEX_NAME  WHERE t1.INDEX_NAME = '" + createIndexStmt.indexName + "' AND t1.TABLE_NAME='" + createIndexStmt.tableName + "'";
            IDbCommand stmt = null;
            IDataReader rs = null;
            try
            {
                stmt = conn.CreateCommand();
                stmt.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = stmt.ExecuteReader();
                while (rs.Read())
                {
                    int num;
                    string source = (string)rs[3];
                    string str3 = (string)rs[7];
                    int chineseOrderByMode = -1;
                    int end = (source.Length < 6) ? source.Length : 6;
                    if (((string.Compare(source.substring(0, end), "SYS_NC", StringComparison.OrdinalIgnoreCase) == 0) && (source.IndexOf("$") == (source.Length - 1))) && (rs[0] != DBNull.Value))
                    {
                        string str4 = (string)rs[0];
                        if (str4.IndexOf("NLSSORT") == 0)
                        {
                            if (str4.IndexOf("SCHINESE_PINYIN_M") != -1)
                            {
                                chineseOrderByMode = 2;
                            }
                            else if (str4.IndexOf("SCHINESE_RADICAL_M") != -1)
                            {
                                chineseOrderByMode = 4;
                            }
                            else if (str4.IndexOf("SCHINESE_STROKE_M") != -1)
                            {
                                chineseOrderByMode = 3;
                            }
                            else
                            {
                                chineseOrderByMode = -1;
                            }
                            source = str4.substring(9, str4.IndexOf("\",'"));
                        }
                    }
                    if (str3.EqualsIgnoreCase("ASC"))
                    {
                        num = 0;
                    }
                    else
                    {
                        if (!str3.EqualsIgnoreCase("DESC"))
                        {
                            throw new System.Exception("unkown descending.");
                        }
                        num = 1;
                    }
                    SqlOrderByItem item = new SqlOrderByItem(new SqlIdentifierExpr(source), num, chineseOrderByMode);
                    createIndexStmt.itemList.Add(item);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
        }

        public static SqlCreateTableStmt buildCreateTableStmt(SqlTable table)
        {
            SqlCreateTableStmt stmt = new SqlCreateTableStmt(table.name);
            IEnumerator enumerator = table.columns.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SqlColumn current = (SqlColumn)enumerator.Current;
                SqlColumnDef column = new SqlColumnDef
                {
                    name = current.name,
                    dataType = current.dataType.ToUpper(),
                    length = current.length,
                    precision = current.precision,
                    scale = current.scale,
                    allowNull = current.isNullable
                };
                if ((current.defaultExpr != null) && (current.defaultExpr != ""))
                {
                    column.defaultValueExpr = new SqlDefaultExpr(current.defaultExpr);
                }
                convertColumnDataType(column);
                stmt.columnList.Add(column);
            }
            return stmt;
        }

        public static IList buildDataStmtSql(IDbConnection conn, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = getUserTables(conn).GetEnumerator();
            while (enumerator.MoveNext())
            {
                SqlTable current = (SqlTable)enumerator.Current;
                IList c = buildDataStmtSql(conn, current, checkExists);
                list.AddRange(c);
            }
            return list;
        }

        public static IList buildDataStmtSql(IDbConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            string str = "select * from " + table.name;
            IDbCommand stmt = conn.CreateCommand();
            IDataReader rs = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                stmt.CommandText = str;
                rs = stmt.ExecuteReader();
                list.AddRange(KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.AllData, table.name, table.name));
                while (rs.Read())
                {
                    IEnumerator enumerator = table.columns.GetEnumerator();
                    SqlInsert insert = new SqlInsert(table.name);
                    for (int i = 0; enumerator.MoveNext(); i++)
                    {
                        SqlColumn current = (SqlColumn)enumerator.Current;
                        string name = current.name;
                        insert.columnList.Add(new SqlIdentifierExpr(name));
                        if (current.dataType.EqualsIgnoreCase("int"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                decimal num2 = (decimal)rs[i];
                                insert.valueList.Add(SqlExpr.toExpr(num2));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("smallint"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                int num3 = (int)rs[i];
                                insert.valueList.Add(SqlExpr.toExpr(num3));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("numeric"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                decimal num4 = (decimal)rs[i];
                                insert.valueList.Add(SqlExpr.toExpr(num4));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("number"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                decimal num5 = (decimal)rs[i];
                                insert.valueList.Add(SqlExpr.toExpr(num5));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("float"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                double num6 = (double)rs[i];
                                insert.valueList.Add(SqlExpr.toExpr(num6));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("DATE"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str3 = rs[i].ToString();
                                insert.valueList.Add(SqlExpr.toDateExpr(str3));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("DATETIME"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str4 = rs[i].ToString();
                                insert.valueList.Add(SqlExpr.toDateTimeExpr(str4));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("TimeStamp(6)"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str5 = rs[i].ToString();
                                insert.valueList.Add(SqlExpr.toCharExpr(str5));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("TimeStamp"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str6 = rs[i].ToString();
                                insert.valueList.Add(SqlExpr.toCharExpr(str6));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("char") || current.dataType.EqualsIgnoreCase("varchar"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str7 = (string)rs[i];
                                str7 = str7.Replace("'", "''");
                                insert.valueList.Add(SqlExpr.toCharExpr(str7));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("nchar") || current.dataType.EqualsIgnoreCase("nvarchar"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str8 = (string)rs[i];
                                str8 = str8.Replace("'", "''");
                                insert.valueList.Add(SqlExpr.toNCharExpr(str8));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("varchar2"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str9 = (string)rs[i];
                                str9 = str9.Replace("'", "''");
                                insert.valueList.Add(SqlExpr.toCharExpr(str9));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("nvarchar2"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str10 = (string)rs[i];
                                str10 = str10.Replace("'", "''");
                                insert.valueList.Add(SqlExpr.toNCharExpr(str10));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("long"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str11 = (string)rs[i];
                                str11 = str11.Replace("'", "''");
                                insert.valueList.Add(SqlExpr.toCharExpr(str11));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("raw") || current.dataType.EqualsIgnoreCase("long raw"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str12 = rs[i].ToString();
                                insert.valueList.Add(SqlExpr.toCharExpr(str12));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("binary") || current.dataType.EqualsIgnoreCase("varbinary"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str13 = (string)rs[i];
                                insert.valueList.Add(SqlExpr.toCharExpr(str13));
                            }
                        }
                        else if ((current.dataType.EqualsIgnoreCase("blob") || current.dataType.EqualsIgnoreCase("clob")) || (current.dataType.EqualsIgnoreCase("nclob") || current.dataType.EqualsIgnoreCase("bfile")))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str14 = (string)rs[i];
                                str14 = str14.Replace("'", "''");
                                insert.valueList.Add(SqlExpr.toNCharExpr(str14));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("XMLTYPE"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str15 = (string)rs[i];
                                str15 = str15.Replace("'", "''");
                                insert.valueList.Add(SqlExpr.toNCharExpr(str15));
                            }
                        }
                        else if (current.dataType.EqualsIgnoreCase("rowid"))
                        {
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str16 = (string)rs[i];
                                insert.valueList.Add(SqlExpr.toNCharExpr(str16));
                            }
                        }
                        else
                        {
                            if (!current.dataType.EqualsIgnoreCase("urowid"))
                            {
                                throw new System.Exception("TODO");
                            }
                            if (rs[i] == DBNull.Value)
                            {
                                insert.valueList.Add(SqlNullExpr.instance);
                            }
                            else
                            {
                                string str17 = (string)rs[i];
                                insert.valueList.Add(SqlExpr.toNCharExpr(str17));
                            }
                        }
                    }
                    list.Add(new SqlInsertStmt(insert));
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
            return list;
        }

        public static IList buildFkStmt(IDbConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            string str = "SELECT * FROM USER_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'R' AND TABLE_NAME ='" + table.name + "'";
            IDbCommand command = null;
            IDataReader rs = null;
            try
            {
                command = conn.CreateCommand();
                command.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = command.ExecuteReader();
                while (rs.Read())
                {
                    SqlTableForeignKey key = new SqlTableForeignKey();
                    key.extendedAttributes().Add("OWNER", rs[0]);
                    key.name = (string)rs[1];
                    key.extendedAttributes().Add("TABLE_NAME", rs[3]);
                    key.extendedAttributes().Add("SEARCH_CONDITION", rs[4]);
                    key.extendedAttributes().Add("R_OWNER", rs[5]);
                    string str2 = (string)rs[6];
                    key.extendedAttributes().Add("R_CONSTRAINT_NAME", str2);
                    key.extendedAttributes().Add("DELETE_RULE", rs[7]);
                    key.extendedAttributes().Add("STATUS", rs[8]);
                    key.extendedAttributes().Add("DEFERRABLE", rs[9]);
                    key.extendedAttributes().Add("DEFERRED", rs[10]);
                    key.extendedAttributes().Add("VALIDATED", rs[11]);
                    key.extendedAttributes().Add("GENERATED", rs[12]);
                    key.extendedAttributes().Add("BAD", rs[13]);
                    key.extendedAttributes().Add("RELY", rs[14]);
                    key.extendedAttributes().Add("LAST_CHANGE", rs[15]);
                    list2.Add(key);
                }
                conn.Close();
                foreach (SqlTableForeignKey key2 in list2)
                {
                    buildFkStmt_buildColumns(conn, key2);
                    conn.Close();
                    buildFkStmt_buildRefInfo(conn, key2, key2.extendedAttributes()["R_CONSTRAINT_NAME"].ToString());
                    SqlAlterTableStmt stmt = new SqlAlterTableStmt(table.name)
                    {
                        item = new SqlAlterTableAddItem(key2)
                    };
                    if (checkExists)
                    {
                        IList c = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.ForeignKey, key2.name, table.name);
                        list.AddRange(c);
                    }
                    list.Add(stmt);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(command, rs);
            }
            return list;
        }

        private static void buildFkStmt_buildColumns(IDbConnection conn, SqlTableForeignKey fk)
        {
            string str = "SELECT * FROM USER_CONS_COLUMNS WHERE CONSTRAINT_NAME = '" + fk.name + "'";
            IDbCommand stmt = conn.CreateCommand();
            IDataReader rs = null;
            try
            {
                stmt.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = stmt.ExecuteReader();
                while (rs.Read())
                {
                    string str2 = (string)rs[3];
                    fk.columnList.Add(str2);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
        }

        private static void buildFkStmt_buildRefInfo(IDbConnection conn, SqlTableForeignKey fk, string r_constraint_name)
        {
            string str = "SELECT * FROM USER_CONSTRAINTS WHERE CONSTRAINT_NAME ='" + r_constraint_name + "'";
            IDbCommand stmt = null;
            IDataReader rs = null;
            try
            {
                stmt = conn.CreateCommand();
                stmt.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = stmt.ExecuteReader();
                if (!rs.Read())
                {
                    throw new System.Exception("ref constraint not found. ref constraint name is '" + r_constraint_name + "'");
                }
                fk.refTableName = (string)rs[3];
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
            stmt = null;
            rs = null;
            str = "SELECT * FROM USER_CONS_COLUMNS WHERE CONSTRAINT_NAME = '" + r_constraint_name + "'";
            try
            {
                stmt = conn.CreateCommand();
                stmt.CommandText = str;
                rs = stmt.ExecuteReader();
                while (rs.Read())
                {
                    string str2 = (string)rs[3];
                    fk.refColumnList.Add(str2);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
        }

        public static IList buildPkStmt(IDbConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            string str = "SELECT * FROM USER_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'P' AND TABLE_NAME ='" + table.name + "'";
            IDbCommand command = conn.CreateCommand();
            IDataReader rs = null;
            try
            {
                command.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = command.ExecuteReader();
                while (rs.Read())
                {
                    SqlTablePrimaryKey key = new SqlTablePrimaryKey();
                    key.extendedAttributes().Add("OWNER", rs[0]);
                    key.name = (string)rs[1];
                    key.extendedAttributes().Add("TABLE_NAME", rs[3]);
                    key.extendedAttributes().Add("SEARCH_CONDITION", rs[4]);
                    key.extendedAttributes().Add("R_OWNER", rs[5]);
                    key.extendedAttributes().Add("R_CONSTRAINT_NAME", rs[6]);
                    key.extendedAttributes().Add("DELETE_RULE", rs[7]);
                    key.extendedAttributes().Add("STATUS", rs[8]);
                    key.extendedAttributes().Add("DEFERRABLE", rs[9]);
                    key.extendedAttributes().Add("DEFERRED", rs[10]);
                    key.extendedAttributes().Add("VALIDATED", rs[11]);
                    key.extendedAttributes().Add("GENERATED", rs[12]);
                    key.extendedAttributes().Add("BAD", rs[13]);
                    key.extendedAttributes().Add("RELY", rs[14]);
                    key.extendedAttributes().Add("LAST_CHANGE", rs[15]);
                    list2.Add(key);
                }
                conn.Close();
                foreach (SqlTablePrimaryKey key2 in list2)
                {
                    buildPkStmt_buildColumns(conn, key2);
                    SqlAlterTableStmt stmt = new SqlAlterTableStmt(table.name)
                    {
                        item = new SqlAlterTableAddItem(key2)
                    };
                    if (checkExists)
                    {
                        IList c = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.PrimaryKey, key2.name, table.name);
                        list.AddRange(c);
                    }
                    list.Add(stmt);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(command, rs);
            }
            return list;
        }

        private static void buildPkStmt_buildColumns(IDbConnection conn, SqlTablePrimaryKey pk)
        {
            string str = "SELECT * FROM USER_CONS_COLUMNS WHERE CONSTRAINT_NAME = '" + pk.name + "'";
            IDbCommand stmt = conn.CreateCommand();
            IDataReader rs = null;
            try
            {
                stmt.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = stmt.ExecuteReader();
                while (rs.Read())
                {
                    string str2 = (string)rs[3];
                    pk.columnList.Add(str2);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
        }

        public static IList buildUniqueStmt(IDbConnection conn, SqlTable table, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            string str = "SELECT * FROM USER_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'U' AND TABLE_NAME ='" + table.name + "'";
            IDbCommand command = conn.CreateCommand();
            command.CommandText = str;
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                SqlTableUnique unique = new SqlTableUnique();
                unique.extendedAttributes().Add("OWNER", reader[0]);
                unique.name = (string)reader[1];
                unique.extendedAttributes().Add("TABLE_NAME", reader[3]);
                unique.extendedAttributes().Add("SEARCH_CONDITION", reader[4]);
                unique.extendedAttributes().Add("R_OWNER", reader[5]);
                unique.extendedAttributes().Add("R_CONSTRAINT_NAME", reader[6]);
                unique.extendedAttributes().Add("DELETE_RULE", reader[7]);
                unique.extendedAttributes().Add("STATUS", reader[8]);
                unique.extendedAttributes().Add("DEFERRABLE", reader[9]);
                unique.extendedAttributes().Add("DEFERRED", reader[10]);
                unique.extendedAttributes().Add("VALIDATED", reader[11]);
                unique.extendedAttributes().Add("GENERATED", reader[12]);
                unique.extendedAttributes().Add("BAD", reader[13]);
                unique.extendedAttributes().Add("RELY", reader[14]);
                unique.extendedAttributes().Add("LAST_CHANGE", reader[15]);
            }
            conn.Close();
            foreach (SqlTableUnique unique2 in list2)
            {
                buildUniqueStmt_buildColumns(conn, unique2);
                SqlAlterTableStmt stmt = new SqlAlterTableStmt(table.name)
                {
                    item = new SqlAlterTableAddItem(unique2)
                };
                if (checkExists)
                {
                    IList c = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Unique, unique2.name, table.name);
                    list.AddRange(c);
                }
                list.Add(stmt);
            }
            return list;
        }

        private static void buildUniqueStmt_buildColumns(IDbConnection conn, SqlTableUnique unique)
        {
            string str = "SELECT * FROM USER_CONS_COLUMNS WHERE CONSTRAINT_NAME = '" + unique.name + "'";
            IDbCommand stmt = conn.CreateCommand();
            IDataReader rs = null;
            try
            {
                stmt.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = stmt.ExecuteReader();
                while (rs.Read())
                {
                    string str2 = (string)rs[3];
                    unique.columnList.Add(str2);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
        }

        public static void convertColumnDataType(SqlColumnDef column)
        {
            string dataType = column.dataType;
            if (!column.dataType.EqualsIgnoreCase("CHAR"))
            {
                if (column.dataType.EqualsIgnoreCase("VARCHAR") || column.dataType.EqualsIgnoreCase("VARCHAR2"))
                {
                    column.dataType = "VARCHAR";
                }
                else if (!column.dataType.EqualsIgnoreCase("NCHAR"))
                {
                    if (column.dataType.EqualsIgnoreCase("NVARCHAR") || column.dataType.EqualsIgnoreCase("NVARCHAR2"))
                    {
                        column.dataType = "NVARCHAR";
                        column.length /= 2;
                    }
                    else if (column.dataType.EqualsIgnoreCase("NUMBER") || column.dataType.EqualsIgnoreCase("DECIMAL"))
                    {
                        column.dataType = "NUMERIC";
                    }
                    else if (column.dataType.EqualsIgnoreCase("INT") || column.dataType.EqualsIgnoreCase("INTEGER"))
                    {
                        column.dataType = "INT";
                    }
                    else if (!column.dataType.EqualsIgnoreCase("SMALLINT"))
                    {
                        if (column.dataType.EqualsIgnoreCase("DATETIME") || column.dataType.EqualsIgnoreCase("DATE"))
                        {
                            column.dataType = "DATETIME";
                        }
                        else if (!column.dataType.EqualsIgnoreCase("LONG"))
                        {
                            if (column.dataType.EqualsIgnoreCase("RAW") || column.dataType.EqualsIgnoreCase("BINARY"))
                            {
                                column.dataType = "BINARY";
                            }
                            else if (column.dataType.EqualsIgnoreCase("LONG RAW") || column.dataType.EqualsIgnoreCase("VARBINARY"))
                            {
                                column.dataType = "VARBINARY";
                            }
                            else if (!column.dataType.EqualsIgnoreCase("ROWID") && ((((((!column.dataType.EqualsIgnoreCase("BLOB") && !column.dataType.EqualsIgnoreCase("CLOB")) && !column.dataType.EqualsIgnoreCase("NCLOB")) && !column.dataType.EqualsIgnoreCase("BFILE")) && !column.dataType.EqualsIgnoreCase("UROWID")) && !column.dataType.EqualsIgnoreCase("FLOAT")) && !column.dataType.EqualsIgnoreCase("REAL")))
                            {
                                if (column.dataType.EqualsIgnoreCase("TIMESTAMP(6)"))
                                {
                                    column.dataType = "DATETIME";
                                }
                                else if (column.dataType.ToLower().StartsWith("interval year"))
                                {
                                    column.dataType = "NVARCHAR";
                                }
                                else if (!column.dataType.EqualsIgnoreCase("XMLTYPE"))
                                {
                                    throw new System.Exception("incompatible datatype : '" + dataType + "'");
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void executeImmediate(IDbCommand stmt, string sql)
        {
            string str = "/*dialect*/BEGIN EXECUTE IMMEDIATE '" + sql + "'; END;";
            stmt.CommandText = str;
            stmt.ExecuteNonQuery();
        }

        public static IList generateCreateIndexSqlDom(IDbConnection conn, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = getUserTables(conn).GetEnumerator();
            while (enumerator.MoveNext())
            {
                SqlTable current = (SqlTable)enumerator.Current;
                IList c = buildCreateIndexStmt(conn, current, checkExists);
                list.AddRange(c);
            }
            return list;
        }

        public static IList generateCreateTableSqlDom(IDbConnection conn, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = getUserTables(conn).GetEnumerator();
            while (enumerator.MoveNext())
            {
                SqlTable current = (SqlTable)enumerator.Current;
                if (checkExists)
                {
                    list.AddRange(KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Table, current.name, current.name));
                }
                SqlCreateTableStmt stmt = buildCreateTableStmt(current);
                list.Add(stmt);
            }
            return list;
        }

        public static string GenerateDataSQL(IDbConnection conn, string tableName, SQLFormater formater, bool multiUse, TextWriter fWriter)
        {
            IList list = new ArrayList();
            tableName = tableName.Trim();
            if (tableName != "")
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SqlTable table = getUserTable(conn, tableName);
                if ((table == null) || (table.name.Trim() == ""))
                {
                    return "";
                }
                list = buildDataStmtSql(conn, table, multiUse);
                int num = 0;
                int num2 = list.Count - 1;
                int num3 = -1;
                ArrayList stmtList = new ArrayList();
                foreach (SqlStmt stmt in list)
                {
                    if (num3 < 0)
                    {
                        if (stmtList.Count > 0)
                        {
                            formater.Format(stmtList);
                            fWriter.WriteLine(formater.GetBuffer());
                            formater.SetBuffer("");
                        }
                        int num4 = ((num2 - num) > 50) ? 50 : (num2 - num);
                        stmtList = new ArrayList();
                        num3 = num4;
                    }
                    stmtList.Add(stmt);
                    num3--;
                    num++;
                }
                if (stmtList.Count > 0)
                {
                    formater.Format(stmtList);
                }
                fWriter.WriteLine(formater.GetBuffer());
                formater.SetBuffer("");
            }
            return "";
        }

        public static string GenerateFKSQL(IDbConnection conn, string tableName, SQLFormater formater, bool multiUse)
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
            SqlTable table = getUserTable(conn, tableName);
            if ((table == null) || (table.name.Trim() == ""))
            {
                return "";
            }
            stmtList = buildFkStmt(conn, table, multiUse);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        public static IList generateFkSqlDom(IDbConnection conn, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = getUserTables(conn).GetEnumerator();
            while (enumerator.MoveNext())
            {
                SqlTable current = (SqlTable)enumerator.Current;
                IList c = buildFkStmt(conn, current, checkExists);
                list.AddRange(c);
            }
            return list;
        }

        public static string GenerateIndexSQL(IDbConnection conn, string tableName, SQLFormater formater, bool multiUse)
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
            SqlTable table = getUserTable(conn, tableName);
            if ((table == null) || (table.name.Trim() == ""))
            {
                return "";
            }
            stmtList = buildCreateIndexStmt(conn, table, multiUse);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        public static string GeneratePKSQL(IDbConnection conn, string tableName, SQLFormater formater, bool multiUse)
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
            SqlTable table = getUserTable(conn, tableName);
            if ((table == null) || (table.name.Trim() == ""))
            {
                return "";
            }
            stmtList = buildPkStmt(conn, table, multiUse);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        public static IList generatePkSqlDom(IDbConnection conn, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = getUserTables(conn).GetEnumerator();
            while (enumerator.MoveNext())
            {
                SqlTable current = (SqlTable)enumerator.Current;
                IList c = buildPkStmt(conn, current, checkExists);
                list.AddRange(c);
            }
            return list;
        }

        public static string generateSchemaSql(IDbConnection conn, SQLFormater formater)
        {
            StringBuilder builder = new StringBuilder();
            formater.SetBuffer(builder.ToString());
            builder.Append("\n\n\n\n/* create talbe */\n\n");
            IList stmtList = generateCreateTableSqlDom(conn, false);
            formater.Format(stmtList);
            builder.Append("\n\n\n\n/* create primary key */\n\n");
            stmtList = generatePkSqlDom(conn, false);
            formater.Format(stmtList);
            builder.Append("\n\n\n\n/* create unique */\n\n");
            stmtList = generateUniqueSqlDom(conn, false);
            formater.Format(stmtList);
            builder.Append("\n\n\n\n/* create foreign key */\n\n");
            stmtList = generateFkSqlDom(conn, false);
            formater.Format(stmtList);
            builder.Append("\n\n\n\n/* create index */\n\n");
            stmtList = generateCreateIndexSqlDom(conn, false);
            formater.Format(stmtList);
            return builder.ToString();
        }

        public static string GenerateTableFullSQL(IDbConnection conn, string tableName, SQLFormater formater, KSqlExportOption expOpt)
        {
            IList stmtList = new ArrayList();
            tableName = tableName.Trim();
            if (tableName != "")
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SqlTable table = getUserTable(conn, tableName);
                if ((table == null) || (table.name.Trim() == ""))
                {
                    return "";
                }
                if (expOpt.ExpTableSql)
                {
                    formater.SetBuffer("\n\n\n\n/* create talbe */\n\n");
                    if (expOpt.CheckTable)
                    {
                        stmtList = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Table, tableName, tableName);
                    }
                    SqlStmt stmt = buildCreateTableStmt(table);
                    stmtList.Add(stmt);
                    formater.Format(stmtList);
                }
                if (expOpt.ExpPrimaryKeySql)
                {
                    formater.AppendString("\n\n\n\n/* create primary key */\n\n");
                    stmtList = buildPkStmt(conn, table, expOpt.CheckPrimaryKey);
                    formater.Format(stmtList);
                }
                if (expOpt.ExpUniqueSql)
                {
                    formater.AppendString("\n\n\n\n/* create unique */\n\n");
                    stmtList = buildUniqueStmt(conn, table, expOpt.CheckUnique);
                    formater.Format(stmtList);
                }
                if (expOpt.ExpForeignKeySql)
                {
                    formater.AppendString("\n\n\n\n/* create foreign key */\n\n");
                    stmtList = buildFkStmt(conn, table, expOpt.CheckForeignKey);
                    formater.Format(stmtList);
                }
                if (expOpt.ExpIndexSql)
                {
                    formater.AppendString("\n\n\n\n/* create index */\n\n");
                    stmtList = buildCreateIndexStmt(conn, table, expOpt.CheckIndex);
                    formater.Format(stmtList);
                }
                if (expOpt.ExpDataSql)
                {
                    formater.AppendString("\n\n\n\n/* create Data */\n\n");
                    stmtList = buildDataStmtSql(conn, table, expOpt.CheckData);
                    formater.Format(stmtList);
                }
                return formater.GetBuffer();
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (expOpt.ExpTableSql)
            {
                formater.SetBuffer("\n\n\n\n/* create talbe */\n\n");
                stmtList = generateCreateTableSqlDom(conn, expOpt.CheckTable);
                formater.Format(stmtList);
            }
            if (expOpt.ExpPrimaryKeySql)
            {
                formater.AppendString("\n\n\n\n/* create primary key */\n\n");
                stmtList = generatePkSqlDom(conn, expOpt.CheckPrimaryKey);
                formater.Format(stmtList);
            }
            if (expOpt.ExpUniqueSql)
            {
                formater.AppendString("\n\n\n\n/* create unique */\n\n");
                stmtList = generateUniqueSqlDom(conn, expOpt.CheckUnique);
                formater.Format(stmtList);
            }
            if (expOpt.ExpForeignKeySql)
            {
                formater.AppendString("\n\n\n\n/* create foreign key */\n\n");
                stmtList = generateFkSqlDom(conn, expOpt.CheckForeignKey);
                formater.Format(stmtList);
            }
            if (expOpt.ExpIndexSql)
            {
                formater.AppendString("\n\n\n\n/* create index */\n\n");
                stmtList = generateCreateIndexSqlDom(conn, expOpt.CheckIndex);
                formater.Format(stmtList);
            }
            if (expOpt.ExpDataSql)
            {
                formater.AppendString("\n\n\n\n/* create Data */\n\n");
                stmtList = buildDataStmtSql(conn, expOpt.CheckData);
                formater.Format(stmtList);
            }
            return formater.GetBuffer();
        }

        public static string GenerateTableSQL(IDbConnection conn, string tableName, SQLFormater formater, bool multiUse)
        {
            string buffer = "";
            IList stmtList = new ArrayList();
            tableName = tableName.Trim();
            if (tableName != "")
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SqlTable table = getUserTable(conn, tableName);
                if ((table == null) || (table.name.Trim() == ""))
                {
                    return "";
                }
                if (multiUse)
                {
                    stmtList = KSqlUtil.GetObjectCheckStmt(KSqlCheckObjType.Table, tableName, tableName);
                }
                SqlStmt stmt = buildCreateTableStmt(table);
                stmtList.Add(stmt);
                formater.Format(stmtList);
                buffer = formater.GetBuffer();
                formater.SetBuffer("");
            }
            return buffer;
        }

        public static string GenerateUniqueSQL(IDbConnection conn, string tableName, SQLFormater formater, bool multiUse)
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
            SqlTable table = getUserTable(conn, tableName);
            if ((table == null) || (table.name.Trim() == ""))
            {
                return "";
            }
            stmtList = buildUniqueStmt(conn, table, multiUse);
            formater.Format(stmtList);
            return formater.GetBuffer();
        }

        public static IList generateUniqueSqlDom(IDbConnection conn, bool checkExists = false)
        {
            ArrayList list = new ArrayList();
            IEnumerator enumerator = getUserTables(conn).GetEnumerator();
            while (enumerator.MoveNext())
            {
                SqlTable current = (SqlTable)enumerator.Current;
                IList c = buildUniqueStmt(conn, current, checkExists);
                list.AddRange(c);
            }
            return list;
        }

        public static void getTableDetail(IDbConnection conn, SqlTable table)
        {
            string str = "select * from ALL_TAB_COLUMNS where table_name = '" + table.name + "' ORDER BY COLUMN_ID";
            IDbCommand stmt = conn.CreateCommand();
            IDataReader rs = null;
            try
            {
                stmt.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = stmt.ExecuteReader();
                while (rs.Read())
                {
                    SqlColumn column = new SqlColumn();
                    column.extendedAttributes.Add("OWNER", rs[0]);
                    column.extendedAttributes.Add("TABLE_NAME", rs[1]);
                    column.name = (string)rs[2];
                    column.dataType = (string)rs[3];
                    column.extendedAttributes.Add("DATA_TYPE_MOD", rs[4]);
                    column.extendedAttributes.Add("DATA_TYPE_OWNER", rs[5]);
                    column.length = Convert.ToInt32(rs[6]);
                    column.precision = (rs[7] == DBNull.Value) ? -1 : Convert.ToInt32(rs[7]);
                    column.scale = (rs[8] == DBNull.Value) ? -1 : Convert.ToInt32(rs[8]);
                    column.isNullable = ((string)rs[9]).EqualsIgnoreCase("Y");
                    column.extendedAttributes.Add("COLUMN_ID", rs[10]);
                    column.extendedAttributes.Add("DEFAULT_LENGTH", rs[11]);
                    column.defaultExpr = (rs[12] == DBNull.Value) ? null : ((string)rs[12]);
                    table.columns.Add(column);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
        }

        public static SqlTable getUserTable(IDbConnection conn, string tableName)
        {
            IDbCommand stmt = conn.CreateCommand();
            string str = "select * from USER_TABLES WHERE TABLE_NAME='" + tableName.ToUpper() + "'";
            IDataReader rs = null;
            SqlTable table = new SqlTable();
            try
            {
                stmt.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = stmt.ExecuteReader();
                if (rs.Read())
                {
                    table.name = (string)rs[0];
                    table.extendedAttributes.Add("TABLESPACE_NAME", rs[1]);
                    table.extendedAttributes.Add("CLUSTER_NAME", rs[2]);
                    table.extendedAttributes.Add("IOT_NAME", rs[3]);
                    table.extendedAttributes.Add("PCT_FREE", rs[4]);
                    table.extendedAttributes.Add("PCT_USED", rs[5]);
                    table.extendedAttributes.Add("INI_TRANS", rs[6]);
                    table.extendedAttributes.Add("MAX_TRANS", rs[7]);
                    table.extendedAttributes.Add("INITIAL_EXTENT", rs[8]);
                    table.extendedAttributes.Add("NEXT_EXTENT", rs[9]);
                    table.extendedAttributes.Add("MIN_EXTENTS", rs[10]);
                    table.extendedAttributes.Add("MAX_EXTENTS", rs[11]);
                    table.extendedAttributes.Add("PCT_INCREASE", rs[12]);
                    getTableDetail(conn, table);
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
            return table;
        }

        public static ArrayList getUserTableNames(IDbConnection conn)
        {
            IDbCommand stmt = conn.CreateCommand();
            ArrayList list = new ArrayList();
            string str = "select TABLE_NAME from USER_TABLES ";
            IDataReader rs = null;
            try
            {
                stmt.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = stmt.ExecuteReader();
                while (rs.Read())
                {
                    if (rs[0].ToString().StartsWith("T_BOS"))
                    {
                        list.Add(rs[0]);
                    }
                }
                rs.Close();
            }
            catch (DataException exception)
            {
                throw new DataException(exception.Message, exception.InnerException);
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
            return list;
        }

        public static ArrayList getUserTables(IDbConnection conn)
        {
            IDbCommand stmt = conn.CreateCommand();
            ArrayList list = new ArrayList();
            string str = "select * from USER_TABLES";
            IDataReader rs = null;
            try
            {
                stmt.CommandText = str;
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                rs = stmt.ExecuteReader();
                while (rs.Read())
                {
                    if (rs[0].ToString().StartsWith("T_") && !(rs[0].ToString().Trim() == ""))
                    {
                        SqlTable table = new SqlTable
                        {
                            name = (string)rs[0]
                        };
                        table.extendedAttributes.Add("TABLESPACE_NAME", rs[1]);
                        table.extendedAttributes.Add("CLUSTER_NAME", rs[2]);
                        table.extendedAttributes.Add("IOT_NAME", rs[3]);
                        table.extendedAttributes.Add("PCT_FREE", rs[4]);
                        table.extendedAttributes.Add("PCT_USED", rs[5]);
                        table.extendedAttributes.Add("INI_TRANS", rs[6]);
                        table.extendedAttributes.Add("MAX_TRANS", rs[7]);
                        table.extendedAttributes.Add("INITIAL_EXTENT", rs[8]);
                        table.extendedAttributes.Add("NEXT_EXTENT", rs[9]);
                        table.extendedAttributes.Add("MIN_EXTENTS", rs[10]);
                        table.extendedAttributes.Add("MAX_EXTENTS", rs[11]);
                        table.extendedAttributes.Add("PCT_INCREASE", rs[12]);
                        list.Add(table);
                    }
                }
                conn.Close();
                for (int i = 0; i < list.Count; i++)
                {
                    SqlTable table2 = (SqlTable)list[i];
                    getTableDetail(conn, table2);
                    list[i] = table2;
                }
            }
            finally
            {
                rs.Close();
                KSqlUtil.cleanUp(stmt, rs);
            }
            return list;
        }
    }


   



}
