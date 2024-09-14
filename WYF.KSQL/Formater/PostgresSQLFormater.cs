using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Parser;
using WYF.KSQL.Util;

namespace WYF.KSQL.Formater
{
    public class PostgresSQLFormater : SQLFormater
    {
        // Methods
        public PostgresSQLFormater() : base(null)
        {
        }

        public PostgresSQLFormater(StringBuilder sb) : base(sb)
        {
        }

        protected override void FormatAlterTableStmt(SqlAlterTableStmt stmt)
        {
            base.buffer.Append("ALTER TABLE ");
            base.buffer.Append(stmt.tableName.ToLowerInvariant());
            if (stmt.item.GetType() == typeof(SqlAlterTableAddItem))
            {
                SqlAlterTableAddItem item = (SqlAlterTableAddItem)stmt.item;
                base.buffer.Append(" ADD ");
                bool flag = false;
                foreach (SqlColumnDef def in item.columnDefItemList)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    this.FormatColumnDef(def);
                    flag = true;
                }
                flag = false;
                foreach (SqlTableConstraint constraint in item.constraintItemList)
                {
                    try
                    {
                        this.FormatTableConstraint(constraint);
                    }
                    catch (FormaterException exception)
                    {
                        throw new FormaterException("alter table statement invalid. table name is '" + stmt.tableName + "', " + exception.Message, exception);
                    }
                }
            }
            else if (stmt.item.GetType() == typeof(SqlAlterTableDropItem))
            {
                SqlAlterTableDropItem item2 = (SqlAlterTableDropItem)stmt.item;
                base.buffer.Append(" DROP ");
                bool flag2 = false;
                foreach (string str in item2.columnDefItemList)
                {
                    if (flag2)
                    {
                        base.buffer.Append(", ");
                    }
                    else
                    {
                        base.buffer.Append("COLUMN ");
                    }
                    base.buffer.Append(str);
                    flag2 = true;
                }
                flag2 = false;
                foreach (string str2 in item2.constraintItemList)
                {
                    if (flag2)
                    {
                        base.buffer.Append(", ");
                    }
                    else
                    {
                        base.buffer.Append("CONSTRAINT ");
                    }
                    base.buffer.Append(str2);
                    flag2 = true;
                }
            }
            else
            {
                if (stmt.item.GetType() != typeof(SqlAlterTableAlterColumnItem))
                {
                    throw new FormaterException("TODO");
                }
                SqlAlterTableAlterColumnItem item3 = (SqlAlterTableAlterColumnItem)stmt.item;
                base.buffer.Append(" ALTER COLUMN ");
                this.FormatColumnDef(item3.columnDef);
            }
        }

        protected override void FormatBinaryOpExpr(SqlBinaryOpExpr expr, bool appendBrace)
        {
            if (expr.Operator == 13)
            {
                base.buffer.Append("(");
                base.FormatExpr(expr.left);
                base.buffer.Append(" IS NULL)");
            }
            else if (expr.Operator == 0x29)
            {
                base.buffer.Append("(");
                base.FormatExpr(expr.left);
                base.buffer.Append(" IS NOT NULL)");
            }
            else if (expr.Operator == 20)
            {
                base.FormatExpr(expr.left);
                base.buffer.Append(".");
                base.FormatExpr(expr.right);
            }
            else if (expr.Operator == 0x2b)
            {
                this.FormatExpr(expr.left, false);
                base.buffer.Append(" ESCAPE ");
                this.FormatExpr(expr.right, false);
            }
            else if (expr.Operator == 10)
            {
                base.FormatExpr(expr.left);
                base.buffer.Append(" = ");
                if ((expr.right is SqlCharExpr) && (expr.left is SqlIdentifierExpr))
                {
                    string str = ((SqlIdentifierExpr)expr.left).value.ToUpper();
                    if ((((str == Token.KSQL_COL_NAME.value) || (str == Token.KSQL_COL_DEFAULT.value)) || ((str == Token.KSQL_COL_TABNAME.value) || (str == Token.INDNAME.value))) || (((str == Token.TABNAME.value) || (str == Token.KSQL_CONS_NAME.value)) || ((str == Token.KSQL_CONS_TABNAME.value) || (str == Token.KSQL_CONS_TYPE.value))))
                    {
                        this.FormatChar((SqlCharExpr)expr.right, true);
                    }
                    else
                    {
                        base.FormatExpr(expr.right);
                    }
                }
                else
                {
                    base.FormatExpr(expr.right);
                }
            }
            else if (expr.Operator == 0)
            {
                if (appendBrace)
                {
                    base.buffer.Append("(");
                }
                if (expr.left.GetType() == typeof(SqlBinaryOpExpr))
                {
                    SqlBinaryOpExpr left = (SqlBinaryOpExpr)expr.left;
                    if (left.Operator == 0)
                    {
                        this.FormatExpr(left, false);
                    }
                    else
                    {
                        base.FormatExpr(left);
                    }
                }
                else
                {
                    base.FormatExpr(expr.left);
                }
                base.buffer.Append(" + ");
                if (expr.right.GetType() == typeof(SqlBinaryOpExpr))
                {
                    SqlBinaryOpExpr right = (SqlBinaryOpExpr)expr.right;
                    if (right.Operator == 0)
                    {
                        this.FormatExpr(right, false);
                    }
                    else
                    {
                        base.FormatExpr(right);
                    }
                }
                else
                {
                    base.FormatExpr(expr.right);
                }
                if (appendBrace)
                {
                    base.buffer.Append(")");
                }
            }
            else if (expr.Operator == 1)
            {
                base.FormatExpr(expr.left);
                base.buffer.Append(" AS ");
                if (expr.right.GetType() == typeof(SqlIdentifierExpr))
                {
                    string str2 = ((SqlIdentifierExpr)expr.right).value.ToLowerInvariant();
                    base.buffer.Append(str2);
                }
                else if (expr.right.GetType() == typeof(SqlCharExpr))
                {
                    string str3 = ((SqlCharExpr)expr.right).text.ToLowerInvariant();
                    base.buffer.Append(str3);
                }
                else if (expr.right.GetType() == typeof(SqlNCharExpr))
                {
                    string str4 = ((SqlNCharExpr)expr.right).text.ToLowerInvariant();
                    base.buffer.Append(str4);
                }
                else
                {
                    base.FormatExpr(expr.right);
                }
            }
            else
            {
                if (appendBrace)
                {
                    base.buffer.Append("(");
                }
                base.FormatExpr(expr.left);
                switch (expr.Operator)
                {
                    case 0:
                        base.buffer.Append(" + ");
                        break;

                    case 1:
                        base.buffer.Append(" AS ");
                        break;

                    case 2:
                        base.buffer.Append(" = ");
                        break;

                    case 3:
                        throw new FormaterException("not support");

                    case 4:
                        throw new FormaterException("not support");

                    case 5:
                        throw new FormaterException("not support");

                    case 7:
                        base.buffer.Append(" AND ");
                        break;

                    case 8:
                        base.buffer.Append(" OR ");
                        break;

                    case 9:
                        base.buffer.Append(" / ");
                        break;

                    case 10:
                        base.buffer.Append(" = ");
                        break;

                    case 11:
                        base.buffer.Append(" > ");
                        break;

                    case 12:
                        base.buffer.Append(" >= ");
                        break;

                    case 14:
                        base.buffer.Append(" < ");
                        break;

                    case 15:
                        base.buffer.Append(" <= ");
                        break;

                    case 0x10:
                        base.buffer.Append(" <> ");
                        break;

                    case 0x11:
                        throw new FormaterException("not support");

                    case 0x12:
                        base.buffer.Append(" LIKE ");
                        break;

                    case 0x13:
                        base.buffer.Append(" >> ");
                        break;

                    case 20:
                        base.buffer.Append(".");
                        break;

                    case 0x15:
                        base.buffer.Append(" % ");
                        break;

                    case 0x16:
                        base.buffer.Append(" * ");
                        break;

                    case 0x17:
                        base.buffer.Append(" != ");
                        break;

                    case 0x18:
                        base.buffer.Append(" !< ");
                        break;

                    case 0x19:
                        base.buffer.Append(" !> ");
                        break;

                    case 0x1a:
                        base.buffer.Append(" - ");
                        break;

                    case 0x1b:
                        base.buffer.Append(" UNION ");
                        break;

                    case 40:
                        base.buffer.Append(" NOT LIKE ");
                        break;

                    case 0x2a:
                        base.buffer.Append(" || ");
                        break;

                    default:
                        throw new FormaterException("not support");
                }
                base.FormatExpr(expr.right);
                if (appendBrace)
                {
                    base.buffer.Append(")");
                }
            }
        }

