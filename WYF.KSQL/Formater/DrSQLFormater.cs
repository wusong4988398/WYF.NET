using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Parser;
using WYF.KSQL.Schema;


namespace WYF.KSQL.Formater
{
    public class DrSQLFormater : TSQLFormater
    {
        // Fields
        public static DrSQLFormater instance = new DrSQLFormater();

        // Methods
        public DrSQLFormater() : base(null)
        {
            base.max_length_of_index_name = 30;
            base.max_length_of_table_name = 30;
            base.max_length_of_constraint_name = 0x12;
            base.max_length_of_column_name = 30;
            base.max_length_of_column_count = 0xff;
            base.max_length_of_row_size = 0x1f40;
        }

        public DrSQLFormater(StringBuilder sb) : base(sb)
        {
            base.max_length_of_index_name = 30;
            base.max_length_of_table_name = 30;
            base.max_length_of_constraint_name = 0x12;
            base.max_length_of_column_name = 30;
            base.max_length_of_column_count = 0xff;
            base.max_length_of_row_size = 0x1f40;
        }

        protected override void FormatAlterTableStmt(SqlAlterTableStmt stmt)
        {
            base.FormatAlterTableStmt(stmt);
        }

        public void FormatBinaryOpExpr(SqlBinaryOpExpr expr, bool appendBrace)
        {
            bool flag;
            string str;
            if (expr.Operator == 13)
            {
                base.buffer.Append("(");
                base.FormatExpr(expr.left);
                base.buffer.Append(" IS ");
                base.FormatExpr(expr.right);
                base.buffer.Append(")");
                return;
            }
            if (expr.Operator == 0x29)
            {
                base.buffer.Append("(");
                base.FormatExpr(expr.left);
                base.buffer.Append(" IS NOT ");
                base.FormatExpr(expr.right);
                base.buffer.Append(")");
                return;
            }
            if (expr.Operator == 20)
            {
                base.FormatExpr(expr.left);
                base.buffer.Append(".");
                base.FormatExpr(expr.right);
                return;
            }
            if (expr.Operator == 0x2b)
            {
                base.FormatExpr(expr.left, false);
                base.buffer.Append(" ESCAPE ");
                base.FormatExpr(expr.right, false);
                return;
            }
            if (expr.Operator == 10)
            {
                base.FormatExpr(expr.left);
                base.buffer.Append(" = ");
                base.FormatExpr(expr.right);
                return;
            }
            if (expr.Operator == 0)
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
                        base.FormatExpr(left, false);
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
                        base.FormatExpr(right, false);
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
                return;
            }
            if (expr.Operator != 8)
            {
                flag = true;
                str = null;
                switch (expr.Operator)
                {
                    case 0:
                        str = " + ";
                        goto Label_0563;

                    case 1:
                        str = " AS ";
                        goto Label_0563;

                    case 2:
                        str = " = ";
                        goto Label_0563;

                    case 7:
                        flag = false;
                        str = " AND ";
                        goto Label_0563;

                    case 8:
                        str = " OR ";
                        goto Label_0563;

                    case 9:
                        str = " / ";
                        goto Label_0563;

                    case 10:
                        str = " = ";
                        goto Label_0563;

                    case 11:
                        str = " > ";
                        goto Label_0563;

                    case 12:
                        str = " >= ";
                        goto Label_0563;

                    case 14:
                        str = " < ";
                        goto Label_0563;

                    case 15:
                        str = " <= ";
                        goto Label_0563;

                    case 0x10:
                        str = " <> ";
                        goto Label_0563;

                    case 0x12:
                        str = " LIKE ";
                        goto Label_0563;

                    case 0x13:
                        str = " >> ";
                        goto Label_0563;

                    case 20:
                        str = ".";
                        goto Label_0563;

                    case 0x15:
                        str = " % ";
                        goto Label_0563;

                    case 0x16:
                        str = " * ";
                        goto Label_0563;

                    case 0x17:
                        str = " != ";
                        goto Label_0563;

                    case 0x18:
                        str = " !< ";
                        goto Label_0563;

                    case 0x19:
                        str = " !> ";
                        goto Label_0563;

                    case 0x1a:
                        str = " - ";
                        goto Label_0563;

                    case 0x1b:
                        str = " UNION ";
                        goto Label_0563;

                    case 40:
                        str = " NOT LIKE ";
                        goto Label_0563;

                    case 0x2a:
                        str = " || ";
                        goto Label_0563;
                }
                throw new FormaterException("not support");
            }
            if (appendBrace)
            {
                base.buffer.Append("(");
            }
            ArrayList list = new ArrayList();
            SqlExpr expr4 = expr;
            while ((expr4 != null) || (list.Count != 0))
            {
                while (expr4 != null)
                {
                    if (!(expr4.GetType() == typeof(SqlBinaryOpExpr)))
                    {
                        base.FormatExpr(expr4);
                        expr4 = null;
                    }
                    else
                    {
                        SqlBinaryOpExpr expr5 = (SqlBinaryOpExpr)expr4;
                        if (expr5.Operator == 8)
                        {
                            if (expr5.left.GetType() == typeof(SqlBinaryOpExpr))
                            {
                                SqlBinaryOpExpr expr6 = (SqlBinaryOpExpr)expr5.left;
                                if (expr6.Operator == 8)
                                {
                                    list.Add(expr5.right);
                                    expr4 = expr6;
                                }
                                else
                                {
                                    base.FormatExpr(expr6);
                                    base.buffer.Append(" OR ");
                                    expr4 = expr5.right;
                                }
                            }
                            else
                            {
                                base.FormatExpr(expr5.left);
                                base.buffer.Append(" OR ");
                                expr4 = expr5.right;
                            }
                        }
                        else
                        {
                            base.FormatExpr(expr4);
                            expr4 = null;
                        }
                    }
                }
                if (list.Count != 0)
                {
                    base.buffer.Append(" OR ");
                    expr4 = (SqlExpr)list[list.Count - 1];
                    list.Remove(list.Count - 1);
                }
            }
            if (appendBrace)
            {
                base.buffer.Append(")");
            }
            return;
            Label_0563:
            if (appendBrace && flag)
            {
                base.buffer.Append("(");
            }
            base.FormatExpr(expr.left);
            base.buffer.Append(str);
            base.FormatExpr(expr.right);
            if (appendBrace && flag)
            {
                base.buffer.Append(")");
            }
        }

