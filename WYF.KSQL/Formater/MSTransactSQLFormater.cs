using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Exception;
using WYF.KSQL.Parser;
using WYF.KSQL.Util;

namespace WYF.KSQL.Formater
{
    public class MSTransactSQLFormater : TSQLFormater
    {
      
        public MSTransactSQLFormater() : base(null)
        {
            base.max_length_of_index_name = 0x80;
            base.max_length_of_table_name = 0x80;
            base.max_length_of_constraint_name = 0x80;
            base.max_length_of_column_name = 0x80;
            base.max_length_of_column_count = 0x3ff;
            base.max_length_of_row_size = 0x1f40;
        }

        public MSTransactSQLFormater(StringBuilder sb) : base(sb)
        {
            base.max_length_of_index_name = 0x80;
            base.max_length_of_table_name = 0x80;
            base.max_length_of_constraint_name = 0x80;
            base.max_length_of_column_name = 0x80;
            base.max_length_of_column_count = 0x3ff;
            base.max_length_of_row_size = 0x1f40;
        }

        protected override void FormatBlockStmt(SqlBlockStmt stmt)
        {
            if ((stmt.declItemList != null) && (stmt.declItemList.Count > 0))
            {
                int num = 0;
                int count = stmt.declItemList.Count;
                while (num < count)
                {
                    SqlBlockStmt.DeclItem item = (SqlBlockStmt.DeclItem)stmt.declItemList[num];
                    if (item.GetType() == typeof(SqlBlockStmt.DeclVarItem))
                    {
                        SqlBlockStmt.DeclVarItem item2 = (SqlBlockStmt.DeclVarItem)item;
                        base.buffer.Append("DECLARE ");
                        string name = item2.name;
                        base.buffer.Append(name);
                        base.buffer.Append(" AS ");
                        if (item2.dataType.EqualsIgnoreCase("BIGINT"))
                        {
                            base.buffer.Append("BIGINT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("BINARY"))
                        {
                            base.buffer.Append("BINARY (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("BIT"))
                        {
                            base.buffer.Append("BIT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("CHAR"))
                        {
                            base.buffer.Append("CHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("DATETIME"))
                        {
                            base.buffer.Append("DATETIME");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("DECIMAL"))
                        {
                            base.buffer.Append("DECIMAL (");
                            base.buffer.Append(item2.precision);
                            base.buffer.Append(", ");
                            base.buffer.Append(item2.scale);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("FLOAT"))
                        {
                            base.buffer.Append("FLOAT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("IMAGE"))
                        {
                            base.buffer.Append("IMAGE");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("INT"))
                        {
                            base.buffer.Append("INT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("MONEY"))
                        {
                            base.buffer.Append("MONEY");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NCHAR"))
                        {
                            base.buffer.Append("NCHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NTEXT"))
                        {
                            base.buffer.Append("NTEXT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NUMERIC"))
                        {
                            base.buffer.Append("NUMERIC (");
                            base.buffer.Append(item2.precision);
                            base.buffer.Append(", ");
                            base.buffer.Append(item2.scale);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NVARCHAR"))
                        {
                            base.buffer.Append("NVARCHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("REAL"))
                        {
                            base.buffer.Append("REAL");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("SMALLDATETIME"))
                        {
                            base.buffer.Append("SMALLDATETIME");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("SMALLINT"))
                        {
                            base.buffer.Append("SMALLINT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("SMALLMONEY"))
                        {
                            base.buffer.Append("SMALLMONEY");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("SQL_VARIANT"))
                        {
                            base.buffer.Append("SQL_VARIANT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("TEXT"))
                        {
                            base.buffer.Append("TEXT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("TIMESTAMP"))
                        {
                            base.buffer.Append("TIMESTAMP");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("TINYINT"))
                        {
                            base.buffer.Append("TINYINT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("UNIQUEIDENTIFIER"))
                        {
                            base.buffer.Append("UNIQUEIDENTIFIER");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("VARCHAR"))
                        {
                            base.buffer.Append("VARCHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("VARBINARY"))
                        {
                            base.buffer.Append("VARBINARY (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("BLOB"))
                        {
                            base.buffer.Append("IMAGE");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("CLOB"))
                        {
                            base.buffer.Append("TEXT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NCLOB"))
                        {
                            base.buffer.Append("NTEXT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("XMLTYPE"))
                        {
                            base.buffer.Append("XML");
                        }
                        else
                        {
                            base.buffer.Append(item2.dataType);
                        }
                        if (item2.defaultValueExpr != null)
                        {
                            base.buffer.Append("\nSET ");
                            base.buffer.Append(item2.name);
                            base.buffer.Append(" = ");
                            base.FormatExpr(item2.defaultValueExpr);
                        }
                    }
                    else
                    {
                        if (!(item.GetType() == typeof(SqlBlockStmt.DeclCurItem)))
                        {
                            throw new FormaterException("unexpected statement: '" + item + "'");
                        }
                        SqlBlockStmt.DeclCurItem item3 = (SqlBlockStmt.DeclCurItem)item;
                        base.buffer.Append("DECLARE ");
                        string str2 = item3.name;
                        if (!string.IsNullOrEmpty(str2) && str2.StartsWith("@"))
                        {
                            str2 = str2.Substring(1);
                            base.buffer.Append(str2);
                        }
                        else
                        {
                            base.buffer.Append(str2);
                        }
                        base.buffer.Append(" CURSOR FOR ");
                        this.FormatSelectBase(item3.select);
                        base.buffer.Append(";\n");
                    }
                    base.buffer.Append("\n");
                    num++;
                }
                base.buffer.Append("\n");
            }
            foreach (SqlStmt stmt2 in stmt.stmtList)
            {
                base.FormatStmt(stmt2);
                base.buffer.Append("\n");
            }
            if ((stmt.declItemList != null) && (stmt.declItemList.Count > 0))
            {
                int num3 = 0;
                int num4 = stmt.declItemList.Count;
                while (num3 < num4)
                {
                    SqlBlockStmt.DeclItem item4 = (SqlBlockStmt.DeclItem)stmt.declItemList[num3];
                    if (item4.GetType() == typeof(SqlBlockStmt.DeclCurItem))
                    {
                        base.buffer.Append("DEALLOCATE ");
                        base.buffer.Append(item4.name);
                        base.buffer.Append("\n");
                    }
                    num3++;
                }
            }
        }

        protected override void FormatBreakStmt(SqlBreakStmt stmt)
        {
            base.buffer.Append("BREAK");
        }

        protected override void FormatCloseStmt(SqlCloseStmt stmt)
        {
            base.buffer.Append("CLOSE ");
            base.buffer.Append(stmt.curName);
        }

        private void FormatColumnCollate(SqlColumnDef column)
        {
            if ((column.collateName != null) && (column.collateName.Length > 0))
            {
                base.buffer.Append(" COLLATE ");
                base.buffer.Append(column.collateName);
            }
        }

        protected override void FormatColumnDef(SqlColumnDef column)
        {
            if (column.name.IndexOf(" ") >= 0)
            {
                base.buffer.Append("[");
                base.buffer.Append(column.name);
                base.buffer.Append("] ");
            }
            else
            {
                base.buffer.Append(column.name);
                base.buffer.Append(" ");
            }
            if (column.IsComputedColumn)
            {
                base.buffer.Append("AS (");
                base.FormatExpr(column.ComputedColumnExpr);
                base.buffer.Append(")");
                if (column.ComputedColumnisPersisted)
                {
                    base.buffer.Append(Token.PERSISTEDToken.toString());
                }
            }
            else
            {
                if (column.dataType.EqualsIgnoreCase("BIGINT"))
                {
                    base.buffer.Append("BIGINT");
                }
                else if (column.dataType.EqualsIgnoreCase("BINARY"))
                {
                    base.buffer.Append("BINARY (");
                    base.buffer.Append(column.length);
                    base.buffer.Append(")");
                }
                else if (column.dataType.EqualsIgnoreCase("BIT"))
                {
                    base.buffer.Append("BIT");
                }
                else if (column.dataType.EqualsIgnoreCase("CHAR"))
                {
                    base.buffer.Append("CHAR (");
                    base.buffer.Append(column.length);
                    base.buffer.Append(")");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("DATETIME"))
                {
                    base.buffer.Append("DATETIME");
                }
                else if (column.dataType.EqualsIgnoreCase("DECIMAL"))
                {
                    base.buffer.Append("DECIMAL (");
                    base.buffer.Append(column.precision);
                    base.buffer.Append(", ");
                    base.buffer.Append(column.scale);
                    base.buffer.Append(")");
                }
                else if (column.dataType.EqualsIgnoreCase("FLOAT"))
                {
                    base.buffer.Append("FLOAT");
                }
                else if (column.dataType.EqualsIgnoreCase("IMAGE"))
                {
                    base.buffer.Append("IMAGE");
                }
                else if (column.dataType.EqualsIgnoreCase("INT") && !column.autoIncrement)
                {
                    base.buffer.Append("INT");
                }
                else if (column.dataType.EqualsIgnoreCase("MONEY"))
                {
                    base.buffer.Append("MONEY");
                }
                else if (column.dataType.EqualsIgnoreCase("NCHAR"))
                {
                    base.buffer.Append("NCHAR (");
                    base.buffer.Append(column.length);
                    base.buffer.Append(")");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("LONG"))
                {
                    base.buffer.Append("NTEXT");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("NTEXT"))
                {
                    base.buffer.Append("NTEXT");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("NUMERIC"))
                {
                    if ((column.precision > 0) && (column.scale > 0))
                    {
                        base.buffer.Append("DECIMAL (");
                        base.buffer.Append(column.precision);
                        base.buffer.Append(", ");
                        base.buffer.Append(column.scale);
                        base.buffer.Append(")");
                    }
                    else
                    {
                        base.buffer.Append("INT");
                    }
                }
                else if (column.dataType.EqualsIgnoreCase("NVARCHAR"))
                {
                    base.buffer.Append("NVARCHAR (");
                    base.buffer.Append(column.length);
                    base.buffer.Append(")");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("REAL"))
                {
                    base.buffer.Append("REAL");
                }
                else if (column.dataType.EqualsIgnoreCase("SMALLDATETIME"))
                {
                    base.buffer.Append("SMALLDATETIME");
                }
                else if (column.dataType.EqualsIgnoreCase("SMALLINT"))
                {
                    base.buffer.Append("SMALLINT");
                }
                else if (column.dataType.EqualsIgnoreCase("SMALLMONEY"))
                {
                    base.buffer.Append("SMALLMONEY");
                }
                else if (column.dataType.EqualsIgnoreCase("SQL_VARIANT"))
                {
                    base.buffer.Append("SQL_VARIANT");
                }
                else if (column.dataType.EqualsIgnoreCase("TEXT"))
                {
                    base.buffer.Append("TEXT");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("TIMESTAMP"))
                {
                    base.buffer.Append("TIMESTAMP");
                }
                else if (column.dataType.EqualsIgnoreCase("TIMESTAMP(6)"))
                {
                    base.buffer.Append("VARCHAR (30)");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("TINYINT"))
                {
                    base.buffer.Append("TINYINT");
                }
                else if (column.dataType.EqualsIgnoreCase("UNIQUEIDENTIFIER"))
                {
                    base.buffer.Append("UNIQUEIDENTIFIER");
                }
                else if (column.dataType.EqualsIgnoreCase("VARCHAR"))
                {
                    base.buffer.Append("VARCHAR (");
                    base.buffer.Append(column.length);
                    base.buffer.Append(")");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("VARBINARY"))
                {
                    base.buffer.Append("VARBINARY (");
                    base.buffer.Append(column.length);
                    base.buffer.Append(")");
                }
                else if (column.dataType.EqualsIgnoreCase("BLOB"))
                {
                    base.buffer.Append("IMAGE");
                }
                else if (column.dataType.EqualsIgnoreCase("CLOB"))
                {
                    base.buffer.Append("TEXT");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("NCLOB"))
                {
                    base.buffer.Append("NTEXT");
                    this.FormatColumnCollate(column);
                }
                else if (column.dataType.EqualsIgnoreCase("XMLTYPE"))
                {
                    base.buffer.Append("XML");
                    this.FormatColumnCollate(column);
                }
                else
                {
                    base.buffer.Append(column.dataType);
                }
                if (!column.autoIncrement)
                {
                    if (column.isPrimaryKey)
                    {
                        base.buffer.Append(" NOT NULL");
                    }
                    else
                    {
                        string str = column.getNullWord();
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            base.buffer.Append(" ");
                            base.buffer.Append(str);
                        }
                    }
                }
                if (column.defaultValueExpr != null)
                {
                    base.buffer.Append(" DEFAULT ");
                    if ((column.defaultValueExpr is SqlDefaultExpr) && (column.dataType.EqualsIgnoreCase("nvarchar") || column.dataType.EqualsIgnoreCase("varchar")))
                    {
                        ((SqlDefaultExpr)column.defaultValueExpr).text = ((SqlDefaultExpr)column.defaultValueExpr).text.Substring(0, 1) + ((SqlDefaultExpr)column.defaultValueExpr).text.Substring(1).Trim();
                    }
                    base.FormatExpr(column.defaultValueExpr);
                }
                if (column.autoIncrement)
                {
                    base.buffer.Append(" IDENTITY(1, 1)");
                }
                if ((column.containtName != null) && (column.containtName.Length != 0))
                {
                    base.ValidConstraintName(column.containtName);
                    base.buffer.Append(" CONSTRAINT ");
                    base.buffer.Append(column.containtName);
                }
                if (column.isPrimaryKey)
                {
                    base.buffer.Append(" PRIMARY KEY");
                    if (!column.clustered)
                    {
                        base.buffer.Append(" NONCLUSTERED");
                    }
                }
                if (column.isUnique)
                {
                    base.buffer.Append(" UNIQUE");
                    if (column.clustered)
                    {
                        base.buffer.Append(" CLUSTERED");
                    }
                }
                if (column.checkExpr != null)
                {
                    base.buffer.Append(" CHECK (");
                    base.FormatExpr(column.checkExpr);
                    base.buffer.Append(")");
                }
            }
        }

        protected override void FormatContinueStmt(SqlContinueStmt stmt)
        {
            base.buffer.Append("CONTINUE");
        }

        protected override void FormatCreateIndexStmt(SqlCreateIndexStmt stmt)
        {
            base.buffer.Append("CREATE ");
            if (stmt.isUnique)
            {
                base.buffer.Append("UNIQUE ");
            }
            if (stmt.isCluster)
            {
                base.buffer.Append("CLUSTERED ");
            }
            base.buffer.Append("INDEX ");
            base.buffer.Append(base.GetIndexName(stmt));
            base.buffer.Append(" ON ");
            base.buffer.Append(this.FormatTableName(stmt.tableName));
            base.buffer.Append(" (");
            bool flag = false;
            foreach (SqlOrderByItem item in stmt.itemList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                base.FormatExpr(item.expr);
                if (item.mode != 0)
                {
                    base.buffer.Append(" DESC");
                }
                flag = true;
            }
            base.buffer.Append(")");
            if (UUTN.isTempTable(stmt.tableName))
            {
                string tempTableSpace = base.options.GetTempTableSpace();
                if (!string.IsNullOrEmpty(tempTableSpace))
                {
                    base.buffer.Append(" ON " + tempTableSpace);
                }
            }
        }

        protected override void FormatCreateTableStmt(SqlCreateTableStmt stmt)
        {
            base.ValidateCreateTableStmt(stmt);
            base.buffer.Append("CREATE TABLE");
            base.buffer.Append(" ");
            base.buffer.Append(this.FormatTableName(stmt.name));
            base.buffer.Append(" (");
            bool flag = false;
            foreach (SqlColumnDef def in stmt.columnList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                this.FormatColumnDef(def);
                flag = true;
            }
            base.FormatTableConstraintList(stmt.constraintList);
            base.buffer.Append(")");
            string tableSpace = null;
            if (UUTN.isTempTable(stmt.name))
            {
                tableSpace = base.options.GetTempTableSpace();
            }
            if (tableSpace == null)
            {
                tableSpace = stmt.tableSpace;
            }
            this.FormatTableSpace(tableSpace);
        }

        protected override void FormatCursorLoopStmt(SqlCursorLoopStmt stmt)
        {
            string curName = stmt.curName;
            if (!string.IsNullOrEmpty(curName) && curName.StartsWith("@"))
            {
                curName = curName.Substring(1);
            }
            base.buffer.Append("OPEN ");
            base.buffer.Append(curName);
            base.buffer.Append("\n");
            base.buffer.Append("FETCH NEXT FROM ");
            base.buffer.Append(curName);
            base.buffer.Append(" INTO ");
            int num = 0;
            int count = stmt.intoList.Count;
            while (num < count)
            {
                if (num != 0)
                {
                    base.buffer.Append(", ");
                }
                SqlExpr expr = (SqlExpr)stmt.intoList[num];
                base.FormatExpr(expr);
                num++;
            }
            base.buffer.Append("\n");
            base.buffer.Append("WHILE (@@FETCH_STATUS = 0)\n");
            base.buffer.Append("BEGIN\n");
            int num3 = 0;
            int num4 = stmt.stmtList.Count;
            while (num3 < num4)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[num3];
                base.FormatStmt(stmt2);
                base.buffer.Append("\n");
                num3++;
            }
            base.buffer.Append("FETCH NEXT FROM ");
            base.buffer.Append(curName);
            base.buffer.Append(" INTO ");
            int num5 = 0;
            int num6 = stmt.intoList.Count;
            while (num5 < num6)
            {
                if (num5 != 0)
                {
                    base.buffer.Append(", ");
                }
                SqlExpr expr2 = (SqlExpr)stmt.intoList[num5];
                base.FormatExpr(expr2);
                num5++;
            }
            base.buffer.Append("\n");
            base.buffer.Append("END\n");
            base.buffer.Append("CLOSE ");
            base.buffer.Append(curName);
        }

        protected override void FormatDeallocateStmt(SqlDeallocateStmt stmt)
        {
            base.buffer.Append("DEALLOCATE ");
            base.buffer.Append(stmt.curName);
        }

        public void FormatExpr(SqlExpr expr, bool appendBrace)
        {
            if (expr.type == 8)
            {
                if (expr.extendedAttributes()["tableSourceAlias"] != null)
                {
                    base.buffer.Append(expr.extendedAttributes()["tableSourceAlias"]).Append(".").Append("*");
                }
                else
                {
                    base.buffer.Append("*");
                }
            }
            else
            {
                base.FormatExpr(expr, appendBrace);
            }
        }

        protected override void FormatFetchStmt(SqlFetchStmt stmt)
        {
            throw new FormaterException("not support format fetch statement");
        }

        protected override void FormatGotoStmt(SqlGotoStmt stmt)
        {
            base.buffer.Append("GOTO ");
            base.buffer.Append(stmt.name);
        }

        protected override void FormatIdentifierExpr(SqlExpr expr)
        {
            string str = ((SqlIdentifierExpr)expr).value;
            if (str.EqualsIgnoreCase(Token.KSQL_COL_NAME.value))
            {
                str = "COLUMN_NAME";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
            {
                str = "DATA_DEFAULT";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
            {
                str = "ISNULLABLE";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
            {
                str = "TABLE_NAME";
            }
            else if (str.EqualsIgnoreCase(Token.INDNAME.value))
            {
                str = "INDEX_NAME";
            }
            else if (str.EqualsIgnoreCase(Token.TABNAME.value))
            {
                str = "TABLE_NAME";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
            {
                str = "CONSTRAINT_NAME";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
            {
                str = "TABLE_NAME";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
            {
                str = "CONSTRAINT_TYPE";
            }
            if (str.startsWith('"'))
            {
                if (expr.extendedAttributes()["tableSourceAlias"] != null)
                {
                    base.buffer.Append(expr.extendedAttributes()["tableSourceAlias"]).Append(".").Append(str.ToUpper());
                }
                else
                {
                    base.buffer.Append(str.ToUpper());
                }
            }
            else if (str.EqualsIgnoreCase("KSQL_SEQ"))
            {
                this.FormatIdentity(expr);
            }
            else if (expr.extendedAttributes()["tableSourceAlias"] != null)
            {
                base.buffer.Append(expr.extendedAttributes()["tableSourceAlias"]).Append(".").Append(str);
            }
            else
            {
                base.buffer.Append(str);
            }
        }

        protected override void FormatIfStmt(SqlIfStmt stmt)
        {
            base.buffer.Append("IF ");
            base.FormatExpr(stmt.condition);
            base.buffer.Append("\n");
            base.buffer.Append("BEGIN\n");
            for (int i = 0; i < stmt.trueStmtList.Count; i++)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.trueStmtList[i];
                base.FormatStmt(stmt2);
                base.buffer.Append("\n");
            }
            base.buffer.Append("END\n");
            if ((stmt.falseStmtList != null) && (stmt.falseStmtList.Count > 0))
            {
                base.buffer.Append("ELSE\n");
                base.buffer.Append("BEGIN\n");
                for (int j = 0; j < stmt.falseStmtList.Count; j++)
                {
                    SqlStmt stmt3 = (SqlStmt)stmt.falseStmtList[j];
                    base.FormatStmt(stmt3);
                    base.buffer.Append("\n");
                }
                base.buffer.Append("END;\n");
            }
        }

        public void FormatInsertStmt(SqlInsertStmt stmt)
        {
            bool flag2;
            base.buffer.Append("INSERT INTO ");
            SqlInsert insert = stmt.insert;
            string str = this.FormatTableName(insert.tableName);
            base.buffer.Append(str);
            if (insert.columnList.Count != 0)
            {
                base.buffer.Append(" (");
                flag2 = false;
                bool flag = false;
                foreach (object obj2 in insert.columnList)
                {
                    if (obj2.GetType() == typeof(SqlIdentifierExpr))
                    {
                        SqlIdentifierExpr expr = (SqlIdentifierExpr)obj2;
                        if (expr.value.EqualsIgnoreCase("KSQL_SEQ"))
                        {
                            flag = true;
                        }
                        if (!flag)
                        {
                            if (flag2)
                            {
                                base.buffer.Append(", ");
                            }
                            base.buffer.Append(expr.value);
                        }
                    }
                    else
                    {
                        if (!(obj2.GetType() == typeof(string)))
                        {
                            throw new FormaterException("unexpect expression: '" + obj2 + "'");
                        }
                        if (flag2)
                        {
                            base.buffer.Append(", ");
                        }
                        base.buffer.Append((string)obj2);
                    }
                    flag2 = !flag || flag2;
                    flag = false;
                }
                base.buffer.Append(")");
            }
            if (insert.valueList.Count != 0)
            {
                base.buffer.Append(" VALUES (");
                flag2 = false;
                foreach (SqlExpr expr2 in insert.valueList)
                {
                    if (flag2)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(expr2);
                    flag2 = true;
                }
                base.buffer.Append(")");
            }
            else
            {
                base.buffer.Append(" ");
                this.FormatSelectBase(stmt.insert.subQuery);
            }
        }

        protected override void FormatLabelStmt(SqlLabelStmt stmt)
        {
            base.buffer.Append(stmt.name);
            base.buffer.Append(":");
        }

        protected override void FormatMergeStmt(SqlMergeStmt stmt)
        {
            SqlMerge merge = stmt.Merge;
            base.buffer.Append("MERGE INTO ");
            base.buffer.Append(merge.UpdateTable.name);
            if (merge.UpdateTable.alias != null)
            {
                base.buffer.Append(" ");
                base.buffer.Append(merge.UpdateTable.alias);
            }
            base.buffer.Append(" ");
            base.buffer.Append(merge.GetUsingWord());
            if (merge.UsingExpr != null)
            {
                base.FormatExpr(merge.UsingExpr);
            }
            else
            {
                base.buffer.Append(" ");
                base.buffer.Append(merge.UsingTable.name);
            }
            base.buffer.Append(" ");
            if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
            {
                base.buffer.Append(merge.UsingTableAlias);
            }
            base.buffer.Append(" ON ");
            base.buffer.Append("(");
            base.FormatExpr(merge.OnExpr);
            base.buffer.Append(")");
            SqlMergeMatched matchedSql = merge.MatchedSql;
            if (matchedSql != null)
            {
                if (matchedSql.DeleteWhere != null)
                {
                    base.buffer.Append(" WHEN MATCHED ");
                    if (matchedSql.UpdateWhere != null)
                    {
                        base.buffer.Append(" AND ");
                        base.FormatExpr(matchedSql.UpdateWhere);
                    }
                    base.buffer.Append(" AND ");
                    base.FormatExpr(matchedSql.DeleteWhere);
                    base.buffer.Append(" THEN ");
                    base.buffer.Append(" DELETE ");
                }
                base.buffer.Append(" WHEN MATCHED ");
                bool flag = false;
                if (matchedSql.SetClauses.Count == 1)
                {
                    string str = ((SqlBinaryOpExpr)matchedSql.SetClauses[0]).left.getOrgValue();
                    string str2 = ((SqlBinaryOpExpr)matchedSql.SetClauses[0]).right.getOrgValue();
                    if (str.EqualsIgnoreCase(str2))
                    {
                        flag = true;
                    }
                }
                if (matchedSql.UpdateWhere != null)
                {
                    base.buffer.Append(" AND ");
                    base.FormatExpr(matchedSql.UpdateWhere);
                }
                if (!flag)
                {
                    base.buffer.Append(" THEN UPDATE SET ");
                    bool flag2 = false;
                    foreach (SqlExpr expr in matchedSql.SetClauses)
                    {
                        if (flag2)
                        {
                            base.buffer.Append(", ");
                        }
                        base.FormatExpr(expr);
                        flag2 = true;
                    }
                }
            }
            SqlMergeNotMatched notMatchedSql = merge.NotMatchedSql;
            if (notMatchedSql != null)
            {
                base.buffer.Append(" WHEN NOT MATCHED ");
                if (notMatchedSql.InsertWhere != null)
                {
                    base.buffer.Append(" AND ");
                    base.FormatExpr(notMatchedSql.InsertWhere);
                }
                base.buffer.Append(" THEN INSERT (");
                bool flag3 = false;
                foreach (object obj2 in notMatchedSql.InsertColumns)
                {
                    if (flag3)
                    {
                        base.buffer.Append(", ");
                    }
                    if (obj2.GetType() == typeof(SqlIdentifierExpr))
                    {
                        SqlIdentifierExpr expr2 = (SqlIdentifierExpr)obj2;
                        base.buffer.Append(base.FormatColumnName(expr2.value));
                    }
                    else
                    {
                        if (obj2.GetType() != typeof(string))
                        {
                            throw new FormaterException("unexpect expression: '" + obj2 + "'");
                        }
                        base.buffer.Append(base.FormatColumnName((string)obj2));
                    }
                    flag3 = true;
                }
                base.buffer.Append(")");
                base.buffer.Append(" VALUES (");
                flag3 = false;
                foreach (SqlExpr expr3 in notMatchedSql.InsertValues)
                {
                    if (flag3)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(expr3);
                    flag3 = true;
                }
                base.buffer.Append(")");
            }
            base.buffer.Append(";");
        }

        protected override void FormatMethodInvokeExpr(SqlMethodInvokeExpr expr)
        {
            if (expr.owner != null)
            {
                base.FormatExpr(expr.owner);
                base.buffer.Append('.');
            }
            string methodName = expr.methodName;
            if (methodName.EqualsIgnoreCase("ABS"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ABS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ACOS"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ACOS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ASIN"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ASIN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ATAN"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ATAN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ATN2"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ATN2(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("CEILING"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CEILING(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("COS"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("COS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("EXP"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("EXP(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("FLOOR"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("FLOOR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("MOD"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" % ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LOG"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOG(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("POWER"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("POWER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ROUND"))
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("ROUND(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("ROUND(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(")");
                }
            }
            else if (methodName.EqualsIgnoreCase("SIGN"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SIGN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("SIN"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SIN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("SQRT"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SQRT(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("TAN"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TAN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("FN_GCD"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("dbo.FN_GCD(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("FN_LCM"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("dbo.FN_LCM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("CONVERT"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                if (!(expr.parameters[0].GetType() == typeof(SqlMethodInvokeExpr)) && !(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("ERROR");
                }
                string str2 = null;
                if (expr.parameters[0].GetType() == typeof(SqlMethodInvokeExpr))
                {
                    str2 = ((SqlMethodInvokeExpr)expr.parameters[0]).methodName;
                }
                if (expr.parameters[0] is SqlIdentifierExpr)
                {
                    str2 = ((SqlIdentifierExpr)expr.parameters[0]).value;
                }
                if ((str2 == null) || (str2.Length == 0))
                {
                    throw new FormaterException("ERROR");
                }
                if (((str2.EqualsIgnoreCase("BINARY") || str2.EqualsIgnoreCase("VARBINARY")) || (str2.EqualsIgnoreCase("CHAR") || str2.EqualsIgnoreCase("VARCHAR"))) || (((str2.EqualsIgnoreCase("NCHAR") || str2.EqualsIgnoreCase("NVARCHAR")) || (str2.EqualsIgnoreCase("DATETIME") || str2.EqualsIgnoreCase("DECIMAL"))) || (str2.EqualsIgnoreCase("INT") || str2.EqualsIgnoreCase("SMALLINT"))))
                {
                    base.buffer.Append("CONVERT(" + str2.ToUpper());
                    if (expr.parameters[0].GetType() == typeof(SqlMethodInvokeExpr))
                    {
                        SqlMethodInvokeExpr expr2 = (SqlMethodInvokeExpr)expr.parameters[0];
                        if ((expr2.parameters.Count > 0) && (expr2.parameters[0].GetType() == typeof(SqlIntExpr)))
                        {
                            base.buffer.Append("(");
                            base.buffer.Append(((SqlIntExpr)expr2.parameters[0]).value);
                            if (expr2.parameters.Count == 2)
                            {
                                base.buffer.Append(",");
                                base.buffer.Append(((SqlIntExpr)expr2.parameters[1]).value);
                            }
                            base.buffer.Append(")");
                        }
                    }
                    if (expr.parameters[0] is SqlConvertTypeExpr)
                    {
                        base.buffer.Append("(");
                        base.buffer.Append(((SqlConvertTypeExpr)expr.parameters[0]).getLen());
                        base.buffer.Append(")");
                    }
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else if (str2.EqualsIgnoreCase("NUMBER"))
                {
                    base.buffer.Append("CONVERT(NUMERIC, ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else if (str2.EqualsIgnoreCase("BLOB"))
                {
                    base.buffer.Append("CONVERT(IMAGE, ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else if (str2.EqualsIgnoreCase("CLOB"))
                {
                    base.buffer.Append("CONVERT(CLOB, ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else
                {
                    if (!str2.EqualsIgnoreCase("NCLOB"))
                    {
                        throw new FormaterException("not support type:" + str2);
                    }
                    base.buffer.Append("CONVERT(NCLOB, ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
            }
            else if (methodName.EqualsIgnoreCase("CURDATE"))
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("CONVERT(DATETIME, CONVERT(VARCHAR(10) ,GETDATE(), 101))");
            }
            else if (methodName.EqualsIgnoreCase("CURTIME"))
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("CONVERT(VARCHAR(50) ,GETDATE(), 108)");
            }
            else if (methodName.EqualsIgnoreCase("DATEADD") || methodName.EqualsIgnoreCase("DATETIMEADD"))
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("DATEADD(SS, ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                    }
                    base.buffer.Append("DATEADD(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(")");
                }
            }
            else if (methodName.EqualsIgnoreCase("DATEDIFF"))
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("DATEDIFF(SS, ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                    }
                    base.buffer.Append("DATEDIFF(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(")");
                }
            }
            else if (methodName.EqualsIgnoreCase("DAYNAME"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATENAME(DW, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("DAYOFMONTH"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEPART(DAY, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("DAYOFWEEK"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEPART(DW, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("DAYOFYEAR"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEPART(DY, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("GETDATE"))
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("GETDATE()");
            }
            else if (methodName.EqualsIgnoreCase("HOUR"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEPART(HH, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("MINUTE"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEPART(MI, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("MONTH"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEPART(MM, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("MONTHNAME"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATENAME(MM, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("NOW"))
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("GETDATE()");
            }
            else if (methodName.EqualsIgnoreCase("QUARTER"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEPART(QQ, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("SECOND"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEPART(SS, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("WEEK"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEPART(WK, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("YEAR"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("YEAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("TO_DATE"))
            {
                if (expr.parameters.Count > 1)
                {
                    expr.parameters.RemoveAt(1);
                }
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("CONVERT(DATETIME, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("MONTHS_BETWEEN"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEDIFF(MM, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("DAYS_BETWEEN"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEDIFF(DD, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ADD_MONTHS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEADD(month, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ADD_YEARS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEADD(year, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ADD_DAYS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEADD(day, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ADD_HOURS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEADD(hour, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ADD_MINUTES"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEADD(minute, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ADD_SECONDS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEADD(second, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ASCII"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ASCII(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("CHAR"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("CHARINDEX"))
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("CHARINDEX(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                    }
                    base.buffer.Append("CHARINDEX(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(")");
                }
            }
            else if (methodName.EqualsIgnoreCase("CONCAT"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
            }
            else if (methodName.EqualsIgnoreCase("LEFT"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("LEFT(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LEN"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("LEN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LENGTH"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexpect parameters: '" + expr.parameters[1] + "'");
                }
                base.buffer.Append("LEN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LOWER"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOWER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LCASE"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOWER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LTRIM"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LTRIM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("REPLACE"))
            {
                if (expr.parameters.Count != 3)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("REPLACE(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[2]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("RIGHT"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("RIGHT(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("RTRIM"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("RTRIM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("SOUNDEX"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SOUNDEX(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("SUBSTRING"))
            {
                if (expr.parameters.Count != 3)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SUBSTRING(");
                this.FormatExpr((SqlExpr)expr.parameters[0], false);
                base.buffer.Append(", ");
                if (expr.parameters[1].GetType() == typeof(SqlIntExpr))
                {
                    int num = ((SqlIntExpr)expr.parameters[1]).value;
                    if (num < 1)
                    {
                        if (num == 0)
                        {
                            expr.parameters[1] = new SqlIntExpr(1);
                        }
                        if (num < 0)
                        {
                            throw new FormaterException("SUBSTRING parameter2 cannot not smaller then 1.");
                        }
                    }
                }
                this.FormatExpr((SqlExpr)expr.parameters[1], false);
                base.buffer.Append(", ");
                this.FormatExpr((SqlExpr)expr.parameters[2], false);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("TRIM"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LTRIM(RTRIM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append("))");
            }
            else if (methodName.EqualsIgnoreCase("UCASE"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("UPPER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("UPPER"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("UPPER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if ((methodName.EqualsIgnoreCase("TO_DECIMAL") || methodName.EqualsIgnoreCase("DECIMAL")) || methodName.EqualsIgnoreCase("DEC"))
            {
                if (expr.parameters.Count == 1)
                {
                    if (expr.parameters[0].GetType() == typeof(SqlNullExpr))
                    {
                        base.FormatNullExpr((SqlNullExpr)expr.parameters[0]);
                    }
                    else
                    {
                        base.buffer.Append("CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(" AS NUMERIC)");
                    }
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("TO_DECIMAL's parameters num: " + expr.parameters.Count);
                    }
                    base.buffer.Append("CAST(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(" AS NUMERIC(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append("))");
                }
            }
            else if (methodName.EqualsIgnoreCase("TO_BLOB") || methodName.EqualsIgnoreCase("BLOB"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                if (expr.parameters[0].GetType() == typeof(SqlNullExpr))
                {
                    base.FormatNullExpr((SqlNullExpr)expr.parameters[0]);
                }
            }
            else if (methodName.EqualsIgnoreCase("TOCHAR") || methodName.EqualsIgnoreCase("TO_CHAR"))
            {
                if (expr.parameters.Count == 1)
                {
                    if (expr.parameters[0].GetType() == typeof(SqlNullExpr))
                    {
                        base.FormatNullExpr((SqlNullExpr)expr.parameters[0]);
                    }
                    else
                    {
                        base.buffer.Append("CONVERT(VARCHAR(8000), ");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(")");
                    }
                }
                else if ((expr.parameters.Count == 2) && ((SqlCharExpr)expr.parameters[1]).text.ToString().EqualsIgnoreCase("YYYY-MM-DD"))
                {
                    base.buffer.Append("CONVERT(CHAR(10), ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", 20)");
                }
                else if ((expr.parameters.Count == 2) && ((SqlCharExpr)expr.parameters[1]).text.ToString().EqualsIgnoreCase("YYYY-MM-DD HH24:MI:SS"))
                {
                    base.buffer.Append("CONVERT(CHAR(19), ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", 20)");
                }
                else
                {
                    if ((expr.parameters.Count != 3) || !((SqlCharExpr)expr.parameters[1]).text.ToString().EqualsIgnoreCase("NUMBER"))
                    {
                        throw new FormaterException("ERROR");
                    }
                    if (expr.parameters[2].GetType() == typeof(SqlCharExpr))
                    {
                        string[] strArray = expr.parameters[2].ToString().Split(new char[] { 'D' });
                        if ((strArray.Length <= 0) || (strArray.Length >= 3))
                        {
                            throw new FormaterException("TO_CHAR()'s NUMBER style not valid.");
                        }
                        char[] chArray = strArray[0].ToCharArray();
                        char[] chArray2 = strArray[1].ToCharArray();
                        int num2 = 0;
                        for (int i = 0; i < chArray.Length; i++)
                        {
                            if (chArray[i] == '9')
                            {
                                num2++;
                            }
                        }
                        int num4 = 0;
                        for (int j = 0; j < chArray2.Length; j++)
                        {
                            if (chArray2[j] == '9')
                            {
                                num4++;
                            }
                        }
                        base.buffer.Append("CONVERT(VARCHAR, ");
                        base.buffer.Append("CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(" AS DECIMAL(");
                        base.buffer.Append((int)(num2 + num4));
                        base.buffer.Append(", ");
                        base.buffer.Append(num4);
                        base.buffer.Append(")");
                        base.buffer.Append("))");
                    }
                    else if (expr.parameters[2].GetType() == typeof(SqlIntExpr))
                    {
                        base.buffer.Append("LTRIM(RTRIM(STR(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(", 38, ");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(")))");
                    }
                }
            }
            else if (methodName.EqualsIgnoreCase("DATENAME"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATENAME(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ISNULL"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ISNULL(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("NULLIF"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("NULLIF(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("TO_NUMBER"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CONVERT(FLOAT, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("TO_INT") || methodName.EqualsIgnoreCase("TO_INTEGER"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                if (expr.parameters[0].GetType() == typeof(SqlNullExpr))
                {
                    base.FormatNullExpr((SqlNullExpr)expr.parameters[0]);
                }
                else
                {
                    base.buffer.Append("CAST(CONVERT(FLOAT, ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(") AS INT)");
                }
            }
            else if (methodName.EqualsIgnoreCase("TO_NVARCHAR") || methodName.EqualsIgnoreCase("TONVARCHAR"))
            {
                if (expr.parameters.Count == 1)
                {
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                }
                else
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("Unrecognized parameters");
                    }
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                }
            }
            else if (methodName.EqualsIgnoreCase("TO_VARCHAR") || methodName.EqualsIgnoreCase("TOVARCHAR"))
            {
                if (expr.parameters.Count == 1)
                {
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                }
                else
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("Unrecognized parameters");
                    }
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                }
            }
            else if (methodName.EqualsIgnoreCase("NEWID"))
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("Unrecognized parameters");
                }
                base.buffer.Append("LOWER(NEWID())");
            }
            else if (methodName.EqualsIgnoreCase("NEWBOSID"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("Unrecognized parameters");
                }
                base.buffer.Append("DBO.NEWBOSID(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else
            {
                base.FormeatUnkownMethodInvokeExpr(expr);
            }
        }

        protected override void FormatOpenStmt(SqlOpenStmt stmt)
        {
            base.buffer.Append("OPEN ");
            base.buffer.Append(stmt.curName);
        }

        protected override void FormatSetLocalVariantStmt(SqlSetLocalVariantStmt stmt)
        {
            base.buffer.Append("SET ");
            base.FormatExpr(stmt.variant);
            base.buffer.Append(" = ");
            base.FormatExpr(stmt.value);
        }

        protected override void FormatShowColumnsStmt(SqlShowColumnsStmt stmt)
        {
            string str2;
            if (!string.IsNullOrEmpty(stmt.tableName))
            {
                str2 = "select a.name COLUMN_NAME, b.name DATA_TYPE, a.length DEFAULT_LENGTH, a.prec DATA_PRECISION, a.scale DATA_SCALE, a.isnullable NULLABLE, object_name(a.id) as TABLE_NAME from syscolumns a inner join systypes b on a.xusertype = b.xusertype where object_name(a.id) = '" + this.FormatTableName(stmt.tableName) + "' order by colorder";
            }
            else
            {
                str2 = "select a.name COLUMN_NAME, b.name DATA_TYPE, a.length DEFAULT_LENGTH, a.prec DATA_PRECISION, a.scale DATA_SCALE, a.isnullable NULLABLE, object_name(a.id) as TABLE_NAME from syscolumns a inner join systypes b on a.xusertype = b.xusertype order by colorder";
            }
            base.buffer.Append(str2);
        }

        protected override void FormatShowTablesStmt(SqlShowTablesStmt stmt)
        {
            string str = "select name from sysobjects where type = N'U' order by name";
            base.buffer.Append(str);
        }

        protected void FormatTableConstraint(SqlTableConstraint constraint)
        {
            base.ValidConstraintName(constraint.name);
            if ((constraint.name != null) && (constraint.name.Length != 0))
            {
                base.buffer.Append("CONSTRAINT ");
                base.buffer.Append(constraint.name);
            }
            if (constraint.GetType() == typeof(SqlTablePrimaryKey))
            {
                SqlTablePrimaryKey key = (SqlTablePrimaryKey)constraint;
                if (key.clustered)
                {
                    base.buffer.Append(" PRIMARY KEY (");
                }
                else
                {
                    base.buffer.Append(" PRIMARY KEY NONCLUSTERED (");
                }
                bool flag = false;
                foreach (string str in key.columnList)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.buffer.Append(str);
                    flag = true;
                }
                base.buffer.Append(")");
            }
            else if (constraint.GetType() == typeof(SqlTableUnique))
            {
                SqlTableUnique unique = (SqlTableUnique)constraint;
                if (unique.clustered)
                {
                    base.buffer.Append(" UNIQUE CLUSTERED (");
                }
                else
                {
                    base.buffer.Append(" UNIQUE (");
                }
                bool flag2 = false;
                foreach (string str2 in unique.columnList)
                {
                    if (flag2)
                    {
                        base.buffer.Append(", ");
                    }
                    base.buffer.Append(str2);
                    flag2 = true;
                }
                base.buffer.Append(")");
            }
            else if (constraint.GetType() == typeof(SqlTableUnique))
            {
                SqlTableUnique unique2 = (SqlTableUnique)constraint;
                base.buffer.Append(" UNIQUE (");
                bool flag3 = false;
                foreach (string str3 in unique2.columnList)
                {
                    if (flag3)
                    {
                        base.buffer.Append(", ");
                    }
                    base.buffer.Append(str3);
                    flag3 = true;
                }
                base.buffer.Append(")");
            }
            else if (constraint.GetType() == typeof(SqlTableForeignKey))
            {
                SqlTableForeignKey key2 = (SqlTableForeignKey)constraint;
                base.buffer.Append(" FOREIGN KEY (");
                bool flag4 = false;
                foreach (string str4 in key2.columnList)
                {
                    if (flag4)
                    {
                        base.buffer.Append(", ");
                    }
                    base.buffer.Append(str4);
                    flag4 = true;
                }
                base.buffer.Append(")");
                base.buffer.Append(" REFERENCES ");
                base.buffer.Append(key2.refTableName);
                base.buffer.Append(" (");
                flag4 = false;
                foreach (string str5 in key2.refColumnList)
                {
                    if (flag4)
                    {
                        base.buffer.Append(", ");
                    }
                    base.buffer.Append(str5);
                    flag4 = true;
                }
                base.buffer.Append(")");
            }
            else
            {
                if (constraint.GetType() != typeof(SqlTableCheck))
                {
                    throw new FormaterException("not support constraint:" + constraint);
                }
                SqlTableCheck check = (SqlTableCheck)constraint;
                base.buffer.Append(" CHECK (");
                this.FormatExpr(check.expr, false);
                base.buffer.Append(")");
            }
        }

        protected override string FormatTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                string message = "\"tableName\" is empty!";
                throw new IllegalStateException(message);
            }
            if (tableName.IndexOf(" ") >= 0)
            {
                tableName = "[" + tableName + "]";
            }
            if (tableName.Length > 30)
            {
                throw new System.Exception("column name greate than 30");
            }
            return tableName;
        }

        protected void FormatTableSpace(string tableSpace)
        {
            if ((tableSpace != null) && (tableSpace.Length > 0))
            {
                base.buffer.Append(" ON " + tableSpace);
            }
        }

        protected override void FormatUpdateStmt(SqlUpdateStmt stmt)
        {
            SqlUpdate update = stmt.update;
            base.buffer.Append("UPDATE ");
            if (update.updateTable.alias != null)
            {
                base.buffer.Append(update.updateTable.alias.ToUpper());
            }
            else
            {
                base.buffer.Append(this.FormatTableName(update.updateTable.name));
            }
            base.buffer.Append(" SET ");
            ArrayList list = new ArrayList();
            int num = 0;
            bool flag = false;
            foreach (AbstractUpdateItem item in update.updateList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                if (item.GetType() == typeof(SqlUpdateItem))
                {
                    SqlUpdateItem item2 = (SqlUpdateItem)item;
                    base.buffer.Append(base.FormatColumnName(item2.name));
                    base.buffer.Append(" = ");
                    base.FormatExpr(item2.expr);
                }
                else
                {
                    if (!(item.GetType() == typeof(SubQueryUpdateItem)))
                    {
                        throw new FormaterException("unexpect update item: '" + item + "'");
                    }
                    SubQueryUpdateItem item3 = (SubQueryUpdateItem)item;
                    list.Add(item);
                    for (int j = 0; j < item3.columnList.Count; j++)
                    {
                        string name = (string)item3.columnList[j];
                        if (item3.extendedAttributes()["tableSourceAlias"] != null)
                        {
                            name = item3.extendedAttributes()["tableSourceAlias"] + "." + name;
                        }
                        if (j != 0)
                        {
                            base.buffer.Append(", ");
                        }
                        base.buffer.Append(base.FormatColumnName(name));
                        base.buffer.Append(" = ");
                        if (!(item3.subQuery.GetType() == typeof(SqlSelect)))
                        {
                            throw new FormaterException("unexpect subquery item: '" + item3.subQuery + "'");
                        }
                        SqlSelect subQuery = (SqlSelect)item3.subQuery;
                        SqlSelectItem item4 = (SqlSelectItem)subQuery.selectList[j];
                        if (((subQuery != null) && (subQuery.tableSource != null)) && !string.IsNullOrEmpty(subQuery.tableSource.alias))
                        {
                            item4.expr.addExtAttr("tableSourceAlias", subQuery.tableSource.alias);
                        }
                        this.FormatExpr(item4.expr, false);
                        num++;
                        flag = true;
                    }
                }
                flag = true;
            }
            if (update.tableSource != null)
            {
                throw new FormaterException("update's tableSource is null");
            }
            if (list.Count == 0)
            {
                if (update.updateTable.alias != null)
                {
                    base.buffer.Append(" FROM ");
                    base.buffer.Append(this.FormatTableName(update.updateTable.name));
                    base.buffer.Append(" " + update.updateTable.alias.ToUpper() + " ");
                }
            }
            else
            {
                bool flag2 = false;
                for (int k = 0; k < list.Count; k++)
                {
                    if (k == 0)
                    {
                        base.buffer.Append(" FROM ");
                        if (update.updateTable.alias != null)
                        {
                            base.buffer.Append(this.FormatTableName(update.updateTable.name));
                            base.buffer.Append(" " + update.updateTable.alias.ToUpper());
                            flag2 = true;
                        }
                    }
                    SubQueryUpdateItem item5 = (SubQueryUpdateItem)list[k];
                    if (!(item5.subQuery.GetType() == typeof(SqlSelect)))
                    {
                        throw new FormaterException("unexpect queryItem subQuery: '" + item5 + "'");
                    }
                    StringBuilder buffer = base.buffer;
                    base.buffer = new StringBuilder();
                    SqlSelect select2 = (SqlSelect)item5.subQuery;
                    this.FormatTableSource(select2.tableSource);
                    if (base.buffer.Length > 0)
                    {
                        if (flag2)
                        {
                            buffer.Append(", ");
                        }
                        buffer.Append(base.buffer.ToString());
                        flag2 = true;
                    }
                    base.buffer = buffer;
                }
            }
            flag = false;
            for (int i = 0; i < list.Count; i++)
            {
                SubQueryUpdateItem item6 = (SubQueryUpdateItem)list[i];
                if (!(item6.subQuery.GetType() == typeof(SqlSelect)))
                {
                    throw new FormaterException("not support query item:" + item6);
                }
                SqlSelect select3 = (SqlSelect)item6.subQuery;
                if (select3.condition != null)
                {
                    if (flag)
                    {
                        base.buffer.Append(" AND ");
                    }
                    else
                    {
                        base.buffer.Append(" WHERE ");
                    }
                    base.FormatExpr(select3.condition);
                    flag = true;
                }
            }
            if (update.condition != null)
            {
                if (flag)
                {
                    base.buffer.Append(" AND ");
                }
                else
                {
                    base.buffer.Append(" WHERE ");
                }
                base.FormatExpr(update.condition);
                flag = true;
            }
        }

        protected override void FormatWhileStmt(SqlWhileStmt stmt)
        {
            base.buffer.Append("WHILE ");
            base.FormatExpr(stmt.condition);
            base.buffer.Append("\n");
            base.buffer.Append("BEGIN\n");
            for (int i = 0; i < stmt.stmtList.Count; i++)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[i];
                base.FormatStmt(stmt2);
                base.buffer.Append("\n");
            }
            base.buffer.Append("END\n");
        }
    }





}
