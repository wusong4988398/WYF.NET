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
    public class OracleSQLFormater : SQLFormater
    {
        // Methods
        public OracleSQLFormater() : base(null)
        {
            base.max_length_of_index_name = 30;
            base.max_length_of_table_name = 30;
            base.max_length_of_constraint_name = 30;
            base.max_length_of_column_name = 30;
            base.max_length_of_column_count = 0x3e8;
            base.max_length_of_row_size = -1;
        }

        public OracleSQLFormater(StringBuilder sb) : base(sb)
        {
            base.max_length_of_index_name = 30;
            base.max_length_of_table_name = 30;
            base.max_length_of_constraint_name = 30;
            base.max_length_of_column_name = 30;
            base.max_length_of_column_count = 0x3e8;
            base.max_length_of_row_size = -1;
        }

        private bool AppendJoinedCondition(ArrayList tableSourceList, SqlJoinedTableSource tableSource, bool whereFlag)
        {
            if (tableSource.left.GetType() == typeof(SqlJoinedTableSource))
            {
                whereFlag = this.AppendJoinedCondition(tableSourceList, (SqlJoinedTableSource)tableSource.left, whereFlag);
            }
            if (tableSource.right.GetType() == typeof(SqlJoinedTableSource))
            {
                whereFlag = this.AppendJoinedCondition(tableSourceList, (SqlJoinedTableSource)tableSource.right, whereFlag);
            }
            if (!whereFlag && (tableSource.condition != null))
            {
                base.buffer.Append(" WHERE ");
            }
            else if (tableSource.condition != null)
            {
                base.buffer.Append(" AND ");
            }
            if ((tableSource.joinType == 0) || (tableSource.joinType == 4))
            {
                if (tableSource.condition != null)
                {
                    base.FormatExpr(tableSource.condition);
                }
            }
            else if (tableSource.joinType == 2)
            {
                this.AppendOuterJoinCondition(tableSourceList, tableSource.condition, false);
            }
            else if (tableSource.joinType == 1)
            {
                this.AppendOuterJoinCondition(tableSourceList, tableSource.condition, true);
            }
            else
            {
                if (tableSource.joinType != 3)
                {
                    throw new FormaterException("not support tableSource:" + tableSource);
                }
                this.AppendOuterJoinCondition(tableSourceList, tableSource.condition, true);
            }
            whereFlag = true;
            return whereFlag;
        }

        private void AppendOuterJoinCondition(ArrayList tableSourceList, SqlExpr conditionExpr, bool leftJoin)
        {
            if (conditionExpr.GetType() == typeof(SqlBinaryOpExpr))
            {
                SqlBinaryOpExpr expr = (SqlBinaryOpExpr)conditionExpr;
                if (expr.Operator == 10)
                {
                    SqlExpr expr2 = getRootExpr(expr.left);
                    SqlExpr expr3 = getRootExpr(expr.right);
                    if (expr2.GetType() != typeof(SqlIdentifierExpr))
                    {
                        throw new FormaterException("not support join condition.");
                    }
                    string str = ((SqlIdentifierExpr)expr2).value.ToUpper();
                    if (expr3.GetType() != typeof(SqlIdentifierExpr))
                    {
                        throw new FormaterException("not support join condition.");
                    }
                    string str2 = ((SqlIdentifierExpr)expr3).value.ToUpper();
                    int index = tableSourceList.IndexOf(str);
                    int num2 = tableSourceList.IndexOf(str2);
                    if (index > num2)
                    {
                        expr = new SqlBinaryOpExpr(expr.right, expr.Operator, expr.left);
                    }
                    if (leftJoin)
                    {
                        base.FormatExpr(expr);
                        base.buffer.Append(" (+)");
                    }
                    else
                    {
                        base.FormatExpr(expr.left);
                        base.buffer.Append(" (+) = ");
                        base.FormatExpr(expr.right);
                    }
                }
                else
                {
                    if (expr.Operator != 7)
                    {
                        throw new FormaterException("not support join condition.");
                    }
                    this.AppendOuterJoinCondition(tableSourceList, expr.left, leftJoin);
                    base.buffer.Append(" AND ");
                    this.AppendOuterJoinCondition(tableSourceList, expr.right, leftJoin);
                }
            }
            else
            {
                base.FormatExpr(conditionExpr);
            }
        }

        private void ComputeTableSourceList(SqlTableSourceBase tableSource, ArrayList tableSourceList)
        {
            if (tableSource != null)
            {
                if (tableSource.GetType() == typeof(SqlTableSource))
                {
                    SqlTableSource source = (SqlTableSource)tableSource;
                    if ((source.alias != null) && (source.alias.Length != 0))
                    {
                        tableSourceList.Add(source.alias.ToUpper());
                    }
                    else
                    {
                        tableSourceList.Add(source.name.ToUpper());
                    }
                }
                else if (tableSource.GetType() == typeof(SqlJoinedTableSource))
                {
                    SqlJoinedTableSource source2 = (SqlJoinedTableSource)tableSource;
                    this.ComputeTableSourceList(source2.left, tableSourceList);
                    this.ComputeTableSourceList(source2.right, tableSourceList);
                }
                else if (tableSource.GetType() == typeof(SqlSubQueryTableSource))
                {
                    SqlSubQueryTableSource source3 = (SqlSubQueryTableSource)tableSource;
                    tableSourceList.Add(source3.alias);
                }
            }
        }

        public override string Format(ICollection stmtList)
        {
            base.context.Clear();
            bool flag = false;
            if (stmtList.Count > 1)
            {
                flag = true;
                base.buffer.Append("BEGIN\n");
            }
            bool flag2 = false;
            foreach (SqlStmt stmt in stmtList)
            {
                if (flag2)
                {
                    if (!base.buffer.endsWith(';'))
                    {
                        base.buffer.Append(";");
                    }
                    base.buffer.Append("\n");
                }
                if (flag && (stmt.GetType() == typeof(SqlSelectStmt)))
                {
                    base.buffer.Append("OPEN ? FOR ");
                }
                base.FormatStmt(stmt);
                flag2 = true;
            }
            if (stmtList.Count > 1)
            {
                if (!base.buffer.endsWith(';'))
                {
                    base.buffer.Append(";\nEND;");
                }
                else
                {
                    base.buffer.Append("\nEND;");
                }
            }
            return base.buffer.ToString();
        }

        protected void Format_TO_DATE_Invoke(SqlMethodInvokeExpr expr)
        {
            if (expr.parameters.Count != 1)
            {
                expr.parameters.RemoveAt(1);
            }
            string str = "'YYYY-MM-DD HH24:MI:SS'";
            if (expr.parameters[0].GetType() == typeof(SqlCharExpr))
            {
                SqlCharExpr expr2 = (SqlCharExpr)expr.parameters[0];
                if (expr2.text.IndexOf(".") > 0)
                {
                    base.buffer.Append("TO_TIMESTAMP");
                    base.buffer.Append("(");
                    str = "'YYYY-MM-DD HH24:MI:SS:FF9'";
                    base.FormatExpr(new SqlCharExpr(expr2.text));
                }
                else if (expr2.text.IndexOf(":") < 0)
                {
                    base.buffer.Append("TO_DATE");
                    base.buffer.Append("(");
                    if (expr2.text.EndsWith(" "))
                    {
                        base.FormatExpr(new SqlCharExpr(expr2.text + "00:00:00"));
                    }
                    else
                    {
                        base.FormatExpr(new SqlCharExpr(expr2.text + " 00:00:00"));
                    }
                }
                else
                {
                    base.buffer.Append("TO_DATE");
                    base.buffer.Append("(");
                    base.FormatExpr(expr2);
                }
            }
            else
            {
                base.buffer.Append("TO_DATE");
                base.buffer.Append("(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
            }
            base.buffer.Append(", " + str + ")");
        }

        protected void Format_TOCHAR_Invoke(SqlMethodInvokeExpr expr)
        {
            if (expr.parameters.Count == 1)
            {
                if (expr.parameters[0].GetType() == typeof(SqlNullExpr))
                {
                    base.FormatNullExpr((SqlNullExpr)expr.parameters[0]);
                }
                else
                {
                    base.buffer.Append("TO_CHAR(");
                    if (expr.parameters[0].GetType() == typeof(SqlDoubleExpr))
                    {
                        string text = ((SqlDoubleExpr)expr.parameters[0]).text;
                        int length = text.Length;
                        int index = text.IndexOf('.');
                        base.buffer.Append(text);
                        base.buffer.Append(", '");
                        for (int i = 1; i < index; i++)
                        {
                            base.buffer.Append("9");
                        }
                        base.buffer.Append("0D");
                        for (int j = 0; j < ((length - index) - 1); j++)
                        {
                            base.buffer.Append("9");
                        }
                        base.buffer.Append("'");
                    }
                    else
                    {
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                    }
                    base.buffer.Append(")");
                }
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
                    throw new FormaterException("unexpect expression: '" + expr + "'");
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
                    int num5 = ((SqlIntExpr)expr.parameters[2]).value;
                    for (int k = 0; k < num5; k++)
                    {
                        base.buffer.Append("9");
                    }
                    base.buffer.Append("'))");
                }
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
                        base.buffer.Append("call xalter_table('" + stmt.tableName + "', '(");
                    }
                    else
                    {
                        builder.Append("), (");
                    }
                    if (obj2.GetType() == typeof(SqlAlterTableAddItem))
                    {
                        SqlAlterTableAddItem item = (SqlAlterTableAddItem)obj2;
                        builder.Append(" ADD (");
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
                        flag2 = false;
                        foreach (SqlTableConstraint constraint in item.constraintItemList)
                        {
                            StringBuilder builder3 = base.buffer;
                            base.buffer = new StringBuilder();
                            this.FormatTableConstraint(constraint);
                            builder.Append(base.buffer);
                            base.buffer = builder3;
                        }
                        builder.Append(")");
                    }
                    else if (obj2.GetType() == typeof(SqlAlterTableDropItem))
                    {
                        SqlAlterTableDropItem item2 = (SqlAlterTableDropItem)obj2;
                        builder.Append(" DROP ");
                        bool flag3 = false;
                        foreach (string str in item2.columnDefItemList)
                        {
                            if (flag3)
                            {
                                builder.Append(", ");
                            }
                            else
                            {
                                builder.Append("(");
                            }
                            builder.Append(str);
                            flag3 = true;
                        }
                        if (flag3)
                        {
                            builder.Append(")");
                        }
                        flag3 = false;
                        foreach (string str2 in item2.constraintItemList)
                        {
                            if (flag3)
                            {
                                builder.Append(", ");
                            }
                            else
                            {
                                builder.Append("CONSTRAINT ");
                            }
                            builder.Append(str2);
                            flag3 = true;
                        }
                    }
                    else if (obj2.GetType() == typeof(SqlAlterTableAlterColumnItem))
                    {
                        SqlAlterTableAlterColumnItem item3 = (SqlAlterTableAlterColumnItem)obj2;
                        builder.Append(" MODIFY ");
                        StringBuilder builder4 = base.buffer;
                        base.buffer = new StringBuilder();
                        this.FormatColumnDef(item3.columnDef);
                        builder.Append(base.buffer);
                        base.buffer = builder4;
                    }
                    else if (obj2.GetType() == typeof(SqlAlterTableAddDefaultItem))
                    {
                        SqlAlterTableAddDefaultItem item4 = (SqlAlterTableAddDefaultItem)obj2;
                        builder.Append(" MODIFY ");
                        builder.Append(item4.columnName);
                        builder.Append(" DEFAULT ");
                        StringBuilder builder5 = base.buffer;
                        base.buffer = new StringBuilder();
                        base.FormatExpr(item4.value);
                        builder.Append(base.buffer.ToString().Replace("'", "''"));
                        base.buffer = builder5;
                    }
                    else
                    {
                        if (obj2.GetType() != typeof(SqlAlterTableDropDefaultItem))
                        {
                            throw new FormaterException("not support statement:" + stmt);
                        }
                        SqlAlterTableDropDefaultItem item5 = (SqlAlterTableDropDefaultItem)obj2;
                        builder.Append(" MODIFY ");
                        builder.Append(item5.columnName);
                        builder.Append(" DEFAULT ");
                        builder.Append("null");
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
            base.buffer.Append(this.FormatTableName(stmt.tableName));
            if (stmt.item.GetType() == typeof(SqlAlterTableAddItem))
            {
                SqlAlterTableAddItem item = (SqlAlterTableAddItem)stmt.item;
                base.buffer.Append(" ADD (");
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
                    this.FormatTableConstraint(constraint);
                }
                base.buffer.Append(")");
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
                        base.buffer.Append("(");
                    }
                    base.buffer.Append(str);
                    flag2 = true;
                }
                if (flag2)
                {
                    base.buffer.Append(")");
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
                base.buffer.Append(" MODIFY ");
                this.FormatColumnDef(item3.columnDef);
            }
            else if (stmt.item.GetType() == typeof(SqlAlterTableAddDefaultItem))
            {
                SqlAlterTableAddDefaultItem item4 = (SqlAlterTableAddDefaultItem)stmt.item;
                base.buffer.Append(" MODIFY ");
                base.buffer.Append(item4.columnName);
                base.buffer.Append(" DEFAULT ");
                base.FormatExpr(item4.value);
            }
            else
            {
                if (stmt.item.GetType() != typeof(SqlAlterTableDropDefaultItem))
                {
                    throw new FormaterException("not support statement:" + stmt);
                }
                SqlAlterTableDropDefaultItem item5 = (SqlAlterTableDropDefaultItem)stmt.item;
                base.buffer.Append(" MODIFY ");
                base.buffer.Append(item5.columnName);
                base.buffer.Append(" DEFAULT ");
                base.buffer.Append("null");
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
                    base.FormatExpr(expr.left);
                    base.buffer.Append(" || ");
                    base.FormatExpr(expr.right);
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
                    if (((expr.left.GetType() == typeof(SqlIdentifierExpr)) && base.IsToUpperCaseExpr((SqlIdentifierExpr)expr.left)) && (expr.right.GetType() == typeof(SqlCharExpr)))
                    {
                        base.context.add("toUpperCase", "toUpperCase");
                        this.FormatChar((SqlCharExpr)expr.right);
                        base.context.add("toUpperCase", null);
                        return;
                    }
                    if (expr.right.type == 3)
                    {
                        string str = ((SqlVarRefExpr)expr.right).text.Replace('@', ':');
                        ((SqlVarRefExpr)expr.right).text = str;
                    }
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
                        string str2 = ((SqlIdentifierExpr)expr.right).value.ToUpper();
                        base.buffer.Append(str2);
                        return;
                    }
                    if (expr.right.GetType() == typeof(SqlCharExpr))
                    {
                        string str3 = ((SqlCharExpr)expr.right).text.ToUpper();
                        base.buffer.Append(str3);
                        return;
                    }
                    if (expr.right.GetType() == typeof(SqlNCharExpr))
                    {
                        string str4 = ((SqlNCharExpr)expr.right).text.ToUpper();
                        base.buffer.Append(str4);
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
                        goto Label_0BC5;

                    case 1:
                        base.buffer.Append(" AS ");
                        goto Label_0BC5;

                    case 2:
                        base.buffer.Append(" = ");
                        goto Label_0BC5;

                    case 3:
                        throw new FormaterException("not support");

                    case 4:
                        throw new FormaterException("not support");

                    case 5:
                        throw new FormaterException("not support");

                    case 7:
                        base.buffer.Append(" AND ");
                        goto Label_0BC5;

                    case 8:
                        base.buffer.Append(" OR ");
                        goto Label_0BC5;

                    case 9:
                        base.buffer.Append(" / ");
                        goto Label_0BC5;

                    case 10:
                        base.buffer.Append(" = ");
                        goto Label_0BC5;

                    case 11:
                        base.buffer.Append(" > ");
                        goto Label_0BC5;

                    case 12:
                        base.buffer.Append(" >= ");
                        goto Label_0BC5;

                    case 14:
                        base.buffer.Append(" < ");
                        goto Label_0BC5;

                    case 15:
                        base.buffer.Append(" <= ");
                        goto Label_0BC5;

                    case 0x10:
                        base.buffer.Append(" <> ");
                        goto Label_0BC5;

                    case 0x11:
                        throw new FormaterException("not support");

                    case 0x12:
                        base.buffer.Append(" LIKE ");
                        if ((!(expr.left.GetType() == typeof(SqlIdentifierExpr)) || !base.IsToUpperCaseExpr((SqlIdentifierExpr)expr.left)) || !(expr.right.GetType() == typeof(SqlCharExpr)))
                        {
                            goto Label_0BC5;
                        }
                        base.context.add("toUpperCase", "toUpperCase");
                        this.FormatChar((SqlCharExpr)expr.right);
                        base.context.add("toUpperCase", null);
                        if (appendBrace)
                        {
                            base.buffer.Append(")");
                        }
                        return;

                    case 0x13:
                        base.buffer.Append(" >> ");
                        goto Label_0BC5;

                    case 20:
                        base.buffer.Append(".");
                        goto Label_0BC5;

                    case 0x15:
                        base.buffer.Append(" % ");
                        goto Label_0BC5;

                    case 0x16:
                        base.buffer.Append(" * ");
                        goto Label_0BC5;

                    case 0x17:
                        base.buffer.Append(" != ");
                        goto Label_0BC5;

                    case 0x18:
                        base.buffer.Append(" !< ");
                        goto Label_0BC5;

                    case 0x19:
                        base.buffer.Append(" !> ");
                        goto Label_0BC5;

                    case 0x1a:
                        base.buffer.Append(" - ");
                        goto Label_0BC5;

                    case 0x1b:
                        base.buffer.Append(" UNION ");
                        goto Label_0BC5;

                    case 40:
                        base.buffer.Append(" NOT LIKE ");
                        goto Label_0BC5;

                    case 0x2a:
                        base.buffer.Append(" || ");
                        goto Label_0BC5;
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
            Label_0BC5:
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
                base.buffer.Append("declare\n");
                int num = 0;
                int num2 = stmt.declItemList.Count;
                while (num < num2)
                {
                    SqlBlockStmt.DeclItem item = (SqlBlockStmt.DeclItem)stmt.declItemList[num];
                    if (item.GetType() == typeof(SqlBlockStmt.DeclVarItem))
                    {
                        SqlBlockStmt.DeclVarItem item2 = (SqlBlockStmt.DeclVarItem)item;
                        string name = item2.name;
                        if (!string.IsNullOrEmpty(name) && name.StartsWith("@"))
                        {
                            name = name.Substring(1);
                            base.buffer.Append(name);
                        }
                        else
                        {
                            base.buffer.Append(name);
                        }
                        base.buffer.Append(" ");
                        if (item2.dataType.EqualsIgnoreCase("CHAR"))
                        {
                            base.buffer.Append("CHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("VARCHAR") || item2.dataType.EqualsIgnoreCase("VARCHAR2"))
                        {
                            base.buffer.Append("VARCHAR2 (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NCHAR"))
                        {
                            base.buffer.Append("CHAR (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NVARCHAR") || item2.dataType.EqualsIgnoreCase("VARCHAR2"))
                        {
                            base.buffer.Append("VARCHAR2 (");
                            base.buffer.Append(item2.length);
                            base.buffer.Append(")");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NUMBER") || item2.dataType.EqualsIgnoreCase("DECIMAL"))
                        {
                            if (item2.precision >= 0)
                            {
                                base.buffer.Append("NUMBER (");
                                base.buffer.Append(item2.precision);
                                base.buffer.Append(", ");
                                base.buffer.Append(item2.scale);
                                base.buffer.Append(")");
                            }
                            else
                            {
                                base.buffer.Append("NUMBER");
                            }
                        }
                        else if (item2.dataType.EqualsIgnoreCase("INT") || item2.dataType.EqualsIgnoreCase("INTEGER"))
                        {
                            base.buffer.Append("NUMBER");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("SMALLINT") || item2.dataType.EqualsIgnoreCase("BIGINT"))
                        {
                            base.buffer.Append("NUMBER");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("DATETIME") || item2.dataType.EqualsIgnoreCase("DATE"))
                        {
                            base.buffer.Append("DATE");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("LONG"))
                        {
                            base.buffer.Append("LONG");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("RAW") || item2.dataType.EqualsIgnoreCase("BINARY"))
                        {
                            base.buffer.Append("RAW");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("LONG RAW") || item2.dataType.EqualsIgnoreCase("VARBINARY"))
                        {
                            base.buffer.Append("LONG RAW");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("ROWID"))
                        {
                            base.buffer.Append("ROWID");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("BLOB"))
                        {
                            base.buffer.Append("BLOB");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("CLOB"))
                        {
                            base.buffer.Append("NCLOB");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("NCLOB"))
                        {
                            base.buffer.Append("NCLOB");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("BFILE"))
                        {
                            base.buffer.Append("BFILE");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("UROWID"))
                        {
                            base.buffer.Append("UROWID");
                        }
                        else if (item2.dataType.EqualsIgnoreCase("FLOAT"))
                        {
                            if (item2.length != 0)
                            {
                                base.buffer.Append("FLOAT (");
                                base.buffer.Append(item2.length);
                                base.buffer.Append(")");
                            }
                            else
                            {
                                base.buffer.Append("FLOAT");
                            }
                        }
                        else
                        {
                            if (!item2.dataType.EqualsIgnoreCase("XMLTYPE") && !item2.dataType.EqualsIgnoreCase("XML"))
                            {
                                throw new FormaterException("not support datatype, column name is '" + item2.name + "' datatype is '" + item2.dataType + "'");
                            }
                            base.buffer.Append("XMLTYPE");
                        }
                        if (item2.defaultValueExpr != null)
                        {
                            base.buffer.Append(" := ");
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
                        base.buffer.Append(" IS ");
                        this.FormatSelectBase(item3.select);
                        base.buffer.Append(";\n");
                    }
                    num++;
                }
            }
            base.buffer.Append("begin\n");
            int num3 = 0;
            int count = stmt.stmtList.Count;
            while (num3 < count)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[num3];
                this.FormatStmt(stmt, stmt2);
                base.buffer.Append(";\n");
                num3++;
            }
            base.buffer.Append("end;");
        }

        protected override void FormatBreakStmt(SqlBreakStmt stmt)
        {
            base.buffer.Append("exit");
        }

        protected override void FormatCaseExpr(SqlCaseExpr expr)
        {
            if (expr.valueExpr != null)
            {
                base.buffer.Append("DECODE(");
                base.FormatExpr(expr.valueExpr);
                foreach (SqlCaseItem item in expr.itemList)
                {
                    base.buffer.Append(", ");
                    base.FormatExpr(item.conditionExpr);
                    base.buffer.Append(", ");
                    base.FormatExpr(item.valueExpr);
                }
                if (expr.elseExpr != null)
                {
                    base.buffer.Append(", ");
                    base.FormatExpr(expr.elseExpr);
                }
                base.buffer.Append(")");
            }
            else
            {
                base.buffer.Append("CASE ");
                foreach (SqlCaseItem item2 in expr.itemList)
                {
                    base.buffer.Append(" WHEN ");
                    base.FormatExpr(item2.conditionExpr);
                    base.buffer.Append(" THEN ");
                    base.FormatExpr(item2.valueExpr);
                }
                if (expr.elseExpr != null)
                {
                    base.buffer.Append(" ELSE ");
                    base.FormatExpr(expr.elseExpr);
                }
                base.buffer.Append(" END");
            }
        }

        protected override void FormatChar(SqlCharExpr expr)
        {
            string text = expr.text;
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
            if (base.context.ContainsKey("toUpperCase") && (base.context["toUpperCase"] != null))
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
            base.buffer.Append(column.name);
            base.buffer.Append(" ");
            if (column.dataType.EqualsIgnoreCase("CHAR"))
            {
                base.buffer.Append("CHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("VARCHAR") || column.dataType.EqualsIgnoreCase("VARCHAR2"))
            {
                base.buffer.Append("VARCHAR2 (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("NCHAR"))
            {
                base.buffer.Append("NCHAR (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("NVARCHAR") || column.dataType.EqualsIgnoreCase("NVARCHAR2"))
            {
                base.buffer.Append("NVARCHAR2 (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("UNIQUEIDENTIFIER"))
            {
                base.buffer.Append("NVARCHAR2 (36)");
            }
            else if ((column.dataType.EqualsIgnoreCase("NUMBER") || column.dataType.EqualsIgnoreCase("DECIMAL")) || column.dataType.EqualsIgnoreCase("NUMERIC"))
            {
                if (column.precision >= 0)
                {
                    base.buffer.Append("NUMBER (");
                    base.buffer.Append(column.precision);
                    base.buffer.Append(", ");
                    base.buffer.Append(column.scale);
                    base.buffer.Append(")");
                }
                else
                {
                    base.buffer.Append("NUMBER");
                }
            }
            else if (column.dataType.EqualsIgnoreCase("INT") || column.dataType.EqualsIgnoreCase("INTEGER"))
            {
                base.buffer.Append("NUMBER (10)");
            }
            else if (column.dataType.EqualsIgnoreCase("BIGINT"))
            {
                base.buffer.Append("NUMBER (19)");
            }
            else if (column.dataType.EqualsIgnoreCase("SMALLINT"))
            {
                base.buffer.Append("NUMBER (10)");
            }
            else if ((column.dataType.EqualsIgnoreCase("TIMESTAMP(6)") || column.dataType.EqualsIgnoreCase("DATETIME")) || (column.dataType.EqualsIgnoreCase("DATE") || column.dataType.EqualsIgnoreCase("SMALLDATETIME")))
            {
                base.buffer.Append("DATE");
            }
            else if (column.dataType.EqualsIgnoreCase("TIMESTAMP"))
            {
                base.buffer.Append("NVARCHAR2 (36)");
            }
            else if (column.dataType.EqualsIgnoreCase("LONG") || column.dataType.EqualsIgnoreCase("BIGINT"))
            {
                base.buffer.Append("LONG");
            }
            else if (column.dataType.EqualsIgnoreCase("RAW") || column.dataType.EqualsIgnoreCase("BINARY"))
            {
                base.buffer.Append("RAW (");
                base.buffer.Append(column.length);
                base.buffer.Append(")");
            }
            else if (column.dataType.EqualsIgnoreCase("LONG RAW") || column.dataType.EqualsIgnoreCase("VARBINARY"))
            {
                base.buffer.Append("LONG RAW");
            }
            else if (column.dataType.EqualsIgnoreCase("ROWID"))
            {
                base.buffer.Append("ROWID");
            }
            else if (column.dataType.EqualsIgnoreCase("BLOB"))
            {
                base.buffer.Append("BLOB");
            }
            else if (column.dataType.EqualsIgnoreCase("IMAGE"))
            {
                base.buffer.Append("BLOB");
            }
            else if (column.dataType.EqualsIgnoreCase("CLOB"))
            {
                base.buffer.Append("CLOB");
            }
            else if (column.dataType.EqualsIgnoreCase("NCLOB"))
            {
                base.buffer.Append("NCLOB");
            }
            else if (column.dataType.EqualsIgnoreCase("BFILE"))
            {
                base.buffer.Append("BFILE");
            }
            else if (column.dataType.EqualsIgnoreCase("UROWID"))
            {
                base.buffer.Append("UROWID");
            }
            else if (column.dataType.EqualsIgnoreCase("FLOAT"))
            {
                if (column.length != 0)
                {
                    base.buffer.Append("FLOAT (");
                    base.buffer.Append(column.length);
                    base.buffer.Append(")");
                }
                else
                {
                    base.buffer.Append("FLOAT");
                }
            }
            else if (column.dataType.EqualsIgnoreCase("INT") && column.autoIncrement)
            {
                base.buffer.Append("INT");
                base.context.add("Identity", column.name);
            }
            else if (column.dataType.EqualsIgnoreCase("BIT"))
            {
                base.buffer.Append("NUMBER (1)");
            }
            else
            {
                if (!column.dataType.EqualsIgnoreCase("XMLTYPE") && !column.dataType.EqualsIgnoreCase("XML"))
                {
                    throw new FormaterException("not support datatype, column name is '" + column.name + "' datatype is '" + column.dataType + "'");
                }
                base.buffer.Append("XMLTYPE");
            }
            if (column.defaultValueExpr != null)
            {
                base.buffer.Append(" DEFAULT ");
                if (((column.defaultValueExpr is SqlCharExpr) || (column.defaultValueExpr is SqlNCharExpr)) && (column.defaultValueExpr.toString() == ""))
                {
                    base.buffer.Append(" ' ' ");
                }
                else
                {
                    base.FormatExpr(column.defaultValueExpr);
                }
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
            throw new FormaterException("not support format continue statement");
        }

        protected void FormatContinueStmt(SqlBlockStmt block, SqlContinueStmt stmt)
        {
            int num = 0;
            if (block == null)
            {
                throw new FormaterException("Fatal Error.");
            }
            Hashtable hashtable = block.extendedAttributes();
            if (!hashtable.ContainsKey("CURRENT_LOOP_COUNT"))
            {
                throw new FormaterException("Fatal Error.");
            }
            num = (int)hashtable["CURRENT_LOOP_COUNT"];
            string str = "ksql_loop_" + num;
            base.buffer.Append("goto ");
            base.buffer.Append(str);
            base.buffer.Append("_end");
        }

        protected override void FormatCreateIndexStmt(SqlCreateIndexStmt stmt)
        {
            base.buffer.Append("CREATE ");
            if (stmt.isUnique)
            {
                base.buffer.Append("UNIQUE ");
            }
            base.buffer.Append("INDEX ");
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
                if (item.chineseOrderByMode != -1)
                {
                    base.buffer.Append("NLSSORT(\"");
                    base.FormatExpr(item.expr);
                    base.buffer.Append("\",'NLS_SORT=");
                    if (item.chineseOrderByMode == 2)
                    {
                        base.buffer.Append("SCHINESE_PINYIN_M')");
                    }
                    else if (item.chineseOrderByMode == 3)
                    {
                        base.buffer.Append("SCHINESE_STROKE_M')");
                    }
                    else if (item.chineseOrderByMode == 4)
                    {
                        base.buffer.Append("SCHINESE_RADICAL_M')");
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
            if (UUTN.isTempTable(stmt.tableName))
            {
                string tempTableSpace = base.options.GetTempTableSpace();
                if (!string.IsNullOrEmpty(tempTableSpace))
                {
                    base.buffer.Append(" TABLESPACE " + tempTableSpace);
                }
                if (base.options.IsNologging())
                {
                    base.buffer.Append(" NOLOGGING");
                }
            }
        }

        protected override void FormatCreateTableStmt(SqlCreateTableStmt stmt)
        {
            base.ValidateCreateTableStmt(stmt);
            base.buffer.Append("CREATE");
            int num = 0;
            if (UUTN.isGlobalTempTable(stmt.name))
            {
                base.buffer.Append(" GLOBAL TEMPORARY");
                stmt.name = stmt.name.Substring(2);
                num = 1;
            }
            else if (UUTN.isTempTable(stmt.name))
            {
                base.buffer.Append(" GLOBAL TEMPORARY");
                stmt.name = stmt.name.Substring(1);
                num = 2;
            }
            base.buffer.Append(" TABLE");
            base.buffer.Append(" " + stmt.name);
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
            if (((num == 0) && (base.options != null)) && base.options.IsNologging())
            {
                base.buffer.Append(" NOLOGGING ");
            }
            string tableSpace = null;
            if (num == 0)
            {
                if (base.options != null)
                {
                    tableSpace = base.options.GetTempTableSpace();
                }
                if (tableSpace == null)
                {
                    tableSpace = stmt.tableSpace;
                }
                this.FormatTableSpace(tableSpace);
            }
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

        protected override void FormatCreateViewStmt(SqlCreateViewStmt stmt)
        {
            base.FormatCreateViewStmt(stmt);
            base.buffer.Append(" with read only ");
        }

        protected override void FormatCursorLoopStmt(SqlCursorLoopStmt stmt)
        {
            this.FormatCursorLoopStmt(null, stmt);
        }

        protected void FormatCursorLoopStmt(SqlBlockStmt block, SqlCursorLoopStmt stmt)
        {
            int num = 0;
            if (block != null)
            {
                Hashtable hashtable = block.extendedAttributes();
                if (hashtable.ContainsKey("LOOP_COUNT"))
                {
                    num = ((int)hashtable["LOOP_COUNT"]) + 1;
                    hashtable["LOOP_COUNT"] = num;
                }
                else
                {
                    hashtable.Add("LOOP_COUNT", num);
                }
                hashtable.Add("CURRENT_LOOP_COUNT", num);
            }
            string curName = stmt.curName;
            if (!string.IsNullOrEmpty(curName) && curName.StartsWith("@"))
            {
                curName = curName.Substring(1);
            }
            base.buffer.Append("open ");
            base.buffer.Append(curName);
            base.buffer.Append(";\n");
            base.buffer.Append("loop\n");
            base.buffer.Append("fetch ");
            base.buffer.Append(curName);
            base.buffer.Append(" into ");
            int num2 = 0;
            int count = stmt.intoList.Count;
            while (num2 < count)
            {
                if (num2 != 0)
                {
                    base.buffer.Append(", ");
                }
                SqlExpr expr = (SqlExpr)stmt.intoList[num2];
                base.FormatExpr(expr);
                num2++;
            }
            base.buffer.Append(";\n");
            int num4 = 0;
            int num5 = stmt.stmtList.Count;
            while (num4 < num5)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[num4];
                this.FormatStmt(block, stmt2);
                base.buffer.Append(";\n");
                num4++;
            }
            base.buffer.Append("exit when ");
            base.buffer.Append(curName);
            base.buffer.Append("%notfound;\n");
            base.buffer.Append("end loop;\n");
            base.buffer.Append("close ");
            base.buffer.Append(curName);
            Hashtable hashtable2 = block.extendedAttributes();
            if (hashtable2.ContainsKey("LOOP_COUNT"))
            {
                num = ((int)hashtable2["LOOP_COUNT"]) - 1;
                hashtable2["CURRENT_LOOP_COUNT"] = num;
            }
        }

        protected override void FormatDateTimeExpr(SqlDateTimeExpr expr)
        {
            base.buffer.Append("TO_DATE('");
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
                base.buffer.Append("', 'YYYY-MM-DD HH24:MI:SS')");
            }
            else if (expr.timeType() == -19001)
            {
                base.buffer.Append((expr.getYear() < 10) ? "0" : "");
                base.buffer.Append(expr.getYear());
                base.buffer.Append((expr.getMonth() < 10) ? "-0" : "-");
                base.buffer.Append(expr.getMonth());
                base.buffer.Append((expr.getDate() < 10) ? "-0" : "-");
                base.buffer.Append(expr.getDate());
                base.buffer.Append("', 'YYYY-MM-DD')");
            }
            else if (expr.timeType() == -19002)
            {
                base.buffer.Append((expr.getHour() < 10) ? "0" : "");
                base.buffer.Append(expr.getHour());
                base.buffer.Append((expr.getMinute() < 10) ? ":0" : ":");
                base.buffer.Append(expr.getMinute());
                base.buffer.Append((expr.getSecond() < 10) ? ":0" : ":");
                base.buffer.Append(expr.getSecond());
                base.buffer.Append("', 'HH24:MI:SS')");
            }
        }

        protected override void FormatDeallocateStmt(SqlDeallocateStmt stmt)
        {
            base.buffer.Append("-- skip deallocate");
        }

        protected override void FormatDropTableStmt(SqlDropTableStmt stmt)
        {
            base.buffer.Append("DROP TABLE ");
            base.buffer.Append(this.FormatTableName(stmt.tableName));
            base.buffer.Append(" PURGE");
        }

        protected void FormateQueryHints(SqlSelect select)
        {
            Oracle10gHints.GetInstance().FormatHints(select.getHints(), select, base.buffer);
        }

        protected override void FormatExecStmt(SqlExecStmt stmt)
        {
            base.buffer.Append("CALL ");
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
            if (expr == null)
            {
                throw new FormaterException("IllegalArgumentException, expr is null");
            }
            if (expr.GetType() == typeof(SqlAllColumnExpr))
            {
                base.buffer.Append("*");
            }
            else if (expr.GetType() == typeof(SqlIdentifierExpr))
            {
                this.FormatIdentifierExpr(expr);
            }
            else if (expr.GetType() == typeof(SqlIntExpr))
            {
                base.buffer.Append(((SqlIntExpr)expr).text);
            }
            else if (expr.GetType() == typeof(SqlLongExpr))
            {
                base.buffer.Append(((SqlLongExpr)expr).text);
            }
            else if (expr.GetType() == typeof(SqlDoubleExpr))
            {
                base.buffer.Append(((SqlDoubleExpr)expr).text);
            }
            else if (expr.GetType() == typeof(SqlBinaryOpExpr))
            {
                this.FormatBinaryOpExpr((SqlBinaryOpExpr)expr, appendBrace);
            }
            else if (expr.GetType() == typeof(SqlMethodInvokeExpr))
            {
                this.FormatMethodInvokeExpr((SqlMethodInvokeExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlAggregateExpr))
            {
                base.FormatAggregateExprExpr((SqlAggregateExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlCharExpr))
            {
                this.FormatChar((SqlCharExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlNCharExpr))
            {
                this.FormatNChar((SqlNCharExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlVarRefExpr))
            {
                this.FormatVarRef((SqlVarRefExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlCaseExpr))
            {
                this.FormatCaseExpr((SqlCaseExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlInListExpr))
            {
                this.FormatInListExpr((SqlInListExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlExistsExpr))
            {
                base.FormatExiststExpr((SqlExistsExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlInSubQueryExpr))
            {
                base.FormatInSubQueryExpr((SqlInSubQueryExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlAllExpr))
            {
                base.FormatAllExpr((SqlAllExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlBetweenExpr))
            {
                base.FormatBetweenExpr((SqlBetweenExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlAnyExpr))
            {
                base.FormatAnyExpr((SqlAnyExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlSomeExpr))
            {
                base.FormatSomeExpr((SqlSomeExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlNullExpr))
            {
                base.buffer.Append("NULL");
            }
            else if (expr.GetType() == typeof(SqlDateTimeExpr))
            {
                this.FormatDateTimeExpr((SqlDateTimeExpr)expr);
            }
            else if (expr.GetType() == typeof(QueryExpr))
            {
                base.FormatQueryExpr((QueryExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlPriorIdentifierExpr))
            {
                this.FormatPriorIdentifierExpr((SqlPriorIdentifierExpr)expr);
            }
            else if (expr.GetType() == typeof(SqlNotExpr))
            {
                base.FormatNotExpr((SqlNotExpr)expr);
            }
            else if (expr.type == 0x1c)
            {
                this.FormatIdentityExpr(expr);
            }
            else if (expr.type == -1)
            {
                this.FormatDefault((SqlDefaultExpr)expr);
            }
            else
            {
                if (expr.type != 30)
                {
                    throw new FormaterException("Not support " + expr);
                }
                this.FormatXmlType((SqlXmlExpr)expr);
            }
        }

        protected override void FormatFetchStmt(SqlFetchStmt stmt)
        {
            base.buffer.Append("fetch ");
            string curName = stmt.curName;
            if (!string.IsNullOrEmpty(curName) && curName.StartsWith("@"))
            {
                curName = curName.Substring(1);
            }
            base.buffer.Append(curName);
            base.buffer.Append(" into ");
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
            throw new FormaterException("not support format goto statement");
        }

        protected override void FormatHintForDelete(SqlDeleteStmt stmt)
        {
            base.buffer.Append(Oracle10gHints.GetInstance().FormatHints(stmt.delete.getHints(), stmt.delete));
        }

        protected override void FormatIdentifierExpr(SqlExpr expr)
        {
            string str = ((SqlIdentifierExpr)expr).value;
            if (str.EqualsIgnoreCase(Token.KSQL_COL_NAME.value))
            {
                str = "COLUMN_NAME";
            }
            else if (str.EqualsIgnoreCase(Token.INDNAME.value))
            {
                str = "INDEX_NAME";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
            {
                str = "TABLE_NAME";
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
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
            {
                str = "DATA_DEFAULT";
            }
            else if (str.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
            {
                str = "NULLABLE";
            }
            if (!string.IsNullOrEmpty(str) && str.StartsWith("\""))
            {
                base.buffer.Append(str.ToUpper());
            }
            else if (str.EqualsIgnoreCase("KSQL_SEQ"))
            {
                this.FormatIdentity(expr);
            }
            else if (str.StartsWith("#"))
            {
                base.buffer.Append(this.FormatTableName(str));
            }
            else
            {
                base.buffer.Append(str);
            }
        }

        protected override void FormatIdentity(SqlExpr stmt)
        {
            base.buffer.Append(" ROWNUM ");
        }

        protected void FormatIdentityExpr(SqlExpr expr)
        {
            if (base.context["Identity"] == null)
            {
                SqlIdentityExpr expr2 = (SqlIdentityExpr)expr;
                base.context.add("Identity", "1|" + expr2.name);
                StringBuilder builder = new StringBuilder();
                builder.Append("CREATE SEQUENCE ");
                builder.Append(expr2.name);
                builder.Append(";\n");
                base.buffer.Insert(0, builder.ToString());
                base.buffer.Append(expr2.name);
                base.buffer.Append(".Nextval");
            }
        }

        protected void FormatIFExistSelectBase(SqlSelectBase select)
        {
            if (!(select.GetType() == typeof(SqlSelect)))
            {
                throw new NotSupportedException("Not support SQL Statement :" + select);
            }
            SqlSelect select2 = (SqlSelect)select;
            SqlExpr condition = select2.condition;
            base.buffer.Append(" select COUNT(*) FCOUNT INTO CNT from (");
            this.FormatSelect(select2);
            if (condition == null)
            {
                base.buffer.Append(" where rownum<=1)");
            }
            else
            {
                base.buffer.Append(" and rownum<=1)");
            }
        }

        protected override void FormatIfStmt(SqlIfStmt stmt)
        {
            this.FormatIfStmt(null, stmt);
        }

        protected void FormatIfStmt(SqlBlockStmt block, SqlIfStmt stmt)
        {
            if (stmt.condition.GetType() == typeof(SqlExistsExpr))
            {
                SqlExistsExpr condition = (SqlExistsExpr)stmt.condition;
                base.buffer.Append("DECLARE CNT PLS_INTEGER;\n");
                base.buffer.Append("BEGIN\n");
                this.FormatIFExistSelectBase(condition.subQuery);
                base.buffer.Append(";\n");
                if (condition.not)
                {
                    base.buffer.Append("IF CNT = 0 THEN\n");
                }
                else
                {
                    base.buffer.Append("IF CNT > 0 THEN\n");
                }
                for (int i = 0; i < stmt.trueStmtList.Count; i++)
                {
                    base.buffer.Append("EXECUTE IMMEDIATE '");
                    base.context.add("isScriptContext", "1");
                    int length = base.buffer.Length;
                    SqlStmt stmt2 = (SqlStmt)stmt.trueStmtList[i];
                    this.FormatStmt(block, stmt2);
                    base.buffer = base.HandleComma(base.buffer, length);
                    base.buffer.Append("';\n");
                    base.context.add("isScriptContext", null);
                }
                int count = stmt.falseStmtList.Count;
                if (count > 0)
                {
                    base.buffer.Append("ELSE\n");
                    for (int j = 0; j < count; j++)
                    {
                        base.buffer.Append("EXECUTE IMMEDIATE '");
                        base.context.add("isScriptContext", "1");
                        int pos = base.buffer.Length;
                        SqlStmt stmt3 = (SqlStmt)stmt.falseStmtList[j];
                        this.FormatStmt(block, stmt3);
                        base.buffer = base.HandleComma(base.buffer, pos);
                        base.buffer.Append("';\n");
                        base.context.add("isScriptContext", null);
                    }
                }
                base.buffer.Append("END IF;\n");
                base.buffer.Append("END;");
            }
            else
            {
                base.buffer.Append("if ");
                base.FormatExpr(stmt.condition);
                base.buffer.Append(" then\n");
                base.buffer.Append("begin\n");
                for (int k = 0; k < stmt.trueStmtList.Count; k++)
                {
                    SqlStmt stmt4 = (SqlStmt)stmt.trueStmtList[k];
                    this.FormatStmt(block, stmt4);
                    base.buffer.Append(";\n");
                }
                base.buffer.Append("end;\n");
                if ((stmt.falseStmtList != null) && (stmt.falseStmtList.Count > 0))
                {
                    base.buffer.Append("else\n");
                    base.buffer.Append("begin\n");
                    for (int m = 0; m < stmt.falseStmtList.Count; m++)
                    {
                        SqlStmt stmt5 = (SqlStmt)stmt.falseStmtList[m];
                        this.FormatStmt(block, stmt5);
                        base.buffer.Append(";\n");
                    }
                    base.buffer.Append("end;\n");
                }
                base.buffer.Append("end if");
            }
        }

        protected override void FormatInListExpr(SqlInListExpr expr)
        {
            base.buffer.Append("(");
            base.FormatExpr(expr.expr);
            if (expr.not)
            {
                base.buffer.Append(" NOT IN (");
            }
            else
            {
                base.buffer.Append(" IN (");
            }
            bool flag = false;
            int num = 1;
            foreach (SqlExpr expr2 in expr.targetList)
            {
                if (num < 0x3e8)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(expr2);
                    flag = true;
                }
                else
                {
                    if (expr.not)
                    {
                        base.buffer.Append(") AND ");
                    }
                    else
                    {
                        base.buffer.Append(") OR ");
                    }
                    base.FormatExpr(expr.expr);
                    if (expr.not)
                    {
                        base.buffer.Append(" NOT IN (");
                    }
                    else
                    {
                        base.buffer.Append(" IN (");
                    }
                    base.FormatExpr(expr2);
                    flag = true;
                    num = 1;
                }
                num++;
            }
            base.buffer.Append(")");
            base.buffer.Append(")");
        }

        protected override void FormatInsertStmt(SqlInsertStmt stmt)
        {
            bool flag;
            SqlInsert sql = stmt.insert;
            base.buffer.Append("INSERT ");
            if (sql.getHints() != null)
            {
                Oracle10gHints.GetInstance().FormatHints(sql.getHints(), sql, base.buffer);
                base.buffer.Append("INTO ");
            }
            else
            {
                base.buffer.Append("INTO ");
            }
            base.buffer.Append(sql.tableName);
            if (sql.columnList.Count != 0)
            {
                base.buffer.Append(" (");
                flag = false;
                foreach (object obj2 in sql.columnList)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    if (obj2.GetType() == typeof(SqlIdentifierExpr))
                    {
                        SqlIdentifierExpr expr = (SqlIdentifierExpr)obj2;
                        base.buffer.Append(expr.value);
                    }
                    else
                    {
                        if (obj2.GetType() != typeof(string))
                        {
                            throw new FormaterException("unexpect expression: '" + obj2 + "'");
                        }
                        base.buffer.Append((string)obj2);
                    }
                    flag = true;
                }
                base.buffer.Append(")");
            }
            if (sql.valueList.Count != 0)
            {
                base.buffer.Append(" VALUES (");
                flag = false;
                foreach (SqlExpr expr2 in sql.valueList)
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
            else
            {
                base.buffer.Append(" ");
                this.FormatSelectBase(stmt.insert.subQuery);
            }
        }

        protected override void FormatLabelStmt(SqlLabelStmt stmt)
        {
            throw new FormaterException("not support format label statement");
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
                base.buffer.Append(" WHEN MATCHED THEN ");
                base.buffer.Append("UPDATE SET ");
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
                if (matchedSql.UpdateWhere != null)
                {
                    base.buffer.Append(" WHERE ");
                    base.FormatExpr(matchedSql.UpdateWhere);
                }
                if (matchedSql.DeleteWhere != null)
                {
                    base.buffer.Append(" DELETE WHERE ");
                    base.FormatExpr(matchedSql.DeleteWhere);
                }
            }
            SqlMergeNotMatched notMatchedSql = merge.NotMatchedSql;
            if (notMatchedSql != null)
            {
                base.buffer.Append(" WHEN NOT MATCHED THEN ");
                base.buffer.Append("INSERT (");
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
                base.buffer.Append(")");
                base.buffer.Append(" VALUES (");
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
                base.buffer.Append(")");
                if (notMatchedSql.InsertWhere != null)
                {
                    base.buffer.Append(" WHERE ");
                    base.FormatExpr(notMatchedSql.InsertWhere);
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
                base.buffer.Append("ATAN2(");
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
                base.buffer.Append("CEIL(");
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
                base.buffer.Append("MOD(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LOG"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LN(");
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
                    SqlExpr expr2 = (SqlExpr)expr.parameters[2];
                    if (!(expr2.GetType() == typeof(SqlIntExpr)))
                    {
                        throw new FormaterException("ERROR");
                    }
                    if (Convert.ToInt32(((SqlIntExpr)expr2).text) == 0)
                    {
                        base.buffer.Append("ROUND(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(", ");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                    }
                    else
                    {
                        base.buffer.Append("TRUNC(");
                        base.FormatExpr((SqlExpr)expr.parameters[0]);
                        base.buffer.Append(", ");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(")");
                    }
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
                base.buffer.Append("FN_GCD(");
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
                base.buffer.Append("FN_LCM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ASCII"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
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
                base.buffer.Append("CHR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("CHARINDEX"))
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("INSTR(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(")");
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                    }
                    base.buffer.Append("INSTR(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
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
                base.buffer.Append("CONCAT(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LEFT"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("SUBSTR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 0, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LEN"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("LENGTH(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LENGTH"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("LENGTH(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LOWER"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("LOWER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LCASE"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("LOWER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("LTRIM"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("LTRIM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("REPLACE"))
            {
                if (expr.parameters.Count != 3)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
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
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("SUBSTR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", CASE WHEN LENGTH(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(") >= ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" THEN ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" * -1 ELSE LENGTH(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(") * -1 END, ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("RTRIM"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("RTRIM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("SOUNDEX"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("SOUNDEX(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("SUBSTRING"))
            {
                if (expr.parameters.Count != 3)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("SUBSTR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                if (expr.parameters[1].GetType() == typeof(SqlIntExpr))
                {
                    int num2 = ((SqlIntExpr)expr.parameters[1]).value;
                    if (num2 < 1)
                    {
                        if (num2 == 0)
                        {
                            expr.parameters[1] = new SqlIntExpr(1);
                        }
                        if (num2 < 0)
                        {
                            throw new FormaterException("SUBSTRING parameter2 cannot not smaller then 1S.");
                        }
                    }
                }
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[2]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("TRIM"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("LTRIM(RTRIM(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append("))");
            }
            else if (methodName.EqualsIgnoreCase("UCASE"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("UPPER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("UPPER"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("unexcept parameters size: " + expr.parameters.Count);
                }
                base.buffer.Append("UPPER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("TOCHAR") || methodName.EqualsIgnoreCase("TO_CHAR"))
            {
                this.Format_TOCHAR_Invoke(expr);
            }
            else if (methodName.EqualsIgnoreCase("CONVERT"))
            {
                if (!(expr.parameters[0].GetType() == typeof(SqlMethodInvokeExpr)) && !(expr.parameters[0] is SqlIdentifierExpr))
                {
                    throw new FormaterException("unexpect parameter:" + expr.parameters[0]);
                }
                string str2 = null;
                if (expr.parameters[0].GetType() == typeof(SqlMethodInvokeExpr))
                {
                    SqlMethodInvokeExpr expr3 = (SqlMethodInvokeExpr)expr.parameters[0];
                    str2 = expr3.methodName;
                }
                if (expr.parameters[0] is SqlIdentifierExpr)
                {
                    SqlIdentifierExpr expr4 = (SqlIdentifierExpr)expr.parameters[0];
                    str2 = expr4.value;
                }
                if (str2.EqualsIgnoreCase("DATETIME"))
                {
                    base.buffer.Append("TO_DATE(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", 'YYYY-MM-DD HH24:MI:SS')");
                }
                else if ((str2.EqualsIgnoreCase("VARCHAR") || str2.EqualsIgnoreCase("NVARCHAR")) || (str2.EqualsIgnoreCase("CHAR") || str2.EqualsIgnoreCase("NCHAR")))
                {
                    base.buffer.Append("TO_CHAR(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(")");
                }
                else
                {
                    if (!str2.EqualsIgnoreCase("DECIMAL"))
                    {
                        throw new FormaterException("unexpect expression:" + str2);
                    }
                    SqlMethodInvokeExpr expr5 = (SqlMethodInvokeExpr)expr.parameters[0];
                    base.buffer.Append("CAST(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" AS NUMBER(");
                    base.buffer.Append(((SqlIntExpr)expr5.parameters[0]).value);
                    base.buffer.Append(", ");
                    base.buffer.Append(((SqlIntExpr)expr5.parameters[1]).value);
                    base.buffer.Append("))");
                }
            }
            else if (methodName.EqualsIgnoreCase("CURDATE"))
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_DATE(TO_CHAR(SYSDATE, 'YYYY-MM-DD'), 'YYYY-MM-DD')");
            }
            else if (methodName.EqualsIgnoreCase("CURTIME"))
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_CHAR(SYSDATE,'HH24:MI:SS')");
            }
            else if (methodName.EqualsIgnoreCase("DATEADD"))
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" + ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append("/60/60/24)");
                }
                else
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("ERROR");
                    }
                    SqlExpr expr6 = (SqlExpr)expr.parameters[0];
                    if (!(expr6.GetType() == typeof(SqlIdentifierExpr)))
                    {
                        throw new FormaterException("illegal datepart.");
                    }
                    string str3 = ((SqlIdentifierExpr)expr6).value;
                    if ((str3 == null) || (str3.Length == 0))
                    {
                        throw new FormaterException("illegal datepart.");
                    }
                    if (("YEAR".EqualsIgnoreCase(str3) || "YY".EqualsIgnoreCase(str3)) || "YYYY".EqualsIgnoreCase(str3))
                    {
                        base.buffer.Append("ADD_MONTHS(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(", TRUNC(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(") * 12)");
                    }
                    else if (("MONTH".EqualsIgnoreCase(str3) || "MM".EqualsIgnoreCase(str3)) || "M".EqualsIgnoreCase(str3))
                    {
                        base.buffer.Append("ADD_MONTHS(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(", TRUNC(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(" ))");
                    }
                    else if (("DAY".EqualsIgnoreCase(str3) || "DD".EqualsIgnoreCase(str3)) || "D".EqualsIgnoreCase(str3))
                    {
                        base.buffer.Append("(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" + TRUNC(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append("))");
                    }
                    else if ("HOUR".EqualsIgnoreCase(str3) || "HH".EqualsIgnoreCase(str3))
                    {
                        base.buffer.Append("(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" + TRUNC(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(") / 24)");
                    }
                    else if (("MINUTE".EqualsIgnoreCase(str3) || "MI".EqualsIgnoreCase(str3)) || "N".EqualsIgnoreCase(str3))
                    {
                        base.buffer.Append("(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" + TRUNC(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(") / 24 / 60)");
                    }
                    else
                    {
                        if ((!"SECOND".EqualsIgnoreCase(str3) && !"SS".EqualsIgnoreCase(str3)) && !"S".EqualsIgnoreCase(str3))
                        {
                            throw new FormaterException("not support datepart:" + str3);
                        }
                        base.buffer.Append("(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" + TRUNC(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(") / 24 / 60 / 60)");
                    }
                }
            }
            else if (methodName.EqualsIgnoreCase("DATETIMEADD"))
            {
                if (expr.parameters.Count != 3)
                {
                    throw new FormaterException("unexpect parameters.");
                }
                SqlExpr expr7 = (SqlExpr)expr.parameters[0];
                if (!(expr7.GetType() == typeof(SqlIdentifierExpr)))
                {
                    throw new FormaterException("illegal datepart.");
                }
                string str4 = ((SqlIdentifierExpr)expr7).value;
                if ((str4 == null) || (str4.Length == 0))
                {
                    throw new FormaterException("illegal datepart.");
                }
                if (("YEAR".EqualsIgnoreCase(str4) || "YY".EqualsIgnoreCase(str4)) || "YYYY".EqualsIgnoreCase(str4))
                {
                    base.buffer.Append("(");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(" + INTERVAL '");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append("' YEAR)");
                }
                else if (("MONTH".EqualsIgnoreCase(str4) || "MM".EqualsIgnoreCase(str4)) || "M".EqualsIgnoreCase(str4))
                {
                    base.buffer.Append("(");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(" + INTERVAL '");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append("' MONTH)");
                }
                else if (("DAY".EqualsIgnoreCase(str4) || "DD".EqualsIgnoreCase(str4)) || "D".EqualsIgnoreCase(str4))
                {
                    base.buffer.Append("(");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(" + INTERVAL '");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append("' DAY)");
                }
                else if ("HOUR".Equals(str4) || "HH".Equals(str4))
                {
                    base.buffer.Append("(");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(" + INTERVAL '");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append("' HOUR)");
                }
                else if (("MINUTE".EqualsIgnoreCase(str4) || "MI".EqualsIgnoreCase(str4)) || "N".EqualsIgnoreCase(str4))
                {
                    base.buffer.Append("(");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(" + INTERVAL '");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append("' MINUTE)");
                }
                else
                {
                    if ((!"SECOND".EqualsIgnoreCase(str4) && !"SS".EqualsIgnoreCase(str4)) && !"S".EqualsIgnoreCase(str4))
                    {
                        throw new FormaterException("unexpect datepart:" + str4);
                    }
                    base.buffer.Append("(");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append(" + INTERVAL '");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append("' SECOND)");
                }
            }
            else if (methodName.EqualsIgnoreCase("DATEDIFF"))
            {
                if (expr.parameters.Count == 2)
                {
                    base.buffer.Append("FLOOR((");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(" - ");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append(") * 60 * 60 * 24)");
                }
                else if (expr.parameters.Count == 3)
                {
                    if (expr.parameters.Count != 3)
                    {
                        throw new FormaterException("unexpect parameters.");
                    }
                    SqlExpr expr8 = (SqlExpr)expr.parameters[0];
                    if (!(expr8.GetType() == typeof(SqlIdentifierExpr)))
                    {
                        throw new FormaterException("illegal datepart.");
                    }
                    string str5 = ((SqlIdentifierExpr)expr8).value;
                    if ((str5 == null) || (str5.Length == 0))
                    {
                        throw new FormaterException("illegal datepart.");
                    }
                    if (("YEAR".EqualsIgnoreCase(str5) || "YY".EqualsIgnoreCase(str5)) || "YYYY".EqualsIgnoreCase(str5))
                    {
                        base.buffer.Append("ROUND(MONTHS_BETWEEN(TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" AS DATE), 'YYYY'), TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(" AS DATE), 'YYYY'))/12)");
                    }
                    else if (("MONTH".EqualsIgnoreCase(str5) || "MM".EqualsIgnoreCase(str5)) || "M".EqualsIgnoreCase(str5))
                    {
                        base.buffer.Append("ROUND(MONTHS_BETWEEN(TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" AS DATE), 'MM'), TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(" AS DATE), 'MM')))");
                    }
                    else if (("DAY".EqualsIgnoreCase(str5) || "DD".EqualsIgnoreCase(str5)) || "D".EqualsIgnoreCase(str5))
                    {
                        base.buffer.Append("ROUND( TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" AS DATE), 'DD') - TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(" AS DATE), 'DD'))");
                    }
                    else if ("HOUR".EqualsIgnoreCase(str5) || "HH".EqualsIgnoreCase(str5))
                    {
                        base.buffer.Append("((TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" AS DATE), 'HH')  - TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(" AS DATE), 'HH') ) * 24)");
                    }
                    else if (("MINUTE".EqualsIgnoreCase(str5) || "MI".EqualsIgnoreCase(str5)) || "N".EqualsIgnoreCase(str5))
                    {
                        base.buffer.Append("(( TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" AS DATE), 'MI') - TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(" AS DATE), 'MI')) * 24 * 60)");
                    }
                    else
                    {
                        if ((!"SECOND".EqualsIgnoreCase(str5) && !"SS".EqualsIgnoreCase(str5)) && !"S".EqualsIgnoreCase(str5))
                        {
                            throw new FormaterException("not support datepart:" + str5);
                        }
                        base.buffer.Append("((TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[2]);
                        base.buffer.Append(" AS DATE), 'SS') - TRUNC( CAST(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(" AS DATE), 'SS')) * 24 * 60)");
                    }
                }
            }
            else if (methodName.EqualsIgnoreCase("DAYNAME"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'DAY')");
            }
            else if (methodName.EqualsIgnoreCase("DAYOFMONTH"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'DD'))");
            }
            else if (methodName.EqualsIgnoreCase("DAYOFWEEK"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'D'))");
            }
            else if (methodName.EqualsIgnoreCase("DAYOFYEAR"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'DDD'))");
            }
            else if (methodName.EqualsIgnoreCase("GETDATE"))
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SYSDATE");
            }
            else if (methodName.EqualsIgnoreCase("HOUR"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'HH24'))");
            }
            else if (methodName.EqualsIgnoreCase("MINUTE"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'MI'))");
            }
            else if (methodName.EqualsIgnoreCase("MONTH"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'MM'))");
            }
            else if (methodName.EqualsIgnoreCase("MONTHNAME"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'MON')");
            }
            else if (methodName.EqualsIgnoreCase("NOW"))
            {
                if (expr.parameters.Count != 0)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("SYSDATE");
            }
            else if (methodName.EqualsIgnoreCase("QUARTER"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'Q'))");
            }
            else if (methodName.EqualsIgnoreCase("SECOND"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'SS'))");
            }
            else if (methodName.EqualsIgnoreCase("WEEK"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'WW'))");
            }
            else if (methodName.EqualsIgnoreCase("YEAR"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'YYYY'))");
            }
            else if (methodName.EqualsIgnoreCase("TO_DATE"))
            {
                this.Format_TO_DATE_Invoke(expr);
            }
            else if (methodName.EqualsIgnoreCase("MONTHS_BETWEEN"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("((TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'YYYY')) - TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", 'YYYY'))) * 12 + TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", 'MM')) - TO_NUMBER(TO_CHAR(");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", 'MM')))");
            }
            else if (methodName.EqualsIgnoreCase("DAYS_BETWEEN"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ROUND((TRUNC( CAST(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" AS DATE), 'DD') - TRUNC( CAST(");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" AS DATE), 'DD')))");
            }
            else if (methodName.EqualsIgnoreCase("ADD_DAYS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ADD_HOURS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append("/24)");
            }
            else if (methodName.EqualsIgnoreCase("ADD_MINUTES"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append("/1440)");
            }
            else if (methodName.EqualsIgnoreCase("ADD_SECONDS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(" + ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append("/86400)");
            }
            else if (methodName.EqualsIgnoreCase("ADD_MONTHS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ADD_MONTHS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("ADD_YEARS"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("ADD_MONTHS(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(" * 12)");
            }
            else if (methodName.EqualsIgnoreCase("DATENAME"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                SqlExpr expr9 = (SqlExpr)expr.parameters[0];
                if (expr9.GetType() == typeof(SqlIdentifierExpr))
                {
                    string str6 = ((SqlIdentifierExpr)expr9).value;
                    if ((str6 == null) || (str6.Length == 0))
                    {
                        throw new FormaterException("illegal datepart.");
                    }
                    if (("YEAR".EqualsIgnoreCase(str6) || "YY".EqualsIgnoreCase(str6)) || "YYYY".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'YYYY')");
                    }
                    else if (("MONTH".EqualsIgnoreCase(str6) || "MM".EqualsIgnoreCase(str6)) || "M".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'MM')");
                    }
                    else if (("QUARTER".EqualsIgnoreCase(str6) || "QQ".EqualsIgnoreCase(str6)) || "Q".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'Q')");
                    }
                    else if (("DAYOFYEAR".EqualsIgnoreCase(str6) || "DY".EqualsIgnoreCase(str6)) || "Y".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'DDD')");
                    }
                    else if (("DAY".EqualsIgnoreCase(str6) || "DD".EqualsIgnoreCase(str6)) || "D".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'DD')");
                    }
                    else if (("WEEK".EqualsIgnoreCase(str6) || "WK".EqualsIgnoreCase(str6)) || "WW".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'IW')");
                    }
                    else if ("WEEKDAY".EqualsIgnoreCase(str6) || "DW".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'DAY')");
                    }
                    else if ("HOUR".EqualsIgnoreCase(str6) || "HH".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'HH24')");
                    }
                    else if (("MINUTE".EqualsIgnoreCase(str6) || "MI".EqualsIgnoreCase(str6)) || "N".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'MI')");
                    }
                    else if (("SECOND".EqualsIgnoreCase(str6) || "SS".EqualsIgnoreCase(str6)) || "S".EqualsIgnoreCase(str6))
                    {
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'SS')");
                    }
                    else
                    {
                        if (!"MILLISECOND".EqualsIgnoreCase(str6) && !"MS".EqualsIgnoreCase(str6))
                        {
                            throw new FormaterException("not support datepart:" + str6);
                        }
                        base.buffer.Append("TO_CHAR(");
                        base.FormatExpr((SqlExpr)expr.parameters[1]);
                        base.buffer.Append(", 'SSxFF3')");
                    }
                }
            }
            else if (methodName.EqualsIgnoreCase("ISNULL"))
            {
                if (expr.parameters.Count != 2)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("NVL(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(")");
            }
            else if (methodName.EqualsIgnoreCase("NULLIF"))
            {
                base.buffer.Append("DECODE(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(", ");
                base.FormatExpr((SqlExpr)expr.parameters[1]);
                base.buffer.Append(", NULL, ");
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
                        base.buffer.Append(" AS NUMBER)");
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
                    base.buffer.Append(" AS NUMBER(");
                    base.FormatExpr((SqlExpr)expr.parameters[1]);
                    base.buffer.Append(", ");
                    base.FormatExpr((SqlExpr)expr.parameters[2]);
                    base.buffer.Append("))");
                }
            }
            else if (methodName.EqualsIgnoreCase("TO_NUMBER"))
            {
                if (expr.parameters.Count != 1)
                {
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("TO_NUMBER(");
                base.FormatExpr((SqlExpr)expr.parameters[0]);
                base.buffer.Append(")");
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
                    base.buffer.Append("FLOOR(TO_NUMBER(");
                    base.FormatExpr((SqlExpr)expr.parameters[0]);
                    base.buffer.Append("))");
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
                    throw new FormaterException("ERROR");
                }
                base.buffer.Append("LOWER(SYS_GUID())");
            }
            else if (methodName.EqualsIgnoreCase("NEWBOSID"))
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

        protected override void FormatNChar(SqlNCharExpr expr)
        {
            base.buffer.Append("N");
            base.buffer.Append("'");
            base.buffer.Append(expr.text);
            base.buffer.Append("'");
        }

        protected override void FormatOpenStmt(SqlOpenStmt stmt)
        {
            base.buffer.Append("OPEN ");
            base.buffer.Append(stmt.curName);
        }

        protected override void FormatPriorIdentifierExpr(SqlPriorIdentifierExpr expr)
        {
            base.buffer.Append("PRIOR ");
            base.buffer.Append(expr.value);
        }

        protected override void FormatSelect(SqlSelect select)
        {
            if (select.into != null)
            {
                base.buffer.Append("CREATE TABLE ");
                base.buffer.Append(select.into.new_table);
                base.buffer.Append(" AS ");
            }
            if ((select.limit != null) && (select.orderBy.Count != 0))
            {
                base.buffer.Append("SELECT * FROM (SELECT ");
            }
            else
            {
                base.buffer.Append("SELECT ");
            }
            this.FormateQueryHints(select);
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
            else
            {
                base.buffer.Append(" FROM DUAL");
            }
            ArrayList tableSourceList = new ArrayList();
            this.ComputeTableSourceList(select.tableSource, tableSourceList);
            bool whereFlag = false;
            if (select.condition != null)
            {
                base.buffer.Append(" WHERE ");
                base.FormatExpr(select.condition);
                whereFlag = true;
            }
            if ((select.limit != null) && (select.orderBy.Count == 0))
            {
                if (!whereFlag)
                {
                    base.buffer.Append(" WHERE ROWNUM <= ");
                }
                else
                {
                    base.buffer.Append(" AND ROWNUM <= ");
                }
                base.buffer.Append(select.limit.value);
                if (select.limit.type == 1)
                {
                    throw new FormaterException("Not support");
                }
                whereFlag = true;
            }
            if (select.tableSource.GetType() == typeof(SqlJoinedTableSource))
            {
                this.AppendJoinedCondition(tableSourceList, (SqlJoinedTableSource)select.tableSource, whereFlag);
            }
            if (select.hierarchicalQueryClause != null)
            {
                if (select.hierarchicalQueryClause.startWithCondition != null)
                {
                    base.buffer.Append(" START WITH ");
                    base.FormatExpr(select.hierarchicalQueryClause.startWithCondition);
                }
                if (select.hierarchicalQueryClause.connectByCondition == null)
                {
                    throw new FormaterException("connectByCondition is null");
                }
                base.buffer.Append(" CONNECT BY ");
                base.FormatExpr(select.hierarchicalQueryClause.connectByCondition);
            }
            if (select.groupBy.Count != 0)
            {
                base.buffer.Append(" GROUP BY ");
                if (select.hasWithRollUp)
                {
                    base.buffer.Append("ROLLUP(");
                }
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
                if (select.hasWithRollUp)
                {
                    base.buffer.Append(")");
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
                    if (item.chineseOrderByMode != -1)
                    {
                        base.buffer.Append("NLSSORT(");
                        base.FormatExpr(item.expr);
                        base.buffer.Append(",'NLS_SORT=");
                        if (item.chineseOrderByMode == 2)
                        {
                            base.buffer.Append("SCHINESE_PINYIN_M')");
                        }
                        else if (item.chineseOrderByMode == 3)
                        {
                            base.buffer.Append("SCHINESE_STROKE_M')");
                        }
                        else if (item.chineseOrderByMode == 4)
                        {
                            base.buffer.Append("SCHINESE_RADICAL_M')");
                        }
                    }
                    else
                    {
                        base.FormatExpr(item.expr);
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
                if (select.limit != null)
                {
                    base.buffer.Append(") WHERE ROWNUM <= ");
                    base.buffer.Append(select.limit.value);
                    if (select.limit.type == 1)
                    {
                        throw new FormaterException("Not support");
                    }
                }
            }
            if (((select.tableSource.GetType() == typeof(SqlTableSource)) && (((SqlTableSource)select.tableSource).lockingHint != null)) && ((((SqlTableSource)select.tableSource).lockingHint.Length > 0) && "holdlock".EqualsIgnoreCase(((SqlTableSource)select.tableSource).lockingHint)))
            {
                base.buffer.Append(" FOR UPDATE");
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
                if (item.expr.GetType() == typeof(SqlIdentifierExpr))
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
                                ((SqlIdentifierExpr)expr).value = "DATA_DEFAULT";
                            }
                            else if (((SqlIdentifierExpr)expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                            {
                                ((SqlIdentifierExpr)expr).value = "NULLABLE";
                            }
                        }
                    }
                }
                if ((item.alias != null) && (item.alias.Length != 0))
                {
                    if (item.expr.GetType() == typeof(SqlIdentifierExpr))
                    {
                        if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value))
                        {
                            SqlIdentifierExpr expr2 = new SqlIdentifierExpr
                            {
                                value = "COLUMN_NAME"
                            };
                            base.FormatExpr(expr2);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.TABNAME.value))
                        {
                            SqlIdentifierExpr expr3 = new SqlIdentifierExpr
                            {
                                value = "TABLE_NAME"
                            };
                            base.FormatExpr(expr3);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
                        {
                            SqlIdentifierExpr expr4 = new SqlIdentifierExpr
                            {
                                value = "TABLE_NAME"
                            };
                            base.FormatExpr(expr4);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.INDNAME.value))
                        {
                            SqlIdentifierExpr expr5 = new SqlIdentifierExpr
                            {
                                value = "INDEX_NAME"
                            };
                            base.FormatExpr(expr5);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
                        {
                            SqlIdentifierExpr expr6 = new SqlIdentifierExpr
                            {
                                value = "CONSTRAINT_NAME"
                            };
                            base.FormatExpr(expr6);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
                        {
                            SqlIdentifierExpr expr7 = new SqlIdentifierExpr
                            {
                                value = "TABLE_NAME"
                            };
                            base.FormatExpr(expr7);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
                        {
                            SqlIdentifierExpr expr8 = new SqlIdentifierExpr
                            {
                                value = "CONSTRAINT_TYPE"
                            };
                            base.FormatExpr(expr8);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                        {
                            SqlIdentifierExpr expr9 = new SqlIdentifierExpr
                            {
                                value = "DATA_DEFAULT"
                            };
                            base.FormatExpr(expr9);
                        }
                        else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                        {
                            SqlIdentifierExpr expr10 = new SqlIdentifierExpr
                            {
                                value = "NULLABLE"
                            };
                            base.FormatExpr(expr10);
                        }
                        else
                        {
                            base.FormatExpr(item.expr);
                        }
                    }
                    else
                    {
                        base.FormatExpr(item.expr);
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
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value))
                    {
                        SqlIdentifierExpr expr12 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr12, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.TABNAME.value))
                    {
                        SqlIdentifierExpr expr13 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr13, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.INDNAME.value))
                    {
                        SqlIdentifierExpr expr14 = new SqlIdentifierExpr
                        {
                            value = "INDEX_NAME"
                        };
                        this.FormatExpr(expr14, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
                    {
                        SqlIdentifierExpr expr15 = new SqlIdentifierExpr
                        {
                            value = "CONSTRAINT_NAME"
                        };
                        this.FormatExpr(expr15, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
                    {
                        SqlIdentifierExpr expr16 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr16, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
                    {
                        SqlIdentifierExpr expr17 = new SqlIdentifierExpr
                        {
                            value = "CONSTRAINT_TYPE"
                        };
                        this.FormatExpr(expr17, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                    {
                        SqlIdentifierExpr expr18 = new SqlIdentifierExpr
                        {
                            value = "DATA_DEFAULT"
                        };
                        this.FormatExpr(expr18, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                    {
                        SqlIdentifierExpr expr19 = new SqlIdentifierExpr
                        {
                            value = "NULLABLE"
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
            return flag;
        }

        private void FormatSelectForUpdate(SqlSelectBase select, SqlUpdate update, SubQueryUpdateItem queryItem, string tPrefix, string vPrefix)
        {
            if (select.GetType() == typeof(SqlSelect))
            {
                SqlSelect select2 = (SqlSelect)select;
                if (!this.TableSourceExist(select, update.updateTable.name, update.updateTable.alias))
                {
                    if (select2.tableSource == null)
                    {
                        select2.tableSource = update.updateTable;
                    }
                    else if (update.updateTable != null)
                    {
                        SqlJoinedTableSource source = new SqlJoinedTableSource
                        {
                            joinType = 4,
                            left = update.updateTable,
                            right = select2.tableSource
                        };
                        select2.tableSource = source;
                    }
                }
                if (update.condition != null)
                {
                    if (select2.condition == null)
                    {
                        select2.condition = update.condition;
                    }
                    else
                    {
                        SqlBinaryOpExpr expr = new SqlBinaryOpExpr
                        {
                            Operator = 7,
                            left = select2.condition,
                            right = update.condition
                        };
                        select2.condition = expr;
                    }
                }
                select2.getHints().Insert(0, new KHint("BYPASS_UJVC"));
                for (int i = 0; i < select2.selectList.Count; i++)
                {
                    SqlSelectItem item = (SqlSelectItem)select2.selectList[i];
                    item.alias = tPrefix + i;
                }
                string alias = update.updateTable.alias;
                if ((alias == null) || (alias.Trim().Length == 0))
                {
                    alias = update.updateTable.name;
                }
                for (int j = 0; j < queryItem.columnList.Count; j++)
                {
                    SqlSelectItem item2 = new SqlSelectItem
                    {
                        alias = vPrefix + j
                    };
                    SqlBinaryOpExpr expr2 = new SqlBinaryOpExpr
                    {
                        Operator = 20,
                        left = new SqlIdentifierExpr(alias),
                        right = new SqlIdentifierExpr((string)queryItem.columnList[j])
                    };
                    item2.expr = expr2;
                    select2.selectList.Add(item2);
                }
            }
            else if (select.GetType() == typeof(SqlUnionSelect))
            {
                SqlUnionSelect select3 = (SqlUnionSelect)select;
                this.FormatSelectForUpdate(select3.left, update, queryItem, tPrefix, vPrefix);
                this.FormatSelectForUpdate(select3.right, update, queryItem, tPrefix, vPrefix);
            }
        }

        public override void FormatSelectList(SqlSelect select)
        {
            this.FormatSelectColumnList(select);
        }

        protected override void FormatSetLocalVariantStmt(SqlSetLocalVariantStmt stmt)
        {
            base.FormatExpr(stmt.variant);
            base.buffer.Append(" := ");
            base.FormatExpr(stmt.value);
        }

        protected override void FormatShowColumnsStmt(SqlShowColumnsStmt stmt)
        {
            string str = "SELECT COLUMN_NAME, DATA_TYPE, DEFAULT_LENGTH, DATA_PRECISION, DATA_SCALE, NULLABLE, TABLE_NAME FROM USER_TAB_COLUMNS ";
            if ((stmt.tableName != null) && (stmt.tableName.Length != 0))
            {
                str = str + "WHERE TABLE_NAME = '" + stmt.tableName + "' ";
            }
            str = str + " ORDER BY COLUMN_ID";
            base.buffer.Append(str);
        }

        protected override void FormatShowTablesStmt(SqlShowTablesStmt stmt)
        {
            string str = "SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME";
            base.buffer.Append(str);
        }

        protected void FormatStmt(SqlBlockStmt block, SqlStmt stmt)
        {
            if (stmt.GetType() == typeof(SqlIfStmt))
            {
                this.FormatIfStmt(block, (SqlIfStmt)stmt);
            }
            else if (stmt.GetType() == typeof(SqlWhileStmt))
            {
                this.FormatWhileStmt(block, (SqlWhileStmt)stmt);
            }
            else if (stmt.GetType() == typeof(SqlCursorLoopStmt))
            {
                this.FormatCursorLoopStmt(block, (SqlCursorLoopStmt)stmt);
            }
            else if (stmt.GetType() == typeof(SqlContinueStmt))
            {
                this.FormatContinueStmt(block, (SqlContinueStmt)stmt);
            }
            else if (stmt.GetType() == typeof(SqlDropTableStmt))
            {
                this.FormatDropTableStmt((SqlDropTableStmt)stmt);
            }
            else
            {
                base.FormatStmt(stmt);
            }
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
                    throw new FormaterException("not support constraint:" + constraint);
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

        protected override string FormatTableName(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                string message = "\"tableName\" is empty!";
                throw new IllegalStateException(message);
            }
            if (UUTN.isGlobalTempTable(tableName))
            {
                tableName = tableName.Substring(2);
            }
            if (UUTN.isTempTable(tableName))
            {
                tableName = tableName.Substring(1);
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

        protected override void FormatTableSource(SqlTableSourceBase tableSource)
        {
            if (tableSource.GetType() == typeof(SqlTableSource))
            {
                SqlTableSource source = (SqlTableSource)tableSource;
                if (source.name.EqualsIgnoreCase(Token.USERTABLES.value))
                {
                    base.buffer.Append("(SELECT TABLE_NAME TABLE_NAME, 'U' TABLE_XTYPE FROM USER_TABLES UNION SELECT VIEW_NAME TABLE_NAME, 'V' TABLE_XTYPE FROM USER_VIEWS)");
                }
                else if (source.name.EqualsIgnoreCase(Token.USERCOLUMNS.value))
                {
                    base.buffer.Append("USER_TAB_COLUMNS");
                }
                else if (source.name.EqualsIgnoreCase(Token.TABLECOLUMNDEFAULTVALUE.value))
                {
                    base.buffer.Append("USER_TAB_COLUMNS");
                }
                else if (source.name.EqualsIgnoreCase(Token.SYSINDEXES.value))
                {
                    base.buffer.Append("USER_INDEXES");
                }
                else if (source.name.EqualsIgnoreCase(Token.INDCOLUMNS.value))
                {
                    base.buffer.Append("(SELECT USER_INDEXES.INDEX_NAME AS INDEX_NAME, USER_IND_COLUMNS.COLUMN_NAME AS COLUMN_NAME FROM USER_INDEXES INNER JOIN USER_IND_COLUMNS ON USER_INDEXES.INDEX_NAME = USER_IND_COLUMNS.INDEX_NAME)");
                }
                else if (source.name.EqualsIgnoreCase(Token.SYSCONSTRAINTS.value))
                {
                    base.buffer.Append("(SELECT TABLE_NAME, CONSTRAINT_NAME, CONSTRAINT_TYPE, NULL AS COLUMN_ID FROM USER_CONSTRAINTS)");
                }
                else if (source.name.EqualsIgnoreCase(Token.USERVIEWS.value))
                {
                    base.buffer.Append("(SELECT VIEW_NAME TABLE_NAME FROM USER_VIEWS)");
                }
                else
                {
                    base.buffer.Append(source.name);
                }
                if ((source.alias != null) && (source.alias.Length != 0))
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(source.alias.ToLower());
                }
            }
            else if (tableSource.GetType() == typeof(SqlJoinedTableSource))
            {
                SqlJoinedTableSource source2 = (SqlJoinedTableSource)tableSource;
                this.FormatTableSource(source2.left);
                base.buffer.Append(", ");
                this.FormatTableSource(source2.right);
            }
            else
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
                    base.buffer.Append(tableSource.alias.ToUpper());
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
            if (update.updateList[0].GetType() == typeof(SubQueryUpdateItem))
            {
                SubQueryUpdateItem item = (SubQueryUpdateItem)update.updateList[0];
                if ((item.subQuery != null) && (item.subQuery.getHints().Count != 0))
                {
                    this.FormatUpdateStmt_New(stmt);
                    return;
                }
            }
            this.FormatUpdateStmt_old(stmt);
        }

        private void FormatUpdateStmt_New(SqlUpdateStmt stmt)
        {
            SqlUpdate sql = stmt.update;
            if (sql.updateList[0].GetType() == typeof(SubQueryUpdateItem))
            {
                string tPrefix = "KSQL_T_";
                string vPrefix = "KSQL_V_";
                base.buffer.Append("UPDATE ");
                base.buffer.Append(Oracle10gHints.GetInstance().FormatHints(sql.getHints(), sql));
                base.buffer.Append("(");
                SubQueryUpdateItem queryItem = (SubQueryUpdateItem)sql.updateList[0];
                this.FormatSelectForUpdate(queryItem.subQuery, sql, queryItem, tPrefix, vPrefix);
                this.FormatSelectBase(queryItem.subQuery);
                base.buffer.Append(") SET ");
                ArrayList columnList = queryItem.columnList;
                for (int i = 0; i < columnList.Count; i++)
                {
                    base.buffer.Append((i > 0) ? ", " : "");
                    base.buffer.Append(string.Concat(new object[] { vPrefix, i, "=", tPrefix, i }));
                }
            }
            else
            {
                base.buffer.Append("UPDATE ");
                base.buffer.Append(Oracle10gHints.GetInstance().FormatHints(sql.getHints(), sql));
                base.buffer.Append(sql.updateTable.name);
                if (sql.updateTable.alias != null)
                {
                    base.buffer.Append(" ");
                    base.buffer.Append(sql.updateTable.alias);
                }
                base.buffer.Append(" SET ");
                bool flag = false;
                foreach (SqlUpdateItem item2 in sql.updateList)
                {
                    base.buffer.Append(flag ? ", " : "");
                    base.buffer.Append(item2.name);
                    base.buffer.Append(" = ");
                    base.FormatExpr(item2.expr);
                    flag = true;
                }
                if (sql.tableSource != null)
                {
                    throw new FormaterException("not support");
                }
                if (sql.condition != null)
                {
                    base.buffer.Append(" WHERE ");
                    base.FormatExpr(sql.condition);
                }
            }
        }

        private void FormatUpdateStmt_old(SqlUpdateStmt stmt)
        {
            SqlUpdate update = stmt.update;
            base.buffer.Append("UPDATE ");
            base.buffer.Append(this.FormatTableName(update.updateTable.name));
            if (update.updateTable.alias != null)
            {
                base.buffer.Append(" ");
                base.buffer.Append(update.updateTable.alias);
            }
            base.buffer.Append(" SET ");
            bool flag = false;
            foreach (AbstractUpdateItem item in update.updateList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                base.FormateUpdateItem(item);
                flag = true;
            }
            if (update.tableSource != null)
            {
                throw new FormaterException("not support");
            }
            bool flag2 = false;
            if (update.condition != null)
            {
                flag2 = true;
                base.buffer.Append(" WHERE ");
                base.FormatExpr(update.condition);
            }
            foreach (AbstractUpdateItem item2 in update.updateList)
            {
                if (item2.GetType() == typeof(SubQueryUpdateItem))
                {
                    if (flag2)
                    {
                        base.buffer.Append(" AND ");
                    }
                    else
                    {
                        base.buffer.Append(" WHERE ");
                        flag2 = true;
                    }
                    base.buffer.Append("EXISTS (");
                    ((SqlSelect)((SubQueryUpdateItem)item2).subQuery).selectList.Clear();
                    SqlSelectItem item3 = new SqlSelectItem
                    {
                        expr = new SqlIdentifierExpr("1")
                    };
                    ((SqlSelect)((SubQueryUpdateItem)item2).subQuery).selectList.Add(item3);
                    this.FormatSelectBase(((SubQueryUpdateItem)item2).subQuery);
                    base.buffer.Append(")");
                }
            }
        }

        protected void FormatUpdateStmtForSepecific(SqlUpdate update)
        {
            base.buffer.Append("UPDATE ");
            base.buffer.Append(update.updateTable.name);
            if (update.updateTable.alias != null)
            {
                base.buffer.Append(" ");
                base.buffer.Append(update.updateTable.alias);
            }
            base.buffer.Append(" SET ");
            bool flag = false;
            foreach (AbstractUpdateItem item in update.updateList)
            {
                if (flag)
                {
                    base.buffer.Append(", ");
                }
                base.FormateUpdateItem(item);
                flag = true;
            }
            if (update.tableSource != null)
            {
                throw new FormaterException("not support");
            }
            if (update.condition != null)
            {
                base.buffer.Append(" WHERE ");
                base.FormatExpr(update.condition);
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
                    text = ':' + text.Substring(1);
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
            this.FormatWhileStmt(null, stmt);
        }

        protected void FormatWhileStmt(SqlBlockStmt block, SqlWhileStmt stmt)
        {
            int num = 0;
            if (block != null)
            {
                Hashtable hashtable = block.extendedAttributes();
                if (hashtable.ContainsKey("LOOP_COUNT"))
                {
                    num = ((int)hashtable["LOOP_COUNT"]) + 1;
                    hashtable["LOOP_COUNT"] = num;
                }
                else
                {
                    hashtable.Add("LOOP_COUNT", num);
                }
                hashtable.Add("CURRENT_LOOP_COUNT", num);
            }
            string str = "ksql_loop_" + num;
            base.buffer.Append("while ");
            base.FormatExpr(stmt.condition);
            base.buffer.Append(" loop\n");
            base.buffer.Append("begin\n");
            for (int i = 0; i < stmt.stmtList.Count; i++)
            {
                SqlStmt stmt2 = (SqlStmt)stmt.stmtList[i];
                this.FormatStmt(block, stmt2);
                base.buffer.Append(";\n");
            }
            base.buffer.Append("end;\n");
            base.buffer.Append("<<");
            base.buffer.Append(str);
            base.buffer.Append("_end>>\nNULL;\n");
            base.buffer.Append("end loop");
            Hashtable hashtable2 = block.extendedAttributes();
            if (hashtable2.ContainsKey("LOOP_COUNT"))
            {
                num = (int)hashtable2["LOOP_COUNT"];
                hashtable2["LOOP_COUNT"] = num - 1;
            }
        }

        private static SqlExpr getRootExpr(SqlExpr expr)
        {
            if (expr.GetType() == typeof(SqlBinaryOpExpr))
            {
                SqlBinaryOpExpr expr2 = (SqlBinaryOpExpr)expr;
                if (expr2.Operator == 20)
                {
                    return getRootExpr(expr2.left);
                }
            }
            return expr;
        }

        private bool tableSourceExist(SqlTableSourceBase selectBase, string name, string alais)
        {
            if (selectBase.GetType() == typeof(SqlTableSource))
            {
                SqlTableSource source = (SqlTableSource)selectBase;
                if (alais == null)
                {
                    return ((source.alias == null) && name.EqualsIgnoreCase(source.name));
                }
                return (alais.Equals(source.alias) && name.EqualsIgnoreCase(source.name));
            }
            if (!(selectBase.GetType() == typeof(SqlJoinedTableSource)))
            {
                return false;
            }
            SqlJoinedTableSource source2 = (SqlJoinedTableSource)selectBase;
            return (this.tableSourceExist(source2.left, name, alais) || this.tableSourceExist(source2.right, name, alais));
        }

        private bool TableSourceExist(SqlSelectBase select, string name, string alais)
        {
            if (select.GetType() == typeof(SqlSelect))
            {
                SqlSelect select2 = (SqlSelect)select;
                return this.tableSourceExist(select2.tableSource, name, alais);
            }
            if (!(select.GetType() == typeof(SqlUnionSelect)))
            {
                return false;
            }
            SqlUnionSelect select3 = (SqlUnionSelect)select;
            return (this.TableSourceExist(select3.left, name, alais) || this.TableSourceExist(select3.right, name, alais));
        }
    }


  



}
