using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Parser;
using WYF.KSQL.Util;

namespace WYF.KSQL.Formater
{
    public abstract class TSQLFormater : SQLFormater
    {
        // Fields
        public bool IsTabFuncFormat;
        private Hashtable selectItemAliasMap;

        // Methods
        public TSQLFormater() : base(null)
        {
            this.IsTabFuncFormat = true;
        }

        public TSQLFormater(StringBuilder sb) : base(sb)
        {
            this.IsTabFuncFormat = true;
        }

        private bool checkHaveChineseOrderBy(IList orderBy)
        {
            foreach (SqlOrderByItem item in orderBy)
            {
                if (item.chineseOrderByMode != -1)
                {
                    return true;
                }
            }
            return false;
        }

        protected void FormatAggregateExprExpr(SqlAggregateExpr expr)
        {
            base.buffer.Append(expr.methodName);
            base.buffer.Append("(");
            if (expr.option == 0)
            {
                base.buffer.Append("DISTINCT ");
            }
            bool flag = false;
            foreach (SqlExpr expr2 in expr.paramList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                this.FormatExpr(expr2, false);
                flag = true;
            }
            base.buffer.Append(")");
            if (expr.HasOver())
            {
                base.buffer.Append(" OVER(");
                flag = false;
                if (expr.overExpr.partition.Count > 0)
                {
                    base.buffer.Append("PARTITION BY ");
                    foreach (SqlExpr expr3 in expr.overExpr.partition)
                    {
                        if (flag)
                        {
                            base.buffer.Append(", ");
                        }
                        base.FormatExpr(expr3);
                        flag = true;
                    }
                }
                if (expr.overExpr.orderBy.Count > 0)
                {
                    base.buffer.Append(" ORDER BY ");
                    flag = false;
                    foreach (SqlOrderByItem item in expr.overExpr.orderBy)
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
                        else if (item.mode == 1)
                        {
                            base.buffer.Append(" DESC");
                        }
                        flag = true;
                    }
                }
                base.buffer.Append(")");
            }
        }

        protected override void FormatAlterTableStmt(SqlAlterTableStmt stmt)
        {
            if ((stmt.items == null) || (stmt.items.Count <= 1))
            {
                this.FormatAlterTableStmtOld(stmt);
            }
            else
            {
                bool flag = false;
                StringBuilder builder = new StringBuilder();
                foreach (object obj2 in stmt.items)
                {
                    if (!flag)
                    {
                        builder.Append("EXEC xalter_table '" + this.FormatTableName(stmt.tableName) + "', '(");
                    }
                    if (flag)
                    {
                        builder.Append("), (");
                    }
                    if (obj2.GetType() == typeof(SqlAlterTableAddItem))
                    {
                        SqlAlterTableAddItem item = (SqlAlterTableAddItem)obj2;
                        builder.Append(" ADD ");
                        bool flag2 = false;
                        foreach (SqlColumnDef def in item.columnDefItemList)
                        {
                            if (flag2)
                            {
                                builder.Append(", ");
                            }
                            StringBuilder buffer = base.buffer;
                            base.buffer = new StringBuilder();
                            this.FormatColumnDef(def);
                            builder.Append(base.buffer);
                            base.buffer = buffer;
                            flag2 = true;
                        }
                        flag = false;
                        foreach (SqlTableConstraint constraint in item.constraintItemList)
                        {
                            try
                            {
                                StringBuilder builder3 = base.buffer;
                                base.buffer = new StringBuilder();
                                this.FormatTableConstraint(constraint);
                                builder.Append(base.buffer);
                                base.buffer = builder3;
                            }
                            catch (FormaterException exception)
                            {
                                throw new FormaterException("alter table statement invalid. table name is '" + stmt.tableName + "', " + exception.Message, exception);
                            }
                        }
                    }
                    else if (obj2.GetType() == typeof(SqlAlterTableAlterColumnItem))
                    {
                        SqlAlterTableAlterColumnItem item2 = (SqlAlterTableAlterColumnItem)obj2;
                        builder.Append(" ALTER COLUMN ");
                        StringBuilder builder4 = base.buffer;
                        base.buffer = new StringBuilder();
                        this.FormatColumnDef(item2.columnDef);
                        builder.Append(base.buffer);
                        base.buffer = builder4;
                    }
                    else if (obj2.GetType() == typeof(SqlAlterTableDropItem))
                    {
                        SqlAlterTableDropItem item3 = (SqlAlterTableDropItem)obj2;
                        builder.Append(" DROP ");
                        bool flag3 = false;
                        foreach (string str in item3.columnDefItemList)
                        {
                            if (flag3)
                            {
                                builder.Append(", ");
                            }
                            else
                            {
                                builder.Append("COLUMN ");
                            }
                            builder.Append(base.FormatColumnName(str));
                            flag3 = true;
                        }
                        flag3 = false;
                        foreach (string str2 in item3.constraintItemList)
                        {
                            if (flag3)
                            {
                                builder.Append(", ");
                            }
                            else
                            {
                                builder.Append("CONSTRAINT ");
                            }
                            builder.Append(base.FormatColumnName(str2));
                            flag3 = true;
                        }
                    }
                    else if (obj2.GetType() == typeof(SqlAlterTableAddDefaultItem))
                    {
                        SqlAlterTableAddDefaultItem item4 = (SqlAlterTableAddDefaultItem)obj2;
                        builder.Append(" ADD CONSTRAINT ");
                        builder.Append("DEF_" + stmt.tableName.ToUpper() + "_" + item4.columnName.ToUpper());
                        builder.Append(" DEFAULT ");
                        StringBuilder builder5 = base.buffer;
                        base.buffer = new StringBuilder();
                        base.FormatExpr(item4.value);
                        builder.Append(base.buffer.Replace("'", "''"));
                        base.buffer = builder5;
                        builder.Append(" FOR ");
                        builder.Append(item4.columnName);
                    }
                    else if (obj2.GetType() == typeof(SqlAlterTableDropDefaultItem))
                    {
                        SqlAlterTableDropDefaultItem item5 = (SqlAlterTableDropDefaultItem)stmt.items[0];
                        base.buffer.Append("EXEC drop_defaulcons '");
                        base.buffer.Append(stmt.tableName);
                        base.buffer.Append("', '");
                        base.buffer.Append(item5.columnName);
                        base.buffer.Append("'");
                        flag = false;
                        break;
                    }
                    flag = true;
                }
                if (flag)
                {
                    base.buffer.Append(builder.ToString());
                    base.buffer.Append(")'");
                }
            }
        }