        protected override void FormatBlockStmt(SqlBlockStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatBreakStmt(SqlBreakStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatChar(SqlCharExpr expr)
        {
            this.FormatChar(expr, false);
        }

        private void FormatChar(SqlCharExpr expr, bool toLower)
        {
            string text;
            if (toLower)
            {
                text = expr.text.ToLower();
            }
            else
            {
                text = expr.text;
            }
            if (text.EqualsIgnoreCase(Token.KSQL_CT_P.value))
            {
                text = "P";
            }
            else if (text.EqualsIgnoreCase(Token.KSQL_CT_F.value))
            {
                text = "R";
            }
            else if (text.EqualsIgnoreCase(Token.KSQL_CT_U.value))
            {
                text = "U";
            }
            else if (text.EqualsIgnoreCase(Token.KSQL_CT_C.value))
            {
                text = "C";
            }
            base.buffer.Append("'");
            base.buffer.Append(text);
            base.buffer.Append("'");
        }

        protected override void FormatCloseStmt(SqlCloseStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatColumnDef(SqlColumnDef column)
        {
            if (column.name == null)
            {
                throw new FormaterException("column name is null");
            }
            if (((base.max_length_of_constraint_name != -1) && (column.name != null)) && (column.name.Length > base.max_length_of_constraint_name))
            {
                throw new FormaterException(string.Concat(new object[] { "column name greate than ", base.max_length_of_column_name, ", column name is '", column.name, "'" }));
            }
            base.buffer.Append(column.name);
            base.buffer.Append(" ");
            if (column.dataType.EqualsIgnoreCase("BIGINT") || column.dataType.EqualsIgnoreCase("INT8"))
            {
                base.buffer.Append("BIGINT");
            }
            else if (column.dataType.EqualsIgnoreCase("BIGSERIAL") || column.dataType.EqualsIgnoreCase("SERIAL8"))
            {
                base.buffer.Append("BIGSERIAL");
            }
            else if (column.dataType.EqualsIgnoreCase("BIT"))
            {
                base.buffer.Append("BIT (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("BIT VARYING") || column.dataType.EqualsIgnoreCase("VARBIT"))
            {
                base.buffer.Append("BIT VARYING (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("BOOLEAN") || column.dataType.EqualsIgnoreCase("BOOL"))
            {
                base.buffer.Append("BOOLEAN");
            }
            else if (column.dataType.EqualsIgnoreCase("BOX"))
            {
                base.buffer.Append("BOX");
            }
            else if (column.dataType.EqualsIgnoreCase("BYTEA"))
            {
                base.buffer.Append("BYTEA");
            }
            else if (column.dataType.EqualsIgnoreCase("CHARACTER") || column.dataType.EqualsIgnoreCase("CHAR"))
            {
                base.buffer.Append("CHARACTER (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("CHARACTER VARYING") || column.dataType.EqualsIgnoreCase("VARCHAR"))
            {
                base.buffer.Append("CHARACTER VARYING (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("CIDR"))
            {
                base.buffer.Append("CIDR");
            }
            else if (column.dataType.EqualsIgnoreCase("CIRCLE"))
            {
                base.buffer.Append("CIRCLE");
            }
            else if (column.dataType.EqualsIgnoreCase("DATE"))
            {
                base.buffer.Append("DATE");
            }
            else if (column.dataType.EqualsIgnoreCase("DOUBLE PRECISION"))
            {
                base.buffer.Append("DOUBLE PRECISION");
            }
            else if (column.dataType.EqualsIgnoreCase("INET"))
            {
                base.buffer.Append("INET");
            }
            else if ((column.dataType.EqualsIgnoreCase("INTEGER") || column.dataType.EqualsIgnoreCase("INT")) || column.dataType.EqualsIgnoreCase("INT4"))
            {
                if (column.autoIncrement)
                {
                    base.buffer.Append("SERIAL");
                }
                else
                {
                    base.buffer.Append("INTEGER");
                }
            }
            else if (column.dataType.EqualsIgnoreCase("INTERVAL"))
            {
                base.buffer.Append("INTERVAL (");
                base.buffer.Append(column.precision);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("LINE"))
            {
                base.buffer.Append("LINE");
            }
            else if (column.dataType.EqualsIgnoreCase("LSEG"))
            {
                base.buffer.Append("LSEG");
            }
            else if (column.dataType.EqualsIgnoreCase("MACADDR"))
            {
                base.buffer.Append("MACADDR");
            }
            else if (column.dataType.EqualsIgnoreCase("MONEY"))
            {
                base.buffer.Append("MONEY");
            }
            else if (column.dataType.EqualsIgnoreCase("NUMERIC"))
            {
                base.buffer.Append("NUMERIC (");
                base.buffer.Append(column.precision);
                base.buffer.Append(", ");
                base.buffer.Append(column.scale);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("PATH"))
            {
                base.buffer.Append("PATH");
            }
            else if (column.dataType.EqualsIgnoreCase("POINT"))
            {
                base.buffer.Append("POINT");
            }
            else if (column.dataType.EqualsIgnoreCase("POLYGON"))
            {
                base.buffer.Append("POLYGON");
            }
            else if (column.dataType.EqualsIgnoreCase("REAL") || column.dataType.EqualsIgnoreCase("FLOAT4"))
            {
                base.buffer.Append("REAL");
            }
            else if (column.dataType.EqualsIgnoreCase("SMALLINT") || column.dataType.EqualsIgnoreCase("INT2"))
            {
                base.buffer.Append("SMALLINT");
            }
            else if (column.dataType.EqualsIgnoreCase("SERIAL") || column.dataType.EqualsIgnoreCase("SERIAL4"))
            {
                base.buffer.Append("SERIAL");
            }
            else if (column.dataType.EqualsIgnoreCase("TEXT") || column.dataType.EqualsIgnoreCase("TEXT"))
            {
                base.buffer.Append("TEXT");
            }
            else if (column.dataType.EqualsIgnoreCase("TIME") || column.dataType.EqualsIgnoreCase("TIME WITHOUT TIME ZONE"))
            {
                if (column.precision == -1)
                {
                    base.buffer.Append("TIME WITHOUT TIME ZONE");
                }
                else
                {
                    base.buffer.Append("TIME (");
                    base.buffer.Append(column.precision);
                    base.buffer.Append(") WITHOUT TIME ZONE");
                }
            }
            else if (column.dataType.EqualsIgnoreCase("TIMETZ") || column.dataType.EqualsIgnoreCase("TIME WITH TIME ZONE"))
            {
                if (column.precision == -1)
                {
                    base.buffer.Append("TIME WITH TIME ZONE");
                }
                else
                {
                    base.buffer.Append("TIME (");
                    base.buffer.Append(column.precision);
                    base.buffer.Append(") WITH TIME ZONE");
                }
            }
            else if (column.dataType.EqualsIgnoreCase("TIMESTAMP") || column.dataType.EqualsIgnoreCase("TIMESTAMP WITHOUT TIME ZONE"))
            {
                if (column.precision == -1)
                {
                    base.buffer.Append("TIMESTAMP WITHOUT TIME ZONE");
                }
                else
                {
                    base.buffer.Append("TIMESTAMP (");
                    base.buffer.Append(column.precision);
                    base.buffer.Append(") WITHOUT TIME ZONE");
                }
            }
            else if (column.dataType.EqualsIgnoreCase("TIMESTAMPZ") || column.dataType.EqualsIgnoreCase("TIMESTAMP WITH TIME ZONE"))
            {
                if (column.precision == -1)
                {
                    base.buffer.Append("TIMESTAMP WITH TIME ZONE");
                }
                else
                {
                    base.buffer.Append("TIMESTAMP (");
                    base.buffer.Append(column.precision);
                    base.buffer.Append(") WITH TIME ZONE");
                }
            }
            else if (column.dataType.EqualsIgnoreCase("BLOB"))
            {
                base.buffer.Append("BYTEA");
            }
            else if (column.dataType.EqualsIgnoreCase("CLOB"))
            {
                base.buffer.Append("TEXT");
            }
            else if (column.dataType.EqualsIgnoreCase("DATETIME"))
            {
                base.buffer.Append("TIMESTAMP WITHOUT TIME ZONE");
            }
            else if (column.dataType.EqualsIgnoreCase("DECIMAL"))
            {
                base.buffer.Append("NUMERIC (");
                base.buffer.Append(column.precision);
                base.buffer.Append(", ");
                base.buffer.Append(column.scale);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("NCHAR"))
            {
                base.buffer.Append("CHARACTER (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("NCLOB"))
            {
                base.buffer.Append("TEXT");
            }
            else if (column.dataType.EqualsIgnoreCase("NVARCHAR"))
            {
                base.buffer.Append("VARCHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("VARBINARY"))
            {
                base.buffer.Append("BIT VARYING (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else
            {
                if (!column.dataType.EqualsIgnoreCase("XMLTYPE") && !column.dataType.EqualsIgnoreCase("XML"))
                {
                    throw new FormaterException("not support datatype, column name is '" + column.name + "' datatype is '" + column.dataType + "'");
                }
                base.buffer.Append("xml");
            }
            bool allowNull = column.allowNull;
            if (column.allowNull)
            {
                if (!column.isPrimaryKey)
                {
                    base.buffer.Append(" NULL");
                }
            }
            else
            {
                bool flag2 = column.allowNull;
                if (!column.allowNull)
                {
                    base.buffer.Append(" NOT NULL");
                }
            }
            if (column.defaultValueExpr != null)
            {
                base.buffer.Append(" DEFAULT ");
                base.FormatExpr(column.defaultValueExpr);
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
            }
            if (column.isUnique)
            {
                base.buffer.Append(" UNIQUE");
            }
            if (column.checkExpr != null)
            {
                base.buffer.Append(" CHECK (");
                base.FormatExpr(column.checkExpr);
                base.buffer.Append(")");
            }
        }

        protected override void FormatContinueStmt(SqlContinueStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatCreateIndexStmt(SqlCreateIndexStmt stmt)
        {
            if (stmt.isUnique)
            {
                base.buffer.Append("CREATE UNIQUE INDEX ");
            }
            else
            {
                base.buffer.Append("CREATE INDEX ");
            }
            base.buffer.Append(stmt.indexName);
            base.buffer.Append(" ON ");
            base.buffer.Append(stmt.tableName);
            base.buffer.Append(" (");
            bool flag = false;
            foreach (SqlOrderByItem item in stmt.itemList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                base.FormatExpr(item.expr);
                int mode = item.mode;
                flag = true;
            }
            base.buffer.Append(")");
        }

        protected override void FormatCreateTableStmt(SqlCreateTableStmt stmt)
        {
            base.ValidateCreateTableStmt(stmt);
            base.buffer.Append("CREATE");
            int num = 0;
            if (UUTN.isGlobalTempTable(stmt.name))
            {
                base.buffer.Append(" TEMPORARY");
                stmt.name = stmt.name.Substring(2);
                num = 1;
            }
            else if (UUTN.isTempTable(stmt.name))
            {
                base.buffer.Append(" TEMPORARY");
                stmt.name = stmt.name.Substring(1);
                num = 2;
            }
            base.buffer.Append(" TABLE ");
            base.buffer.Append(stmt.name);
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
            this.FormatTableConstraintList(stmt.constraintList);
            base.buffer.Append(")");
            switch (num)
            {
                case 1:
                    base.buffer.Append(" ON COMMIT DELETE ROWS ");
                    return;

                case 2:
                    base.buffer.Append(" ON COMMIT PRESERVE ROWS ");
                    break;
            }
        }

        protected override void FormatCursorLoopStmt(SqlCursorLoopStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatDateTimeExpr(SqlDateTimeExpr expr)
        {
            if (expr.getYear() < 10)
            {
                base.buffer.Append("'0");
            }
            else
            {
                base.buffer.Append("'");
            }
            base.buffer.Append(expr.getYear());
            if (expr.getMonth() < 10)
            {
                base.buffer.Append("-0");
            }
            else
            {
                base.buffer.Append("-");
            }
            base.buffer.Append(expr.getMonth());
            if (expr.getDate() < 10)
            {
                base.buffer.Append("-0");
            }
            else
            {
                base.buffer.Append("-");
            }
            base.buffer.Append(expr.getDate());
            if (expr.getHour() < 10)
            {
                base.buffer.Append(" 0");
            }
            else
            {
                base.buffer.Append(" ");
            }
            base.buffer.Append(expr.getHour());
            if (expr.getMinute() < 10)
            {
                base.buffer.Append(":0");
            }
            else
            {
                base.buffer.Append(":");
            }
            base.buffer.Append(expr.getMinute());
            if (expr.getSecond() < 10)
            {
                base.buffer.Append(":0");
            }
            else
            {
                base.buffer.Append(":");
            }
            base.buffer.Append(expr.getSecond());
            base.buffer.Append("'");
        }

        protected override void FormatDeallocateStmt(SqlDeallocateStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatDeleteStmt(SqlDeleteStmt stmt)
        {
            SqlDelete delete = stmt.delete;
            base.buffer.Append("DELETE ");
            if (!string.IsNullOrEmpty(delete.tableName))
            {
                if (delete.tableSource == null)
                {
                    base.buffer.Append("FROM ");
                    base.buffer.Append(this.FormatTableName(delete.tableName));
                    if (delete.condition != null)
                    {
                        base.buffer.Append(" WHERE ");
                        base.FormatExpr(delete.condition);
                    }
                }
                else
                {
                    base.buffer.Append(" FROM ");
                    base.buffer.Append(delete.tableName);
                    base.buffer.Append(" using ");
                    this.FormatTableSource(delete.tableSource);
                    if (delete.condition != null)
                    {
                        base.buffer.Append(" AND ");
                        base.FormatExpr(delete.condition);
                    }
                }
            }
            else
            {
                base.buffer.Append("FROM ");
                if (delete.tableSource != null)
                {
                    if (delete.tableSource.GetType() == typeof(SqlJoinedTableSource))
                    {
                        this.FormatDeleteUsing(delete.tableSource);
                        if (delete.condition != null)
                        {
                            base.buffer.Append(" AND ");
                            base.FormatExpr(delete.condition);
                        }
                    }
                    else
                    {
                        this.FormatDeleteUsing(delete.tableSource);
                        if (delete.condition != null)
                        {
                            base.buffer.Append(" WHERE ");
                            base.FormatExpr(delete.condition);
                        }
                    }
                }
            }
        }

        private void FormatDeleteUsing(SqlTableSourceBase tableSource)
        {
            if (tableSource != null)
            {
                if (tableSource.GetType() == typeof(SqlTableSource))
                {
                    SqlTableSource source = (SqlTableSource)tableSource;
                    string str = "";
                    if (source.name.EqualsIgnoreCase(Token.USERTABLES.value))
                    {
                        base.buffer.Append("(select table_name, case when table_type = 'BASE TABLE' then 'U' else 'V' end table_xtype from information_schema.tables)");
                        str = " AS KSQL_USERTABLES";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.USERCOLUMNS.value))
                    {
                        base.buffer.Append("(SELECT table_name, column_name, ordinal_position AS column_id, data_type, 0 data_length, NUMERIC_PRECISION DATA_PRECISION, NUMERIC_SCALE DATA_SCALE, IS_NULLABLE AS NULLABLE, COLUMN_DEFAULT AS DATA_DEFAULT FROM information_schema.columns)");
                        str = " AS KSQL_USERCOLUMNS";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.TABLECOLUMNDEFAULTVALUE.value))
                    {
                        base.buffer.Append("(SELECT table_name, column_name, ordinal_position AS column_id, data_type, 0 data_length, NUMERIC_PRECISION DATA_PRECISION, NUMERIC_SCALE DATA_SCALE, IS_NULLABLE AS NULLABLE, COLUMN_DEFAULT AS DATA_DEFAULT FROM information_schema.columns)");
                        str = " AS KSQL_TABLECOLUMNDEFAULTVALUE";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.SYSINDEXES.value))
                    {
                        base.buffer.Append("(SELECT bc.relname AS table_name, ic.relname AS index_name FROM pg_namespace n, pg_class bc, pg_class ic, pg_index i WHERE bc.relnamespace = n.oid and i.indrelid = bc.oid and i.indexrelid = ic.oid and n.nspname = 'public')");
                        str = " AS KSQL_INDEXES";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.INDCOLUMNS.value))
                    {
                        base.buffer.Append("(SELECT bc.relname AS table_name, ic.relname AS index_name, a.attname  AS column_name FROM pg_namespace n, pg_class bc, pg_class ic, pg_index i, pg_attribute a WHERE bc.relnamespace = n.oid and i.indrelid = bc.oid and i.indexrelid = ic.oid and i.indkey[0] = a.attnum and i.indnatts = 1 and a.attrelid = bc.oid and n.nspname = 'public'");
                        str = " AS KSQL_INDCOLUMNS";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.SYSCONSTRAINTS.value))
                    {
                        base.buffer.Append("(SELECT table_name, CONSTRAINT_NAME, CONSTRAINT_TYPE, NULL AS COLUMN_ID FROM information_schema.table_constraints)");
                        str = " AS KSQL_CONSTRAINTS";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.USERVIEWS.value))
                    {
                        base.buffer.Append("(select * from information_schema.views )");
                        str = " AS KSQL_USERVIEWS";
                    }
                    else
                    {
                        string str2 = source.name.ToLowerInvariant();
                        base.buffer.Append(str2);
                    }
                    if ((source.alias != null) && (source.alias.Length != 0))
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(source.alias.ToLowerInvariant());
                    }
                    else
                    {
                        base.buffer.Append(str);
                    }
                }
                else if (tableSource.GetType() == typeof(SqlJoinedTableSource))
                {
                    SqlJoinedTableSource source2 = (SqlJoinedTableSource)tableSource;
                    this.FormatTableSource(source2.left);
                    if (source2.joinType != 0)
                    {
                        throw new FormaterException("error");
                    }
                    base.buffer.Append(" using ");
                    this.FormatTableSource(source2.right);
                    if (source2.condition != null)
                    {
                        base.buffer.Append(" where ");
                        base.FormatExpr(source2.condition);
                    }
                }
                else if (tableSource.GetType() == typeof(SqlSubQueryTableSource))
                {
                    SqlSubQueryTableSource source3 = (SqlSubQueryTableSource)tableSource;
                    base.buffer.Append("(");
                    this.FormatSelectBase(source3.subQuery);
                    base.buffer.Append(")");
                    if (tableSource.alias != null)
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(tableSource.alias.ToLowerInvariant());
                    }
                }
                else
                {
                    if (!(tableSource.GetType() == typeof(SqlTableFunctionTableSource)))
                    {
                        throw new FormaterException("TODO");
                    }
                    SqlTableFunctionTableSource source4 = (SqlTableFunctionTableSource)tableSource;
                    string str3 = source4.PipeFunction.Replace(" ", "");
                    StringBuilder builder = new StringBuilder(str3);
                    if (str3.Contains(",1"))
                    {
                        base.buffer.Append("udt_inttable");
                        builder = builder.Remove(0, 0x11).Replace(",',',1)", "");
                    }
                    if (str3.Contains(",2"))
                    {
                        base.buffer.Append("udt_varchartable");
                        builder = builder.Remove(0, 0x11).Replace(",',',2)", "");
                    }
                    if (str3.Contains(",3"))
                    {
                        base.buffer.Append("udt_varchartable");
                        builder = builder.Remove(0, 0x11).Replace(",',',3)", "");
                    }
                    base.buffer.Append(builder.ToString());
                    if ((source4.alias != null) && (source4.alias.Length != 0))
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(source4.alias.ToLower());
                    }
                }
            }
        }

        protected override void FormatExecStmt(SqlExecStmt stmt)
        {
            base.buffer.Append("select * from ");
            base.buffer.Append(stmt.processName);
            if (stmt.paramList.Count != 0)
            {
                base.buffer.Append(" (");
                bool flag = false;
                foreach (SqlExpr expr in stmt.paramList)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(expr);
                    flag = true;
                }
                base.buffer.Append(')');
            }
        }

        public override void FormatExpr(SqlExpr expr, bool appendBrace)
        {
            base.FormatExpr(expr, appendBrace);
        }

        protected override void FormatFetchStmt(SqlFetchStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatGotoStmt(SqlGotoStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatIdentifierExpr(SqlExpr expr)
        {
            string str = ((SqlIdentifierExpr)expr).value;
            if (str.EqualsIgnoreCase(Token.KSQL_COL_NAME.value))
            {
                str = "column_name";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
            {
                str = "table_name";
            }
            else if (str.EqualsIgnoreCase(Token.INDNAME.value))
            {
                str = "index_name";
            }
            else if (str.EqualsIgnoreCase(Token.TABNAME.value))
            {
                str = "table_name";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
            {
                str = "constraint_name";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
            {
                str = "table_name";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
            {
                str = "data_default";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
            {
                str = "nullable";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
            {
                str = "constraint_type";
            }
            if (!string.IsNullOrEmpty(str) && str.StartsWith("\""))
            {
                base.buffer.Append(str.ToLowerInvariant());
            }
            else if (str.EqualsIgnoreCase("KSQL_SEQ"))
            {
                this.FormatIdentity(expr);
            }
            else
            {
                base.buffer.Append(str);
            }
        }

        protected override void FormatIdentity(SqlExpr stmt)
        {
            base.buffer.Append(" Serial ");
        }

        protected override void FormatIfStmt(SqlIfStmt stmt)
        {
            base.buffer.Append("DO $do$ BEGIN ");
            base.buffer.Append("IF ");
            base.FormatExpr(stmt.condition);
            base.buffer.Append(" THEN\n");
            for (int i = 0; i < stmt.trueStmtList.Count; i++)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.trueStmtList[i];
                base.FormatStmt(stmt2);
                base.buffer.Append(";\n");
            }
            if ((stmt.falseStmtList != null) && (stmt.falseStmtList.Count > 0))
            {
                base.buffer.Append("ELSE\n");
                for (int j = 0; j < stmt.falseStmtList.Count; j++)
                {
                    SqlStmt stmt3 = (SqlStmt)stmt.falseStmtList[j];
                    base.FormatStmt(stmt3);
                    base.buffer.Append(";\n");
                }
            }
            base.buffer.Append("END IF; END $do$;");
        }

        public void formatInsertStmt(SqlInsertStmt stmt)
        {
            bool flag2;
            base.buffer.Append("INSERT INTO ");
            SqlInsert insert = stmt.insert;
            string str = this.FormatTableName(insert.tableName);
            base.buffer.Append(str);
            if (insert.columnList.Count > 0)
            {
                base.buffer.Append(" (");
                flag2 = false;
                bool flag = false;
                foreach (object obj2 in insert.columnList)
                {
                    if (obj2 is SqlIdentifierExpr)
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
                        if (!(obj2 is string))
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
            throw new FormaterException("TODO");
        }

        protected override void FormatMergeStmt(SqlMergeStmt stmt)
        {
            SqlMerge merge = stmt.Merge;
            SqlMergeMatched matchedSql = merge.MatchedSql;
            SqlMergeNotMatched notMatchedSql = merge.NotMatchedSql;
            if (string.IsNullOrWhiteSpace(merge.UpdateTable.alias))
            {
                throw new FormaterException(merge.UpdateTable.name + ": Must specify the table alias!");
            }
            string str = merge.UpdateTable.alias.ToLowerInvariant();
            if (matchedSql != null)
            {
                if (matchedSql.DeleteWhere != null)
                {
                    base.buffer.Append("DELETE FROM ");
                    base.buffer.Append(merge.UpdateTable.name.ToLowerInvariant());
                    if (merge.UpdateTable.alias != null)
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(str);
                    }
                    base.buffer.Append(" USING ");
                    if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
                    {
                        base.buffer.Append("(");
                    }
                    if (merge.UsingExpr != null)
                    {
                        base.FormatExpr(merge.UsingExpr);
                    }
                    else
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(merge.UsingTable.name.ToLowerInvariant());
                    }
                    base.buffer.Append(" ");
                    if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
                    {
                        base.buffer.Append(")");
                        base.buffer.Append(merge.UsingTableAlias.ToLowerInvariant());
                    }
                    base.buffer.Append(" WHERE ");
                    base.buffer.Append("(");
                    base.FormatExpr(merge.OnExpr);
                    base.buffer.Append(")");
                    base.buffer.Append(" AND ");
                    base.FormatExpr(matchedSql.UpdateWhere);
                    base.buffer.Append(" AND ");
                    base.FormatExpr(matchedSql.DeleteWhere);
                    base.buffer.Append(";");
                }
                bool flag = false;
                if (matchedSql.SetClauses.Count == 1)
                {
                    string str2 = ((SqlBinaryOpExpr)matchedSql.SetClauses[0]).left.getOrgValue();
                    string str3 = ((SqlBinaryOpExpr)matchedSql.SetClauses[0]).right.getOrgValue();
                    if (str2.EqualsIgnoreCase(str3))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    base.buffer.Append("UPDATE ");
                    base.buffer.Append(merge.UpdateTable.name.ToLowerInvariant());
                    if (merge.UpdateTable.alias != null)
                    {
                        base.buffer.Append(" AS ");
                        base.buffer.Append(str);
                    }
                    base.buffer.Append(" SET ");
                    StringBuilder builder = new StringBuilder(base.buffer.ToString());
                    bool flag2 = false;
                    if (merge.UsingTableAlias.EndsWith(merge.UpdateTable.alias, StringComparison.InvariantCultureIgnoreCase))
                    {
                        flag2 = true;
                    }
                    bool flag3 = false;
                    Regex regex = new Regex(merge.UpdateTable.alias + @"\.", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    Regex regex2 = new Regex(merge.UsingTableAlias + @"\.", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    Regex regex3 = new Regex(merge.UsingTableAlias + @"_xxx\.", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    foreach (SqlExpr expr in matchedSql.SetClauses)
                    {
                        base.buffer.Clear();
                        base.FormatExpr(expr);
                        string input = base.buffer.ToString();
                        if (merge.UpdateTable.alias != null)
                        {
                            if (flag2)
                            {
                                input = regex2.Replace(input, merge.UsingTableAlias + "_xxx.");
                            }
                            input = regex.Replace(input, "");
                            if (flag2)
                            {
                                input = regex3.Replace(input, merge.UsingTableAlias + ".");
                            }
                        }
                        if (flag3)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(input);
                        base.buffer.Clear();
                        flag3 = true;
                    }
                    base.buffer = builder;
                    base.buffer.Append(" FROM ");
                    if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
                    {
                        base.buffer.Append("(");
                    }
                    if (merge.UsingExpr != null)
                    {
                        base.FormatExpr(merge.UsingExpr);
                    }
                    else
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(merge.UsingTable.name.ToLowerInvariant());
                    }
                    base.buffer.Append(" ");
                    if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
                    {
                        base.buffer.Append(")");
                        base.buffer.Append(merge.UsingTableAlias.ToLowerInvariant());
                    }
                    base.buffer.Append(" WHERE ");
                    base.buffer.Append("(");
                    base.FormatExpr(merge.OnExpr);
                    base.buffer.Append(")");
                    if (matchedSql.UpdateWhere != null)
                    {
                        base.buffer.Append(" AND ");
                        base.FormatExpr(matchedSql.UpdateWhere);
                    }
                    base.buffer.Append(";");
                }
            }
            if (notMatchedSql != null)
            {
                base.buffer.Append("INSERT INTO ");
                base.buffer.Append(merge.UpdateTable.name);
                base.buffer.Append("(");
                bool flag4 = false;
                foreach (object obj2 in notMatchedSql.InsertColumns)
                {
                    if (flag4)
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
                    flag4 = true;
                }
                base.buffer.Append(") ");
                if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
                {
                    base.buffer.AppendFormat("SELECT {0}.* FROM ", merge.UsingTableAlias);
                }
                else
                {
                    base.buffer.Append("SELECT ");
                    flag4 = false;
                    foreach (SqlExpr expr3 in notMatchedSql.InsertValues)
                    {
                        if (flag4)
                        {
                            base.buffer.Append(", ");
                        }
                        base.FormatExpr(expr3);
                        flag4 = true;
                    }
                    base.buffer.Append(" FROM ");
                }
                if (merge.UsingExpr != null)
                {
                    base.FormatExpr(merge.UsingExpr);
                }
                else
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(merge.UsingTable.name);
                }
                if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(merge.UsingTableAlias);
                }
                base.buffer.Append(" WHERE NOT EXISTS (SELECT 1 FROM ");
                base.buffer.Append(merge.UpdateTable.name);
                if (merge.UpdateTable.alias != null)
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(str);
                }
                base.buffer.Append(" WHERE ");
                base.FormatExpr(merge.OnExpr);
                base.buffer.Append(")");
                if (notMatchedSql.InsertWhere != null)
                {
                    base.buffer.Append(" AND ");
                    base.FormatExpr(notMatchedSql.InsertWhere);
                }
                base.buffer.Append(";");
            }
        }

        protected void FormatMergeStmt_9_4(SqlMergeStmt stmt)
        {
            SqlMerge merge = stmt.Merge;
            SqlMergeMatched matchedSql = merge.MatchedSql;
            SqlMergeNotMatched notMatchedSql = merge.NotMatchedSql;
            if (notMatchedSql == null)
            {
                base.buffer.Append("UPDATE ");
                base.buffer.Append(merge.UpdateTable.name);
                base.buffer.Append(" SET ");
                bool flag = false;
                foreach (SqlExpr expr in matchedSql.SetClauses)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(expr);
                    flag = true;
                }
                base.buffer.Append(" FROM ");
                base.buffer.Append(merge.UpdateTable.name);
                if (merge.UpdateTable.alias != null)
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(merge.UpdateTable.alias);
                }
                base.buffer.Append(" INNER JOIN ");
                if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
                {
                    base.buffer.Append("(");
                }
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
                    base.buffer.Append(")");
                    base.buffer.Append(merge.UsingTableAlias);
                }
                base.buffer.Append(" ON ");
                base.buffer.Append("(");
                base.FormatExpr(merge.OnExpr);
                base.buffer.Append(")");
            }
            else if (notMatchedSql != null)
            {
                base.buffer.Append("INSERT INTO ");
                base.buffer.Append(merge.UpdateTable.name);
                base.buffer.Append("(");
                bool flag2 = false;
                foreach (object obj2 in notMatchedSql.InsertColumns)
                {
                    if (flag2)
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
                    flag2 = true;
                }
                base.buffer.Append(") ");
                if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
                {
                    base.buffer.AppendFormat("SELECT {0}.* FROM ", merge.UsingTableAlias);
                }
                else
                {
                    base.buffer.Append("SELECT ");
                    flag2 = false;
                    foreach (SqlExpr expr3 in notMatchedSql.InsertValues)
                    {
                        if (flag2)
                        {
                            base.buffer.Append(", ");
                        }
                        base.FormatExpr(expr3);
                        flag2 = true;
                    }
                    base.buffer.Append(" FROM ");
                }
                if (merge.UsingExpr != null)
                {
                    base.FormatExpr(merge.UsingExpr);
                }
                else
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(merge.UsingTable.name);
                }
                if (!string.IsNullOrWhiteSpace(merge.UsingTableAlias))
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(merge.UsingTableAlias);
                }
                base.buffer.Append(" INNER JOIN ");
                base.buffer.Append(merge.UpdateTable.name);
                base.buffer.Append(" ON ");
                base.FormatExpr(merge.OnExpr);
                base.buffer.Append(" ON CONFLICT ");
                if (matchedSql.SetClauses.Count > 0)
                {
                    base.buffer.Append(" DO UPDATE ");
                    flag2 = false;
                    foreach (SqlExpr expr4 in matchedSql.SetClauses)
                    {
                        if (flag2)
                        {
                            base.buffer.Append(", ");
                        }
                        base.FormatExpr(expr4);
                        flag2 = true;
                    }
                }
                else
                {
                    base.buffer.Append(" DO NOTHING ");
                }
                base.buffer.Append(";");
            }
        }

        protected override void FormatMethodInvokeExpr(SqlMethodInvokeExpr expr)
        {
            if (expr.owner != null)
            {
                base.FormatExpr(expr.owner);
                base.buffer.Append('.');
            }
            string str = expr.methodName.ToUpper();
            if (str.CompareTo("ABS") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ABS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("ACOS") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ACOS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("ASIN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ASIN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("ATAN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ATAN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("ATN2") == 0)
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
            else if (str.CompareTo("CEILING") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CEILING(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("COS") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("COS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("EXP") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("EXP(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("FLOOR") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("FLOOR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("MOD") == 0)
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
            else if (str.CompareTo("LOG") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOG(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("POWER") == 0)
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
            else if (str.CompareTo("ROUND") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ROUND(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (str.EqualsIgnoreCase("SIGN"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SIGN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("SIN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SIN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("SQRT") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SQRT(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("TAN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TAN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("CONVERT") == 0)
            {
                if (!(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("TODO");
                }
                string str2 = ((SqlIdentifierExpr)expr.parameters[0]).value;
                if (str2.EqualsIgnoreCase("DATETIME"))
                {
                    base.buffer.Append("CONVERT(DATETIME, ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else if (str2.EqualsIgnoreCase("VARCHAR") || str2.EqualsIgnoreCase("NVARCHAR"))
                {
                    base.buffer.Append("CAST(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" AS VARCHAR)");
                }
            }
            else if (str.CompareTo("CURDATE") == 0)
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("current_date");
            }
            else if (str.CompareTo("CURTIME") == 0)
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("current_time");
            }
            else if (str.CompareTo("DATEADD") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                if (!(expr.parameters[0] is SqlBinaryOpExpr) && !(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("ADD_SECONDS parameter(0) error");
                }
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + '");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" second'");
            }
            else if (str.CompareTo("DATEDIFF") == 0)
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("DATEDIFF(SS, ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("ERROR");
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
            else if (str.CompareTo("DAYNAME") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATENAME(DW, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("DAYOFMONTH") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('DAY', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("DAYOFWEEK") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('ISODOW', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("DAYOFYEAR") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('DOY', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("GETDATE") == 0)
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOCALTIMESTAMP");
            }
            else if (str.CompareTo("HOUR") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('HOUR', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("MINUTE") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('MINUTE', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("MONTH") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('MONTH', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("MONTHNAME") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATENAME(MM, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("NOW") == 0)
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOCALTIMESTAMP");
            }
            else if (str.CompareTo("QUARTER") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('QUARTER', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("SECOND") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('SECOND', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("WEEK") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('WEEK', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("YEAR") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_PART('YEAR', ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("TO_DATE") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CAST(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" AS TIMESTAMP)");
            }
            else if (str.CompareTo("MONTHS_BETWEEN") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATEDIFF(MM, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("ADD_MONTHS") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                if (!(expr.parameters[0] is SqlBinaryOpExpr) && !(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("ADD_MONTHS parameter(0) error");
                }
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + '");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" month'");
            }
            else if (str.CompareTo("ADD_YEARS") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                if (!(expr.parameters[0] is SqlBinaryOpExpr) && !(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("ADD_YEARS parameter(0) error");
                }
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + '");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" year'");
            }
            else if (str.CompareTo("ADD_DAYS") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                if (!(expr.parameters[0] is SqlBinaryOpExpr) && !(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("ADD_DAYS parameter(0) error");
                }
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + '");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" day'");
            }
            else if (str.CompareTo("ADD_HOURS") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                if (!(expr.parameters[0] is SqlBinaryOpExpr) && !(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("ADD_HOURS parameter(0) error");
                }
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + '");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" hour'");
            }
            else if (str.CompareTo("ADD_MINUTES") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                if (!(expr.parameters[0] is SqlBinaryOpExpr) && !(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("ADD_MINUTES parameter(0) error");
                }
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + '");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" minute'");
            }
            else if (str.CompareTo("ADD_SECONDS") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                if (!(expr.parameters[0] is SqlBinaryOpExpr) && !(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("ADD_SECONDS parameter(0) error");
                }
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + '");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" second'");
            }
            else if (str.CompareTo("ASCII") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ASCII(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("CHAR") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.EqualsIgnoreCase("CHARINDEX"))
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("POSITION(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(" IN ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("POSITION(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(" IN ");
                    base.buffer.Append("SUBSTRING(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" from ");
                    base.buffer.Append((SqlExpr)expr.parameters[2]);
                    base.buffer.Append("))");
                }
            }
            else if (str.CompareTo("CONCAT") == 0)
            {
                if (expr.parameters.Count < 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("CONCAT(");
                int num = 0;
                int num2 = expr.parameters.Count - 1;
                while (num < num2)
                {
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    num++;
                }
                base.FormatExpr((SqlExpr)expr.parameters[expr.parameters.Count - 1]);
                base.buffer.Append(")");
            }
            else if (str.EqualsIgnoreCase("LEFT"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SUBSTR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 0, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("LEN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CHAR_LENGTH(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("LENGTH") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CHAR_LENGTH(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("LOWER") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOWER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("LCASE") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOWER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("LTRIM") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LTRIM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("REPLACE") == 0)
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
            else if (str.CompareTo("RIGHT") == 0)
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
            else if (str.CompareTo("RTRIM") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("RTRIM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("SOUNDEX") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SOUNDEX(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("SUBSTRING") == 0)
            {
                if (expr.parameters.Count != 3)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SUBSTRING(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[2]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("TRIM") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LTRIM(RTRIM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append("))");
            }
            else if (str.CompareTo("UCASE") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("UPPER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (str.CompareTo("UPPER") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("UPPER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if ((str.CompareTo("TOCHAR") == 0) || (str.CompareTo("TO_CHAR") == 0))
            {
                if (expr.parameters.Count == 1)
                {
                    base.buffer.Append("CAST(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(" AS VARCHAR)");
                }
                else if ((expr.parameters.Count == 2) && ((SqlCharExpr)expr.parameters[1]).text.ToString().EqualsIgnoreCase("YYYY-MM-DD"))
                {
                    base.buffer.Append("TO_CHAR(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", 'YYYY-MM-DD')");
                }
                else if ((expr.parameters.Count == 2) && ((SqlCharExpr)expr.parameters[1]).text.ToString().EqualsIgnoreCase("YYYY-MM-DD HH24:MI:SS"))
                {
                    base.buffer.Append("TO_CHAR(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", 'YYYY-MM-DD HH24:MI:SS')");
                }
                else if ((expr.parameters.Count == 2) && ((SqlCharExpr)expr.parameters[1]).text.ToString().EqualsIgnoreCase("YYYY-MM-DD HH24:MI:SS.FF9"))
                {
                    base.buffer.Append("TO_CHAR(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", 'YYYY-MM-DD HH24:MI:SS.FF9')");
                }
                else
                {
                    if ((expr.parameters.Count != 3) || !((SqlCharExpr)expr.parameters[1]).text.ToString().EqualsIgnoreCase("NUMBER"))
                    {
                        throw new FormaterException("ERROR");
                    }
                    if (expr.parameters[2].GetType() == typeof(SqlCharExpr))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(", ");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(")");
                    }
                    else
                    {
                        if (!(expr.parameters[2].GetType() == typeof(SqlIntExpr)))
                        {
                            throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                        }
                        base.buffer.Append("Trim(TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(", '999999999999999999999999999990D");
                        int num3 = ((SqlIntExpr)expr.parameters[2]).value;
                        for (int i = 0; i < num3; i++)
                        {
                            base.buffer.Append("9");
                        }
                        base.buffer.Append("'))");
                    }
                }
            }
            else if (str.CompareTo("ISNULL") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CASE WHEN ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" IS NULL THEN ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" ELSE ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" END");
            }
            else if (str.CompareTo("NULLIF") == 0)
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
            else if (str.CompareTo("TO_NUMBER") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CAST(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" AS FLOAT)");
            }
            else if ((str.CompareTo("TO_INT") == 0) || (str.CompareTo("TO_INTEGER") == 0))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CAST(CAST(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" AS FLOAT) AS INTEGER)");
            }
            else if ((str.EqualsIgnoreCase("TO_DECIMAL") || str.EqualsIgnoreCase("DECIMAL")) || str.EqualsIgnoreCase("DEC"))
            {
                if (expr.parameters.Count == 1)
                {
                    base.buffer.Append("CAST(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(" AS DECIMAL)");
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("TO_DECIMAL's parameters num: " + expr.parameters.Count);
                    }
                    base.buffer.Append("CAST(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(" AS DECIMAL(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append("))");
                }
            }
            else if (str.CompareTo("NEWID") == 0)
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("NEWID()");
            }
            else
            {
                base.FormeatUnkownMethodInvokeExpr(expr);
            }
        }

        protected override void FormatOpenStmt(SqlOpenStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatPriorIdentifierExpr(SqlPriorIdentifierExpr expr)
        {
            throw new FormaterException("Not Support. PriorIdentifierExpr");
        }

        protected override void FormatSelect(SqlSelect select)
        {
            if (select.into != null)
            {
                base.buffer.Append("CREATE TABLE ");
                base.buffer.Append(select.into.new_table);
                base.buffer.Append(" AS ");
            }
            if (((select.limit != null) && (select.limit.value != -1)) && (select.orderBy.Count != 0))
            {
                base.buffer.Append("SELECT * FROM (SELECT ");
            }
            else
            {
                base.buffer.Append("SELECT ");
            }
            if (select.distinct == 1)
            {
                base.buffer.Append("DISTINCT ");
            }
            else if (select.distinct != 0)
            {
                throw new FormaterException("distinct option not support.");
            }
            bool flag = this.FormatSelectColumnList(select);
            if (select.tableSource != null)
            {
                base.buffer.Append(" FROM ");
                this.FormatTableSource(select.tableSource);
            }
            if (select.condition != null)
            {
                base.buffer.Append(" WHERE ");
                base.FormatExpr(select.condition);
            }
            if (select.hierarchicalQueryClause != null)
            {
                throw new FormaterException("NOT SUPPORT hierarchicalQueryClause");
            }
            if (select.groupBy.Count != 0)
            {
                base.buffer.Append(" GROUP BY ");
                flag = false;
                foreach (SqlExpr expr in select.groupBy)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(expr);
                    flag = true;
                }
            }
            if (select.having != null)
            {
                base.buffer.Append(" HAVING ");
                base.FormatExpr(select.having);
            }
            if (select.orderBy.Count != 0)
            {
                base.buffer.Append(" ORDER BY ");
                flag = false;
                foreach (SqlOrderByItem item in select.orderBy)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(item.expr);
                    if (item.mode == 0)
                    {
                        base.buffer.Append(" ASC");
                    }
                    else
                    {
                        base.buffer.Append(" DESC");
                    }
                    flag = true;
                }
            }
            if ((select.limit != null) && (select.limit.value != -1))
            {
                if (select.orderBy.Count != 0)
                {
                    base.buffer.Append(") L_JOBS LIMIT ");
                }
                else
                {
                    base.buffer.Append(" LIMIT ");
                }
                base.buffer.Append(select.limit.value);
                if (select.limit.type == 1)
                {
                    throw new FormaterException("Not support");
                }
                if (select.limit.offset != 0)
                {
                    base.buffer.Append(" OFFSET ");
                    base.buffer.Append(select.limit.offset);
                }
            }
        }

        private bool FormatSelectColumnList(SqlSelect select)
        {
            bool flag = false;
            foreach (SqlSelectItem item in select.selectList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                if ((item.alias != null) && (item.alias.Length != 0))
                {
                    base.FormatExpr(item.expr);
                    base.buffer.Append(" AS ");
                    base.buffer.Append(item.alias.ToLowerInvariant());
                }
                else
                {
                    this.FormatExpr(item.expr, false);
                }
                flag = true;
            }
            return flag;
        }

        public override void FormatSelectList(SqlSelect select)
        {
            this.FormatSelectColumnList(select);
        }

        protected override void FormatSetLocalVariantStmt(SqlSetLocalVariantStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected override void FormatShowColumnsStmt(SqlShowColumnsStmt stmt)
        {
            throw new FormaterException("not support Operation");
        }

        protected override void FormatShowTablesStmt(SqlShowTablesStmt stmt)
        {
            throw new FormaterException("not support Operation");
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
                    base.buffer.Append(" PRIMARY KEY (");
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
                    throw new FormaterException("TODO");
                }
                SqlTableCheck check = (SqlTableCheck)constraint;
                base.buffer.Append(" CHECK (");
                this.FormatExpr(check.expr, false);
                base.buffer.Append(")");
            }
        }

        protected void FormatTableConstraintList(ICollection constraintList)
        {
            foreach (SqlTableConstraint constraint in constraintList)
            {
                base.buffer.Append(", ");
                this.FormatTableConstraint(constraint);
            }
        }

        protected override void FormatTableSource(SqlTableSourceBase tableSource)
        {
            if (tableSource != null)
            {
                if (tableSource.GetType() == typeof(SqlTableSource))
                {
                    SqlTableSource source = (SqlTableSource)tableSource;
                    string str = "";
                    if (source.name.EqualsIgnoreCase(Token.USERTABLES.value))
                    {
                        base.buffer.Append("(select table_name, case when table_type = 'BASE TABLE' then 'U' else 'V' end table_xtype from information_schema.tables)");
                        str = " AS KSQL_USERTABLES";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.USERCOLUMNS.value))
                    {
                        base.buffer.Append("(SELECT table_name, column_name, ordinal_position AS column_id, data_type, 0 data_length, NUMERIC_PRECISION DATA_PRECISION, NUMERIC_SCALE DATA_SCALE, IS_NULLABLE AS NULLABLE, COLUMN_DEFAULT AS DATA_DEFAULT FROM information_schema.columns)");
                        str = " AS KSQL_USERCOLUMNS";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.TABLECOLUMNDEFAULTVALUE.value))
                    {
                        base.buffer.Append("(SELECT table_name, column_name, ordinal_position AS column_id, data_type, 0 data_length, NUMERIC_PRECISION DATA_PRECISION, NUMERIC_SCALE DATA_SCALE, IS_NULLABLE AS NULLABLE, COLUMN_DEFAULT AS DATA_DEFAULT FROM information_schema.columns)");
                        str = " AS KSQL_TABLECOLUMNDEFAULTVALUE";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.SYSINDEXES.value))
                    {
                        base.buffer.Append("(SELECT bc.relname AS table_name, ic.relname AS index_name FROM pg_namespace n, pg_class bc, pg_class ic, pg_index i WHERE bc.relnamespace = n.oid and i.indrelid = bc.oid and i.indexrelid = ic.oid and n.nspname = 'public')");
                        str = " AS KSQL_INDEXES";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.INDCOLUMNS.value))
                    {
                        base.buffer.Append("(SELECT bc.relname AS table_name, ic.relname AS index_name, a.attname  AS column_name FROM pg_namespace n, pg_class bc, pg_class ic, pg_index i, pg_attribute a WHERE bc.relnamespace = n.oid and i.indrelid = bc.oid and i.indexrelid = ic.oid and i.indkey[0] = a.attnum and i.indnatts = 1 and a.attrelid = bc.oid and n.nspname = 'public'");
                        str = " AS KSQL_INDCOLUMNS";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.SYSCONSTRAINTS.value))
                    {
                        base.buffer.Append("(SELECT table_name, CONSTRAINT_NAME, CONSTRAINT_TYPE, NULL AS COLUMN_ID FROM information_schema.table_constraints)");
                        str = " AS KSQL_CONSTRAINTS";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.USERVIEWS.value))
                    {
                        base.buffer.Append("(select * from information_schema.views )");
                        str = " AS KSQL_USERVIEWS";
                    }
                    else
                    {
                        string str2 = source.name.ToLowerInvariant();
                        base.buffer.Append(str2);
                    }
                    if ((source.alias != null) && (source.alias.Length != 0))
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(source.alias.ToLowerInvariant());
                    }
                    else
                    {
                        base.buffer.Append(str);
                    }
                }
                else if (!(tableSource.GetType() == typeof(SqlJoinedTableSource)))
                {
                    if (tableSource.GetType() == typeof(SqlSubQueryTableSource))
                    {
                        SqlSubQueryTableSource source3 = (SqlSubQueryTableSource)tableSource;
                        base.buffer.Append("(");
                        this.FormatSelectBase(source3.subQuery);
                        base.buffer.Append(")");
                        if (tableSource.alias != null)
                        {
                            base.buffer.Append(" ");
                            base.buffer.Append(tableSource.alias.ToLowerInvariant());
                        }
                    }
                    else
                    {
                        if (!(tableSource.GetType() == typeof(SqlTableFunctionTableSource)))
                        {
                            throw new FormaterException("TODO");
                        }
                        SqlTableFunctionTableSource source4 = (SqlTableFunctionTableSource)tableSource;
                        string str3 = source4.PipeFunction.Replace(" ", "");
                        StringBuilder builder = new StringBuilder(str3);
                        if (str3.Contains(",1"))
                        {
                            base.buffer.Append("udt_inttable");
                            builder = builder.Remove(0, 0x11).Replace(",',',1)", "");
                        }
                        if (str3.Contains(",2"))
                        {
                            base.buffer.Append("udt_varchartable");
                            builder = builder.Remove(0, 0x11).Replace(",',',2)", "");
                        }
                        if (str3.Contains(",3"))
                        {
                            base.buffer.Append("udt_varchartable");
                            builder = builder.Remove(0, 0x11).Replace(",',',3)", "");
                        }
                        base.buffer.Append(builder.ToString());
                        if ((source4.alias != null) && (source4.alias.Length != 0))
                        {
                            base.buffer.Append(" ");
                            base.buffer.Append(source4.alias.ToLower());
                        }
                    }
                }
                else
                {
                    SqlJoinedTableSource source2 = (SqlJoinedTableSource)tableSource;
                    this.FormatTableSource(source2.left);
                    switch (source2.joinType)
                    {
                        case 0:
                            base.buffer.Append(" INNER JOIN ");
                            break;

                        case 1:
                            base.buffer.Append(" LEFT OUTER JOIN ");
                            break;

                        case 2:
                            base.buffer.Append(" RIGHT OUTER JOIN ");
                            break;

                        case 3:
                            base.buffer.Append(" FULL OUTER JOIN ");
                            break;

                        case 4:
                            base.buffer.Append(", ");
                            break;

                        default:
                            throw new FormaterException("error");
                    }
                    this.FormatTableSource(source2.right);
                    if (source2.condition != null)
                    {
                        base.buffer.Append(" ON ");
                        base.FormatExpr(source2.condition);
                    }
                }
            }
        }

        protected override void FormatUpdateStmt(SqlUpdateStmt stmt)
        {
            SqlUpdate update = stmt.update;
            base.buffer.Append("UPDATE ");
            base.buffer.Append(update.updateTable.name.ToLowerInvariant());
            if (update.updateTable.alias != null)
            {
                base.buffer.Append(" ");
                base.buffer.Append(update.updateTable.alias.ToLowerInvariant());
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
                    base.buffer.Append(item2.name.ToLowerInvariant());
                    base.buffer.Append(" = ");
                    base.FormatExpr(item2.expr);
                }
                else
                {
                    if (!(item.GetType() == typeof(SubQueryUpdateItem)))
                    {
                        throw new FormaterException("not support");
                    }
                    SubQueryUpdateItem item3 = (SubQueryUpdateItem)item;
                    if (item3.columnList.Count == 1)
                    {
                        base.buffer.Append(((string)item3.columnList[0]).ToLowerInvariant());
                        base.buffer.Append(" = (");
                        this.FormatSelectBase(item3.subQuery);
                        base.buffer.Append(")");
                    }
                    else
                    {
                        list.Add(item);
                        for (int k = 0; k < item3.columnList.Count; k++)
                        {
                            string str2 = (string)item3.columnList[k];
                            if (k != 0)
                            {
                                base.buffer.Append(", ");
                            }
                            base.buffer.Append(str2.ToLowerInvariant());
                            base.buffer.Append(" = ");
                            if (!(item3.subQuery.GetType() == typeof(SqlSelect)))
                            {
                                throw new FormaterException("TODO");
                            }
                            SqlSelect subQuery = (SqlSelect)item3.subQuery;
                            SqlSelectItem item4 = (SqlSelectItem)subQuery.selectList[k];
                            if (((subQuery != null) && (subQuery.tableSource != null)) && ((subQuery.tableSource.alias != null) && !subQuery.tableSource.alias.Trim().Equals("")))
                            {
                                item4.expr.addExtAttr("tableSourceAlias", subQuery.tableSource.alias);
                            }
                            this.FormatExpr(item4.expr, false);
                            num++;
                            flag = true;
                        }
                    }
                }
                flag = true;
            }
            if (update.tableSource != null)
            {
                throw new FormaterException("not support");
            }
            Hashtable literalMap = new Hashtable();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    base.buffer.Append(" FROM ");
                }
                else
                {
                    base.buffer.Append(", ");
                }
                SubQueryUpdateItem item5 = (SubQueryUpdateItem)list[i];
                if (!(item5.subQuery.GetType() == typeof(SqlSelect)))
                {
                    throw new FormaterException("TODO");
                }
                SqlSelect select2 = (SqlSelect)item5.subQuery;
                this.FormatTableSource(select2.tableSource);
                if (select2.tableSource.GetType() == typeof(SqlSubQueryTableSource))
                {
                    string alias = select2.tableSource.alias;
                    SqlSelectBase base2 = ((SqlSubQueryTableSource)select2.tableSource).subQuery;
                    if (base2.GetType() == typeof(SqlSelect))
                    {
                        ArrayList selectList = ((SqlSelect)base2).selectList;
                        for (int m = 0; m < selectList.Count; m++)
                        {
                            SqlSelectItem item6 = (SqlSelectItem)selectList[m];
                            if (((item6.alias != null) && (item6.alias.Length != 0)) && (item6.expr.GetType() == typeof(SqlCharExpr)))
                            {
                                string str4 = alias.ToLowerInvariant() + "." + item6.alias.ToLowerInvariant();
                                literalMap[str4] = item6.expr;
                            }
                        }
                    }
                }
                if (select2.condition != null)
                {
                    select2.condition = this.replaceLiteral(select2.condition, literalMap);
                }
            }
            flag = false;
            if (update.condition != null)
            {
                base.buffer.Append(" WHERE ");
                base.FormatExpr(update.condition);
                flag = true;
            }
            for (int j = 0; j < list.Count; j++)
            {
                SubQueryUpdateItem item7 = (SubQueryUpdateItem)list[j];
                if (!(item7.subQuery.GetType() == typeof(SqlSelect)))
                {
                    throw new FormaterException("TODO");
                }
                SqlSelect select3 = (SqlSelect)item7.subQuery;
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
        }

        protected override void FormatWhileStmt(SqlWhileStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        private SqlExpr replaceLiteral(SqlBinaryOpExpr expr, Hashtable literalMap)
        {
            if (((expr.left.GetType() == typeof(SqlIdentifierExpr)) && (expr.right.GetType() == typeof(SqlIdentifierExpr))) && (expr.Operator == 20))
            {
                SqlIdentifierExpr left = (SqlIdentifierExpr)expr.left;
                SqlIdentifierExpr right = (SqlIdentifierExpr)expr.right;
                string str = left.value.ToLowerInvariant() + "." + right.value.ToLowerInvariant();
                SqlExpr expr4 = (SqlExpr)literalMap[str];
                if (expr4 != null)
                {
                    return expr4;
                }
                return expr;
            }
            expr.left = this.replaceLiteral(expr.left, literalMap);
            expr.right = this.replaceLiteral(expr.right, literalMap);
            return expr;
        }

        private SqlExpr replaceLiteral(SqlExpr expr, Hashtable literalMap)
        {
            if (expr.GetType() == typeof(SqlBinaryOpExpr))
            {
                expr = this.replaceLiteral((SqlBinaryOpExpr)expr, literalMap);
            }
            return expr;
        }
    }






}
