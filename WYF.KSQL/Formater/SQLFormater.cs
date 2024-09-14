using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Exception;
using WYF.KSQL.Parser;
using WYF.KSQL.Schema;
using WYF.KSQL.Util;

namespace WYF.KSQL.Formater
{
    public abstract class SQLFormater : SqlBinaryOpExprProcessor
    {
        // Fields
        protected StringBuilder buffer;
        protected Dictionary<string, string> context = new Dictionary<string, string>();
        private ArrayList funcList = new ArrayList();
        private Hashtable funcMap = new Hashtable();
        public const string HIERARCHICAL_QUERY_FLAG = "HIERARCHICAL_QUERY_FLAG";
        protected const string Identity = "Identity";
        protected const string isScriptContext = "isScriptContext";
        protected int max_length_of_column_count = -1;
        protected int max_length_of_column_name = -1;
        protected int max_length_of_constraint_name = -1;
        protected int max_length_of_index_name = -1;
        protected int max_length_of_row_size = -1;
        protected int max_length_of_table_name = -1;
        protected FormatOptions options;
        protected const string toUpperCase = "toUpperCase";

        // Methods
        public SQLFormater(StringBuilder sb)
        {
            if (sb == null)
            {
                sb = new StringBuilder();
            }
            this.buffer = sb;
            this.options = new FormatOptions();
        }

        protected void AddFunction(FunctionDef func)
        {
            this.funcList.Add(func);
            this.funcMap.Add(func.name, func);
        }

        protected void AfterFormat(StringBuilder buffer)
        {
        }

        public void AppendString(string buffer)
        {
            if (this.buffer == null)
            {
                this.buffer = new StringBuilder(buffer);
            }
            else
            {
                this.buffer.AppendLine(buffer);
            }
        }

        protected void ClearFunction()
        {
            this.funcList.Clear();
            this.funcMap.Clear();
        }

        public virtual string Format(ICollection stmtList)
        {
            this.context.Clear();
            bool flag = false;
            foreach (SqlStmt stmt in stmtList)
            {
                if (flag)
                {
                    if (!this.buffer.endsWith(';'))
                    {
                        this.buffer.Append(";");
                    }
                    this.buffer.Append("\n");
                }
                this.FormatStmt(stmt);
                flag = true;
            }
            if ((stmtList.Count > 1) && !this.buffer.ToString().EndsWith(";"))
            {
                this.buffer.Append(";");
            }
            this.AfterFormat(this.buffer);
            return this.buffer.ToString();
        }

        protected void FormatAggregateExprExpr(SqlAggregateExpr expr)
        {
            this.buffer.Append(expr.methodName);
            this.buffer.Append("(");
            if (expr.option == 0)
            {
                this.buffer.Append("DISTINCT ");
            }
            bool flag = false;
            foreach (SqlExpr expr2 in expr.paramList)
            {
                if (flag)
                {
                    this.buffer.Append(", ");
                }
                this.FormatExpr(expr2, false);
                flag = true;
            }
            this.buffer.Append(")");
            if (expr.HasOver())
            {
                this.buffer.Append(" OVER(");
                flag = false;
                if (expr.overExpr.partition.Count > 0)
                {
                    this.buffer.Append("PARTITION BY ");
                }
                foreach (SqlExpr expr3 in expr.overExpr.partition)
                {
                    if (flag)
                    {
                        this.buffer.Append(", ");
                    }
                    this.FormatExpr(expr3);
                    flag = true;
                }
                if (expr.overExpr.orderBy.Count > 0)
                {
                    this.buffer.Append(" ORDER BY ");
                }
                flag = false;
                foreach (SqlOrderByItem item in expr.overExpr.orderBy)
                {
                    if (flag)
                    {
                        this.buffer.Append(", ");
                    }
                    this.FormatExpr(item.expr);
                    if (item.mode == 0)
                    {
                        this.buffer.Append(" ASC");
                    }
                    else if (item.mode == 1)
                    {
                        this.buffer.Append(" DESC");
                    }
                    flag = true;
                }
                this.buffer.Append(")");
            }
        }

        protected void FormatAllExpr(SqlAllExpr expr)
        {
            this.buffer.Append("ALL (");
            this.FormatSelectBase(expr.subQuery);
            this.buffer.Append(")");
        }

        protected abstract void FormatAlterTableStmt(SqlAlterTableStmt stmt);
        protected void FormatAlterViewStmt(SqlAlterViewStmt stmt)
        {
            this.buffer.Append("ALTER VIEW " + this.FormatViewName(stmt.name));
            this.buffer.Append(" AS ");
            this.FormatSelectBase(stmt.select);
        }

        protected void FormatAnyExpr(SqlAnyExpr expr)
        {
            this.buffer.Append("ANY (");
            this.FormatSelectBase(expr.subQuery);
            this.buffer.Append(")");
        }

        protected void FormatBetweenExpr(SqlBetweenExpr expr)
        {
            this.buffer.Append("(");
            this.FormatExpr(expr.testExpr);
            if (expr.not)
            {
                this.buffer.Append(" NOT BETWEEN ");
            }
            else
            {
                this.buffer.Append(" BETWEEN ");
            }
            this.FormatExpr(expr.beginExpr);
            this.buffer.Append(" AND ");
            this.FormatExpr(expr.endExpr);
            this.buffer.Append(")");
        }