        protected void FormatAlterTableStmtOld(SqlAlterTableStmt stmt)
        {
            base.buffer.Append("ALTER TABLE ");
            base.buffer.Append(this.FormatTableName(stmt.tableName));
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
                    base.buffer.Append(base.FormatColumnName(str));
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
                    base.buffer.Append(base.FormatColumnName(str2));
                    flag2 = true;
                }
            }
            else if (stmt.item.GetType() == typeof(SqlAlterTableAlterColumnItem))
            {
                SqlAlterTableAlterColumnItem item3 = (SqlAlterTableAlterColumnItem)stmt.item;
                base.buffer.Append(" ALTER COLUMN ");
                this.FormatColumnDef(item3.columnDef);
            }
            else if (stmt.item.GetType() == typeof(SqlAlterTableAddDefaultItem))
            {
                SqlAlterTableAddDefaultItem item4 = (SqlAlterTableAddDefaultItem)stmt.item;
                base.buffer.Append(" ADD CONSTRAINT ");
                base.buffer.Append("DEF_" + stmt.tableName.ToUpper() + "_" + item4.columnName.ToUpper());
                base.buffer.Append(" DEFAULT ");
                base.FormatExpr(item4.value);
                base.buffer.Append(" FOR ");
                base.buffer.Append(item4.columnName);
            }
            else
            {
                if (stmt.item.GetType() != typeof(SqlAlterTableDropDefaultItem))
                {
                    throw new FormaterException("unexpect statement: '" + stmt + "'");
                }
                SqlAlterTableDropDefaultItem item5 = (SqlAlterTableDropDefaultItem)stmt.item;
                int startIndex = (base.buffer.Length - stmt.tableName.Length) - 12;
                base.buffer.Remove(startIndex, base.buffer.Length - startIndex);
                base.buffer.Append(" DECLARE @SYSNAME AS SYSNAME ");
                base.buffer.Append(" SELECT @SYSNAME=D.NAME ");
                base.buffer.Append(" FROM SYS.DEFAULT_CONSTRAINTS D ");
                base.buffer.Append(" INNER JOIN SYS.COLUMNS C ");
                base.buffer.Append(" ON D.PARENT_COLUMN_ID = C.COLUMN_ID AND D.PARENT_OBJECT_ID=C.OBJECT_ID ");
                base.buffer.Append(" WHERE D.PARENT_OBJECT_ID = OBJECT_ID(N'" + stmt.tableName + "', N'U') ");
                base.buffer.Append(" AND C.NAME = '" + item5.columnName + "' ");
                base.buffer.Append(" IF LEN(@SYSNAME)>0 ");
                base.buffer.Append(" EXEC('ALTER TABLE " + stmt.tableName + " DROP CONSTRAINT ['+@SYSNAME+']')");
            }
        }

        protected override void FormatBinaryOpExpr(SqlBinaryOpExpr expr, bool appendBrace)
        {
            if (expr.Operator == 13)
            {
                base.buffer.Append("(");
                base.FormatExpr(expr.left);
                if ((expr.left.type == 4) && ((SqlIdentifierExpr)expr.left).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                {
                    base.buffer.Append(" = 0)");
                    return;
                }
                base.buffer.Append(" IS NULL)");
                return;
            }
            if (expr.Operator == 0x29)
            {
                base.buffer.Append("(");
                base.FormatExpr(expr.left);
                if ((expr.left.type == 4) && ((SqlIdentifierExpr)expr.left).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                {
                    base.buffer.Append(" <> 0)");
                    return;
                }
                base.buffer.Append(" IS NOT NULL)");
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
                this.FormatExpr(expr.left, false);
                base.buffer.Append(" ESCAPE ");
                this.FormatExpr(expr.right, false);
                return;
            }
            if (expr.Operator == 10)
            {
                base.FormatExpr(expr.left);
                base.buffer.Append(" = ");
                if (((expr.right.type == 5) && (expr.left.type == 4)) && ((SqlIdentifierExpr)expr.left).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                {
                    this.FormatValueKSQL_COL_NULLABLE(expr);
                    return;
                }
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
                return;
            }
            if (expr.Operator != 8)
            {
                if (expr.Operator == 1)
                {
                    base.FormatExpr(expr.left);
                    base.buffer.Append(" AS ");
                    if (expr.right.GetType() == typeof(SqlIdentifierExpr))
                    {
                        string str = ((SqlIdentifierExpr)expr.right).value.ToUpper();
                        base.buffer.Append(str);
                        return;
                    }
                    if (expr.right.GetType() == typeof(SqlCharExpr))
                    {
                        string str2 = ((SqlCharExpr)expr.right).text.ToUpper();
                        base.buffer.Append(str2);
                        return;
                    }
                    if (expr.right.GetType() == typeof(SqlNCharExpr))
                    {
                        string str3 = ((SqlNCharExpr)expr.right).text.ToUpper();
                        base.buffer.Append(str3);
                        return;
                    }
                    base.FormatExpr(expr.right);
                    return;
                }
                if (appendBrace)
                {
                    base.buffer.Append("(");
                }
                base.FormatExpr(expr.left);
                switch (expr.Operator)
                {
                    case 0:
                        base.buffer.Append(" + ");
                        goto Label_08AD;

                    case 1:
                        base.buffer.Append(" AS ");
                        goto Label_08AD;

                    case 2:
                        base.buffer.Append(" = ");
                        goto Label_08AD;

                    case 3:
                        throw new FormaterException("unexpect BinaryOpType: '" + expr.Operator + "'");

                    case 4:
                        throw new FormaterException("unexpect BinaryOpType: '" + expr.Operator + "'");

                    case 5:
                        throw new FormaterException("unexpect BinaryOpType: '" + expr.Operator + "'");

                    case 7:
                        base.buffer.Append(" AND ");
                        goto Label_08AD;

                    case 8:
                        base.buffer.Append(" OR ");
                        goto Label_08AD;

                    case 9:
                        base.buffer.Append(" / ");
                        goto Label_08AD;

                    case 10:
                        base.buffer.Append(" = ");
                        goto Label_08AD;

                    case 11:
                        base.buffer.Append(" > ");
                        goto Label_08AD;

                    case 12:
                        base.buffer.Append(" >= ");
                        goto Label_08AD;

                    case 14:
                        base.buffer.Append(" < ");
                        goto Label_08AD;

                    case 15:
                        base.buffer.Append(" <= ");
                        goto Label_08AD;

                    case 0x10:
                        base.buffer.Append(" <> ");
                        goto Label_08AD;

                    case 0x11:
                        throw new FormaterException("unexpect BinaryOpType: '" + expr.Operator + "'");

                    case 0x12:
                        base.buffer.Append(" LIKE ");
                        base.context.add("like_predicate", "1");
                        goto Label_08AD;

                    case 0x13:
                        base.buffer.Append(" >> ");
                        goto Label_08AD;

                    case 20:
                        base.buffer.Append(".");
                        goto Label_08AD;

                    case 0x15:
                        base.buffer.Append(" % ");
                        goto Label_08AD;

                    case 0x16:
                        base.buffer.Append(" * ");
                        goto Label_08AD;

                    case 0x17:
                        base.buffer.Append(" != ");
                        goto Label_08AD;

                    case 0x18:
                        base.buffer.Append(" !< ");
                        goto Label_08AD;

                    case 0x19:
                        base.buffer.Append(" !> ");
                        goto Label_08AD;

                    case 0x1a:
                        base.buffer.Append(" - ");
                        goto Label_08AD;

                    case 0x1b:
                        base.buffer.Append(" UNION ");
                        goto Label_08AD;

                    case 40:
                        base.buffer.Append(" NOT LIKE ");
                        goto Label_08AD;

                    case 0x2a:
                        base.buffer.Append(" + ");
                        goto Label_08AD;
                }
                throw new FormaterException("unexpect BinaryOpType: '" + expr.Operator + "'");
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
                    list.RemoveAt(list.Count - 1);
                }
            }
            if (appendBrace)
            {
                base.buffer.Append(")");
            }
            return;
            Label_08AD:
            base.FormatExpr(expr.right);
            if (!base.context.ContainsKey("like_predicate"))
            {
                base.context.add("like_predicate", "0");
            }
            else
            {
                base.context["like_predicate"] = "0";
            }
            if (appendBrace)
            {
                base.buffer.Append(")");
            }
        }

        protected override void FormatBlockStmt(SqlBlockStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatBreakStmt(SqlBreakStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatChar(SqlCharExpr expr)
        {
            string text = expr.text;
            if (text.EqualsIgnoreCase(Token.KSQL_CT_P.value))
            {
                text = "PK";
            }
            else if (text.EqualsIgnoreCase(Token.KSQL_CT_F.value))
            {
                text = "F";
            }
            else if (text.EqualsIgnoreCase(Token.KSQL_CT_U.value))
            {
                text = "UQ";
            }
            else if (text.EqualsIgnoreCase(Token.KSQL_CT_C.value))
            {
                text = "C";
            }
            if (base.context.ContainsKey("like_predicate") && (base.context["like_predicate"] == "1"))
            {
                text = text.Replace(@"\[", "[[]");
                base.context.add("like_predicate", "0");
            }
            base.buffer.Append("'");
            base.buffer.Append(text);
            base.buffer.Append("'");
        }

        protected override void FormatCloseStmt(SqlCloseStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatColumnDef(SqlColumnDef column)
        {
            throw new NotImplementedException();
        }

        protected override void FormatContinueStmt(SqlContinueStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatCreateTableStmt(SqlCreateTableStmt stmt)
        {
            base.ValidateCreateTableStmt(stmt);
            base.buffer.Append("CREATE TABLE ");
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
            this.FormatTableConstraintList(stmt.constraintList);
            base.buffer.Append(")");
        }

        protected override void FormatCursorLoopStmt(SqlCursorLoopStmt stmt)
        {
            throw new NotImplementedException();
        }

        public void FormatDataType(string dataType)
        {
            if (dataType.EqualsIgnoreCase("BIGINT"))
            {
                base.buffer.Append("BIGINT");
            }
            else if (dataType.EqualsIgnoreCase("BINARY"))
            {
                base.buffer.Append("BINARY");
            }
            else if (dataType.EqualsIgnoreCase("BIT"))
            {
                base.buffer.Append("BIT");
            }
            else if (dataType.EqualsIgnoreCase("CHAR"))
            {
                base.buffer.Append("CHAR");
            }
            else if (dataType.EqualsIgnoreCase("DATETIME"))
            {
                base.buffer.Append("DATETIME");
            }
            else if (dataType.EqualsIgnoreCase("DECIMAL"))
            {
                base.buffer.Append("DECIMAL");
            }
            else if (dataType.EqualsIgnoreCase("FLOAT"))
            {
                base.buffer.Append("FLOAT");
            }
            else if (dataType.EqualsIgnoreCase("IMAGE"))
            {
                base.buffer.Append("IMAGE");
            }
            else if (dataType.EqualsIgnoreCase("INT"))
            {
                base.buffer.Append("INT");
            }
            else if (dataType.EqualsIgnoreCase("MONEY"))
            {
                base.buffer.Append("MONEY");
            }
            else if (dataType.EqualsIgnoreCase("NCHAR"))
            {
                base.buffer.Append("NCHAR");
            }
            else if (dataType.EqualsIgnoreCase("NTEXT"))
            {
                base.buffer.Append("NTEXT");
            }
            else if (dataType.EqualsIgnoreCase("NUMERIC"))
            {
                base.buffer.Append("NUMERIC");
            }
            else if (dataType.EqualsIgnoreCase("NVARCHAR"))
            {
                base.buffer.Append("NVARCHAR");
            }
            else if (dataType.EqualsIgnoreCase("REAL"))
            {
                base.buffer.Append("REAL");
            }
            else if (dataType.EqualsIgnoreCase("SMALLDATETIME"))
            {
                base.buffer.Append("SMALLDATETIME");
            }
            else if (dataType.EqualsIgnoreCase("SMALLINT"))
            {
                base.buffer.Append("SMALLINT");
            }
            else if (dataType.EqualsIgnoreCase("SMALLMONEY"))
            {
                base.buffer.Append("SMALLMONEY");
            }
            else if (dataType.EqualsIgnoreCase("SQL_VARIANT"))
            {
                base.buffer.Append("SQL_VARIANT");
            }
            else if (dataType.EqualsIgnoreCase("TEXT"))
            {
                base.buffer.Append("TEXT");
            }
            else if (dataType.EqualsIgnoreCase("TIMESTAMP"))
            {
                base.buffer.Append("TIMESTAMP");
            }
            else if (dataType.EqualsIgnoreCase("TINYINT"))
            {
                base.buffer.Append("TINYINT");
            }
            else if (dataType.EqualsIgnoreCase("UNIQUEIDENTIFIER"))
            {
                base.buffer.Append("UNIQUEIDENTIFIER");
            }
            else if (dataType.EqualsIgnoreCase("VARCHAR"))
            {
                base.buffer.Append("VARCHAR");
            }
            else if (dataType.EqualsIgnoreCase("VARBINARY"))
            {
                base.buffer.Append("VARBINARY");
            }
            else if (dataType.EqualsIgnoreCase("BLOB"))
            {
                base.buffer.Append("IMAGE");
            }
            else if (dataType.EqualsIgnoreCase("CLOB"))
            {
                base.buffer.Append("TEXT");
            }
            else if (dataType.EqualsIgnoreCase("NCLOB"))
            {
                base.buffer.Append("NTEXT");
            }
            else if (dataType.EqualsIgnoreCase("XMLTYPE"))
            {
                base.buffer.Append("XMLTYPE");
            }
            else
            {
                base.buffer.Append(dataType);
            }
        }

        protected override void FormatDateTimeExpr(SqlDateTimeExpr expr)
        {
            base.buffer.Append("'");
            if (expr.timeType() == -19000)
            {
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
                base.buffer.Append((expr.getYear() < 10) ? "0" : "");
                base.buffer.Append(expr.getYear());
                base.buffer.Append((expr.getMonth() < 10) ? "-0" : "-");
                base.buffer.Append(expr.getMonth());
                base.buffer.Append((expr.getDate() < 10) ? "-0" : "-");
                base.buffer.Append(expr.getDate());
            }
            else if (expr.timeType() == -19002)
            {
                base.buffer.Append((expr.getHour() < 10) ? "0" : "");
                base.buffer.Append(expr.getHour());
                base.buffer.Append((expr.getMinute() < 10) ? ":0" : ":");
                base.buffer.Append(expr.getMinute());
                base.buffer.Append((expr.getSecond() < 10) ? ":0" : ":");
                base.buffer.Append(expr.getSecond());
            }
            base.buffer.Append("'");
        }

        protected override void FormatDeallocateStmt(SqlDeallocateStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected sealed override void FormatDropIndexStmt(SqlDropIndexStmt stmt)
        {
            base.buffer.Append("DROP INDEX ");
            base.buffer.Append(this.FormatTableName(stmt.tableName));
            base.buffer.Append(".");
            base.buffer.Append(base.FormateIndexName(stmt.indexName));
        }

        protected void FormateUpdateItem(AbstractUpdateItem abstract_item)
        {
            if (abstract_item.GetType() == typeof(SqlUpdateItem))
            {
                SqlUpdateItem item = (SqlUpdateItem)abstract_item;
                base.buffer.Append(base.FormatColumnName(item.name));
                base.buffer.Append(" = ");
                base.FormatExpr(item.expr);
            }
            else
            {
                if (!(abstract_item.GetType() == typeof(SubQueryUpdateItem)))
                {
                    throw new FormaterException("unexpect update item: '" + abstract_item + "'");
                }
                SubQueryUpdateItem item2 = (SubQueryUpdateItem)abstract_item;
                if (item2.columnList.Count != 1)
                {
                    throw new FormaterException("unexpect column list size: '" + item2.columnList.Count + "'");
                }
                string name = (string)item2.columnList[0];
                base.buffer.Append(base.FormatColumnName(name));
                base.buffer.Append(" = (");
                this.FormatSelectBase(item2.subQuery);
                base.buffer.Append(")");
            }
        }

        protected override void FormatExecStmt(SqlExecStmt stmt)
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

        public void FormatExpr(SqlExpr expr, bool appendBrace)
        {
            if (expr == null)
            {
                throw new ArgumentException("expr is null");
            }
            if (expr.type == 8)
            {
                base.buffer.Append("*");
            }
            else if (expr.type == 4)
            {
                this.FormatIdentifierExpr(expr);
            }
            else if (expr.type == 1)
            {
                base.buffer.Append(((SqlIntExpr)expr).text);
            }
            else if (expr.type == 0x1b)
            {
                base.buffer.Append(((SqlLongExpr)expr).text);
            }
            else if (expr.type == 2)
            {
                base.buffer.Append(((SqlDoubleExpr)expr).text);
            }
            else if (expr.type == 0)
            {
                this.FormatBinaryOpExpr((SqlBinaryOpExpr)expr, appendBrace);
            }
            else if (expr.type == 10)
            {
                this.FormatMethodInvokeExpr((SqlMethodInvokeExpr)expr);
            }
            else if (expr.type == 11)
            {
                this.FormatAggregateExprExpr((SqlAggregateExpr)expr);
            }
            else if (expr.type == 5)
            {
                this.FormatChar((SqlCharExpr)expr);
            }
            else if (expr.type == 6)
            {
                this.FormatNChar((SqlNCharExpr)expr);
            }
            else if (expr.type == 3)
            {
                this.FormatVarRef((SqlVarRefExpr)expr);
            }
            else if (expr.type == 12)
            {
                this.FormatCaseExpr((SqlCaseExpr)expr);
            }
            else if (expr.type == 14)
            {
                this.FormatInListExpr((SqlInListExpr)expr);
            }
            else if (expr.type == 15)
            {
                base.FormatExiststExpr((SqlExistsExpr)expr);
            }
            else if (expr.type == 13)
            {
                base.FormatInSubQueryExpr((SqlInSubQueryExpr)expr);
            }
            else if (expr.type == 0x10)
            {
                base.FormatAllExpr((SqlAllExpr)expr);
            }
            else if (expr.type == 0x11)
            {
                base.FormatBetweenExpr((SqlBetweenExpr)expr);
            }
            else if (expr.type == 0x12)
            {
                base.FormatAnyExpr((SqlAnyExpr)expr);
            }
            else if (expr.type == 0x13)
            {
                base.FormatSomeExpr((SqlSomeExpr)expr);
            }
            else if (expr.type == 20)
            {
                base.FormatNullExpr((SqlNullExpr)expr);
            }
            else if (expr.type == 0x1d)
            {
                base.FormatEmptyExpr((SqlEmptyExpr)expr);
            }
            else if (expr.type == 0x15)
            {
                this.FormatDateTimeExpr((SqlDateTimeExpr)expr);
            }
            else if (expr.type == 0x18)
            {
                base.FormatQueryExpr((QueryExpr)expr);
            }
            else if (expr.type == 0x19)
            {
                this.FormatPriorIdentifierExpr((SqlPriorIdentifierExpr)expr);
            }
            else if (expr.type == 9)
            {
                base.FormatNotExpr((SqlNotExpr)expr);
            }
            else if (expr.type == 0x1a)
            {
                JavaObjectValueExpr expr2 = (JavaObjectValueExpr)expr;
                object obj2 = expr2.value;
                if (obj2 == null)
                {
                    base.buffer.Append("NULL");
                }
                else if (obj2.GetType() == typeof(string))
                {
                    string str = (string)obj2;
                    str = str.Replace("'", "''");
                    base.buffer.Append(str);
                }
                else if (obj2.GetType() == typeof(DateTime))
                {
                    DateTime time = (DateTime)obj2;
                    base.buffer.Append(string.Concat(new object[] { "{", time.Year, "-", time.Month, "-", time.Day, " ", time.Hour, ":", time.Minute, ":", time.Second, "}" }));
                }
                else
                {
                    base.buffer.Append(obj2.ToString());
                }
            }
            else if (expr.type == 0x1c)
            {
                this.FormatIdentityExpr(expr);
            }
            else
            {
                if (expr.type != -1)
                {
                    throw new FormaterException("unexpect expression: '" + expr + "'");
                }
                this.FormatDefault((SqlDefaultExpr)expr);
            }
        }

        protected override void FormatFetchStmt(SqlFetchStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatGotoStmt(SqlGotoStmt stmt)
        {
            throw new NotImplementedException();
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
            if (!string.IsNullOrEmpty(str) && str.StartsWith("\""))
            {
                base.buffer.Append(str.ToUpper());
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
            base.buffer.Append(" Identity(int,1,1) ");
        }

        protected void FormatIdentityExpr(SqlExpr sqlexpr)
        {
            SqlIdentityExpr expr = (SqlIdentityExpr)sqlexpr;
            base.buffer.Append("IDENTITY(");
            this.FormatDataType(expr.dataType);
            base.buffer.Append(", ");
            base.buffer.Append(expr.seed);
            base.buffer.Append(", ");
            base.buffer.Append(expr.increment);
            base.buffer.Append(")");
        }

        protected override void FormatIfStmt(SqlIfStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatLabelStmt(SqlLabelStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatMethodInvokeExpr(SqlMethodInvokeExpr expr)
        {
        }

        protected override void FormatOpenStmt(SqlOpenStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatPriorIdentifierExpr(SqlPriorIdentifierExpr expr)
        {
            throw new FormaterException("Not Support. PriorIdentifierExpr");
        }

        protected override void FormatSelect(SqlSelect select)
        {
            bool flag2;
            bool flag3;
            bool flag4;
            Hashtable hashtable = select.getOptionMapDirect();
            base.buffer.Append("SELECT ");
            bool flag = false;
            if ((select.distinct == 1) && (select.orderBy.Count != 0))
            {
                flag = this.checkHaveChineseOrderBy(select.orderBy);
            }
            this.selectItemAliasMap = null;
            if (this.checkHaveChineseOrderBy(select.orderBy))
            {
                this.selectItemAliasMap = new Hashtable();
                foreach (SqlSelectItem item in select.selectList)
                {
                    if (!string.IsNullOrEmpty(item.alias))
                    {
                        this.selectItemAliasMap.Add(item.alias.ToUpper(), item.expr);
                    }
                }
            }
            this.FormatSelectDistinct(select);
            this.FormatSelectLimit(select);
            if (flag)
            {
                base.buffer.Append("* ");
                this.FormatSelectInto(select);
                base.buffer.Append("FROM ( SELECT TOP 2147483647 ");
            }
            this.FormatSelectColumnList(select, out flag2, out flag3, out flag4);
            if (!flag)
            {
                this.FormatSelectInto(select);
            }
            this.FormatSelectTabSrc(select);
            this.FormatSelectCondition(select, flag2, flag3, flag4);
            if (select.hierarchicalQueryClause != null)
            {
                throw new FormaterException("NOT SUPPORT hierarchicalQueryClause");
            }
            this.FormatSelectGroupBy(select);
            this.FormatSelectHaving(select);
            this.FormatSelectOrderBy(select);
            if ((hashtable != null) && (hashtable.Count != 0))
            {
                base.buffer.Append(" OPTION (");
                IEnumerator enumerator = hashtable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DictionaryEntry current = (DictionaryEntry)enumerator.Current;
                    string key = (string)current.Key;
                    if ("HASH GROUP".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" HASH GROUP");
                    }
                    else if ("ORDER GROUP".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" ORDER GROUP");
                    }
                    else if ("CONCAT UNION".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" CONCAT UNION");
                    }
                    else if ("HASH UNION".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" HASH UNION");
                    }
                    else if ("MERGE UNION".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" MERGE UNION");
                    }
                    else if ("LOOP JOIN".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" LOOP JOIN");
                    }
                    else if ("MERGE JOIN".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" MERGE JOIN");
                    }
                    else if ("HASH JOIN".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" HASH JOIN");
                    }
                    else if ("FAST".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" FAST ");
                        base.buffer.Append(current.Value);
                    }
                    else if ("FASTFIRSTROW".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" FASTFIRSTROW");
                    }
                    else if ("FORCE ORDER".EqualsIgnoreCase(key) || "ORDERED".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" FORCE ORDER");
                    }
                    else if ("MAXDOP".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" MAXDOP ");
                        base.buffer.Append(current.Value);
                    }
                    else if ("ROBUST PLAN".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" ROBUST PLAN");
                    }
                    else if ("KEEP PLAN".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" KEEP PLAN");
                    }
                    else if ("KEEPFIXED PLAN".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" KEEPFIXED PLAN");
                    }
                    else if ("EXPAND VIEWS".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append(" EXPAND VIEWS");
                    }
                    else if ("RECOMPILE".EqualsIgnoreCase(key))
                    {
                        base.buffer.Append("RECOMPILE");
                    }
                    base.buffer.Append(",");
                }
                base.buffer.Remove(base.buffer.Length - 1, 1);
                base.buffer.Append(")");
            }
            if (flag)
            {
                base.buffer.Append(") ");
                base.buffer.Append(new UUTN().ToString());
            }
        }

        private void FormatSelectColumnList(SqlSelect select, out bool sysTableFlag, out bool sysIndexFlag, out bool sysConstraintFlag)
        {
            bool flag = false;
            sysTableFlag = false;
            sysIndexFlag = false;
            sysConstraintFlag = false;
            foreach (SqlSelectItem item in select.selectList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                if (item.expr.GetType() == typeof(SqlIdentifierExpr))
                {
                    if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_COL_NAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_COL_DEFAULT.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_COL_NULLABLE.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_COL_TABNAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.TABNAME.value) && (item.alias == null))
                    {
                        sysTableFlag = true;
                        item.alias = Token.TABNAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.INDNAME.value) && (item.alias == null))
                    {
                        sysIndexFlag = true;
                        item.alias = Token.INDNAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value) && (item.alias == null))
                    {
                        sysConstraintFlag = true;
                        item.alias = Token.KSQL_CONS_NAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value) && (item.alias == null))
                    {
                        sysConstraintFlag = true;
                        item.alias = Token.KSQL_CONS_TABNAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value) && (item.alias == null))
                    {
                        sysConstraintFlag = true;
                        item.alias = Token.KSQL_CONS_TYPE.value;
                    }
                }
                else if (item.expr.GetType() == typeof(SqlAggregateExpr))
                {
                    foreach (SqlExpr expr in ((SqlAggregateExpr)item.expr).paramList)
                    {
                        if (expr.GetType() == typeof(SqlIdentifierExpr))
                        {
                            if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "COLUMN_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "DATA_DEFAULT";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "ISNULLABLE";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "TABLE_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.TABNAME.value))
                            {
                                sysTableFlag = true;
                                ((SqlIdentifierExpr)expr).value = "TABLE_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.INDNAME.value))
                            {
                                sysIndexFlag = true;
                                ((SqlIdentifierExpr)expr).value = "NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
                            {
                                sysConstraintFlag = true;
                                ((SqlIdentifierExpr)expr).value = "CONSTRAINT_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
                            {
                                sysConstraintFlag = true;
                                ((SqlIdentifierExpr)expr).value = "TABLE_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
                            {
                                sysConstraintFlag = true;
                                ((SqlIdentifierExpr)expr).value = "CASE CONSTRAINT_TYPE WHEN 'PK' THEN 'P' WHEN 'UQ' THEN 'U' ELSE CONSTRAINT_TYPE END";
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(item.alias))
                {
                    if (item.expr.GetType() == typeof(SqlIdentifierExpr))
                    {
                        if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value))
                        {
                            SqlIdentifierExpr expr2 = new SqlIdentifierExpr
                            {
                                value = "COLUMN_NAME"
                            };
                            this.FormatExpr(expr2, true);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                        {
                            SqlIdentifierExpr expr3 = new SqlIdentifierExpr
                            {
                                value = "DATA_DEFAULT"
                            };
                            this.FormatExpr(expr3, true);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
                        {
                            SqlIdentifierExpr expr4 = new SqlIdentifierExpr
                            {
                                value = "TABLE_NAME"
                            };
                            this.FormatExpr(expr4, true);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                        {
                            SqlIdentifierExpr expr5 = new SqlIdentifierExpr
                            {
                                value = "ISNULLABLE"
                            };
                            this.FormatExpr(expr5, true);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.TABNAME.value))
                        {
                            SqlIdentifierExpr expr6 = new SqlIdentifierExpr();
                            sysTableFlag = true;
                            expr6.value = "TABLE_NAME";
                            this.FormatExpr(expr6, true);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.INDNAME.value))
                        {
                            SqlIdentifierExpr expr7 = new SqlIdentifierExpr();
                            sysIndexFlag = true;
                            expr7.value = "INDEX_NAME";
                            this.FormatExpr(expr7, true);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
                        {
                            SqlIdentifierExpr expr8 = new SqlIdentifierExpr();
                            sysConstraintFlag = true;
                            expr8.value = "CONSTRAINT_NAME";
                            this.FormatExpr(expr8, true);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
                        {
                            SqlIdentifierExpr expr9 = new SqlIdentifierExpr();
                            sysConstraintFlag = true;
                            expr9.value = "TABLE_NAME";
                            this.FormatExpr(expr9, true);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
                        {
                            SqlIdentifierExpr expr10 = new SqlIdentifierExpr();
                            sysConstraintFlag = true;
                            expr10.value = "CASE CONSTRAINT_TYPE WHEN 'PK' THEN 'P' WHEN 'UQ' THEN 'U' ELSE CONSTRAINT_TYPE END";
                            this.FormatExpr(expr10, true);
                        }
                        else
                        {
                            this.FormatExpr(item.expr, true);
                        }
                    }
                    else
                    {
                        this.FormatExpr(item.expr, true);
                    }
                    base.buffer.Append(" ");
                    base.buffer.Append(item.alias.ToLower());
                }
                else if (item.expr.GetType() == typeof(SqlIdentifierExpr))
                {
                    if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value))
                    {
                        SqlIdentifierExpr expr11 = new SqlIdentifierExpr
                        {
                            value = "COLUMN_NAME"
                        };
                        this.FormatExpr(expr11, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                    {
                        SqlIdentifierExpr expr12 = new SqlIdentifierExpr
                        {
                            value = "DATA_DEFAULT"
                        };
                        this.FormatExpr(expr12, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                    {
                        SqlIdentifierExpr expr13 = new SqlIdentifierExpr
                        {
                            value = "ISNULLABLE"
                        };
                        this.FormatExpr(expr13, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
                    {
                        SqlIdentifierExpr expr14 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr14, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.TABNAME.value))
                    {
                        SqlIdentifierExpr expr15 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr15, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.INDNAME.value))
                    {
                        SqlIdentifierExpr expr16 = new SqlIdentifierExpr
                        {
                            value = "INDEX_NAME"
                        };
                        this.FormatExpr(expr16, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
                    {
                        SqlIdentifierExpr expr17 = new SqlIdentifierExpr
                        {
                            value = "CONSTRAINT_NAME"
                        };
                        this.FormatExpr(expr17, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
                    {
                        SqlIdentifierExpr expr18 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr18, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
                    {
                        SqlIdentifierExpr expr19 = new SqlIdentifierExpr
                        {
                            value = "CASE CONSTRAINT_TYPE WHEN 'PK' THEN 'P' WHEN 'UQ' THEN 'U' ELSE CONSTRAINT_TYPE END"
                        };
                        this.FormatExpr(expr19, false);
                    }
                    else
                    {
                        this.FormatExpr(item.expr, false);
                    }
                }
                else
                {
                    this.FormatExpr(item.expr, false);
                }
                flag = true;
            }
        }

        private void FormatSelectCondition(SqlSelect select, bool sysTableFlag, bool sysIndexFlag, bool sysConstraintFlag)
        {
            if (select.condition != null)
            {
                base.buffer.Append(" WHERE ");
                if (sysIndexFlag)
                {
                    base.buffer.Append("INDID >0 AND INDID <255 AND ");
                }
                else if (sysConstraintFlag)
                {
                    base.buffer.Append("");
                }
                base.FormatExpr(select.condition);
            }
            else if (sysIndexFlag)
            {
                base.buffer.Append(" WHERE INDID >0 AND INDID <255");
            }
            else if (sysConstraintFlag)
            {
                base.buffer.Append("");
            }
        }

        private void FormatSelectDistinct(SqlSelect select)
        {
            if (select.distinct == 1)
            {
                base.buffer.Append("DISTINCT ");
            }
            else if (select.distinct != 0)
            {
                throw new FormaterException("unexpect distinct option.");
            }
        }

        private void FormatSelectGroupBy(SqlSelect select)
        {
            if (select.groupBy.Count != 0)
            {
                base.buffer.Append(" GROUP BY ");
                bool flag = false;
                foreach (SqlExpr expr in select.groupBy)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(expr);
                    flag = true;
                }
                if (select.hasWithRollUp)
                {
                    base.buffer.Append(" WITH ROLLUP");
                }
            }
        }

        private void FormatSelectHaving(SqlSelect select)
        {
            if (select.having != null)
            {
                base.buffer.Append(" HAVING ");
                base.FormatExpr(select.having);
            }
        }

        private void FormatSelectInto(SqlSelect select)
        {
            if (select.into != null)
            {
                base.buffer.Append(" INTO ");
                base.buffer.Append(this.FormatTableName(select.into.new_table));
            }
        }

        private void FormatSelectLimit(SqlSelect select)
        {
            if (select.limit != null)
            {
                base.buffer.Append("TOP ");
                base.buffer.Append(select.limit.value);
                base.buffer.Append(" ");
                if (select.limit.type == 1)
                {
                    base.buffer.Append("PERCENT ");
                }
            }
        }

        public override void FormatSelectList(SqlSelect select)
        {
            bool sysTableFlag = false;
            bool sysIndexFlag = false;
            bool sysConstraintFlag = false;
            this.FormatSelectColumnList(select, out sysTableFlag, out sysIndexFlag, out sysConstraintFlag);
        }

        private void FormatSelectOrderBy(SqlSelect select)
        {
            if (select.orderBy.Count != 0)
            {
                base.buffer.Append(" ORDER BY ");
                bool flag = false;
                foreach (SqlOrderByItem item in select.orderBy)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    SqlExpr expr = item.expr;
                    if ((item.chineseOrderByMode != -1) && (item.expr.GetType() == typeof(SqlIdentifierExpr)))
                    {
                        string key = ((SqlIdentifierExpr)item.expr).value.ToUpper();
                        if (this.selectItemAliasMap.ContainsKey(key))
                        {
                            expr = (SqlExpr)this.selectItemAliasMap[key];
                            if (!(expr.GetType() == typeof(SqlIdentifierExpr)) && !(expr.GetType() == typeof(SqlBinaryOpExpr)))
                            {
                                expr = item.expr;
                            }
                        }
                    }
                    base.FormatExpr(expr);
                    if (item.chineseOrderByMode != -1)
                    {
                        if (item.chineseOrderByMode == 2)
                        {
                            base.buffer.Append(" COLLATE ");
                            base.buffer.Append("Chinese_PRC_CI_AS");
                        }
                        else if (item.chineseOrderByMode == 3)
                        {
                            base.buffer.Append(" COLLATE ");
                            base.buffer.Append("Chinese_PRC_STROKE_CI_AS");
                        }
                        else
                        {
                            int chineseOrderByMode = item.chineseOrderByMode;
                        }
                    }
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
        }

        private void FormatSelectTabSrc(SqlSelect select)
        {
            if (select.tableSource != null)
            {
                base.buffer.Append(" FROM ");
                this.FormatTableSource(select.tableSource);
            }
        }

        protected override void FormatSetLocalVariantStmt(SqlSetLocalVariantStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatShowColumnsStmt(SqlShowColumnsStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatShowTablesStmt(SqlShowTablesStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected void FormatTableConstraint(SqlTableConstraint constraint)
        {
            base.ValidConstraintName(constraint.name);
            if (!string.IsNullOrEmpty(constraint.name))
            {
                base.buffer.Append("CONSTRAINT ");
                base.buffer.Append(base.FormatConstraintName(constraint.name));
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
                    base.buffer.Append(base.FormatColumnName(str));
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
                    base.buffer.Append(base.FormatColumnName(str2));
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
                    base.buffer.Append(base.FormatColumnName(str3));
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
                    base.buffer.Append(base.FormatColumnName(str4));
                    flag4 = true;
                }
                base.buffer.Append(")");
                base.buffer.Append(" REFERENCES ");
                base.buffer.Append(this.FormatTableName(key2.refTableName));
                base.buffer.Append(" (");
                flag4 = false;
                foreach (string str5 in key2.refColumnList)
                {
                    if (flag4)
                    {
                        base.buffer.Append(", ");
                    }
                    base.buffer.Append(base.FormatColumnName(str5));
                    flag4 = true;
                }
                base.buffer.Append(")");
            }
            else
            {
                if (constraint.GetType() != typeof(SqlTableCheck))
                {
                    throw new FormaterException("unexpect constraint: '" + constraint + "'");
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
                        base.buffer.Append("(SELECT NAME AS TABLE_NAME, XTYPE AS TABLE_XTYPE FROM SYSOBJECTS WHERE XTYPE = 'U' OR XTYPE = 'V')");
                        str = " AS KSQL_USERTABLES";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.USERCOLUMNS.value))
                    {
                        base.buffer.Append("(SELECT SYSOBJECTS.NAME AS TABLE_NAME, SYSCOLUMNS.NAME AS COLUMN_NAME, SYSCOLUMNS.COLID AS COLUMN_ID, SYSTYPES.NAME AS DATA_TYPE, SYSCOLUMNS.LENGTH AS DATA_LENGTH, SYSCOLUMNS.PREC AS DATA_PRECISION, SYSCOLUMNS.SCALE AS DATA_SCALE, SYSCOLUMNS.ISNULLABLE AS NULLABLE, SYSCOMMENTS.TEXT AS DATA_DEFAULT FROM SYSCOLUMNS INNER JOIN SYSOBJECTS ON SYSCOLUMNS.ID = SYSOBJECTS.ID AND SYSOBJECTS.XTYPE = 'U' INNER JOIN SYSTYPES ON SYSCOLUMNS.XUSERTYPE = SYSTYPES.XUSERTYPE LEFT JOIN SYSCOMMENTS ON SYSCOLUMNS.CDEFAULT = SYSCOMMENTS.ID)");
                        str = " AS KSQL_USERCOLUMNS";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.TABLECOLUMNDEFAULTVALUE.value))
                    {
                        base.buffer.Append("(SELECT SYSOBJECTS.NAME AS TABLE_NAME, SYSCOLUMNS.NAME AS COLUMN_NAME, SYSCOLUMNS.COLID AS COLUMN_ID, SYSTYPES.NAME AS DATA_TYPE, SYSCOLUMNS.LENGTH AS DATA_LENGTH, SYSCOLUMNS.PREC AS DATA_PRECISION, SYSCOLUMNS.SCALE AS DATA_SCALE, SYSCOLUMNS.ISNULLABLE AS NULLABLE, SYSCOMMENTS.TEXT AS DATA_DEFAULT FROM SYSCOLUMNS INNER JOIN SYSOBJECTS ON SYSCOLUMNS.ID = SYSOBJECTS.ID AND SYSOBJECTS.XTYPE = 'U' INNER JOIN SYSTYPES ON SYSCOLUMNS.XUSERTYPE = SYSTYPES.XUSERTYPE LEFT JOIN SYSCOMMENTS ON SYSCOLUMNS.CDEFAULT = SYSCOMMENTS.ID AND SYSCOLUMNS.CDEFAULT>0)");
                        str = " AS KSQL_TABLECOLUMNDEFAULTVALUE";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.SYSINDEXES.value))
                    {
                        base.buffer.Append("(SELECT SYSOBJECTS.NAME AS TABLE_NAME, SYSINDEXES.NAME AS INDEX_NAME FROM SYSOBJECTS INNER JOIN SYSINDEXES ON SYSINDEXES.ID = SYSOBJECTS.ID)");
                        str = " AS KSQL_INDEXES";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.INDCOLUMNS.value))
                    {
                        base.buffer.Append("(SELECT SYSINDEXES.NAME AS INDEX_NAME, SYSCOLUMNS.NAME AS COLUMN_NAME FROM SYSINDEXES INNER JOIN SYSINDEXKEYS ON SYSINDEXES.ID = SYSINDEXKEYS.ID AND SYSINDEXES.INDID = SYSINDEXKEYS.INDID INNER JOIN SYSCOLUMNS ON SYSCOLUMNS.ID = SYSINDEXKEYS.ID AND SYSCOLUMNS.COLID = SYSINDEXKEYS.COLID)");
                        str = " AS KSQL_INDCOLUMNS";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.SYSCONSTRAINTS.value))
                    {
                        base.buffer.Append("(SELECT TABLE_OBJ.NAME AS TABLE_NAME, CONST_OBJ.NAME AS CONSTRAINT_NAME, CONST_OBJ.XTYPE AS CONSTRAINT_TYPE, CONST.COLID AS COLUMN_ID FROM SYSCONSTRAINTS CONST INNER JOIN SYSOBJECTS TABLE_OBJ ON CONST.ID = TABLE_OBJ.ID INNER JOIN SYSOBJECTS CONST_OBJ ON CONST.CONSTID = CONST_OBJ.ID)");
                        str = " AS KSQL_CONSTRAINTS";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.SYSCONSTRAINTS.value))
                    {
                        base.buffer.Append("(SELECT SYSOBJECTS.NAME AS TABLE_NAME, SYSCOLUMNS.NAME AS COLUMN_NAME, SYSCOLUMNS.COLID AS COLUMN_ID, SYSTYPES.NAME AS DATA_TYPE, SYSCOLUMNS.LENGTH AS DATA_LENGTH, SYSCOLUMNS.PREC AS DATA_PRECISION, SYSCOLUMNS.SCALE AS DATA_SCALE, SYSCOLUMNS.ISNULLABLE AS NULLABLE, SYSCOMMENTS.TEXT AS DATA_DEFAULT FROM SYSCOLUMNS INNER JOIN SYSOBJECTS ON SYSCOLUMNS.ID = SYSOBJECTS.ID AND SYSOBJECTS.XTYPE = 'U' INNER JOIN SYSTYPES ON SYSCOLUMNS.XUSERTYPE = SYSTYPES.XUSERTYPE LEFT JOIN SYSCOMMENTS ON SYSCOLUMNS.CDEFAULT = SYSCOMMENTS.ID AND SYSCOLUMNS.CDEFAULT>0)");
                        str = " AS KSQL_TABLECOLUMNDEFAULTVALUE";
                    }
                    else if (source.name.EqualsIgnoreCase(Token.USERVIEWS.value))
                    {
                        base.buffer.Append("(SELECT NAME AS TABLE_NAME FROM SYS.VIEWS)");
                        str = " AS KSQL_USERVIEWS";
                    }
                    else
                    {
                        base.buffer.Append(this.FormatTableName(source.name));
                    }
                    if (!string.IsNullOrEmpty(source.alias))
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(source.alias);
                    }
                    else
                    {
                        base.buffer.Append(str);
                    }
                    if (!string.IsNullOrEmpty(source.lockingHint))
                    {
                        base.buffer.Append(" WITH (");
                        base.buffer.Append(source.lockingHint.ToUpper());
                        base.buffer.Append(")");
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
                            base.buffer.Append(tableSource.alias.ToLower());
                        }
                    }
                    else
                    {
                        if (!(tableSource.GetType() == typeof(SqlTableFunctionTableSource)))
                        {
                            throw new FormaterException("unexpect tableSource: '" + tableSource + "'");
                        }
                        SqlTableFunctionTableSource source4 = (SqlTableFunctionTableSource)tableSource;
                        string pipeFunction = source4.PipeFunction;
                        if (this.IsTabFuncFormat)
                        {
                            pipeFunction = pipeFunction.Substring(6, pipeFunction.Length - 7).Replace(" ", "");
                            if (pipeFunction.substring(0, 11).EqualsIgnoreCase("fn_StrSplit"))
                            {
                                pipeFunction = pipeFunction.Replace(",',',", "_udt");
                                pipeFunction = pipeFunction.Substring(pipeFunction.IndexOf("@"), pipeFunction.IndexOf(")") - pipeFunction.IndexOf("@"));
                            }
                            base.buffer.Append(pipeFunction);
                            if ((source4.alias != null) && (source4.alias.Length != 0))
                            {
                                base.buffer.Append(" ");
                                base.buffer.Append(source4.alias);
                            }
                        }
                        else
                        {
                            base.buffer.Append(pipeFunction);
                            if ((source4.alias != null) && (source4.alias.Length != 0))
                            {
                                base.buffer.Append(" ");
                                base.buffer.Append(source4.alias);
                            }
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
                            throw new FormaterException("unexpect Join Type: '" + source2.joinType + "'");
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
                    for (int k = 0; k < item3.columnList.Count; k++)
                    {
                        string name = (string)item3.columnList[k];
                        if (k != 0)
                        {
                            base.buffer.Append(", ");
                        }
                        base.buffer.Append(base.FormatColumnName(name));
                        base.buffer.Append(" = ");
                        if (item3.subQuery.GetType() != typeof(SqlSelect))
                        {
                            throw new FormaterException("unexpect subquery item: '" + item3.subQuery + "'");
                        }
                        SqlSelect subQuery = (SqlSelect)item3.subQuery;
                        SqlSelectItem item4 = (SqlSelectItem)subQuery.selectList[k];
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
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    base.buffer.Append(" FROM ");
                    if (update.updateTable.alias != null)
                    {
                        base.buffer.Append(this.FormatTableName(update.updateTable.name));
                        base.buffer.Append(" " + update.updateTable.alias.ToUpper() + ", ");
                    }
                }
                else
                {
                    base.buffer.Append(", ");
                }
                SubQueryUpdateItem item5 = (SubQueryUpdateItem)list[i];
                if (item5.subQuery.GetType() != typeof(SqlSelect))
                {
                    throw new FormaterException("unexpect queryItem subQuery: '" + item5 + "'");
                }
                SqlSelect select2 = (SqlSelect)item5.subQuery;
                this.FormatTableSource(select2.tableSource);
            }
            flag = false;
            for (int j = 0; j < list.Count; j++)
            {
                SubQueryUpdateItem item6 = (SubQueryUpdateItem)list[j];
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

        protected void FormatValueKSQL_COL_NULLABLE(SqlBinaryOpExpr expr)
        {
            SqlCharExpr right = (SqlCharExpr)expr.right;
            if (right.text.EqualsIgnoreCase("Y"))
            {
                base.buffer.Append("1");
            }
            else
            {
                if (!right.text.EqualsIgnoreCase("N"))
                {
                    throw new NotSupportedException("unexpected expression: " + expr.ToString());
                }
                base.buffer.Append("0");
            }
        }

        protected override void FormatWhileStmt(SqlWhileStmt stmt)
        {
            throw new NotImplementedException();
        }

        private Hashtable GetUpdateStmtReplaceMap(SqlUpdate update, Hashtable updateTableAliasMap, SqlJoinedTableSource joinTableSource, bool firstFlag)
        {
            if (joinTableSource.left.GetType() == typeof(SqlJoinedTableSource))
            {
                this.GetUpdateStmtReplaceMap(update, updateTableAliasMap, (SqlJoinedTableSource)joinTableSource.left, firstFlag);
            }
            else if (joinTableSource.left.GetType() == typeof(SqlTableSource))
            {
                SqlTableSource left = (SqlTableSource)joinTableSource.left;
                if (((left.alias == null) && (update.updateTable.alias != null)) && (left.name.EqualsIgnoreCase(update.updateTable.name) && firstFlag))
                {
                    left.alias = update.updateTable.alias;
                    firstFlag = false;
                }
                else if (((left.name.EqualsIgnoreCase(update.updateTable.name) && (left.alias != null)) && ((update.updateTable.alias != null) && !left.alias.EqualsIgnoreCase(update.updateTable.alias))) && firstFlag)
                {
                    updateTableAliasMap.Add(left.alias.ToUpper(), update.updateTable.alias.ToUpper());
                    firstFlag = false;
                }
            }
            if (joinTableSource.right.GetType() == typeof(SqlJoinedTableSource))
            {
                this.GetUpdateStmtReplaceMap(update, updateTableAliasMap, (SqlJoinedTableSource)joinTableSource.right, firstFlag);
                return updateTableAliasMap;
            }
            if (joinTableSource.right.GetType() == typeof(SqlTableSource))
            {
                SqlTableSource right = (SqlTableSource)joinTableSource.right;
                if (((right.alias == null) && (update.updateTable.alias != null)) && (right.name.EqualsIgnoreCase(update.updateTable.name) && firstFlag))
                {
                    right.alias = update.updateTable.alias;
                    firstFlag = false;
                    return updateTableAliasMap;
                }
                if (((right.name.EqualsIgnoreCase(update.updateTable.name) && (right.alias != null)) && ((update.updateTable.alias != null) && !right.alias.EqualsIgnoreCase(update.updateTable.alias))) && firstFlag)
                {
                    updateTableAliasMap.Add(right.alias.ToUpper(), update.updateTable.alias.ToUpper());
                    firstFlag = false;
                }
            }
            return updateTableAliasMap;
        }

        private bool isUpdateSelf(SqlUpdate update, SqlJoinedTableSource joinTableSource)
        {
            if (joinTableSource.left.GetType() == typeof(SqlJoinedTableSource))
            {
                if (this.isUpdateSelf(update, (SqlJoinedTableSource)joinTableSource.left))
                {
                    return true;
                }
            }
            else if (joinTableSource.left.GetType() == typeof(SqlTableSource))
            {
                SqlTableSource left = (SqlTableSource)joinTableSource.left;
                if (left.name.EqualsIgnoreCase(update.updateTable.name))
                {
                    return true;
                }
            }
            if (joinTableSource.right.GetType() == typeof(SqlJoinedTableSource))
            {
                if (this.isUpdateSelf(update, (SqlJoinedTableSource)joinTableSource.right))
                {
                    return true;
                }
            }
            else if (joinTableSource.right.GetType() == typeof(SqlTableSource))
            {
                SqlTableSource right = (SqlTableSource)joinTableSource.right;
                if (right.name.EqualsIgnoreCase(update.updateTable.name))
                {
                    return true;
                }
            }
            return false;
        }

        protected bool replaceUpdateStmt(SqlUpdate update)
        {
            Hashtable updateTableAliasMap = null;
            bool firstFlag = true;
            bool flag2 = false;
            ArrayList list = new ArrayList();
            foreach (AbstractUpdateItem item in update.updateList)
            {
                if (item.GetType() == typeof(SubQueryUpdateItem))
                {
                    list.Add(item);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                SubQueryUpdateItem item2 = (SubQueryUpdateItem)list[i];
                if (!(item2.subQuery.GetType() == typeof(SqlSelect)))
                {
                    throw new FormaterException("unexpect subQuery item: '" + item2 + "'");
                }
                SqlSelect subQuery = (SqlSelect)item2.subQuery;
                if (update.updateTable.alias != null)
                {
                    if (subQuery.tableSource.GetType() == typeof(SqlJoinedTableSource))
                    {
                        SqlTableSourceBase tableSource = subQuery.tableSource;
                        if (tableSource.GetType() == typeof(SqlJoinedTableSource))
                        {
                            updateTableAliasMap = new Hashtable();
                            updateTableAliasMap = this.GetUpdateStmtReplaceMap(update, updateTableAliasMap, (SqlJoinedTableSource)tableSource, firstFlag);
                            if (!flag2)
                            {
                                flag2 = this.isUpdateSelf(update, (SqlJoinedTableSource)tableSource);
                            }
                        }
                        if (updateTableAliasMap != null)
                        {
                            this.ReplaceUpdateTableAlias(subQuery.tableSource, updateTableAliasMap);
                        }
                    }
                    else if (subQuery.tableSource.GetType() == typeof(SqlTableSource))
                    {
                        SqlTableSource source = (SqlTableSource)subQuery.tableSource;
                        if (update.updateTable.name.Equals(source.name))
                        {
                            if (updateTableAliasMap == null)
                            {
                                updateTableAliasMap = new Hashtable();
                            }
                            updateTableAliasMap.Add(subQuery.tableSource.alias, update.updateTable.alias.ToUpper());
                            subQuery.tableSource.alias = update.updateTable.alias.ToUpper();
                            flag2 = true;
                        }
                    }
                }
            }
            if (!flag2)
            {
                return false;
            }
            foreach (AbstractUpdateItem item3 in update.updateList)
            {
                if (item3.GetType() == typeof(SubQueryUpdateItem))
                {
                    SubQueryUpdateItem item4 = (SubQueryUpdateItem)item3;
                    for (int k = 0; k < item4.columnList.Count; k++)
                    {
                        if (item4.subQuery.GetType() != typeof(SqlSelect))
                        {
                            throw new FormaterException("unexpect subQuery item: '" + item4 + "'");
                        }
                        SqlSelect select2 = (SqlSelect)item4.subQuery;
                        SqlSelectItem item5 = (SqlSelectItem)select2.selectList[k];
                        if (updateTableAliasMap != null)
                        {
                            base.ReplaceExpr(item5.expr, updateTableAliasMap);
                        }
                    }
                }
            }
            for (int j = 0; j < list.Count; j++)
            {
                SubQueryUpdateItem item6 = (SubQueryUpdateItem)list[j];
                if (item6.subQuery.GetType() != typeof(SqlSelect))
                {
                    throw new FormaterException("unexpect subQuery item: '" + item6 + "'");
                }
                SqlSelect select3 = (SqlSelect)item6.subQuery;
                if ((select3.condition != null) && (updateTableAliasMap != null))
                {
                    base.ReplaceExpr(select3.condition, updateTableAliasMap);
                    firstFlag = false;
                }
            }
            return flag2;
        }

        protected void ReplaceUpdateTableAlias(SqlTableSourceBase tableSource, Hashtable updateTableAliasMap)
        {
            if (tableSource != null)
            {
                if (tableSource.GetType() == typeof(SqlTableSource))
                {
                    if ((tableSource.alias != null) && (updateTableAliasMap[tableSource.alias.ToUpper()] != null))
                    {
                        tableSource.alias = (string)updateTableAliasMap[tableSource.alias.ToUpper()];
                    }
                }
                else if (tableSource.GetType() == typeof(SqlJoinedTableSource))
                {
                    SqlJoinedTableSource source = (SqlJoinedTableSource)tableSource;
                    this.ReplaceUpdateTableAlias(source.left, updateTableAliasMap);
                    this.ReplaceUpdateTableAlias(source.right, updateTableAliasMap);
                    if (source.condition != null)
                    {
                        base.ReplaceExpr(source.condition, updateTableAliasMap);
                    }
                }
                else if (tableSource.GetType() != typeof(SqlSubQueryTableSource))
                {
                    throw new FormaterException("unexpect tableSource: '" + tableSource + "'");
                }
            }
        }
    }






}
