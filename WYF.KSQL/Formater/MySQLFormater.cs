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
    public class MySQLFormater : SQLFormater
    {
        // Fields
        private static int tempColIndex = 1;
        private static int tempTabIndex = 1;

        // Methods
        public MySQLFormater() : base(null)
        {
        }

        public MySQLFormater(StringBuilder sb) : base(sb)
        {
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
                        builder.Append("call xalter_table('" + stmt.tableName + "', '(");
                    }
                    else
                    {
                        builder.Append("), (");
                    }
                    if (obj2.GetType() == typeof(SqlAlterTableAddItem))
                    {
                        SqlAlterTableAddItem item = (SqlAlterTableAddItem)obj2;
                        bool flag2 = false;
                        foreach (SqlColumnDef def in item.columnDefItemList)
                        {
                            if (flag2)
                            {
                                builder.Append("), (");
                            }
                            builder.Append(" ADD ");
                            StringBuilder buffer = base.buffer;
                            base.buffer = new StringBuilder();
                            this.FormatColumnDef(def);
                            builder.Append(base.buffer);
                            base.buffer = buffer;
                            flag2 = true;
                        }
                        flag2 = false;
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
                    else if (obj2.GetType() == typeof(SqlAlterTableDropItem))
                    {
                        SqlAlterTableDropItem item2 = (SqlAlterTableDropItem)obj2;
                        bool flag3 = false;
                        foreach (string str in item2.columnDefItemList)
                        {
                            if (flag3)
                            {
                                builder.Append("), (");
                            }
                            builder.Append(" DROP COLUMN ");
                            builder.Append(str);
                            flag3 = true;
                        }
                        flag3 = false;
                        foreach (string str2 in item2.constraintItemList)
                        {
                            if (flag3)
                            {
                                builder.Append("), (");
                            }
                            else
                            {
                                builder.Append(" DROP CONSTRAINT ");
                            }
                            builder.Append(str2);
                            flag3 = true;
                        }
                    }
                    else
                    {
                        if (obj2.GetType() != typeof(SqlAlterTableAlterColumnItem))
                        {
                            throw new FormaterException("TODO");
                        }
                        SqlAlterTableAlterColumnItem item3 = (SqlAlterTableAlterColumnItem)obj2;
                        builder.Append(" MODIFY COLUMN ");
                        StringBuilder builder4 = base.buffer;
                        base.buffer = new StringBuilder();
                        this.FormatColumnDef(item3.columnDef);
                        builder.Append(base.buffer);
                        base.buffer = builder4;
                    }
                    flag = true;
                }
                if (flag)
                {
                    base.buffer.Append(builder.ToString());
                    base.buffer.Append(")'");
                }
                base.buffer.Append(")");
            }
        }

        protected void FormatAlterTableStmtOld(SqlAlterTableStmt stmt)
        {
            base.buffer.Append("ALTER TABLE ");
            base.buffer.Append(stmt.tableName);
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
            else if (stmt.item.GetType() == typeof(SqlAlterTableAlterColumnItem))
            {
                SqlAlterTableAlterColumnItem item3 = (SqlAlterTableAlterColumnItem)stmt.item;
                base.buffer.Append(" ALTER COLUMN ");
                this.FormatColumnDef(item3.columnDef);
            }
            else if (stmt.item is SqlAlterTableAddDefaultItem)
            {
                SqlAlterTableAddDefaultItem item4 = (SqlAlterTableAddDefaultItem)stmt.item;
                base.buffer.Append(" ALTER COLUMN ");
                base.buffer.Append(item4.columnName);
                base.buffer.Append(" SET DEFAULT ");
                base.FormatExpr(item4.value);
            }
            else
            {
                if (!(stmt.item is SqlAlterTableDropDefaultItem))
                {
                    throw new FormaterException("TODO");
                }
                SqlAlterTableDropDefaultItem item5 = (SqlAlterTableDropDefaultItem)stmt.item;
                base.buffer.Append(" ALTER COLUMN ");
                base.buffer.Append(item5.columnName);
                base.buffer.Append(" DROP DEFAULT");
            }
        }

        protected override void FormatBinaryOpExpr(SqlBinaryOpExpr expr, bool appendBrace)
        {
            if (expr.Operator == 0x15)
            {
                base.buffer.Append("MOD(");
                base.FormatExpr(expr.left);
                base.buffer.Append(", ");
                base.FormatExpr(expr.right);
                base.buffer.Append(")");
                return;
            }
            if (expr.Operator == 0x2a)
            {
                base.buffer.Append("CONCAT(");
                base.FormatExpr(expr.left);
                base.buffer.Append(", ");
                base.FormatExpr(expr.right);
                base.buffer.Append(")");
                return;
            }
            if (expr.Operator == 13)
            {
                base.buffer.Append("(");
                base.FormatExpr(expr.left);
                base.buffer.Append(" IS NULL)");
                return;
            }
            if (expr.Operator == 0x29)
            {
                base.buffer.Append("(");
                base.FormatExpr(expr.left);
                base.buffer.Append(" IS NOT NULL)");
                return;
            }
            if (expr.Operator == 0)
            {
                if (((expr.left.GetType() == typeof(SqlCharExpr)) || (expr.left.GetType() == typeof(SqlNCharExpr))) || ((expr.right.GetType() == typeof(SqlCharExpr)) || (expr.right.GetType() == typeof(SqlNCharExpr))))
                {
                    base.buffer.Append("CONCAT(");
                    base.FormatExpr(expr.left);
                    base.buffer.Append(", ");
                    base.FormatExpr(expr.right);
                    base.buffer.Append(")");
                    return;
                }
                if (expr.left.GetType() == typeof(SqlMethodInvokeExpr))
                {
                    SqlMethodInvokeExpr left = (SqlMethodInvokeExpr)expr.left;
                    if (((left.methodName.Equals("CHAR") || left.methodName.Equals("LTRIM")) || (left.methodName.Equals("RTRIM") || left.methodName.Equals("LCASE"))) || ((left.methodName.Equals("LOWER") || left.methodName.Equals("UPPER")) || (left.methodName.Equals("UCASE") || left.methodName.Equals("SUBSTR"))))
                    {
                        base.FormatExpr(expr.left);
                        base.buffer.Append(" || ");
                        base.FormatExpr(expr.right);
                        return;
                    }
                }
                if (expr.right.GetType() == typeof(SqlMethodInvokeExpr))
                {
                    SqlMethodInvokeExpr right = (SqlMethodInvokeExpr)expr.right;
                    if (right.methodName.Equals("CHAR"))
                    {
                        base.FormatExpr(expr.left);
                        base.buffer.Append(" || ");
                        base.FormatExpr(expr.right);
                        return;
                    }
                }
                if (expr.left.GetType() == typeof(SqlBinaryOpExpr))
                {
                    SqlBinaryOpExpr expr4 = (SqlBinaryOpExpr)expr.left;
                    if (((expr4.left.GetType() == typeof(SqlCharExpr)) || (expr4.left.GetType() == typeof(SqlNCharExpr))) || ((expr4.right.GetType() == typeof(SqlCharExpr)) || (expr4.right.GetType() == typeof(SqlNCharExpr))))
                    {
                        base.FormatExpr(expr.left);
                        base.buffer.Append(" || ");
                        base.FormatExpr(expr.right);
                        return;
                    }
                }
                if (expr.right.GetType() == typeof(SqlBinaryOpExpr))
                {
                    SqlBinaryOpExpr expr5 = (SqlBinaryOpExpr)expr.right;
                    if (((expr5.left.GetType() == typeof(SqlCharExpr)) || (expr5.left.GetType() == typeof(SqlNCharExpr))) || ((expr5.right.GetType() == typeof(SqlCharExpr)) || (expr5.right.GetType() == typeof(SqlNCharExpr))))
                    {
                        base.FormatExpr(expr.left);
                        base.buffer.Append(" || ");
                        base.FormatExpr(expr.right);
                        return;
                    }
                }
            }
            if (expr.Operator != 8)
            {
                if (expr.Operator == 10)
                {
                    base.FormatExpr(expr.left);
                    base.buffer.Append(" = ");
                    base.FormatExpr(expr.right);
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
                        goto Label_0ABF;

                    case 1:
                        base.buffer.Append(" AS ");
                        goto Label_0ABF;

                    case 2:
                        base.buffer.Append(" = ");
                        goto Label_0ABF;

                    case 3:
                        throw new FormaterException("not support");

                    case 4:
                        throw new FormaterException("not support");

                    case 5:
                        throw new FormaterException("not support");

                    case 7:
                        base.buffer.Append(" AND ");
                        goto Label_0ABF;

                    case 8:
                        base.buffer.Append(" OR ");
                        goto Label_0ABF;

                    case 9:
                        base.buffer.Append(" / ");
                        goto Label_0ABF;

                    case 10:
                        base.buffer.Append(" = ");
                        goto Label_0ABF;

                    case 11:
                        base.buffer.Append(" > ");
                        goto Label_0ABF;

                    case 12:
                        base.buffer.Append(" >= ");
                        goto Label_0ABF;

                    case 14:
                        base.buffer.Append(" < ");
                        goto Label_0ABF;

                    case 15:
                        base.buffer.Append(" <= ");
                        goto Label_0ABF;

                    case 0x10:
                        base.buffer.Append(" <> ");
                        goto Label_0ABF;

                    case 0x11:
                        throw new FormaterException("not support");

                    case 0x12:
                        base.buffer.Append(" LIKE ");
                        goto Label_0ABF;

                    case 0x13:
                        base.buffer.Append(" >> ");
                        goto Label_0ABF;

                    case 20:
                        base.buffer.Append(".");
                        goto Label_0ABF;

                    case 0x15:
                        base.buffer.Append(" % ");
                        goto Label_0ABF;

                    case 0x16:
                        base.buffer.Append(" * ");
                        goto Label_0ABF;

                    case 0x17:
                        base.buffer.Append(" != ");
                        goto Label_0ABF;

                    case 0x18:
                        base.buffer.Append(" !< ");
                        goto Label_0ABF;

                    case 0x19:
                        base.buffer.Append(" !> ");
                        goto Label_0ABF;

                    case 0x1a:
                        base.buffer.Append(" - ");
                        goto Label_0ABF;

                    case 0x1b:
                        base.buffer.Append(" UNION ");
                        goto Label_0ABF;

                    case 40:
                        base.buffer.Append(" NOT LIKE ");
                        goto Label_0ABF;
                }
                throw new FormaterException("not support");
            }
            if (appendBrace)
            {
                base.buffer.Append("(");
            }
            ArrayList list = new ArrayList();
            SqlExpr expr6 = expr;
            while ((expr6 != null) || (list.Count != 0))
            {
                while (expr6 != null)
                {
                    if (!(expr6.GetType() == typeof(SqlBinaryOpExpr)))
                    {
                        base.FormatExpr(expr6);
                        expr6 = null;
                    }
                    else
                    {
                        SqlBinaryOpExpr expr7 = (SqlBinaryOpExpr)expr6;
                        if (expr7.Operator == 8)
                        {
                            if (expr7.left.GetType() == typeof(SqlBinaryOpExpr))
                            {
                                SqlBinaryOpExpr expr8 = (SqlBinaryOpExpr)expr7.left;
                                if (expr8.Operator == 8)
                                {
                                    list.Add(expr7.right);
                                    expr6 = expr8;
                                }
                                else
                                {
                                    base.FormatExpr(expr8);
                                    base.buffer.Append(" OR ");
                                    expr6 = expr7.right;
                                }
                            }
                            else
                            {
                                base.FormatExpr(expr7.left);
                                base.buffer.Append(" OR ");
                                expr6 = expr7.right;
                            }
                        }
                        else
                        {
                            base.FormatExpr(expr6);
                            expr6 = null;
                        }
                    }
                }
                if (list.Count != 0)
                {
                    base.buffer.Append(" OR ");
                    expr6 = (SqlExpr)list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                }
            }
            if (appendBrace)
            {
                base.buffer.Append(")");
            }
            return;
            Label_0ABF:
            base.FormatExpr(expr.right);
            if (appendBrace)
            {
                base.buffer.Append(")");
            }
        }

        protected override void FormatBlockStmt(SqlBlockStmt stmt)
        {
            if ((stmt.declItemList != null) && (stmt.declItemList.Count > 0))
            {
                int num = 0;
                int num2 = stmt.declItemList.Count;
                while (num < num2)
                {
                    SqlBlockStmt.DeclItem item = (SqlBlockStmt.DeclItem)stmt.declItemList[num];
                    if (item is SqlBlockStmt.DeclVarItem)
                    {
                        SqlBlockStmt.DeclVarItem item2 = (SqlBlockStmt.DeclVarItem)item;
                        base.buffer.Append("DECLARE ");
                        string name = item2.name;
                        base.buffer.Append(name);
                        base.buffer.Append(" ");
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
                            base.buffer.Append("BLOB");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("INT"))
                        {
                            base.buffer.Append("INT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NCHAR"))
                        {
                            base.buffer.Append("NCHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NTEXT"))
                        {
                            base.buffer.Append("LONGTEXT");
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
                            base.buffer.Append("VARCHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("REAL"))
                        {
                            base.buffer.Append("REAL");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("SMALLDATETIME"))
                        {
                            base.buffer.Append("DATETIME");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("SMALLINT"))
                        {
                            base.buffer.Append("SMALLINT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("TEXT"))
                        {
                            base.buffer.Append("LONGTEXT");
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
                        else if (item2.dataType.EqualsIgnoreCase("BLOB"))
                        {
                            base.buffer.Append("LONGBLOB");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("CLOB"))
                        {
                            base.buffer.Append("LONGTEXT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NCLOB"))
                        {
                            base.buffer.Append("LONGTEXT");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("XMLTYPE") || item2.dataType.EqualsIgnoreCase("XML"))
                        {
                            base.buffer.Append("LONGTEXT");
                        }
                        else
                        {
                            base.buffer.Append(item2.dataType);
                        }
                        if (item2.defaultValueExpr != null)
                        {
                            base.buffer.Append(" DEFAULT ");
                            base.FormatExpr(item2.defaultValueExpr);
                        }
                    }
                    else
                    {
                        if (!(item is SqlBlockStmt.DeclCurItem))
                        {
                            throw new FormaterException("unexpected statement: '" + item + "'");
                        }
                        SqlBlockStmt.DeclCurItem item3 = (SqlBlockStmt.DeclCurItem)item;
                        base.buffer.Append("DECLARE ");
                        string str = item3.name;
                        if (((str != null) && (str.Length > 0)) && (str.CharAt(0) == '@'))
                        {
                            str = str.Substring(1);
                            base.buffer.Append(str);
                        }
                        else
                        {
                            base.buffer.Append(str);
                        }
                        base.buffer.Append(" CURSOR FOR ");
                        this.FormatSelectBase(item3.select);
                    }
                    base.buffer.Append(";\n");
                    num++;
                }
                base.buffer.Append("\n");
            }
            int num3 = 0;
            int count = stmt.stmtList.Count;
            while (num3 < count)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[num3];
                base.FormatStmt(stmt2);
                base.buffer.Append(";\n");
                num3++;
            }
            if ((stmt.declItemList != null) && (stmt.declItemList.Count > 0))
            {
                int num5 = 0;
                int num6 = stmt.declItemList.Count;
                while (num5 < num6)
                {
                    SqlBlockStmt.DeclItem item4 = (SqlBlockStmt.DeclItem)stmt.declItemList[num5];
                    if (item4 is SqlBlockStmt.DeclCurItem)
                    {
                        base.buffer.Append("DEALLOCATE ");
                        base.buffer.Append(item4.name);
                        base.buffer.Append(";\n");
                    }
                    num5++;
                }
            }
        }

        protected override void FormatBreakStmt(SqlBreakStmt stmt)
        {
            throw new FormaterException("TODO");
        }

        protected void FormatChar(SqlCharExpr expr)
        {
            string text = expr.text;
            if (text.EqualsIgnoreCase(Token.KSQL_CT_P.value))
            {
                text = "PRIMARY KEY";
            }
            else if (text.EqualsIgnoreCase(Token.KSQL_CT_F.value))
            {
                text = "FOREIGN KEY";
            }
            else if (text.EqualsIgnoreCase(Token.KSQL_CT_U.value))
            {
                text = "UNIQUE";
            }
            else if (text.EqualsIgnoreCase(Token.KSQL_CT_C.value))
            {
                text = "CHECK";
            }
            base.buffer.Append("'");
            if (base.context["toUpperCase"] != null)
            {
                base.buffer.Append(text.ToUpper());
            }
            else
            {
                base.buffer.Append(text);
            }
            base.buffer.Append("'");
        }

        protected override void FormatCloseStmt(SqlCloseStmt stmt)
        {
            base.buffer.Append("CLOSE ");
            base.buffer.Append(stmt.curName);
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
            if (column.dataType.EqualsIgnoreCase("BIGINT"))
            {
                base.buffer.Append("BIGINT");
            }
            else if (column.dataType.EqualsIgnoreCase("BINARY"))
            {
                base.buffer.Append("CHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("CHAR"))
            {
                base.buffer.Append("CHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
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
            else if (column.dataType.EqualsIgnoreCase("IMAGE") || column.dataType.EqualsIgnoreCase("BLOB"))
            {
                base.buffer.Append("BLOB");
            }
            else if (column.dataType.EqualsIgnoreCase("INT"))
            {
                base.buffer.Append("INT");
            }
            else if (column.dataType.EqualsIgnoreCase("NCHAR"))
            {
                base.buffer.Append("CHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("NTEXT"))
            {
                base.buffer.Append("LONGTEXT");
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
                base.buffer.Append("VARCHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("REAL"))
            {
                base.buffer.Append("REAL");
            }
            else if (column.dataType.EqualsIgnoreCase("TEXT"))
            {
                base.buffer.Append("LONGTEXT");
            }
            else if (column.dataType.EqualsIgnoreCase("TIMESTAMP"))
            {
                base.buffer.Append("TIMESTAMP");
            }
            else if (column.dataType.EqualsIgnoreCase("TIME"))
            {
                base.buffer.Append("TIME");
            }
            else if (column.dataType.EqualsIgnoreCase("DATE"))
            {
                base.buffer.Append("DATE");
            }
            else if (column.dataType.EqualsIgnoreCase("YEAR"))
            {
                base.buffer.Append("YEAR");
            }
            else if (column.dataType.EqualsIgnoreCase("VARCHAR"))
            {
                base.buffer.Append("VARCHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("VARBINARY"))
            {
                base.buffer.Append("TINYBLOB");
            }
            else if (column.dataType.EqualsIgnoreCase("BLOB"))
            {
                base.buffer.Append("LONGBLOB");
            }
            else if (column.dataType.EqualsIgnoreCase("CLOB"))
            {
                base.buffer.Append("LONGTEXT");
            }
            else if (column.dataType.EqualsIgnoreCase("NCLOB"))
            {
                base.buffer.Append("LONGTEXT");
            }
            else if (column.dataType.EqualsIgnoreCase("XMLTYPE") || column.dataType.EqualsIgnoreCase("XML"))
            {
                base.buffer.Append("LONGTEXT");
            }
            else
            {
                base.buffer.Append(column.dataType);
            }
            if (column.IsComputedColumn)
            {
                base.buffer.Append(" AS (");
                base.FormatExpr(column.ComputedColumnExpr);
                base.buffer.Append(")");
            }
            else
            {
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
                if (((!column.dataType.EqualsIgnoreCase("IMAGE") && !column.dataType.EqualsIgnoreCase("BLOB")) && (!column.dataType.EqualsIgnoreCase("CLOB") && !column.dataType.EqualsIgnoreCase("NCLOB"))) && ((!column.dataType.EqualsIgnoreCase("TEXT") && !column.dataType.EqualsIgnoreCase("NTEXT")) && (!column.dataType.EqualsIgnoreCase("XMLTYPE") && !column.dataType.EqualsIgnoreCase("XML"))))
                {
                    if (column.defaultValueExpr != null)
                    {
                        base.buffer.Append(" DEFAULT ");
                        base.FormatExpr(column.defaultValueExpr);
                    }
                    if (column.autoIncrement)
                    {
                        base.buffer.Append(" AUTO_INCREMENT");
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
            }
        }

        protected override void FormatContinueStmt(SqlContinueStmt stmt)
        {
            base.buffer.Append("CONTINUE");
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
            foreach (object obj2 in stmt.itemList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                SqlOrderByItem item = (SqlOrderByItem)obj2;
                if (item.chineseOrderByMode != -1)
                {
                    base.buffer.Append("CONVERT(");
                    base.FormatExpr(item.expr);
                    base.buffer.Append(" USING ");
                    if (item.chineseOrderByMode == 2)
                    {
                        base.buffer.Append("GBK)");
                    }
                    else if (item.chineseOrderByMode == 3)
                    {
                        base.buffer.Append("GBK)");
                    }
                    else if (item.chineseOrderByMode == 4)
                    {
                        base.buffer.Append("GBK)");
                    }
                }
                else
                {
                    base.FormatExpr(item.expr);
                }
                if (item.mode != 0)
                {
                    base.buffer.Append(" DESC");
                }
                flag = true;
            }
            base.buffer.Append(")");
            string tableSpace = null;
            if (UUTN.isTempTable(stmt.tableName) && (base.options != null))
            {
                tableSpace = base.options.GetTempTableSpace();
            }
            this.FormatTableSpace(tableSpace);
        }

        protected override void FormatCreateTableStmt(SqlCreateTableStmt stmt)
        {
            base.ValidateCreateTableStmt(stmt);
            base.buffer.Append("CREATE");
            if (UUTN.isGlobalTempTable(stmt.name))
            {
                base.buffer.Append(" TEMPORARY");
                stmt.name = stmt.name.Substring(2);
            }
            else if (UUTN.isTempTable(stmt.name))
            {
                base.buffer.Append(" TEMPORARY");
                stmt.name = stmt.name.Substring(1);
            }
            if (stmt.name.IndexOf(" ") >= 0)
            {
                base.buffer.Append(" TABLE [");
                base.buffer.Append(stmt.name);
                base.buffer.Append("] (");
            }
            else
            {
                base.buffer.Append(" TABLE ");
                base.buffer.Append(stmt.name);
                base.buffer.Append(" (");
            }
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
            string tableSpace = null;
            if (UUTN.isTempTable(stmt.name) && (base.options != null))
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
            if (((curName != null) && (curName.Length > 0)) && (curName.CharAt(0) == '@'))
            {
                curName = curName.Substring(1);
            }
            base.buffer.Append("OPEN ");
            base.buffer.Append(curName);
            base.buffer.Append(";\n");
            base.buffer.Append("FETCH ");
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
            base.buffer.Append(";\n");
            base.buffer.Append("WHILE FETCH_STATUS = 0 DO\n");
            base.buffer.Append("BEGIN\n");
            int num3 = 0;
            int num4 = stmt.stmtList.Count;
            while (num3 < num4)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[num3];
                base.FormatStmt(stmt2);
                base.buffer.Append(";\n");
                num3++;
            }
            base.buffer.Append("FETCH ");
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
            base.buffer.Append(";\n");
            base.buffer.Append("END;\nEND WHILE;\n");
            base.buffer.Append("CLOSE ");
            base.buffer.Append(curName);
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

        private bool FormatDateTimeFunc(string methodNameI, SqlMethodInvokeExpr expr)
        {
            if (methodNameI.CompareTo("CURDATE") == 0)
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CURDATE()");
                return true;
            }
            if (methodNameI.CompareTo("CURTIME") == 0)
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CONCAT('', CURTIME())");
                return true;
            }
            if (methodNameI.CompareTo("DATEADD") == 0)
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("DATE_ADD(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", INTERVAL ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" SECOND)");
                    return true;
                }
                if (expr.parameters.Count != 3)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DATE_ADD(");
                base.FormatExpr((SqlExpr)expr.parameters[2]);
                base.buffer.Append(", INTERVAL ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                SqlExpr expr2 = (SqlExpr)expr.parameters[0];
                if (!(expr2 is SqlIdentifierExpr))
                {
                    throw new FormaterException("illegal datepart.");
                }
                string str = ((SqlIdentifierExpr)expr2).value;
                if ((str == null) || (str.Length == 0))
                {
                    throw new FormaterException("illegal datepart.");
                }
                str = str.ToUpper();
                if (("YEAR".Equals(str) || "YY".Equals(str)) || "YYYY".Equals(str))
                {
                    base.buffer.Append(" YEAR)");
                }
                else if (("MONTH".Equals(str) || "MM".Equals(str)) || "M".Equals(str))
                {
                    base.buffer.Append(" MONTH)");
                }
                else if (("DAY".Equals(str) || "DD".Equals(str)) || "D".Equals(str))
                {
                    base.buffer.Append(" DAY)");
                }
                else if ("HOUR".Equals(str) || "HH".Equals(str))
                {
                    base.buffer.Append(" HOUR)");
                }
                else if (("MINUTE".Equals(str) || "MI".Equals(str)) || "N".Equals(str))
                {
                    base.buffer.Append(" MINUTE)");
                }
                else
                {
                    if ((!"SECOND".Equals(str) && !"SS".Equals(str)) && !"S".Equals(str))
                    {
                        throw new FormaterException("not support datepart:" + str);
                    }
                    base.buffer.Append(" SECOND)");
                }
                return true;
            }
            if (methodNameI.CompareTo("DATEDIFF") == 0)
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("DATEDIFF(SS, ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (expr.parameters.Count != 3)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("TIMESTAMPDIFF(");
                SqlExpr expr3 = (SqlExpr)expr.parameters[0];
                if (expr3 is SqlIdentifierExpr)
                {
                    string str2 = ((SqlIdentifierExpr)expr3).value;
                    if ((str2 == null) || (str2.Length == 0))
                    {
                        throw new FormaterException("illegal datepart.");
                    }
                    str2 = str2.ToUpper();
                    if (("YEAR".Equals(str2) || "YY".Equals(str2)) || "YYYY".Equals(str2))
                    {
                        base.buffer.Append("YEAR");
                    }
                    else if (("MONTH".Equals(str2) || "MM".Equals(str2)) || "M".Equals(str2))
                    {
                        base.buffer.Append("MONTH");
                    }
                    else if (("DAY".Equals(str2) || "DD".Equals(str2)) || "D".Equals(str2))
                    {
                        base.buffer.Append("DAY");
                    }
                    else if ("HOUR".Equals(str2) || "HH".Equals(str2))
                    {
                        base.buffer.Append("HOUR");
                    }
                    else if (("MINUTE".Equals(str2) || "MI".Equals(str2)) || "N".Equals(str2))
                    {
                        base.buffer.Append("MINUTE");
                    }
                    else
                    {
                        if ((!"SECOND".Equals(str2) && !"SS".Equals(str2)) && !"S".Equals(str2))
                        {
                            throw new FormaterException("not support datepart:" + str2);
                        }
                        base.buffer.Append("SECOND");
                    }
                }
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[2]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("MONTHS_BETWEEN") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEDIFF(MONTH, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("DAYS_BETWEEN") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("DATEDIFF(DAY, ");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("DAYNAME") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DAYNAME(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("DATENAME") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                SqlExpr expr4 = (SqlExpr)expr.parameters[0];
                if (expr4 is SqlIdentifierExpr)
                {
                    string str3 = ((SqlIdentifierExpr)expr4).value;
                    if ((str3 == null) || (str3.Length == 0))
                    {
                        throw new FormaterException("illegal datepart.");
                    }
                    str3 = str3.ToUpper();
                    if (("YEAR".Equals(str3) || "YY".Equals(str3)) || "YYYY".Equals(str3))
                    {
                        base.buffer.Append("YEAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                        return true;
                    }
                    if (("MONTH".Equals(str3) || "MM".Equals(str3)) || "M".Equals(str3))
                    {
                        base.buffer.Append("MONTH(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                        return true;
                    }
                    if (("QUARTER".Equals(str3) || "QQ".Equals(str3)) || "Q".Equals(str3))
                    {
                        base.buffer.Append("QUARTER(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                        return true;
                    }
                    if (("DAYOFYEAR".Equals(str3) || "DY".Equals(str3)) || "Y".Equals(str3))
                    {
                        base.buffer.Append("DAYOFYEAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                        return true;
                    }
                    if (("DAY".Equals(str3) || "DD".Equals(str3)) || "D".Equals(str3))
                    {
                        base.buffer.Append("DAY(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                        return true;
                    }
                    if (("WEEK".Equals(str3) || "WK".Equals(str3)) || "WW".Equals(str3))
                    {
                        base.buffer.Append("WEEK(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(") + 1");
                        return true;
                    }
                    if ("WEEKDAY".Equals(str3) || "DW".Equals(str3))
                    {
                        base.buffer.Append("(WEEKDAY(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(") + 1) % 7 + 1)");
                        return true;
                    }
                    if ("HOUR".Equals(str3) || "HH".Equals(str3))
                    {
                        base.buffer.Append("HOUR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                        return true;
                    }
                    if (("MINUTE".Equals(str3) || "MI".Equals(str3)) || "N".Equals(str3))
                    {
                        base.buffer.Append("MINUTE(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                        return true;
                    }
                    if (("SECOND".Equals(str3) || "SS".Equals(str3)) || "S".Equals(str3))
                    {
                        base.buffer.Append("SECOND(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                        return true;
                    }
                    if (!"MILLISECOND".Equals(str3) && !"MS".Equals(str3))
                    {
                        throw new FormaterException("not support datepart:" + str3);
                    }
                    base.buffer.Append("MICROSECOND(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                    return true;
                }
            }
            else
            {
                if (methodNameI.CompareTo("DAYOFMONTH") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("DAYOFMONTH(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if ((methodNameI.CompareTo("DAYOFWEEK") == 0) || (methodNameI.CompareTo("WEEKDAY") == 0))
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("((WEEKDAY(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(") + 1) % 7 + 1)");
                    return true;
                }
                if (methodNameI.CompareTo("DAYOFYEAR") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("TO_DAYS(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (methodNameI.CompareTo("GETDATE") == 0)
                {
                    if (expr.parameters.Count != 0)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("NOW()");
                    return true;
                }
                if (methodNameI.CompareTo("HOUR") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("HOUR(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (methodNameI.CompareTo("MINUTE") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("MINUTE(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (methodNameI.CompareTo("MONTH") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("MONTH(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (methodNameI.CompareTo("MONTHNAME") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("MONTHNAME(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (methodNameI.CompareTo("NOW") == 0)
                {
                    if (expr.parameters.Count != 0)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("NOW()");
                    return true;
                }
                if (methodNameI.CompareTo("QUARTER") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("QUARTER(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (methodNameI.CompareTo("SECOND") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("SECOND(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (methodNameI.CompareTo("WEEK") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("WEEK(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(") + 1");
                    return true;
                }
                if (methodNameI.CompareTo("YEAR") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("YEAR(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (methodNameI.CompareTo("TO_DATE") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (methodNameI.CompareTo("MONTHS_BETWEEN") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("((YEAR(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(") - YEAR(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")) * 12 + ");
                    base.buffer.Append("MONTH(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(") - MONTH(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append("))");
                    return true;
                }
                if (methodNameI.CompareTo("ADD_MONTHS") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("DATE_ADD(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", INTERVAL ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" MONTH)");
                    return true;
                }
                if (methodNameI.CompareTo("ADD_YEARS") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("DATE_ADD(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", INTERVAL ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" YEAR)");
                    return true;
                }
                if (methodNameI.CompareTo("ADD_DAYS") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("DATE_ADD(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", INTERVAL ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" DAY)");
                    return true;
                }
                if (methodNameI.CompareTo("ADD_HOURS") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("DATE_ADD(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", INTERVAL ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" HOUR)");
                    return true;
                }
                if (methodNameI.CompareTo("ADD_MINUTES") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("DATE_ADD(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", INTERVAL ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" MINUTE)");
                    return true;
                }
                if (methodNameI.CompareTo("ADD_SECONDS") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("DATE_ADD(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", INTERVAL ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" SECOND)");
                    return true;
                }
            }
            return false;
        }

        protected override void FormatDeallocateStmt(SqlDeallocateStmt stmt)
        {
            base.buffer.Append("DEALLOCATE ");
            base.buffer.Append(stmt.curName);
        }

        protected void FormatDeleteStmt(SqlDeleteStmt stmt)
        {
            SqlDelete delete = stmt.delete;
            base.buffer.Append("DELETE ");
            this.FormatHintForDelete(stmt);
            if ((delete.tableName != null) && (delete.tableName.Length != 0))
            {
                if (delete.tableSource != null)
                {
                    throw new FormaterException("not support");
                }
                base.buffer.Append("FROM ");
                base.buffer.Append(this.FormatTableName(delete.tableName));
            }
            else
            {
                base.buffer.Append("FROM ");
                this.FormatTableSource(delete.tableSource);
            }
            if (delete.condition != null)
            {
                if (delete.condition is SqlInSubQueryExpr)
                {
                    SqlInSubQueryExpr condition = (SqlInSubQueryExpr)delete.condition;
                    if (!condition.not)
                    {
                        if ((delete.tableName != null) && (delete.tableName.Length != 0))
                        {
                            if (delete.tableSource != null)
                            {
                                throw new FormaterException("not support");
                            }
                            base.buffer.Append(" USING ");
                            base.buffer.Append(this.FormatTableName(delete.tableName));
                        }
                        else
                        {
                            base.buffer.Append(" USING ");
                            this.FormatTableSource(delete.tableSource);
                        }
                        base.buffer.Append(", (");
                        this.FormatSelectBase(condition.subQuery);
                        base.buffer.Append(") subQueryTempTable where ");
                        if (condition.expr is SqlIdentifierExpr)
                        {
                            if ((delete.tableName != null) && (delete.tableName.Length != 0))
                            {
                                if (delete.tableSource != null)
                                {
                                    throw new FormaterException("not support");
                                }
                                base.buffer.Append(this.FormatTableName(delete.tableName));
                            }
                            else if ((delete.tableSource.alias != null) && (delete.tableSource.alias != ""))
                            {
                                base.buffer.Append(delete.tableSource.alias);
                            }
                            else
                            {
                                if (!(delete.tableSource is SqlTableSource))
                                {
                                    throw new FormaterException("not support");
                                }
                                SqlTableSource tableSource = (SqlTableSource)delete.tableSource;
                                string name = tableSource.name;
                                base.buffer.Append(name);
                            }
                            base.buffer.Append(".");
                        }
                        base.FormatExpr(condition.expr);
                        base.buffer.Append(" = subQueryTempTable.");
                        SqlSelectBase subQuery = condition.subQuery;
                        while (subQuery is SqlUnionSelect)
                        {
                            subQuery = ((SqlUnionSelect)subQuery).left;
                        }
                        if (!(subQuery is SqlSelect))
                        {
                            throw new FormaterException("not support");
                        }
                        SqlSelect select = (SqlSelect)subQuery;
                        ArrayList selectList = select.selectList;
                        if ((selectList == null) || (selectList.Count != 1))
                        {
                            throw new FormaterException("not support");
                        }
                        SqlSelectItem item = (SqlSelectItem)selectList[0];
                        if ((item.alias != null) && (item.alias.Length != 0))
                        {
                            base.buffer.Append(item.alias.ToUpper());
                        }
                        else
                        {
                            SqlExpr expr = item.expr;
                            while (expr is SqlBinaryOpExpr)
                            {
                                expr = ((SqlBinaryOpExpr)expr).right;
                            }
                            if (!(expr is SqlIdentifierExpr))
                            {
                                throw new FormaterException("not support");
                            }
                            base.FormatExpr(expr);
                        }
                    }
                }
                else
                {
                    base.buffer.Append(" WHERE ");
                    base.FormatExpr(delete.condition);
                }
            }
        }

        protected void FormatDropIndexStmt(SqlDropIndexStmt stmt)
        {
            base.buffer.Append("DROP INDEX ");
            base.buffer.Append(base.GetIndexName(stmt));
            if ((stmt.tableName != null) && (stmt.tableName != ""))
            {
                base.buffer.Append(" ON ");
                base.buffer.Append(stmt.tableName);
            }
        }

        protected override void FormatExecStmt(SqlExecStmt stmt)
        {
            base.buffer.Append("CALL ");
            base.buffer.Append(stmt.processName);
            if (stmt.paramList.Count != 0)
            {
                base.buffer.Append(" (");
                bool flag = false;
                foreach (object obj2 in stmt.paramList)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    SqlExpr expr = (SqlExpr)obj2;
                    base.FormatExpr(expr);
                    flag = true;
                }
                base.buffer.Append(')');
            }
        }

        protected override void FormatFetchStmt(SqlFetchStmt stmt)
        {
            base.buffer.Append("FETCH ");
            string curName = stmt.curName;
            if (((curName != null) && (curName.Length > 0)) && (curName.CharAt(0) == '@'))
            {
                curName = curName.Substring(1);
            }
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
                str = "COLUMN_DEFAULT";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
            {
                str = "IS_NULLABLE";
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
            if (((str != null) && (str.Length != 0)) && (str.CharAt(0) == '"'))
            {
                base.buffer.Append(str.ToUpper());
            }
            else
            {
                base.buffer.Append(str);
            }
        }

        protected override void FormatIdentity(SqlExpr stmt)
        {
            base.buffer.Append(" AUTO_INCREMENT ");
        }

        protected override void FormatIfStmt(SqlIfStmt stmt)
        {
            base.buffer.Append("CREATE PROCEDURE ");
            long num = (long)Math.Round((double)(new Random().NextDouble() * 9999.0));
            string str = num.ToString();
            if (num < 10L)
            {
                str = "0" + str;
            }
            if (num < 100L)
            {
                str = "0" + str;
            }
            if (num < 0x3e8L)
            {
                str = "0" + str;
            }
            string str2 = "KSQL_TEMP_PROCEDURE_" + str + GetRandomString();
            base.buffer.Append(str2);
            base.buffer.Append("() ");
            base.buffer.Append("BEGIN ");
            base.buffer.Append("IF ");
            base.FormatExpr(stmt.condition);
            base.buffer.Append(" THEN ");
            base.buffer.Append("BEGIN ");
            int length = base.buffer.Length;
            for (int i = 0; i < stmt.trueStmtList.Count; i++)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.trueStmtList[i];
                StringBuilder buffer = base.buffer;
                base.buffer = new StringBuilder();
                base.FormatStmt(stmt2);
                string[] strArray = base.buffer.ToString().Split(new char[] { '\r', '\n' });
                base.buffer = buffer;
                for (int j = 0; j < (strArray.Length - 1); j++)
                {
                    buffer = new StringBuilder(strArray[j]);
                    buffer = base.HandleComma(buffer, 0);
                    base.buffer.Append("PREPARE stmt from '");
                    base.buffer.Append(buffer.substring(0, buffer.Length - 1));
                    base.buffer.Append("'; ");
                    base.buffer.Append("EXECUTE stmt; ");
                }
                buffer = new StringBuilder(strArray[strArray.Length - 1]);
                buffer = base.HandleComma(buffer, 0);
                base.buffer.Append("PREPARE stmt from '");
                if (strArray.Length == 1)
                {
                    base.buffer.Append(buffer.ToString());
                }
                else
                {
                    base.buffer.Append(buffer.substring(0, buffer.Length - 1));
                }
                base.buffer.Append("'; ");
                base.buffer.Append("EXECUTE stmt; ");
            }
            base.buffer.Append("END; ");
            if ((stmt.falseStmtList != null) && (stmt.falseStmtList.Count > 0))
            {
                base.buffer.Append("ELSE ");
                base.buffer.Append("BEGIN ");
                length = base.buffer.Length;
                for (int k = 0; k < stmt.falseStmtList.Count; k++)
                {
                    SqlStmt stmt3 = (SqlStmt)stmt.falseStmtList[k];
                    StringBuilder buff = base.buffer;
                    base.buffer = new StringBuilder();
                    base.FormatStmt(stmt3);
                    string[] strArray2 = base.buffer.ToString().Split(new char[] { '\r', '\n' });
                    base.buffer = buff;
                    for (int m = 0; m < (strArray2.Length - 1); m++)
                    {
                        buff = new StringBuilder(strArray2[m]);
                        buff = base.HandleComma(buff, 0);
                        base.buffer.Append("PREPARE stmt from '");
                        base.buffer.Append(buff.substring(0, buff.Length - 1));
                        base.buffer.Append("'; ");
                        base.buffer.Append("EXECUTE stmt; ");
                    }
                    buff = new StringBuilder(strArray2[strArray2.Length - 1]);
                    buff = base.HandleComma(buff, 0);
                    base.buffer.Append("PREPARE stmt from '");
                    if (strArray2.Length == 1)
                    {
                        base.buffer.Append(buff.ToString());
                    }
                    else
                    {
                        base.buffer.Append(buff.substring(0, buff.Length - 1));
                    }
                    base.buffer.Append("'; ");
                    base.buffer.Append("EXECUTE stmt; ");
                }
                base.buffer.Append("END; ");
            }
            base.buffer.Append("END IF; ");
            base.buffer.Append("END;\r\n");
            base.buffer.Append("CALL ");
            base.buffer.Append(str2);
            base.buffer.Append("();\r\n");
            base.buffer.Append("DROP PROCEDURE IF EXISTS ");
            base.buffer.Append(str2);
            base.buffer.Append(";");
            length++;
        }

        protected void FormatInsertStmt(SqlInsertStmt stmt)
        {
            bool flag;
            base.buffer.Append("INSERT INTO ");
            SqlInsert insert = stmt.insert;
            base.buffer.Append(this.FormatTableName(insert.tableName));
            if (insert.columnList.Count > 0)
            {
                base.buffer.Append(" (");
                flag = false;
                foreach (object obj2 in insert.columnList)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    if (obj2 is SqlIdentifierExpr)
                    {
                        SqlIdentifierExpr expr = (SqlIdentifierExpr)obj2;
                        if (expr.value.EqualsIgnoreCase("KSQL_SEQ"))
                        {
                            continue;
                        }
                        base.buffer.Append(base.FormatColumnName(expr.value));
                    }
                    else
                    {
                        if (!(obj2 is string))
                        {
                            throw new FormaterException("unexpect expression: '" + obj2 + "'");
                        }
                        base.buffer.Append(base.FormatColumnName((string)obj2));
                    }
                    flag = true;
                }
                base.buffer.Append(")");
            }
            if (insert.valueList.Count != 0)
            {
                base.buffer.Append(" VALUES (");
                flag = false;
                foreach (object obj3 in insert.valueList)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    SqlExpr expr2 = (SqlExpr)obj3;
                    base.FormatExpr(expr2);
                    flag = true;
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

        private bool FormatMathFunc(string methodNameI, SqlMethodInvokeExpr expr)
        {
            if (methodNameI.CompareTo("ABS") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ABS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("ACOS") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ACOS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("ASIN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ASIN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("ATAN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ATAN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if ((methodNameI.CompareTo("ATN2") == 0) || (methodNameI.CompareTo("ATAN2") == 0))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ATAN2(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("CEILING") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("CEILING(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("COS") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("COS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("COT") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("COT(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("DEGREE") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("DEGREE(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("EXP") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("EXP(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("FLOOR") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("FLOOR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("GREATEAST") == 0)
            {
                if (expr.parameters.Count <= 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("GREATEAST(");
                for (int i = 0; i < expr.parameters.Count; i++)
                {
                    if (i != 0)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                }
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("LEAST") == 0)
            {
                if (expr.parameters.Count <= 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LEAST(");
                for (int j = 0; j < expr.parameters.Count; j++)
                {
                    if (j != 0)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                }
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("MOD") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("MOD(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("LOG") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOG(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("LOG10") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOG10(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("PI") == 0)
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("PI()");
                return true;
            }
            if ((methodNameI.CompareTo("POW") == 0) || (methodNameI.CompareTo("POWER") == 0))
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
                return true;
            }
            if (methodNameI.CompareTo("RADIANS") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("RADIANS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("RAND") == 0)
            {
                if (expr.parameters.Count == 0)
                {
                    base.buffer.Append("RAND()");
                    return true;
                }
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("RAND(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("ROUND") == 0)
            {
                if (expr.parameters.Count == 1)
                {
                    base.buffer.Append("ROUND(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                    return true;
                }
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ROUND(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("SIGN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SIGN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("SIN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SIN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("SQRT") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SQRT(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("TAN") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TAN(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("TRUNCATE") == 0)
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TRUNCATE(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("FN_GCD") == 0)
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("FN_GCD(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
                return true;
            }
            if (methodNameI.CompareTo("FN_LCM") != 0)
            {
                return false;
            }
            if (expr.parameters.Count != 2)
            {
                throw new FormaterException("ERROR");
            }
            base.buffer.Append("FN_LCM(");
            base.FormatExpr((SqlExpr)expr.parameters[0]);
            base.buffer.Append(", ");
            base.FormatExpr((SqlExpr)expr.parameters[1]);
            base.buffer.Append(")");
            return true;
        }

        protected override void FormatMergeStmt(SqlMergeStmt stmt)
        {
            SqlMerge merge = stmt.Merge;
            SqlMergeMatched matchedSql = merge.MatchedSql;
            SqlMergeNotMatched notMatchedSql = merge.NotMatchedSql;
            if (notMatchedSql == null)
            {
                base.buffer.Append("UPDATE ");
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
                base.buffer.Append(" ON DUPLICATE KEY UPDATE ");
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
        }

        protected override void FormatMethodInvokeExpr(SqlMethodInvokeExpr expr)
        {
            if (expr.owner != null)
            {
                base.FormatExpr(expr.owner);
                base.buffer.Append('.');
            }
            string methodNameI = expr.methodName.ToUpper();
            if (!this.FormatMathFunc(methodNameI, expr) && !this.FormatDateTimeFunc(methodNameI, expr))
            {
                if (methodNameI.CompareTo("ASCII") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("ASCII(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("CHAR") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("CHAR(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("CHARINDEX") == 0)
                {
                    if (expr.parameters.Count == 2)
                    {
                        base.buffer.Append("LOCATE(");
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
                        base.buffer.Append("LOCATE(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(", ");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", ");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(")");
                    }
                }
                else if (methodNameI.CompareTo("CONCAT") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("CONCAT(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("LEFT") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("LEFT(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else if ((methodNameI.CompareTo("LEN") == 0) || (methodNameI.CompareTo("LENGTH") == 0))
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("LENGTH(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("LENGTH") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("LEN(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("LOWER") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("LOWER(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("LCASE") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("LOWER(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("LTRIM") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("LTRIM(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("REPLACE") == 0)
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
                else if (methodNameI.CompareTo("RIGHT") == 0)
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
                else if (methodNameI.CompareTo("RTRIM") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("RTRIM(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("SOUNDEX") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("SOUNDEX(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("SUBSTRING") == 0)
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("SUBSTRING(");
                    this.FormatExpr((SqlExpr)expr.parameters[0], false);
                    base.buffer.Append(", ");
                    if (expr.parameters[1] is SqlIntExpr)
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
                                throw new FormaterException("SUBSTRING parameter2 cannot not smaller then 1S.");
                            }
                        }
                    }
                    this.FormatExpr((SqlExpr)expr.parameters[1], false);
                    base.buffer.Append(", ");
                    this.FormatExpr((SqlExpr)expr.parameters[2], false);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("TRIM") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("LTRIM(RTRIM(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append("))");
                }
                else if (methodNameI.CompareTo("UCASE") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("UPPER(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("UPPER") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("UPPER(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else if (((methodNameI.CompareTo("TO_DECIMAL") == 0) || (methodNameI.CompareTo("DECIMAL") == 0)) || (methodNameI.CompareTo("DEC") == 0))
                {
                    if (expr.parameters.Count == 1)
                    {
                        if (expr.parameters[0] is SqlNullExpr)
                        {
                            base.FormatNullExpr((SqlNullExpr)expr.parameters[0]);
                        }
                        else
                        {
                            base.buffer.Append("CAST(");
                            base.FormatExpr((SqlExpr)expr.parameters[0]);
                            base.buffer.Append(" AS DECIMAL)");
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
                        base.buffer.Append(" AS DECIMAL(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", ");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append("))");
                    }
                }
                else if ((methodNameI.CompareTo("TO_BLOB") == 0) || (methodNameI.CompareTo("BLOB") == 0))
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    if (expr.parameters[0] is SqlNullExpr)
                    {
                        base.FormatNullExpr((SqlNullExpr)expr.parameters[0]);
                    }
                }
                else if ((methodNameI.CompareTo("TOCHAR") == 0) || (methodNameI.CompareTo("TO_CHAR") == 0))
                {
                    if (expr.parameters.Count == 1)
                    {
                        base.buffer.Append("CONCAT('', ");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(")");
                    }
                    else if ((expr.parameters.Count == 2) && expr.parameters[1].ToString().EqualsIgnoreCase("YYYY-MM-DD"))
                    {
                        base.buffer.Append("CONCAT('', DATE(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append("))");
                    }
                    else if ((expr.parameters.Count == 2) && expr.parameters[1].ToString().EqualsIgnoreCase("YYYY-MM-DD HH24:MI:SS"))
                    {
                        base.buffer.Append("CONCAT('', TIMESTAMP(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append("))");
                    }
                    else
                    {
                        if ((expr.parameters.Count != 3) || !expr.parameters[1].ToString().EqualsIgnoreCase("NUMBER"))
                        {
                            throw new FormaterException("ERROR");
                        }
                        if (expr.parameters[2] is SqlCharExpr)
                        {
                            string str2 = expr.parameters[2].ToString();
                            if ((str2.Split(new char[] { 'D' }).Length <= 0) || (str2.Split(new char[] { 'D' }).Length >= 3))
                            {
                                throw new FormaterException("TO_CHAR()'s NUMBER style not valid.");
                            }
                            char[] chArray = str2.Split(new char[] { 'D' })[0].ToCharArray();
                            char[] chArray2 = str2.Split(new char[] { 'D' })[1].ToCharArray();
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
                            base.buffer.Append("CONVERT(");
                            base.buffer.Append("CAST(");
                            base.FormatExpr((SqlExpr)expr.parameters[0]);
                            base.buffer.Append(" AS DECIMAL(");
                            base.buffer.Append((int)(num2 + num4));
                            base.buffer.Append(", ");
                            base.buffer.Append(num4);
                            base.buffer.Append("), CHAR");
                            base.buffer.Append("))");
                        }
                        else if (expr.parameters[2] is SqlIntExpr)
                        {
                            base.buffer.Append("LTRIM(RTRIM(STR(");
                            base.FormatExpr((SqlExpr)expr.parameters[0]);
                            base.buffer.Append(", 38, ");
                            base.FormatExpr((SqlExpr)expr.parameters[2]);
                            base.buffer.Append(")))");
                        }
                    }
                }
                else if (methodNameI.CompareTo("ISNULL") == 0)
                {
                    if (expr.parameters.Count != 2)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("COALESCE(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else if (methodNameI.CompareTo("NULLIF") == 0)
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
                else if (methodNameI.CompareTo("TO_NUMBER") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("CONVERT(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(", DECIMAL)");
                }
                else if ((methodNameI.CompareTo("TO_INT") == 0) || (methodNameI.CompareTo("TO_INTEGER") == 0))
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("FLOOR(CONVERT(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(",DECIMAL))");
                }
                else if ((methodNameI.CompareTo("TO_NVARCHAR") == 0) || (methodNameI.CompareTo("TONVARCHAR") == 0))
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
                else if ((methodNameI.CompareTo("TO_VARCHAR") == 0) || (methodNameI.CompareTo("TOVARCHAR") == 0))
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
                else if (methodNameI.CompareTo("CONVERT") == 0)
                {
                    if (!(expr.parameters[0] is SqlIdentifierExpr))
                    {
                        throw new FormaterException("unexpect parameter:" + expr.parameters[0]);
                    }
                    SqlIdentifierExpr expr2 = (SqlIdentifierExpr)expr.parameters[0];
                    if (expr2.value.EqualsIgnoreCase("DATETIME"))
                    {
                        base.buffer.Append("CONVERT(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", DATETIME)");
                    }
                    else
                    {
                        if ((!expr2.value.EqualsIgnoreCase("VARCHAR") && !expr2.value.EqualsIgnoreCase("NVARCHAR")) && (!expr2.value.EqualsIgnoreCase("CHAR") && !expr2.value.EqualsIgnoreCase("NCHAR")))
                        {
                            throw new FormaterException("unexpect expression:" + expr2.value);
                        }
                        base.buffer.Append("CONVERT(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", CHAR)");
                    }
                }
                else if (methodNameI.CompareTo("NEWID") == 0)
                {
                    if (expr.parameters.Count != 0)
                    {
                        throw new FormaterException("ERROR");
                    }
                    base.buffer.Append("''");
                }
                else if (methodNameI.CompareTo("NEWBOSID") == 0)
                {
                    if (expr.parameters.Count != 1)
                    {
                        throw new FormaterException("Unrecognized parameters");
                    }
                    base.buffer.Append("NEWBOSID(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else
                {
                    base.FormeatUnkownMethodInvokeExpr(expr);
                }
            }
        }

        protected override void FormatOpenStmt(SqlOpenStmt stmt)
        {
            base.buffer.Append("OPEN ");
            base.buffer.Append(stmt.curName);
        }

        protected override void FormatPriorIdentifierExpr(SqlPriorIdentifierExpr expr)
        {
            throw new FormaterException("Not Support. PriorIdentifierExpr");
        }

        private void FormatRowNumberExpr(SqlAggregateExpr expr, SqlSelect select, int rowNumberColumnCount)
        {
            if (expr.HasOver())
            {
                foreach (SqlOrderByItem item in expr.overExpr.orderBy)
                {
                    select.orderBy.Add(item);
                }
            }
        }

        protected override void FormatSelect(SqlSelect select)
        {
            int rowNumberColumnCount = 0;
            StringBuilder builder = new StringBuilder();
            StringBuilder buffer = new StringBuilder();
            if (select.into != null)
            {
                base.buffer.Append("CREATE TABLE ");
                base.buffer.Append(this.FormatTableName(select.into.new_table));
                base.buffer.Append(" AS (");
                buffer = base.buffer;
                base.buffer = new StringBuilder();
                base.buffer.Append("SELECT ");
            }
            else
            {
                buffer = base.buffer;
                base.buffer = new StringBuilder();
                base.buffer.Append("SELECT ");
            }
            if (select.distinct == 1)
            {
                base.buffer.Append("DISTINCT ");
            }
            else if (select.distinct != 0)
            {
                if (select.distinct != 2)
                {
                    throw new FormaterException("distinct option not support.");
                }
                base.buffer.Append("DISTINCTROW ");
            }
            bool flag = false;
            foreach (SqlSelectItem item in select.selectList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                if (item.expr is SqlIdentifierExpr)
                {
                    if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_COL_NAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.TABNAME.value) && (item.alias == null))
                    {
                        item.alias = Token.TABNAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_COL_TABNAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.INDNAME.value) && (item.alias == null))
                    {
                        item.alias = Token.INDNAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_CONS_NAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_CONS_TABNAME.value;
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_CONS_TYPE.value;
                    }
                }
                else if (item.expr is SqlAggregateExpr)
                {
                    foreach (object obj2 in ((SqlAggregateExpr)item.expr).paramList)
                    {
                        SqlExpr expr = (SqlExpr)obj2;
                        if (expr is SqlIdentifierExpr)
                        {
                            if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "COLUMN_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "TABLE_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.TABNAME.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "TABLE_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.INDNAME.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "INDEX_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "CONSTRAINT_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "TABLE_NAME";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "CONSTRAINT_TYPE";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "COLUMN_DEFAULT";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "IS_NULLABLE";
                            }
                        }
                    }
                }
                bool appendBrace = false;
                if ((item.alias != null) && (item.alias.Length != 0))
                {
                    appendBrace = true;
                }
                if (item.expr is SqlIdentifierExpr)
                {
                    if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value))
                    {
                        SqlIdentifierExpr expr2 = new SqlIdentifierExpr
                        {
                            value = "COLUMN_NAME"
                        };
                        this.FormatExpr(expr2, appendBrace);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
                    {
                        SqlIdentifierExpr expr3 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr3, appendBrace);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.TABNAME.value))
                    {
                        SqlIdentifierExpr expr4 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr4, appendBrace);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.INDNAME.value))
                    {
                        SqlIdentifierExpr expr5 = new SqlIdentifierExpr
                        {
                            value = "INDEX_NAME"
                        };
                        this.FormatExpr(expr5, appendBrace);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
                    {
                        SqlIdentifierExpr expr6 = new SqlIdentifierExpr
                        {
                            value = "CONSTRAINT_NAME"
                        };
                        this.FormatExpr(expr6, appendBrace);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
                    {
                        SqlIdentifierExpr expr7 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr7, appendBrace);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
                    {
                        SqlIdentifierExpr expr8 = new SqlIdentifierExpr
                        {
                            value = "CONSTRAINT_TYPE"
                        };
                        this.FormatExpr(expr8, appendBrace);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                    {
                        SqlIdentifierExpr expr9 = new SqlIdentifierExpr
                        {
                            value = "COLUMN_DEFAULT"
                        };
                        this.FormatExpr(expr9, appendBrace);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                    {
                        SqlIdentifierExpr expr10 = new SqlIdentifierExpr
                        {
                            value = "IS_NULLABLE"
                        };
                        this.FormatExpr(expr10, appendBrace);
                    }
                    else
                    {
                        this.FormatExpr(item.expr, appendBrace);
                    }
                    if ((item.alias != null) && (item.alias.Length != 0))
                    {
                        base.buffer.Append(" AS ");
                        base.buffer.Append(item.alias.ToUpper());
                    }
                    flag = true;
                }
                else if (item.expr is SqlAggregateExpr)
                {
                    SqlAggregateExpr expr11 = (SqlAggregateExpr)item.expr;
                    if (expr11.methodName.Equals("ROW_NUMBER", StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.FormatRowNumberExpr(expr11, select, rowNumberColumnCount);
                        builder.Append(", ");
                        builder.AppendFormat("(@rowNum{0}:=@rowNum{0}+1)", rowNumberColumnCount);
                        if ((item.alias != null) && (item.alias.Length != 0))
                        {
                            builder.Append(" AS ");
                            builder.Append(item.alias.ToUpper());
                        }
                        rowNumberColumnCount++;
                        if (flag)
                        {
                            base.buffer.Remove(base.buffer.Length - 2, 2);
                        }
                    }
                    else
                    {
                        this.FormatExpr(item.expr, appendBrace);
                        if ((item.alias != null) && (item.alias.Length != 0))
                        {
                            base.buffer.Append(" AS ");
                            base.buffer.Append(item.alias.ToUpper());
                        }
                        flag = true;
                    }
                }
                else
                {
                    this.FormatExpr(item.expr, appendBrace);
                    if ((item.alias != null) && (item.alias.Length != 0))
                    {
                        base.buffer.Append(" AS ");
                        base.buffer.Append(item.alias.ToUpper());
                    }
                    flag = true;
                }
            }
            if (select.tableSource != null)
            {
                base.buffer.Append(" FROM ");
                this.FormatTableSource(select.tableSource);
            }
            else
            {
                base.buffer.Append(" FROM dual ");
            }
            if (select.condition != null)
            {
                base.buffer.Append(" WHERE ");
                bool flag3 = false;
                if (select.tableSource is SqlTableSource)
                {
                    SqlTableSource tableSource = (SqlTableSource)select.tableSource;
                    if ((tableSource.name.EqualsIgnoreCase(Token.USERTABLES.value) || tableSource.name.EqualsIgnoreCase(Token.USERCOLUMNS.value)) || (tableSource.name.EqualsIgnoreCase(Token.SYSINDEXES.value) || tableSource.name.EqualsIgnoreCase(Token.SYSCONSTRAINTS.value)))
                    {
                        base.buffer.Append("TABLE_SCHEMA = SCHEMA() AND (");
                        flag3 = true;
                    }
                }
                base.FormatExpr(select.condition);
                if (flag3)
                {
                    base.buffer.Append(")");
                }
            }
            if (select.hierarchicalQueryClause != null)
            {
                throw new FormaterException("NOT SUPPORT hierarchicalQueryClause");
            }
            if (select.groupBy.Count != 0)
            {
                base.buffer.Append(" GROUP BY ");
                flag = false;
                foreach (SqlExpr expr12 in select.groupBy)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(expr12);
                    flag = true;
                }
                if (select.hasWithRollUp)
                {
                    base.buffer.Append(" WITH ROLLUP");
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
                    SqlExpr expr13 = item2.expr;
                    if (item2.chineseOrderByMode != -1)
                    {
                        base.buffer.Append("CONVERT(");
                        if (expr13 is SqlIdentifierExpr)
                        {
                            string str = ((SqlIdentifierExpr)expr13).value;
                            if (((str != null) && (str.Length != 0)) && (str.CharAt(0) == '"'))
                            {
                                base.buffer.Append(str.ToUpper());
                            }
                            else
                            {
                                base.FormatExpr(expr13);
                            }
                        }
                        else
                        {
                            base.FormatExpr(expr13);
                        }
                        base.buffer.Append(" USING ");
                        if (item2.chineseOrderByMode == 2)
                        {
                            base.buffer.Append("GBK)");
                        }
                        else if (item2.chineseOrderByMode == 3)
                        {
                            base.buffer.Append("GBK)");
                        }
                        else if (item2.chineseOrderByMode == 4)
                        {
                            base.buffer.Append("GBK)");
                        }
                    }
                    else if (expr13 is SqlIdentifierExpr)
                    {
                        string str2 = ((SqlIdentifierExpr)expr13).value;
                        if (((str2 != null) && (str2.Length != 0)) && (str2.CharAt(0) == '"'))
                        {
                            base.buffer.Append(str2.ToUpper());
                        }
                        else
                        {
                            base.FormatExpr(expr13);
                        }
                    }
                    else
                    {
                        base.FormatExpr(expr13);
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
            if (select.limit != null)
            {
                base.buffer.Append(" LIMIT 0, ");
                base.buffer.Append(select.limit.value);
                if (select.limit.type == 1)
                {
                    throw new FormaterException("Not support");
                }
            }
            if (rowNumberColumnCount > 0)
            {
                buffer.Append("select row_number_buffer.* ");
                buffer.Append(builder);
                buffer.Append(" from ( ").Append(base.buffer).Append(") row_number_buffer ");
                for (int i = 0; i < rowNumberColumnCount; i++)
                {
                    buffer.Append(string.Format(", (Select (@rowNum{0} :=0) ) ROW_NUMBER{0} ", i));
                }
            }
            else
            {
                buffer.Append(base.buffer);
            }
            base.buffer = buffer;
            if (select.into != null)
            {
                base.buffer.Append(")");
            }
        }

        public override void FormatSelectBase(SqlSelectBase select)
        {
            if (select.GetType() == typeof(SqlSelect))
            {
                this.FormatSelect((SqlSelect)select);
            }
            else if (select.GetType() == typeof(SqlUnionSelect))
            {
                SqlUnionSelect select2 = (SqlUnionSelect)select;
                if ((select2.left.GetType() == typeof(SqlUnionSelect)) && (((SqlUnionSelect)select2.left).option != select2.option))
                {
                    base.buffer.Append("(");
                    this.FormatSelectBase(select2.left);
                    base.buffer.Append(")");
                }
                else
                {
                    base.buffer.Append("(");
                    this.FormatSelectBase(select2.left);
                    base.buffer.Append(")");
                    select.subQueries.Add(select2.left.subQueries);
                }
                if (select2.option.Equals(SqlUnionSelect.Union))
                {
                    base.buffer.Append(" UNION ");
                }
                else
                {
                    if (select2.option != SqlUnionSelect.UnionAll)
                    {
                        throw new FormaterException("Eorr Union Option.");
                    }
                    base.buffer.Append(" UNION ALL ");
                }
                if ((select2.right.GetType() == typeof(SqlUnionSelect)) && (((SqlUnionSelect)select2.right).option != select2.option))
                {
                    base.buffer.Append("(");
                    this.FormatSelectBase(select2.right);
                    base.buffer.Append(")");
                }
                else
                {
                    base.buffer.Append("(");
                    this.FormatSelectBase(select2.right);
                    base.buffer.Append(")");
                    select.subQueries.Add(select2.right.subQueries);
                }
                if (select2.orderBy.Count != 0)
                {
                    base.buffer.Append(" ORDER BY ");
                    bool flag = false;
                    foreach (SqlOrderByItem item in select2.orderBy)
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

        public override void FormatSelectList(SqlSelect select)
        {
            bool sysTableFlag = false;
            bool sysIndexFlag = false;
            bool sysConstraintFlag = false;
            this.FormatSelectColumnList(select, out sysTableFlag, out sysIndexFlag, out sysConstraintFlag);
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
            string str = "SHOW COLUMNS FROM " + stmt.tableName;
            base.buffer.Append(str);
        }

        protected override void FormatShowTablesStmt(SqlShowTablesStmt stmt)
        {
            string str = "SHOW TABLES";
            base.buffer.Append(str);
        }

        protected void FormatTableConstraint(SqlTableConstraint constraint)
        {
            base.ValidConstraintName(constraint.name);
            if (!string.IsNullOrEmpty(constraint.name))
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
                base.buffer.Append(" UNIQUE (");
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
            if (tableSource.GetType() == typeof(SqlTableSource))
            {
                SqlTableSource source = (SqlTableSource)tableSource;
                string str = "";
                if (source.name.EqualsIgnoreCase(Token.USERTABLES.value))
                {
                    base.buffer.Append("(select TABLE_NAME, case when TABLE_TYPE = 'BASE TABLE' then 'U' else 'V' end TABLE_XTYPE, TABLE_SCHEMA from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = SCHEMA())");
                    str = " AS KSQL_USERTABLES";
                }
                else if (source.name.EqualsIgnoreCase(Token.USERCOLUMNS.value))
                {
                    base.buffer.Append("INFORMATION_SCHEMA.COLUMNS");
                }
                else if (source.name.EqualsIgnoreCase(Token.TABLECOLUMNDEFAULTVALUE.value))
                {
                    base.buffer.Append("INFORMATION_SCHEMA.COLUMNS");
                }
                else if (source.name.EqualsIgnoreCase(Token.SYSINDEXES.value))
                {
                    base.buffer.Append("INFORMATION_SCHEMA.STATISTICS");
                }
                else if (source.name.EqualsIgnoreCase(Token.INDCOLUMNS.value))
                {
                    base.buffer.Append("INFORMATION_SCHEMA.STATISTICS");
                }
                else if (source.name.EqualsIgnoreCase(Token.SYSCONSTRAINTS.value))
                {
                    base.buffer.Append("INFORMATION_SCHEMA.TABLE_CONSTRAINTS");
                }
                else if (source.name.EqualsIgnoreCase(Token.USERVIEWS.value))
                {
                    base.buffer.Append("(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = SCHEMA() )");
                    str = " AS KSQL_USERVIEWS";
                }
                else
                {
                    string name = source.name;
                    base.buffer.Append(name);
                }
                if ((source.alias != null) && (source.alias.Length != 0))
                {
                    base.buffer.Append(" AS ");
                    string alias = source.alias;
                    base.buffer.Append(alias);
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
                        base.buffer.Append(tableSource.alias);
                    }
                }
                else
                {
                    if (!(tableSource.GetType() == typeof(SqlTableFunctionTableSource)))
                    {
                        throw new FormaterException("TODO");
                    }
                    SqlTableFunctionTableSource source4 = (SqlTableFunctionTableSource)tableSource;
                    string pipeFunction = source4.PipeFunction;
                    pipeFunction = pipeFunction.Substring(6, pipeFunction.Length - 7).Replace(" ", "");
                    if (pipeFunction.substring(0, 11).EqualsIgnoreCase("fn_StrSplit"))
                    {
                        pipeFunction = pipeFunction.Replace(",',',", "_udt");
                        pipeFunction = string.Format("?{0}", pipeFunction.Substring(pipeFunction.IndexOf("@") + 1, (pipeFunction.IndexOf(")") - pipeFunction.IndexOf("@")) - 1));
                    }
                    base.buffer.Append(pipeFunction);
                    if ((source4.alias != null) && (source4.alias.Length != 0))
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(source4.alias);
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
                        base.buffer.Append(" CROSS JOIN ");
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

        protected void FormatTableSpace(string tableSpace)
        {
            if ((tableSpace != null) && (tableSpace.Length > 0))
            {
                base.buffer.Append(" TABLESPACE " + tableSpace);
            }
        }

        protected override void FormatUpdateStmt(SqlUpdateStmt stmt)
        {
            SqlUpdate update = stmt.update;
            if (update.tableSource != null)
            {
                throw new FormaterException("update's tableSource is null");
            }
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            ArrayList list3 = new ArrayList();
            int length = base.buffer.Length;
            if (tempTabIndex > 0x3e8)
            {
                tempTabIndex = 0;
            }
            if (tempColIndex > 0x7d0)
            {
                tempColIndex = 0;
            }
            base.buffer.Append("UPDATE ");
            StringBuilder buffer = base.buffer;
            base.buffer = new StringBuilder();
            base.buffer.Append(" SET ");
            ArrayList list4 = new ArrayList();
            int num2 = 0;
            bool flag = false;
            foreach (object obj2 in update.updateList)
            {
                AbstractUpdateItem item = (AbstractUpdateItem)obj2;
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                if (item is SqlUpdateItem)
                {
                    SqlUpdateItem item2 = (SqlUpdateItem)item;
                    base.buffer.Append(base.FormatColumnName(item2.name));
                    base.buffer.Append(" = ");
                    if (item2.expr is QueryExpr)
                    {
                        QueryExpr expr = (QueryExpr)item2.expr;
                        if (expr.subQuery is SqlSelect)
                        {
                            if (this.HasTableSource(((SqlSelect)expr.subQuery).tableSource, update.updateTable.name))
                            {
                                string str = "KSQL_TEMP_TABLE_" + tempTabIndex;
                                list3.Add(str);
                                tempTabIndex++;
                                base.buffer.Append("(SELECT * FROM " + str + ")");
                                string str2 = "CREATE TABLE " + str + " AS (";
                                StringBuilder builder2 = base.buffer;
                                base.buffer = new StringBuilder();
                                this.FormatSelect((SqlSelect)expr.subQuery);
                                str2 = str2 + base.buffer.ToString() + ");";
                                list.Add(str2);
                                base.buffer = builder2;
                            }
                            else
                            {
                                base.FormatExpr(item2.expr);
                            }
                        }
                        else
                        {
                            base.FormatExpr(item2.expr);
                        }
                    }
                    else
                    {
                        base.FormatExpr(item2.expr);
                    }
                }
                else
                {
                    if (!(item is SubQueryUpdateItem))
                    {
                        throw new FormaterException("unexpect update item: '" + item + "'");
                    }
                    SubQueryUpdateItem item3 = (SubQueryUpdateItem)item;
                    bool flag3 = false;
                    if (item3.subQuery is SqlSelect)
                    {
                        SqlSelect subQuery = (SqlSelect)item3.subQuery;
                        flag3 = this.HasTableSource(subQuery.tableSource, update.updateTable.name);
                    }
                    string str3 = null;
                    if (flag3)
                    {
                        str3 = "KSQL_TEMP_TABLE_" + tempTabIndex;
                        list2.Add(str3);
                        tempTabIndex++;
                    }
                    else
                    {
                        list4.Add(item);
                    }
                    for (int j = 0; j < item3.columnList.Count; j++)
                    {
                        string name = (string)item3.columnList[j];
                        if (item3.extendedAttributes()["tableSourceAlias"] != null)
                        {
                            name = item3.extendedAttributes()["tableSourceAlias"] + "." + name;
                        }
                        else
                        {
                            name = this.FormatTableName(update.updateTable.name) + "." + name;
                        }
                        if (j != 0)
                        {
                            base.buffer.Append(", ");
                        }
                        base.buffer.Append(base.FormatColumnName(name));
                        base.buffer.Append(" = ");
                        if (!(item3.subQuery is SqlSelect))
                        {
                            throw new FormaterException("unexpect subquery item: '" + item3.subQuery + "'");
                        }
                        SqlSelect select2 = (SqlSelect)item3.subQuery;
                        SqlSelectItem item4 = (SqlSelectItem)select2.selectList[j];
                        if (!flag3)
                        {
                            if (((select2 != null) && (select2.tableSource != null)) && ((select2.tableSource.alias != null) && !select2.tableSource.alias.Trim().Equals("")))
                            {
                                item4.expr.addExtAttr("tableSourceAlias", select2.tableSource.alias);
                            }
                            this.FormatExpr(item4.expr, false);
                        }
                        else
                        {
                            item4.alias = "KSQL_TEMP_COL" + tempColIndex;
                            tempColIndex++;
                            base.buffer.Append(str3 + "." + item4.alias);
                        }
                        num2++;
                        flag = true;
                    }
                    if (flag3)
                    {
                        string str5 = "CREATE TABLE " + str3 + " AS (";
                        StringBuilder builder3 = base.buffer;
                        base.buffer = new StringBuilder();
                        this.FormatSelect((SqlSelect)item3.subQuery);
                        str5 = str5 + base.buffer.ToString() + ");";
                        list.Add(str5);
                        base.buffer = builder3;
                    }
                }
                flag = true;
            }
            StringBuilder builder4 = base.buffer;
            base.buffer = new StringBuilder();
            if (list4.Count == 0)
            {
                base.buffer.Append(this.FormatTableName(update.updateTable.name));
                if (update.updateTable.alias != null)
                {
                    base.buffer.Append(" " + update.updateTable.alias.ToUpper() + " ");
                }
            }
            else
            {
                bool flag4 = false;
                for (int k = 0; k < list4.Count; k++)
                {
                    if (k == 0)
                    {
                        base.buffer.Append(this.FormatTableName(update.updateTable.name));
                        if (update.updateTable.alias != null)
                        {
                            base.buffer.Append(" AS " + update.updateTable.alias.ToUpper());
                        }
                        flag4 = true;
                    }
                    SubQueryUpdateItem item5 = (SubQueryUpdateItem)list4[k];
                    if (!(item5.subQuery is SqlSelect))
                    {
                        throw new FormaterException("unexpect queryItem subQuery: '" + item5 + "'");
                    }
                    StringBuilder builder5 = base.buffer;
                    base.buffer = new StringBuilder();
                    SqlSelect select3 = (SqlSelect)item5.subQuery;
                    this.FormatTableSource(select3.tableSource);
                    if (base.buffer.Length > 0)
                    {
                        if (flag4)
                        {
                            builder5.Append(", ");
                        }
                        builder5.Append(base.buffer.ToString());
                        flag4 = true;
                    }
                    base.buffer = builder5;
                }
            }
            if (list2.Count != 0)
            {
                int num5 = 0;
                int count = list2.Count;
                while (num5 < count)
                {
                    base.buffer.Append(", ");
                    base.buffer.Append(list2[num5]);
                    num5++;
                }
            }
            buffer.Append(base.buffer);
            buffer.Append(builder4);
            base.buffer = buffer;
            flag = false;
            for (int i = 0; i < list4.Count; i++)
            {
                SubQueryUpdateItem item6 = (SubQueryUpdateItem)list4[i];
                if (!(item6.subQuery is SqlSelect))
                {
                    throw new FormaterException("not support query item:" + item6);
                }
                SqlSelect select4 = (SqlSelect)item6.subQuery;
                if (select4.condition != null)
                {
                    if (flag)
                    {
                        base.buffer.Append(" AND ");
                    }
                    else
                    {
                        base.buffer.Append(" WHERE ");
                    }
                    base.FormatExpr(select4.condition);
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
            if (list.Count != 0)
            {
                StringBuilder builder6 = new StringBuilder();
                int num8 = 0;
                int num9 = list.Count;
                while (num8 < num9)
                {
                    builder6.Append(list[num8]);
                    builder6.Append("\r\n");
                    num8++;
                }
                base.buffer.Insert(length, builder6.ToString());
                base.buffer.Append(";\r\n");
                int num10 = 0;
                int num11 = list2.Count;
                while (num10 < num11)
                {
                    base.buffer.Append("DROP TABLE ");
                    base.buffer.Append(list2[num10]);
                    base.buffer.Append(";\r\n");
                    num10++;
                }
                int num12 = 0;
                int num13 = list3.Count;
                while (num12 < num13)
                {
                    base.buffer.Append("DROP TABLE IF EXISTS ");
                    base.buffer.Append(list3[num12]);
                    base.buffer.Append(";\r\n");
                    num12++;
                }
            }
        }

        protected override void FormatVarRef(SqlVarRefExpr expr)
        {
            string text = expr.text;
            if (!string.IsNullOrEmpty(text) && text.StartsWith("@"))
            {
                if (TransUtil.OleDBDriver)
                {
                    base.buffer.Append("?");
                }
                else
                {
                    text = "?" + text.Substring(1);
                    base.buffer.Append(text);
                }
            }
            else if (TransUtil.OleDBDriver)
            {
                base.buffer.Append("?");
            }
            else
            {
                base.buffer.Append(text);
            }
        }

        protected override void FormatWhileStmt(SqlWhileStmt stmt)
        {
            base.buffer.Append("WHILE ");
            base.FormatExpr(stmt.condition);
            base.buffer.Append(" DO\n");
            base.buffer.Append("BEGIN\n");
            for (int i = 0; i < stmt.stmtList.Count; i++)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[i];
                base.FormatStmt(stmt2);
                base.buffer.Append(";\n");
            }
            base.buffer.Append("END;\n");
            base.buffer.Append("END WHILE;\n");
        }

        private static string GetRandomString()
        {
            long ticks = DateTime.Now.Ticks;
            Random random = new Random(((int)(((ulong)ticks) & 0xffffffffL)) | ((int)(ticks >> 0x20)));
            return random.Next().ToString();
        }

        private bool HasTableSource(SqlTableSourceBase tableSource, string tabName)
        {
            if (tableSource is SqlTableSource)
            {
                SqlTableSource source = (SqlTableSource)tableSource;
                return source.name.EqualsIgnoreCase(tabName);
            }
            if (tableSource is SqlJoinedTableSource)
            {
                SqlJoinedTableSource source2 = (SqlJoinedTableSource)tableSource;
                bool flag = this.HasTableSource(source2.left, tabName);
                if (!flag)
                {
                    flag = this.HasTableSource(source2.right, tabName);
                }
                return flag;
            }
            if (tableSource is SqlSubQueryTableSource)
            {
                SqlSubQueryTableSource source3 = (SqlSubQueryTableSource)tableSource;
                if (source3.subQuery is SqlSelect)
                {
                    return this.HasTableSource(((SqlSelect)source3.subQuery).tableSource, tabName);
                }
            }
            return false;
        }
    }






}