        public void FormatBlockStmt(SqlBlockStmt stmt)
        {
            if ((stmt.declItemList != null) && (stmt.declItemList.Count != 0))
            {
                base.buffer.Append("DECLARE ");
                for (int i = 0; i < stmt.declItemList.Count; i++)
                {
                    SqlBlockStmt.DeclItem item = (SqlBlockStmt.DeclItem)stmt.declItemList[i];
                    if (item.GetType() == typeof(SqlBlockStmt.DeclVarItem))
                    {
                        SqlBlockStmt.DeclVarItem item2 = (SqlBlockStmt.DeclVarItem)item;
                        base.buffer.Append(item.name);
                        base.buffer.Append(" ");
                        if (item2.dataType.EqualsIgnoreCase("BINARY"))
                        {
                            base.buffer.Append("BINARY (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("BLOB"))
                        {
                            base.buffer.Append("BLOB");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("CHAR"))
                        {
                            base.buffer.Append("CHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("CLOB"))
                        {
                            base.buffer.Append("CLOB");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("DATETIME"))
                        {
                            base.buffer.Append("DATETIME");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("DECIMAL"))
                        {
                            base.buffer.Append("NUMERIC (");
                            base.buffer.Append(item2.precision);
                            base.buffer.Append(", ");
                            base.buffer.Append(item2.scale);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("INT") || item2.dataType.EqualsIgnoreCase("INTTEGER"))
                        {
                            base.buffer.Append("INT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NCHAR"))
                        {
                            base.buffer.Append("NCHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NCLOB"))
                        {
                            base.buffer.Append("NCLOB");
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
                        else if (item2.dataType.EqualsIgnoreCase("SMALLINT"))
                        {
                            base.buffer.Append("SMALLINT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("VARBINARY"))
                        {
                            base.buffer.Append("VARBINARY (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else
                        {
                            if (!item2.dataType.EqualsIgnoreCase("VARCHAR"))
                            {
                                throw new FormaterException("not support datatype, column name is '" + item2.name + "' datatype is '" + item2.dataType + "'");
                            }
                            base.buffer.Append("VARCHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        if (item2.defaultValueExpr != null)
                        {
                            base.buffer.Append(" = ");
                            base.FormatExpr(item2.defaultValueExpr);
                        }
                        base.buffer.Append(";\n");
                    }
                    else
                    {
                        if (!(item.GetType() == typeof(SqlBlockStmt.DeclCurItem)))
                        {
                            throw new FormaterException("not support statement:" + item);
                        }
                        SqlBlockStmt.DeclCurItem item3 = (SqlBlockStmt.DeclCurItem)item;
                        base.buffer.Append("CURSOR ");
                        string name = item3.name;
                        if (!string.IsNullOrEmpty(name) && name.StartsWith("@"))
                        {
                            name = name.Substring(1);
                            base.buffer.Append(name);
                        }
                        else
                        {
                            base.buffer.Append(name);
                        }
                        base.buffer.Append(" IS ");
                        this.FormatSelectBase(item3.select);
                        base.buffer.Append(";\n");
                    }
                }
            }
            base.buffer.Append("BEGIN\n");
            int num2 = 0;
            int count = stmt.stmtList.Count;
            while (num2 < count)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[num2];
                base.FormatStmt(stmt2);
                base.buffer.Append(";\n");
                num2++;
            }
            base.buffer.Append("END;");
        }

        protected override void FormatBreakStmt(SqlBreakStmt stmt)
        {
            base.buffer.Append("BREAK");
        }

        public void FormatCloseStmt(SqlCloseStmt stmt)
        {
            base.buffer.Append("CLOSE ");
            base.buffer.Append(stmt.curName);
        }

        public void FormatColumnDef(SqlColumnDef column)
        {
            if (column.name == null)
            {
                throw new FormaterException("column name is null");
            }
            if (((base.max_length_of_column_name != -1) && (column.name != null)) && (column.name.Length > base.max_length_of_column_name))
            {
                throw new FormaterException(string.Concat(new object[] { "column name greate than ", base.max_length_of_column_name, ", column name is '", column.name, "'" }));
            }
            base.buffer.Append(column.name);
            base.buffer.Append(" ");
            if (column.dataType.EqualsIgnoreCase("BINARY"))
            {
                base.buffer.Append("BINARY (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("BLOB"))
            {
                base.buffer.Append("BLOB (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("CHAR"))
            {
                base.buffer.Append("CHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("CLOB"))
            {
                base.buffer.Append("CLOB (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("DATETIME"))
            {
                base.buffer.Append("DATETIME");
            }
            else if (column.dataType.EqualsIgnoreCase("DECIMAL"))
            {
                base.buffer.Append("NUMERIC (");
                base.buffer.Append(column.precision);
                base.buffer.Append(", ");
                base.buffer.Append(column.scale);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("INT") || column.dataType.EqualsIgnoreCase("INTTEGER"))
            {
                base.buffer.Append("INT");
            }
            else if (column.dataType.EqualsIgnoreCase("NCHAR"))
            {
                base.buffer.Append("NCHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("NCLOB"))
            {
                base.buffer.Append("NCLOB (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("NUMERIC"))
            {
                base.buffer.Append("NUMERIC (");
                base.buffer.Append(column.precision);
                base.buffer.Append(", ");
                base.buffer.Append(column.scale);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("NVARCHAR"))
            {
                base.buffer.Append("NVARCHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("SMALLINT"))
            {
                base.buffer.Append("SMALLINT");
            }
            else if (column.dataType.EqualsIgnoreCase("VARBINARY"))
            {
                base.buffer.Append("VARBINARY (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else
            {
                if (!column.dataType.EqualsIgnoreCase("VARCHAR"))
                {
                    throw new FormaterException("not support datatype, column name is '" + column.name + "' datatype is '" + column.dataType + "'");
                }
                base.buffer.Append("VARCHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            if (column.allowNull)
            {
                if (!column.isPrimaryKey)
                {
                    base.buffer.Append(" NULL");
                }
            }
            else if (!column.allowNull)
            {
                base.buffer.Append(" NOT NULL");
            }
            if (column.defaultValueExpr != null)
            {
                base.buffer.Append(" DEFAULT ");
                base.FormatExpr(column.defaultValueExpr);
            }
            if (!string.IsNullOrEmpty(column.containtName))
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
            base.buffer.Append("CONTINUE");
        }

        protected override void FormatCreateTableStmt(SqlCreateTableStmt stmt)
        {
            base.FormatCreateTableStmt(stmt);
        }

        public void FormatCursorLoopStmt(SqlCursorLoopStmt stmt)
        {
            base.buffer.Append("CURSOR_LOOP ");
            base.buffer.Append(stmt.curName);
            if ((stmt.fieldList != null) && (stmt.fieldList.Count > 0))
            {
                base.buffer.Append(" (");
                for (int i = 0; i < stmt.fieldList.Count; i++)
                {
                    if (i != 0)
                    {
                        base.buffer.Append(", ");
                    }
                    SqlExpr expr = (SqlExpr)stmt.fieldList[i];
                    base.FormatExpr(expr);
                }
                base.buffer.Append(")");
            }
            base.buffer.Append(" INTO ");
            if ((stmt.fieldList != null) && (stmt.intoList.Count > 0))
            {
                base.buffer.Append(" (");
                for (int j = 0; j < stmt.intoList.Count; j++)
                {
                    if (j != 0)
                    {
                        base.buffer.Append(", ");
                    }
                    SqlExpr expr2 = (SqlExpr)stmt.intoList[j];
                    base.FormatExpr(expr2);
                }
                base.buffer.Append(")");
            }
            base.buffer.Append(" DO\n");
            int num3 = 0;
            int count = stmt.stmtList.Count;
            while (num3 < count)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[num3];
                base.FormatStmt(stmt2);
                base.buffer.Append(";\n");
                num3++;
            }
            base.buffer.Append("END CURSOR_LOOP");
        }

        protected override void FormatDateTimeExpr(SqlDateTimeExpr expr)
        {
            base.buffer.Append("{");
            if (expr.timeType() == -19000)
            {
                base.buffer.Append("TS '");
                base.buffer.Append((expr.getYear() < 10) ? "0" : "");
                base.buffer.Append(expr.getYear());
                base.buffer.Append((expr.getMonth() < 10) ? "-0" : "-");
                base.buffer.Append(expr.getMonth());
                base.buffer.Append((expr.getDate() < 10) ? "-0" : "-");
                base.buffer.Append(expr.getDate());
                base.buffer.Append((expr.getHour() < 10) ? " 0" : " ");
                base.buffer.Append(expr.getHour());
                base.buffer.Append((expr.getMinute() < 10) ? ":0" : ":");
                base.buffer.Append(expr.getMinute());
                base.buffer.Append((expr.getSecond() < 10) ? ":0" : ":");
                base.buffer.Append(expr.getSecond());
            }
            else if (expr.timeType() == -19001)
            {
                base.buffer.Append("D '");
                base.buffer.Append((expr.getYear() < 10) ? "0" : "");
                base.buffer.Append(expr.getYear());
                base.buffer.Append((expr.getMonth() < 10) ? "-0" : "-");
                base.buffer.Append(expr.getMonth());
                base.buffer.Append((expr.getDate() < 10) ? "-0" : "-");
                base.buffer.Append(expr.getDate());
            }
            else if (expr.timeType() == -19002)
            {
                base.buffer.Append("T '");
                base.buffer.Append((expr.getHour() < 10) ? "0" : "");
                base.buffer.Append(expr.getHour());
                base.buffer.Append((expr.getMinute() < 10) ? ":0" : ":");
                base.buffer.Append(expr.getMinute());
                base.buffer.Append((expr.getSecond() < 10) ? ":0" : ":");
                base.buffer.Append(expr.getSecond());
            }
            base.buffer.Append("'}");
        }

        public void FormatDeallocateStmt(SqlDeallocateStmt stmt)
        {
            base.buffer.Append("DEALLOCATE ");
            base.buffer.Append(stmt.curName);
        }

        public void FormatDeleteStmt(SqlDeleteStmt stmt)
        {
            base.FormatDeleteStmt(stmt);
        }

        protected void FormatDropTableStmt(SqlDropTableStmt stmt)
        {
            base.FormatDropTableStmt(stmt);
        }

        public void FormatExecStmt(SqlExecStmt stmt, FormatOptions options)
        {
            base.buffer.Append("EXEC ");
            base.buffer.Append(stmt.processName);
            if (stmt.paramList.Count != 0)
            {
                base.buffer.Append(" ");
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
            }
        }

        protected override void FormatFetchStmt(SqlFetchStmt stmt)
        {
            throw new FormaterException("not support formater fetch statement");
        }

        protected override void FormatGotoStmt(SqlGotoStmt stmt)
        {
            base.buffer.Append("GOTO ");
            base.buffer.Append(stmt.name);
        }

        public void FormatIfStmt(SqlIfStmt stmt)
        {
            base.buffer.Append("IF (");
            base.FormatExpr(stmt.condition);
            base.buffer.Append(")\n");
            base.buffer.Append("BEGIN\n");
            for (int i = 0; i < stmt.trueStmtList.Count; i++)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.trueStmtList[i];
                base.FormatStmt(stmt2);
                base.buffer.Append(";\n");
            }
            base.buffer.Append("END");
            if ((stmt.falseStmtList != null) && (stmt.falseStmtList.Count > 0))
            {
                base.buffer.Append("ELSE\n");
                base.buffer.Append("BEGIN\n");
                for (int j = 0; j < stmt.falseStmtList.Count; j++)
                {
                    SqlStmt stmt3 = (SqlStmt)stmt.falseStmtList[j];
                    base.FormatStmt(stmt3);
                    base.buffer.Append(";\n");
                }
                base.buffer.Append("END");
            }
        }

        public void FormatInsertStmt(SqlInsertStmt stmt)
        {
            base.FormatInsertStmt(stmt);
        }

        protected override void FormatLabelStmt(SqlLabelStmt stmt)
        {
            base.buffer.Append(stmt.name);
            base.buffer.Append(":");
        }

        protected override void FormatMethodInvokeExpr(SqlMethodInvokeExpr expr)
        {
            if (expr.owner != null)
            {
                base.FormatExpr(expr.owner);
                base.buffer.Append('.');
            }
            string str = expr.methodName.ToUpper();
            base.buffer.Append(str);
            base.buffer.Append("(");
            bool flag = false;
            foreach (SqlExpr expr2 in expr.parameters)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                base.FormatExpr(expr2);
                flag = true;
            }
            base.buffer.Append(")");
        }

        public void FormatOpenStmt(SqlOpenStmt stmt)
        {
            base.buffer.Append("OPEN ");
            base.buffer.Append(stmt.curName);
        }

        public void FormatSelect(SqlSelect select)
        {
            base.buffer.Append("SELECT ");
            if (select.distinct == 1)
            {
                base.buffer.Append("DISTINCT ");
            }
            else if (select.distinct != 0)
            {
                throw new FormaterException("distinct option not support.");
            }
            if (select.limit != null)
            {
                base.buffer.Append("TOP ");
                base.buffer.Append(select.limit.value);
                base.buffer.Append(" ");
                if (select.limit.type == 1)
                {
                    throw new FormaterException("'TOP PERCENT' is not support.");
                }
            }
            bool flag = false;
            foreach (SqlSelectItem item in select.selectList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                this.FormatSelectItem(item);
                flag = true;
            }
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
                foreach (SqlOrderByItem item2 in select.orderBy)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(item2.expr);
                    if (((item2.chineseOrderByMode == 2) || (item2.chineseOrderByMode == 4)) || (item2.chineseOrderByMode == 3))
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(item2.getOrgChineseOrderByType());
                    }
                    if (item2.mode == 0)
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
        }

        public void FormatSelectItem(SqlSelectItem item)
        {
            if (!string.IsNullOrEmpty(item.alias))
            {
                base.FormatExpr(item.expr);
                base.buffer.Append(" ");
                base.buffer.Append(item.alias);
            }
            else
            {
                base.FormatExpr(item.expr, false);
            }
        }

        protected void FormatSelectStmt(SqlSelectStmt stmt)
        {
            base.FormatSelectStmt(stmt);
        }

        public void FormatSetLocalVariantStmt(SqlSetLocalVariantStmt stmt)
        {
            base.buffer.Append("SET ");
            base.FormatExpr(stmt.variant);
            base.buffer.Append(" = ");
            base.FormatExpr(stmt.value);
        }

        public void FormatShowColumnsStmt(SqlShowColumnsStmt stmt)
        {
            string str = "SHOW COLUMNS FROM '" + stmt.tableName + "'";
            base.buffer.Append(str);
        }

        public void FormatShowTablesStmt(SqlShowTablesStmt stmt)
        {
            string str = "SHOW TABLES";
            base.buffer.Append(str);
        }

        public void FormatTableSource(SqlTableSourceBase tableSource)
        {
            if (tableSource.GetType() == typeof(SqlTableSource))
            {
                SqlTableSource source = (SqlTableSource)tableSource;
                base.buffer.Append(source.name);
                if ((source.alias != null) && (source.alias.Length != 0))
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(source.alias);
                }
            }
            else if (!(tableSource.GetType() == typeof(SqlJoinedTableSource)))
            {
                if (tableSource.GetType() != typeof(SqlSubQueryTableSource))
                {
                    throw new FormaterException("not support tableSource:" + tableSource);
                }
                SqlSubQueryTableSource source3 = (SqlSubQueryTableSource)tableSource;
                base.buffer.Append("(");
                this.FormatSelectBase(source3.subQuery);
                base.buffer.Append(")");
                if (tableSource.alias != null)
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(tableSource.alias);
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

        public void FormatUpdateStmt(SqlUpdateStmt stmt)
        {
            base.FormatUpdateStmt(stmt);
        }

        public void FormatWhileStmt(SqlWhileStmt stmt)
        {
            base.buffer.Append("WHILE ");
            base.FormatExpr(stmt.condition);
            base.buffer.Append("\n");
            base.buffer.Append("BEGIN\n");
            for (int i = 0; i < stmt.stmtList.Count; i++)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[i];
                base.FormatStmt(stmt2);
                base.buffer.Append(";\n");
            }
            base.buffer.Append("END");
        }

        public static IList GetStandardFuncList_0()
        {
            ArrayList list = new ArrayList();
            FunctionDef def = new FunctionDef("ABS", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("ACOS", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("ASIN", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("ATAN", "FLOAT")
            {
                name = "ATAN"
            };
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("ATN2", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("CEILING", "INT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("COS", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("EXPR", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("FLOOR", "INT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("MOD", "INT");
            def.addParam(new ParameterDef(null, "INT"));
            def.addParam(new ParameterDef(null, "INT"));
            list.Add(def);
            def = new FunctionDef("LOG", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("POWER", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("ROUND", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            def.addParam(new ParameterDef(null, "INT"));
            list.Add(def);
            def = new FunctionDef("SIGN", "BOOL");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("SIN", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("SQRT", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("TAN", "FLOAT");
            def.addParam(new ParameterDef(null, "FLOAT"));
            list.Add(def);
            def = new FunctionDef("FN_GCD", "INT");
            def.addParam(new ParameterDef(null, "INT"));
            def.addParam(new ParameterDef(null, "INT"));
            list.Add(def);
            def = new FunctionDef("FN_LCM", "INT");
            def.addParam(new ParameterDef(null, "INT"));
            def.addParam(new ParameterDef(null, "INT"));
            list.Add(def);
            return list;
        }

        public static IList GetStandardFuncList_1()
        {
            ArrayList list = new ArrayList();
            FunctionDef def = new FunctionDef("ASCII", "INT");
            def.addParam(new ParameterDef(null, "CHAR(1)"));
            list.Add(def);
            def = new FunctionDef("CHAR", "CHAR(1)");
            def.addParam(new ParameterDef(null, "INT"));
            list.Add(def);
            return list;
        }

        public bool IsValidateColumnName(string columnName)
        {
            if ((columnName == null) || (columnName.Length == 0))
            {
                return false;
            }
            if (columnName.Length > base.max_length_of_column_name)
            {
                return false;
            }
            return this.IsValidateIdentifier(columnName);
        }

        public bool IsValidateConstraintName(string columnName)
        {
            if ((columnName == null) || (columnName.Length == 0))
            {
                return false;
            }
            if (columnName.Length > base.max_length_of_constraint_name)
            {
                return false;
            }
            return this.IsValidateIdentifier(columnName);
        }

        public bool IsValidateIdentifier(string ident)
        {
            char c = Convert.ToChar(ident.Substring(0, 1));
            if ((c != '_') && !char.IsLetter(c))
            {
                return false;
            }
            foreach (char ch2 in ident.ToCharArray(1, ident.Length - 1))
            {
                if ((ch2 != '_') && !char.IsLetterOrDigit(ch2))
                {
                    return false;
                }
            }
            if (KeyWord.instance.IsKeyWord(ident))
            {
                return false;
            }
            return true;
        }

        public bool IsValidateIndexName(string columnName)
        {
            if ((columnName == null) || (columnName.Length == 0))
            {
                return false;
            }
            if (columnName.Length > base.max_length_of_index_name)
            {
                return false;
            }
            return this.IsValidateIdentifier(columnName);
        }

        public bool IsValidateTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return false;
            }
            if (tableName.Length > base.max_length_of_table_name)
            {
                return false;
            }
            return this.IsValidateIdentifier(tableName);
        }

        protected void ValidateCreateTableStmt(SqlCreateTableStmt stmt)
        {
            base.ValidateCreateTableStmt(stmt);
            if (stmt.name.IndexOf(" ") >= 0)
            {
                throw new FormaterException("table name cannot contians space.");
            }
        }

        // Nested Types
        public static class Types
        {
        }
    }





}