        protected abstract void FormatBinaryOpExpr(SqlBinaryOpExpr expr, bool appendBrace);
        protected abstract void FormatBlockStmt(SqlBlockStmt stmt);
        protected abstract void FormatBreakStmt(SqlBreakStmt stmt);
        protected void FormatCallStmt(CallStmt stmt)
        {
            this.buffer.Append('{');
            if (stmt.returnExpr != null)
            {
                this.FormatExpr(stmt.returnExpr);
                this.buffer.Append(" = CALL ");
            }
            else
            {
                this.buffer.Append("CALL ");
            }
            this.buffer.Append(stmt.procName);
            if ((stmt.paramList != null) && (stmt.paramList.Count != 0))
            {
                this.buffer.Append('(');
                for (int i = 0; i < stmt.paramList.Count; i++)
                {
                    if (i != 0)
                    {
                        this.buffer.Append(", ");
                    }
                    SqlExpr expr = (SqlExpr)stmt.paramList[i];
                    this.FormatExpr(expr);
                }
                this.buffer.Append(')');
            }
            this.buffer.Append('}');
        }

        protected virtual void FormatCaseExpr(SqlCaseExpr expr)
        {
            this.buffer.Append("CASE ");
            if (expr.valueExpr != null)
            {
                this.FormatExpr(expr.valueExpr);
            }
            foreach (SqlCaseItem item in expr.itemList)
            {
                this.buffer.Append(" WHEN ");
                this.FormatExpr(item.conditionExpr);
                this.buffer.Append(" THEN ");
                this.FormatExpr(item.valueExpr);
            }
            if (expr.elseExpr != null)
            {
                this.buffer.Append(" ELSE ");
                this.FormatExpr(expr.elseExpr);
            }
            this.buffer.Append(" END");
        }

        protected virtual void FormatChar(SqlCharExpr expr)
        {
            this.buffer.Append("'");
            this.buffer.Append(expr.text);
            this.buffer.Append("'");
        }

        protected abstract void FormatCloseStmt(SqlCloseStmt stmt);
        protected abstract void FormatColumnDef(SqlColumnDef column);
        protected string FormatColumnName(string name)
        {
            return name;
        }

        protected string FormatConstraintName(string name)
        {
            return name;
        }

        protected abstract void FormatContinueStmt(SqlContinueStmt stmt);
        protected virtual void FormatCreateIndexStmt(SqlCreateIndexStmt stmt)
        {
            this.buffer.Append("CREATE ");
            if (stmt.isUnique)
            {
                this.buffer.Append("UNIQUE ");
            }
            if (stmt.isCluster)
            {
                this.buffer.Append("CLUSTERED ");
            }
            this.buffer.Append("INDEX ");
            this.buffer.Append(this.GetIndexName(stmt));
            this.buffer.Append(" ON ");
            this.buffer.Append(this.FormatTableName(stmt.tableName));
            this.buffer.Append(" (");
            bool flag = false;
            foreach (SqlOrderByItem item in stmt.itemList)
            {
                if (flag)
                {
                    this.buffer.Append(", ");
                }
                this.FormatExpr(item.expr);
                if (item.mode != 0)
                {
                    this.buffer.Append(" DESC");
                }
                flag = true;
            }
            this.buffer.Append(")");
        }

        protected abstract void FormatCreateTableStmt(SqlCreateTableStmt stmt);
        protected virtual void FormatCreateViewStmt(SqlCreateViewStmt stmt)
        {
            this.buffer.Append("CREATE VIEW " + this.FormatViewName(stmt.name));
            if (stmt.columnList.Count != 0)
            {
                this.buffer.Append("(");
                for (int i = 0; i < stmt.columnList.Count; i++)
                {
                    if (i != 0)
                    {
                        this.buffer.Append(", ");
                    }
                    string name = (string)stmt.columnList[i];
                    this.buffer.Append(this.FormatColumnName(name));
                }
                this.buffer.Append(")");
            }
            this.buffer.Append(" AS ");
            this.FormatSelectBase(stmt.select);
        }

        protected abstract void FormatCursorLoopStmt(SqlCursorLoopStmt stmt);
        protected abstract void FormatDateTimeExpr(SqlDateTimeExpr expr);
        protected abstract void FormatDeallocateStmt(SqlDeallocateStmt stmt);
        protected virtual void FormatDefault(SqlDefaultExpr expr)
        {
            this.buffer.Append(expr.text);
        }

        protected virtual void FormatDeleteStmt(SqlDeleteStmt stmt)
        {
            SqlDelete delete = stmt.delete;
            this.buffer.Append("DELETE ");
            this.FormatHintForDelete(stmt);
            if (!string.IsNullOrEmpty(delete.tableName))
            {
                if (delete.tableSource != null)
                {
                    this.buffer.Append(delete.tableName);
                    this.buffer.Append(" FROM ");
                    this.FormatTableSource(delete.tableSource);
                }
                else
                {
                    this.buffer.Append("FROM ");
                    this.buffer.Append(this.FormatTableName(delete.tableName));
                }
            }
            else
            {
                this.buffer.Append("FROM ");
                this.FormatTableSource(delete.tableSource);
            }
            if (delete.condition != null)
            {
                this.buffer.Append(" WHERE ");
                this.FormatExpr(delete.condition);
            }
        }

