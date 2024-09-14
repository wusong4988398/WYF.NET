using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Parser;

namespace WYF.KSQL.Formater
{
    public class Oracle9SQLFormater : OracleSQLFormater
    {
        // Methods
        public Oracle9SQLFormater() : base(null)
        {
        }

        public Oracle9SQLFormater(StringBuilder sb) : base(sb)
        {
        }

        protected override void FormatColumnDef(SqlColumnDef column)
        {
            base.buffer.Append(column.name);
            base.buffer.Append(" ");
            if (column.IsComputedColumn)
            {
                base.buffer.Append("AS (");
                base.FormatExpr(column.ComputedColumnExpr);
                base.buffer.Append(")");
            }
            else
            {
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
                else if (column.dataType.EqualsIgnoreCase("NUMBER") || column.dataType.EqualsIgnoreCase("DECIMAL"))
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
                else if ((column.dataType.EqualsIgnoreCase("INT") && !column.autoIncrement) || (column.dataType.EqualsIgnoreCase("INTEGER") && !column.autoIncrement))
                {
                    base.buffer.Append("NUMBER (10)");
                }
                else if (column.dataType.EqualsIgnoreCase("SMALLINT"))
                {
                    base.buffer.Append("NUMBER (10)");
                }
                else if (column.dataType.EqualsIgnoreCase("BIGINT"))
                {
                    base.buffer.Append("NUMBER (19)");
                }
                else if (column.dataType.EqualsIgnoreCase("DATETIME") || column.dataType.EqualsIgnoreCase("DATE"))
                {
                    base.buffer.Append("DATE");
                }
                else if (column.dataType.EqualsIgnoreCase("LONG"))
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
        }

        protected override void FormatInsertStmt(SqlInsertStmt stmt)
        {
            bool flag2;
            bool flag = false;
            SqlInsert sql = stmt.insert;
            base.buffer.Append("INSERT ");
            if ((sql.getHints() != null) && (sql.getHints().Count != 0))
            {
                Oracle9iHints.GetInstance().FormatHints(sql.getHints(), sql, base.buffer);
                base.buffer.Append("INTO ");
            }
            else
            {
                base.buffer.Append("INTO ");
            }
            base.buffer.Append(this.FormatTableName(sql.tableName));
            if (sql.columnList.Count != 0)
            {
                base.buffer.Append(" (");
                flag2 = false;
                foreach (object obj2 in sql.columnList)
                {
                    if (flag2)
                    {
                        base.buffer.Append(", ");
                    }
                    if (obj2.GetType() == typeof(SqlIdentifierExpr))
                    {
                        SqlIdentifierExpr expr = (SqlIdentifierExpr)obj2;
                        if (expr.value.EqualsIgnoreCase("KSQL_SEQ"))
                        {
                            flag = true;
                        }
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
                    flag2 = true;
                }
                base.buffer.Append(")");
            }
            if (sql.valueList.Count != 0)
            {
                base.buffer.Append(" VALUES (");
                flag2 = false;
                foreach (SqlExpr expr2 in sql.valueList)
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
                if (flag)
                {
                    base.buffer.Append("SELECT ROWNUM,KSQL_V1.* FROM (");
                }
                this.FormatSelectBase(stmt.insert.subQuery);
                if (flag)
                {
                    base.buffer.Append(") KSQL_V1");
                }
            }
        }

        protected override void FormatNChar(SqlNCharExpr expr)
        {
            base.buffer.Append("N'");
            base.buffer.Append(expr.text);
            base.buffer.Append("'");
        }

        protected override void FormatSelect(SqlSelect select)
        {
            if (select.into != null)
            {
                base.buffer.Append("CREATE TABLE ");
                base.buffer.Append(this.FormatTableName(select.into.new_table));
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
            Oracle9iHints.GetInstance().FormatHints(select.getHints(), select, base.buffer);
            if (select.distinct == 1)
            {
                base.buffer.Append("DISTINCT ");
            }
            else if (select.distinct != 0)
            {
                throw new FormaterException("distinct option not support.");
            }
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
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value) && (item.alias == null))
                    {
                        item.alias = Token.KSQL_COL_DEFAULT.value;
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
                    base.buffer.Append(item.alias.ToUpper());
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
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.TABNAME.value))
                    {
                        SqlIdentifierExpr expr12 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr12, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.INDNAME.value))
                    {
                        SqlIdentifierExpr expr13 = new SqlIdentifierExpr
                        {
                            value = "INDEX_NAME"
                        };
                        this.FormatExpr(expr13, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))
                    {
                        SqlIdentifierExpr expr14 = new SqlIdentifierExpr
                        {
                            value = "CONSTRAINT_NAME"
                        };
                        this.FormatExpr(expr14, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TABNAME.value))
                    {
                        SqlIdentifierExpr expr15 = new SqlIdentifierExpr
                        {
                            value = "TABLE_NAME"
                        };
                        this.FormatExpr(expr15, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_CONS_TYPE.value))
                    {
                        SqlIdentifierExpr expr16 = new SqlIdentifierExpr
                        {
                            value = "CONSTRAINT_TYPE"
                        };
                        this.FormatExpr(expr16, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value))
                    {
                        SqlIdentifierExpr expr17 = new SqlIdentifierExpr
                        {
                            value = "DATA_DEFAULT"
                        };
                        this.FormatExpr(expr17, false);
                    }
                    else if (((SqlIdentifierExpr)item.expr).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value))
                    {
                        SqlIdentifierExpr expr18 = new SqlIdentifierExpr
                        {
                            value = "NULLABLE"
                        };
                        this.FormatExpr(expr18, false);
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
            if (select.tableSource != null)
            {
                base.buffer.Append(" FROM ");
                this.FormatTableSource(select.tableSource);
            }
            else
            {
                base.buffer.Append(" FROM DUAL");
            }
            bool flag2 = false;
            if (select.condition != null)
            {
                base.buffer.Append(" WHERE ");
                base.FormatExpr(select.condition);
                flag2 = true;
            }
            if ((select.limit != null) && (select.orderBy.Count == 0))
            {
                if (!flag2)
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
                flag2 = true;
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
                foreach (SqlExpr expr19 in select.groupBy)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    base.FormatExpr(expr19);
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
                foreach (SqlOrderByItem item2 in select.orderBy)
                {
                    if (flag)
                    {
                        base.buffer.Append(", ");
                    }
                    if (item2.chineseOrderByMode != -1)
                    {
                        base.buffer.Append("NLSSORT(");
                        base.FormatExpr(item2.expr);
                        base.buffer.Append(",'NLS_SORT=");
                        if (item2.chineseOrderByMode == 2)
                        {
                            base.buffer.Append("SCHINESE_PINYIN_M')");
                        }
                        else if (item2.chineseOrderByMode == 3)
                        {
                            base.buffer.Append("SCHINESE_STROKE_M')");
                        }
                        else if (item2.chineseOrderByMode == 4)
                        {
                            base.buffer.Append("SCHINESE_RADICAL_M')");
                        }
                    }
                    else
                    {
                        base.FormatExpr(item2.expr);
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
        }

        protected override void FormatTableSource(SqlTableSourceBase tableSource)
        {
            if (tableSource != null)
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
                        base.buffer.Append(this.FormatTableName(source.name));
                    }
                    if ((source.alias != null) && (source.alias.Length != 0))
                    {
                        base.buffer.Append(" ");
                        base.buffer.Append(source.alias.ToUpper());
                    }
                    if (((source.lockingHint != null) && (source.lockingHint.Length > 0)) && "holdlock".EqualsIgnoreCase(source.lockingHint))
                    {
                        base.buffer.Append(" FOR UPDATE");
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
                            base.buffer.Append(tableSource.alias.ToUpper());
                        }
                    }
                    else
                    {
                        if (!(tableSource.GetType() == typeof(SqlTableFunctionTableSource)))
                        {
                            throw new FormaterException("not support tableSource:" + tableSource);
                        }
                        SqlTableFunctionTableSource source4 = (SqlTableFunctionTableSource)tableSource;
                        string str = source4.PipeFunction.Replace("@", ":").Replace(" ", "");
                        if (str.Contains(",1"))
                        {
                            str = str.Replace("fn_StrSplit", "udt_inttable").Replace(",',',1", "");
                        }
                        if (str.Contains(",2"))
                        {
                            str = str.Replace("fn_StrSplit", "udt_varchartable").Replace(",',',2", "");
                        }
                        if (str.Contains(",3"))
                        {
                            str = str.Replace("fn_StrSplit", "udt_nvarchartable").Replace(",',',3", "");
                        }
                        string str2 = str.Substring(str.IndexOf(":"), str.IndexOf(")") - str.IndexOf(":"));
                        string newValue = ":" + str2.substring(1, str2.Length) + "_udt";
                        base.buffer.Append("( select /*+ dynamic_sampling(2)*/ COLUMN_VALUE as FID from " + str.Replace(str2, newValue) + ")");
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
    }


  



}