        protected void FormatDropFunction(SqlDropFunctionStmt stmt)
        {
            this.buffer.Append("DROP FUNCTION ");
            if (stmt.schema != null)
            {
                this.buffer.Append(stmt.schema);
                this.buffer.Append(".");
            }
            if (stmt.functionName == null)
            {
                throw new FormaterException("function name cannot be null");
            }
            this.buffer.Append(stmt.functionName);
        }

        protected virtual void FormatDropIndexStmt(SqlDropIndexStmt stmt)
        {
            this.buffer.Append("DROP INDEX ");
            this.buffer.Append(this.GetIndexName(stmt));
        }

        protected virtual void FormatDropTableStmt(SqlDropTableStmt stmt)
        {
            this.buffer.Append("DROP TABLE ");
            this.buffer.Append(this.FormatTableName(stmt.tableName));
        }

        protected void FormatDropTriggerStmt(SqlDropTriggerStmt stmt)
        {
            this.buffer.Append("DROP TRIGGER ");
            this.buffer.Append(stmt.trggerName);
        }

        protected void FormatDropViewStmt(SqlDropViewStmt stmt)
        {
            this.buffer.Append("DROP VIEW ");
            this.buffer.Append(stmt.viewName);
        }

        protected string FormateIndexName(string indexName)
        {
            return indexName;
        }

        protected void FormatEmptyExpr(SqlEmptyExpr expr)
        {
            this.buffer.Append("EMPTY");
        }

        protected void FormateUpdateItem(AbstractUpdateItem abstract_item)
        {
            if (abstract_item.GetType() == typeof(SqlUpdateItem))
            {
                SqlUpdateItem item = (SqlUpdateItem)abstract_item;
                this.buffer.Append(item.name);
                this.buffer.Append(" = ");
                this.FormatExpr(item.expr);
            }
            else
            {
                if (!(abstract_item.GetType() == typeof(SubQueryUpdateItem)))
                {
                    throw new FormaterException("unexpect update item: '" + abstract_item + "'");
                }
                SubQueryUpdateItem item2 = (SubQueryUpdateItem)abstract_item;
                bool flag = false;
                this.buffer.Append('(');
                foreach (string str in item2.columnList)
                {
                    if (flag)
                    {
                        this.buffer.Append(", ");
                    }
                    this.buffer.Append(str);
                    flag = true;
                }
                this.buffer.Append(") = (");
                this.FormatSelectBase(item2.subQuery);
                this.buffer.Append(")");
            }
        }

        protected abstract void FormatExecStmt(SqlExecStmt stmt);
        protected void FormatExiststExpr(SqlExistsExpr expr)
        {
            if (expr.not)
            {
                this.buffer.Append("NOT EXISTS (");
            }
            else
            {
                this.buffer.Append("EXISTS (");
            }
            this.FormatSelectBase(expr.subQuery);
            this.buffer.Append(")");
        }

        public void FormatExpr(SqlExpr expr)
        {
            this.FormatExpr(expr, true);
        }

        public virtual void FormatExpr(SqlExpr expr, bool appendBrace)
        {
            if (expr == null)
            {
                throw new IllegalStateException("expr is null");
            }
            if (expr.type == 8)
            {
                this.buffer.Append("*");
            }
            else if (expr.type == 4)
            {
                this.FormatIdentifierExpr(expr);
            }
            else if (expr.type == 1)
            {
                this.buffer.Append(((SqlIntExpr)expr).text);
            }
            else if (expr.type == 0x1b)
            {
                this.buffer.Append(((SqlLongExpr)expr).text);
            }
            else if (expr.type == 2)
            {
                this.buffer.Append(((SqlDoubleExpr)expr).text);
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
                this.FormatExiststExpr((SqlExistsExpr)expr);
            }
            else if (expr.type == 13)
            {
                this.FormatInSubQueryExpr((SqlInSubQueryExpr)expr);
            }
            else if (expr.type == 0x10)
            {
                this.FormatAllExpr((SqlAllExpr)expr);
            }
            else if (expr.type == 0x11)
            {
                this.FormatBetweenExpr((SqlBetweenExpr)expr);
            }
            else if (expr.type == 0x12)
            {
                this.FormatAnyExpr((SqlAnyExpr)expr);
            }
            else if (expr.type == 0x13)
            {
                this.FormatSomeExpr((SqlSomeExpr)expr);
            }
            else if (expr.type == 20)
            {
                this.FormatNullExpr((SqlNullExpr)expr);
            }
            else if (expr.type == 0x15)
            {
                this.FormatDateTimeExpr((SqlDateTimeExpr)expr);
            }
            else if (expr.type == 0x18)
            {
                this.FormatQueryExpr((QueryExpr)expr);
            }
            else if (expr.type == 0x19)
            {
                this.FormatPriorIdentifierExpr((SqlPriorIdentifierExpr)expr);
            }
            else if (expr.type == 9)
            {
                this.FormatNotExpr((SqlNotExpr)expr);
            }
            else if (expr.type == 0x1a)
            {
                JavaObjectValueExpr expr2 = (JavaObjectValueExpr)expr;
                object obj2 = expr2.value;
                if (obj2 == null)
                {
                    this.buffer.Append("NULL");
                }
                else if (obj2.GetType() == typeof(string))
                {
                    string str = Convert.ToString(obj2).Replace("'", "''");
                    this.buffer.Append(str);
                }
                else if (obj2.GetType() == typeof(DateTime))
                {
                    DateTime time = (DateTime)obj2;
                    this.buffer.Append(string.Concat(new object[] { "{", time.Year, "-", time.Month, "-", time.Day, " ", time.Hour, ":", time.Minute, ":", time.Second, "}" }));
                }
                else
                {
                    this.buffer.Append(obj2.ToString());
                }
            }
            else if (expr.type == -1)
            {
                this.FormatDefault((SqlDefaultExpr)expr);
            }
            else if (expr.type != 0x1c)
            {
                if (expr.type != 30)
                {
                    throw new FormaterException("unexpect expression: '" + expr + "'");
                }
                this.FormatXmlType((SqlXmlExpr)expr);
            }
        }

        protected abstract void FormatFetchStmt(SqlFetchStmt stmt);
        protected abstract void FormatGotoStmt(SqlGotoStmt stmt);
        protected virtual void FormatHintForDelete(SqlDeleteStmt stmt)
        {
        }

        protected virtual void FormatIdentifierExpr(SqlExpr expr)
        {
            string str = ((SqlIdentifierExpr)expr).value;
            if (!string.IsNullOrEmpty(str) && str.StartsWith("\""))
            {
                this.buffer.Append(str.ToUpper());
            }
            else if (str.EqualsIgnoreCase("KSQL_SEQ"))
            {
                this.FormatIdentity(expr);
            }
            else
            {
                this.buffer.Append(str);
            }
        }

        protected abstract void FormatIdentity(SqlExpr stmt);
        protected abstract void FormatIfStmt(SqlIfStmt stmt);
        protected virtual void FormatInListExpr(SqlInListExpr expr)
        {
            this.FormatExpr(expr.expr);
            if (expr.not)
            {
                this.buffer.Append(" NOT IN (");
            }
            else
            {
                this.buffer.Append(" IN (");
            }
            bool flag = false;
            foreach (SqlExpr expr2 in expr.targetList)
            {
                if (flag)
                {
                    this.buffer.Append(", ");
                }
                this.FormatExpr(expr2);
                flag = true;
            }
            this.buffer.Append(")");
        }

        protected virtual void FormatInsertStmt(SqlInsertStmt stmt)
        {
            bool flag;
            this.buffer.Append("INSERT INTO ");
            SqlInsert insert = stmt.insert;
            this.buffer.Append(this.FormatTableName(insert.tableName));
            if (insert.columnList.Count != 0)
            {
                this.buffer.Append(" (");
                flag = false;
                foreach (object obj2 in insert.columnList)
                {
                    if (flag)
                    {
                        this.buffer.Append(", ");
                    }
                    if (obj2.GetType() == typeof(SqlIdentifierExpr))
                    {
                        SqlIdentifierExpr expr = (SqlIdentifierExpr)obj2;
                        this.buffer.Append(this.FormatColumnName(expr.value));
                    }
                    else
                    {
                        if (obj2.GetType() != typeof(string))
                        {
                            throw new FormaterException("unexpect expression: '" + obj2 + "'");
                        }
                        this.buffer.Append(this.FormatColumnName((string)obj2));
                    }
                    flag = true;
                }
                this.buffer.Append(")");
            }
            if (insert.valueList.Count != 0)
            {
                this.buffer.Append(" VALUES (");
                flag = false;
                foreach (SqlExpr expr2 in insert.valueList)
                {
                    if (flag)
                    {
                        this.buffer.Append(", ");
                    }
                    this.FormatExpr(expr2);
                    flag = true;
                }
                this.buffer.Append(")");
            }
            else
            {
                this.buffer.Append(" ");
                this.FormatSelectBase(stmt.insert.subQuery);
            }
        }

        protected void FormatInSubQueryExpr(SqlInSubQueryExpr expr)
        {
            this.FormatExpr(expr.expr);
            if (expr.not)
            {
                this.buffer.Append(" NOT IN (");
            }
            else
            {
                this.buffer.Append(" IN (");
            }
            this.FormatSelectBase(expr.subQuery);
            this.buffer.Append(")");
        }

        protected abstract void FormatLabelStmt(SqlLabelStmt stmt);
        protected virtual void FormatMergeStmt(SqlMergeStmt stmt)
        {
            throw new FormaterException("Not Implementation");
        }

        protected abstract void FormatMethodInvokeExpr(SqlMethodInvokeExpr expr);
        protected virtual void FormatNChar(SqlNCharExpr expr)
        {
            this.buffer.Append("N'");
            this.buffer.Append(expr.text);
            this.buffer.Append("'");
        }

        protected void FormatNotExpr(SqlNotExpr notExpr)
        {
            this.buffer.Append("NOT ");
            this.buffer.Append("(");
            this.FormatExpr(notExpr.expr);
            this.buffer.Append(")");
        }

        protected void FormatNullExpr(SqlNullExpr expr)
        {
            this.buffer.Append("NULL");
        }

        protected abstract void FormatOpenStmt(SqlOpenStmt stmt);
        protected abstract void FormatPriorIdentifierExpr(SqlPriorIdentifierExpr expr);
        protected void FormatQueryExpr(QueryExpr expr)
        {
            this.buffer.Append('(');
            this.FormatSelectBase(expr.subQuery);
            this.buffer.Append(')');
        }

        protected abstract void FormatSelect(SqlSelect select);
        public virtual void FormatSelectBase(SqlSelectBase select)
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
                    this.buffer.Append('(');
                    this.FormatSelectBase(select2.left);
                    this.buffer.Append(')');
                }
                else
                {
                    this.FormatSelectBase(select2.left);
                    select.subQueries.Add(select2.left.subQueries);
                }
                if (select2.option.Equals(SqlUnionSelect.Union))
                {
                    this.buffer.Append(" UNION ");
                }
                else
                {
                    if (select2.option != SqlUnionSelect.UnionAll)
                    {
                        throw new FormaterException("Eorr Union Option.");
                    }
                    this.buffer.Append(" UNION ALL ");
                }
                if ((select2.right.GetType() == typeof(SqlUnionSelect)) && (((SqlUnionSelect)select2.right).option != select2.option))
                {
                    this.buffer.Append('(');
                    this.FormatSelectBase(select2.right);
                    this.buffer.Append(')');
                }
                else
                {
                    this.FormatSelectBase(select2.right);
                    select.subQueries.Add(select2.right.subQueries);
                }
                if (select2.orderBy.Count != 0)
                {
                    this.buffer.Append(" ORDER BY ");
                    bool flag = false;
                    foreach (SqlOrderByItem item in select2.orderBy)
                    {
                        if (flag)
                        {
                            this.buffer.Append(", ");
                        }
                        this.FormatExpr(item.expr);
                        if (item.mode == 0)
                        {
                            this.buffer.Append(" ASC");
                        }
                        else
                        {
                            this.buffer.Append(" DESC");
                        }
                        flag = true;
                    }
                }
            }
        }

        public virtual void FormatSelectList(SqlSelect select)
        {
        }

        protected void FormatSelectStmt(SqlSelectStmt stmt)
        {
            this.FormatSelectBase(stmt.select);
        }

        protected abstract void FormatSetLocalVariantStmt(SqlSetLocalVariantStmt stmt);
        protected abstract void FormatShowColumnsStmt(SqlShowColumnsStmt stmt);
        protected abstract void FormatShowTablesStmt(SqlShowTablesStmt stmt);
        protected void FormatSomeExpr(SqlSomeExpr expr)
        {
            this.buffer.Append("SOME (");
            this.FormatSelectBase(expr.subQuery);
            this.buffer.Append(")");
        }

        public void FormatStmt(SqlStmt stmt)
        {
            if (stmt.type == 0)
            {
                this.FormatSelectStmt((SqlSelectStmt)stmt);
                IList list = new ArrayList();
                list.Add(((SqlSelectStmt)stmt).select.subQueries);
                stmt.addExtAttr("SubQuery", list);
            }
            else if (stmt.type == 2)
            {
                this.FormatInsertStmt((SqlInsertStmt)stmt);
            }
            else if (stmt.type == 1)
            {
                this.FormatDeleteStmt((SqlDeleteStmt)stmt);
            }
            else if (stmt.type == 0x31)
            {
                this.FormatMergeStmt((SqlMergeStmt)stmt);
            }
            else if (stmt.type == 3)
            {
                this.FormatUpdateStmt((SqlUpdateStmt)stmt);
            }
            else
            {
                if (stmt.type == 0x18)
                {
                    SqlCreateTableStmt stmt2 = (SqlCreateTableStmt)stmt;
                    try
                    {
                        this.FormatCreateTableStmt(stmt2);
                        return;
                    }
                    catch (FormaterException exception)
                    {
                        throw new FormaterException("formate table stmt error. table name is '" + stmt2.name + "'\n" + exception.Message);
                    }
                }
                if (stmt.type == 0x21)
                {
                    this.FormatAlterTableStmt((SqlAlterTableStmt)stmt);
                }
                else if (stmt.type == 0x2d)
                {
                    this.FormatDropViewStmt((SqlDropViewStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlDropTriggerStmt))
                {
                    this.FormatDropTriggerStmt((SqlDropTriggerStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlDropTableStmt))
                {
                    this.FormatDropTableStmt((SqlDropTableStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlCreateViewStmt))
                {
                    this.FormatCreateViewStmt((SqlCreateViewStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlDropIndexStmt))
                {
                    this.FormatDropIndexStmt((SqlDropIndexStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlCreateIndexStmt))
                {
                    this.FormatCreateIndexStmt((SqlCreateIndexStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlAlterViewStmt))
                {
                    this.FormatAlterViewStmt((SqlAlterViewStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlTrancateTableStmt))
                {
                    this.FormatTrancateTableStmt((SqlTrancateTableStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlShowTablesStmt))
                {
                    this.FormatShowTablesStmt((SqlShowTablesStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlShowColumnsStmt))
                {
                    this.FormatShowColumnsStmt((SqlShowColumnsStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlExecStmt))
                {
                    this.FormatExecStmt((SqlExecStmt)stmt);
                }
                else if (stmt.GetType() == typeof(CallStmt))
                {
                    this.FormatCallStmt((CallStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlBlockStmt))
                {
                    this.FormatBlockStmt((SqlBlockStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlIfStmt))
                {
                    this.FormatIfStmt((SqlIfStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlWhileStmt))
                {
                    this.FormatWhileStmt((SqlWhileStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlCloseStmt))
                {
                    this.FormatCloseStmt((SqlCloseStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlDeallocateStmt))
                {
                    this.FormatDeallocateStmt((SqlDeallocateStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlSetLocalVariantStmt))
                {
                    this.FormatSetLocalVariantStmt((SqlSetLocalVariantStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlOpenStmt))
                {
                    this.FormatOpenStmt((SqlOpenStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlCursorLoopStmt))
                {
                    this.FormatCursorLoopStmt((SqlCursorLoopStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlFetchStmt))
                {
                    this.FormatFetchStmt((SqlFetchStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlBreakStmt))
                {
                    this.FormatBreakStmt((SqlBreakStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlContinueStmt))
                {
                    this.FormatContinueStmt((SqlContinueStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlLabelStmt))
                {
                    this.FormatLabelStmt((SqlLabelStmt)stmt);
                }
                else if (stmt.GetType() == typeof(SqlGotoStmt))
                {
                    this.FormatGotoStmt((SqlGotoStmt)stmt);
                }
                else
                {
                    if (stmt.GetType() != typeof(SqlDropFunctionStmt))
                    {
                        throw new FormaterException("unexpect statement: '" + stmt.GetType().Name + "'");
                    }
                    this.FormatDropFunction((SqlDropFunctionStmt)stmt);
                }
            }
        }

        protected virtual string FormatTableName(string tableName)
        {
            return tableName;
        }

        protected abstract void FormatTableSource(SqlTableSourceBase tableSource);
        protected void FormatTrancateTableStmt(SqlTrancateTableStmt stmt)
        {
            this.buffer.Append("TRUNCATE TABLE ");
            this.buffer.Append(this.FormatTableName(stmt.tableName));
        }

        protected virtual void FormatUpdateStmt(SqlUpdateStmt stmt)
        {
            SqlUpdate update = stmt.update;
            this.buffer.Append("UPDATE ");
            this.buffer.Append(update.updateTable.name);
            if (update.updateTable.alias != null)
            {
                this.buffer.Append(" ");
                this.buffer.Append(update.updateTable.alias);
            }
            this.buffer.Append(" SET ");
            bool flag = false;
            foreach (AbstractUpdateItem item in update.updateList)
            {
                if (flag)
                {
                    this.buffer.Append(", ");
                }
                this.FormateUpdateItem(item);
                flag = true;
            }
            if (update.tableSource != null)
            {
                throw new FormaterException("update tableSource can't be null");
            }
            if (update.condition != null)
            {
                this.buffer.Append(" WHERE ");
                this.FormatExpr(update.condition);
            }
        }

        protected virtual void FormatVarRef(SqlVarRefExpr expr)
        {
            if (expr.text.StartsWith("@"))
            {
                if (TransUtil.OleDBDriver)
                {
                    this.buffer.Append("?");
                }
                else
                {
                    this.buffer.Append(expr.text);
                }
            }
            else
            {
                this.buffer.Append(expr.text);
            }
        }

        protected string FormatViewName(string name)
        {
            return name;
        }

        protected abstract void FormatWhileStmt(SqlWhileStmt stmt);
        protected virtual void FormatXmlType(SqlXmlExpr expr)
        {
            this.buffer.Append("XMLTYPE=N'");
            this.buffer.Append(expr.text);
            this.buffer.Append("'");
        }

        protected void FormeatUnkownMethodInvokeExpr(SqlMethodInvokeExpr expr)
        {
            throw new FormaterException("unexpect function, function name is '" + expr.methodName + "'");
        }

        protected int FunctionCount()
        {
            return this.funcList.Count;
        }

        public string GetBuffer()
        {
            return this.buffer.ToString();
        }

        protected FunctionDef GetFunction(int funcIndex)
        {
            return (FunctionDef)this.funcList[funcIndex];
        }

        protected FunctionDef GetFunction(string funcName)
        {
            return (FunctionDef)this.funcMap[funcName];
        }

        protected string GetIndexName(SqlCreateIndexStmt stmt)
        {
            return this.FormateIndexName(stmt.indexName);
        }

        protected string GetIndexName(SqlDropIndexStmt stmt)
        {
            return stmt.indexName;
        }

        public FormatOptions GetOptions()
        {
            return this.options;
        }

        protected StringBuilder HandleComma(StringBuilder buff, int pos)
        {
            string oldValue = buff.substring(pos, buff.Length);
            string newValue = oldValue.Replace("'", "''");
            return buff.Replace(oldValue, newValue, pos, buff.Length - pos);
        }

        protected bool IsToUpperCaseExpr(SqlIdentifierExpr expr)
        {
            if (((!expr.value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value) && !expr.value.EqualsIgnoreCase(Token.INDNAME.value)) && (!expr.value.EqualsIgnoreCase(Token.KSQL_COL_TABNAME.value) && !expr.value.EqualsIgnoreCase(Token.TABNAME.value))) && ((!expr.value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value) && !expr.value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value)) && !expr.value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value)))
            {
                return false;
            }
            return true;
        }

        public void process(SqlBinaryOpExpr expr, int leftBracket, int rightBracket, Hashtable options)
        {
            bool appendBrace = true;
            if ((options != null) && (options["appendBrace"] != null))
            {
                appendBrace = (bool)options["appendBrace"];
            }
            for (int i = 0; i < leftBracket; i++)
            {
                this.buffer.Append("(");
            }
            if (expr.left.GetType() == typeof(SqlBinaryOpExpr))
            {
                SqlBinaryOpExpr left = (SqlBinaryOpExpr)expr.left;
                if ((left.Operator != 8) && (left.Operator != 7))
                {
                    this.FormatBinaryOpExpr(left, appendBrace);
                }
            }
            else
            {
                this.FormatExpr(expr.left, appendBrace);
            }
            if (expr.Operator == 8)
            {
                this.buffer.Append(" OR ");
            }
            else if (expr.Operator == 7)
            {
                this.buffer.Append(" AND ");
            }
            if (expr.right.GetType() == typeof(SqlBinaryOpExpr))
            {
                SqlBinaryOpExpr right = (SqlBinaryOpExpr)expr.right;
                if ((right.Operator != 8) && (right.Operator != 7))
                {
                    this.FormatBinaryOpExpr(right, appendBrace);
                }
            }
            else
            {
                this.FormatExpr(expr.right, appendBrace);
            }
            for (int j = 0; j < rightBracket; j++)
            {
                this.buffer.Append(")");
            }
        }

        protected void RemoveFunctionAt(int index)
        {
            FunctionDef def = (FunctionDef)this.funcList[index];
            this.funcList.Remove(index);
            this.funcMap.Remove(def.name);
        }

        protected void RemoveParameter(FunctionDef func)
        {
            this.funcList.Remove(func);
            this.funcMap.Remove(func.name);
        }

        public void ReplaceBinaryOpExpr(SqlBinaryOpExpr expr, Hashtable replaceMap)
        {
            if (expr.Operator == 13)
            {
                this.ReplaceExpr(expr.left, replaceMap);
            }
            else if (expr.Operator == 0x29)
            {
                this.ReplaceExpr(expr.left, replaceMap);
            }
            else if (expr.Operator == 20)
            {
                this.ReplaceExpr(expr.left, replaceMap);
                this.ReplaceExpr(expr.right, replaceMap);
            }
            else if (expr.Operator == 0x2b)
            {
                this.ReplaceExpr(expr.left, replaceMap);
                this.ReplaceExpr(expr.right, replaceMap);
            }
            else if (expr.Operator == 10)
            {
                this.ReplaceExpr(expr.left, replaceMap);
                this.ReplaceExpr(expr.right, replaceMap);
            }
            else if (expr.Operator == 0)
            {
                if (expr.left.GetType() == typeof(SqlBinaryOpExpr))
                {
                    SqlBinaryOpExpr left = (SqlBinaryOpExpr)expr.left;
                    if (left.Operator == 0)
                    {
                        this.ReplaceExpr(left, replaceMap);
                    }
                    else
                    {
                        this.ReplaceExpr(left, replaceMap);
                    }
                }
                else
                {
                    this.ReplaceExpr(expr.left, replaceMap);
                }
                if (expr.right.GetType() == typeof(SqlBinaryOpExpr))
                {
                    SqlBinaryOpExpr right = (SqlBinaryOpExpr)expr.right;
                    if (right.Operator == 0)
                    {
                        this.ReplaceExpr(right, replaceMap);
                    }
                    else
                    {
                        this.ReplaceExpr(right, replaceMap);
                    }
                }
                else
                {
                    this.ReplaceExpr(expr.right, replaceMap);
                }
            }
            else if (expr.Operator == 8)
            {
                if (expr.left.GetType() == typeof(SqlBinaryOpExpr))
                {
                    SqlBinaryOpExpr expr4 = (SqlBinaryOpExpr)expr.left;
                    if (expr4.Operator == 8)
                    {
                        this.ReplaceExpr(expr4, replaceMap);
                    }
                    else
                    {
                        this.ReplaceExpr(expr4, replaceMap);
                    }
                }
                else
                {
                    this.ReplaceExpr(expr.left, replaceMap);
                }
                if (expr.right.GetType() == typeof(SqlBinaryOpExpr))
                {
                    SqlBinaryOpExpr expr5 = (SqlBinaryOpExpr)expr.right;
                    if (expr5.Operator == 8)
                    {
                        this.ReplaceExpr(expr5, replaceMap);
                    }
                    else
                    {
                        this.ReplaceExpr(expr5, replaceMap);
                    }
                }
                else
                {
                    this.ReplaceExpr(expr.right, replaceMap);
                }
            }
            else if (expr.Operator == 1)
            {
                this.ReplaceExpr(expr.left, replaceMap);
                if (((expr.right.GetType() != typeof(SqlIdentifierExpr)) && (expr.right.GetType() != typeof(SqlCharExpr))) && (expr.right.GetType() != typeof(SqlNCharExpr)))
                {
                    this.ReplaceExpr(expr.right, replaceMap);
                }
            }
            else
            {
                this.ReplaceExpr(expr.left, replaceMap);
                this.ReplaceExpr(expr.right, replaceMap);
            }
        }

        protected void ReplaceCaseExpr(SqlCaseExpr expr, Hashtable replaceMap)
        {
            if (expr.valueExpr != null)
            {
                this.ReplaceExpr(expr.valueExpr, replaceMap);
            }
            foreach (SqlCaseItem item in expr.itemList)
            {
                this.ReplaceExpr(item.conditionExpr, replaceMap);
                this.ReplaceExpr(item.valueExpr, replaceMap);
            }
            if (expr.elseExpr != null)
            {
                this.ReplaceExpr(expr.elseExpr, replaceMap);
            }
        }

        protected void ReplaceExpr(SqlExpr expr, Hashtable replaceMap)
        {
            if (expr == null)
            {
                throw new ArgumentException("expr is null");
            }
            if (expr.GetType() != typeof(SqlAllColumnExpr))
            {
                if (expr.GetType() == typeof(SqlIdentifierExpr))
                {
                    string str = ((SqlIdentifierExpr)expr).value;
                    if (replaceMap[str.ToUpper()] != null)
                    {
                        ((SqlIdentifierExpr)expr).value = (string)replaceMap[str.ToUpper()];
                    }
                }
                else if ((expr.GetType() != typeof(SqlIntExpr)) && (expr.GetType() != typeof(SqlDoubleExpr)))
                {
                    if (expr.GetType() == typeof(SqlBinaryOpExpr))
                    {
                        this.ReplaceBinaryOpExpr((SqlBinaryOpExpr)expr, replaceMap);
                    }
                    else if (expr.GetType() == typeof(SqlMethodInvokeExpr))
                    {
                        this.ReplaceMethodInvokeExpr((SqlMethodInvokeExpr)expr, replaceMap);
                    }
                    else if ((((expr.GetType() != typeof(SqlAggregateExpr)) && (expr.GetType() != typeof(SqlCharExpr))) && (expr.GetType() != typeof(SqlNCharExpr))) && (expr.GetType() != typeof(SqlVarRefExpr)))
                    {
                        if (expr.GetType() == typeof(SqlCaseExpr))
                        {
                            this.ReplaceCaseExpr((SqlCaseExpr)expr, replaceMap);
                        }
                        else if (((((((((((expr.GetType() != typeof(SqlInListExpr)) && (expr.GetType() != typeof(SqlExistsExpr))) && (expr.GetType() != typeof(SqlInSubQueryExpr))) && (expr.GetType() != typeof(SqlAllExpr))) && (expr.GetType() != typeof(SqlBetweenExpr))) && (expr.GetType() != typeof(SqlAnyExpr))) && (expr.GetType() != typeof(SqlSomeExpr))) && (expr.GetType() != typeof(SqlNullExpr))) && (expr.GetType() != typeof(SqlDateTimeExpr))) && (expr.GetType() != typeof(QueryExpr))) && (expr.GetType() != typeof(SqlPriorIdentifierExpr)))
                        {
                            throw new FormaterException("unexpect expression: '" + expr + "'");
                        }
                    }
                }
            }
        }

        private void ReplaceMethodInvokeExpr(SqlMethodInvokeExpr expr, Hashtable replaceMap)
        {
            for (int i = 0; i < expr.parameters.Count; i++)
            {
                this.ReplaceExpr((SqlExpr)expr.parameters[i], replaceMap);
            }
        }

        public void SetBuffer(string buffer)
        {
            this.buffer = new StringBuilder(buffer);
        }

        public void SetOptions(FormatOptions options)
        {
            this.options = options;
        }

        protected void ValidateCreateTableStmt(SqlCreateTableStmt stmt)
        {
            if (stmt.name == null)
            {
                throw new FormaterException("table name cannot be null.");
            }
            if ((this.max_length_of_table_name != -1) && (stmt.name.Length > this.max_length_of_table_name))
            {
                throw new FormaterException(string.Concat(new object[] { "table name greate than ", this.max_length_of_table_name, ", table name is '", stmt.name, "'" }));
            }
        }

        protected void ValidConstraintName(string constriantName)
        {
            if (((constriantName != null) && (constriantName.Length != 0)) && ((this.max_length_of_constraint_name != -1) && (constriantName.Length > this.max_length_of_constraint_name)))
            {
                throw new FormaterException(string.Concat(new object[] { "constraint name greate than ", this.max_length_of_constraint_name, ", constraint name is '", constriantName, "'" }));
            }
        }
    }






}
