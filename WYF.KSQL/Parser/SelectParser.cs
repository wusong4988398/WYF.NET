using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public class SelectParser : SqlParserBase
    {
        // Fields
        private TokenList _tokenList;
        private SqlExprParser exprParser;

        // Methods
        public SelectParser(TokenList tokenList)
        {
            this._tokenList = tokenList;
            this.exprParser = new SqlExprParser(this._tokenList);
        }

        public SelectParser(string sql)
        {
            Lexer l = new Lexer(sql);
            this._tokenList = new TokenList(l);
            this.exprParser = new SqlExprParser(this._tokenList);
        }

        private string As(string[] sb)
        {
            string str = null;
            if (this._tokenList.lookup(0).Equals(Token.AsToken))
            {
                sb[0] = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                if (this._tokenList.lookup(0).type == 6)
                {
                    str = "'" + this._tokenList.lookup(0).value + "'";
                    sb[1] = str;
                    this._tokenList.match();
                    return str;
                }
                if (this._tokenList.lookup(0).type != 1)
                {
                    throw new ParserException("Error", 0, 0);
                }
                str = this._tokenList.lookup(0).value;
                sb[1] = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                return str;
            }
            sb[0] = "";
            if (this._tokenList.lookup(0).type == 6)
            {
                str = "'" + this._tokenList.lookup(0).value + "'";
                sb[1] = str;
                this._tokenList.match();
                return str;
            }
            if (this._tokenList.lookup(0).type == 1)
            {
                str = this._tokenList.lookup(0).value;
                sb[1] = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
            }
            return str;
        }

        private static void checkOuterJoinCondition(SqlExpr expr)
        {
        }

        private void from(SqlSelect select)
        {
            if (this._tokenList.lookup(0).Equals(Token.FromToken))
            {
                select.setFromWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                select.tableSource = this.tableSource();
            }
        }

        private void groupBy(SqlSelect select)
        {
            if (this._tokenList.lookup(0).Equals(Token.GroupToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.ByToken);
                select.setGroupByWord(orgValue);
                this.exprParser.exprList(select.groupBy);
                this.withRollUp(select);
                this.having(select);
            }
        }

        private void having(SqlSelect select)
        {
            if (this._tokenList.lookup(0).Equals(Token.HavingToken))
            {
                select.setHavingWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                select.having = this.exprParser.expr();
            }
        }

        private void hierarchicalQueryClause(SqlSelect select)
        {
            HierarchicalQueryClause clause = null;
            string startWithWord = "";
            if (this._tokenList.lookup(0).Equals(Token.StartToken))
            {
                startWithWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                startWithWord = startWithWord + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.WithToken);
                clause = new HierarchicalQueryClause();
                clause.setStartWithWord(startWithWord);
                clause.startWithCondition = this.exprParser.expr();
            }
            if (this._tokenList.lookup(0).Equals(Token.ConnectToken))
            {
                startWithWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                startWithWord = startWithWord + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.ByToken);
                if (clause == null)
                {
                    clause = new HierarchicalQueryClause();
                }
                clause.setConnectByWord(startWithWord);
                clause.connectByCondition = this.exprParser.expr();
            }
            select.hierarchicalQueryClause = clause;
        }

        private void into(SqlSelect select)
        {
            if (this._tokenList.lookup(0).Equals(Token.IntoToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                if (this._tokenList.lookup(0).type != 1)
                {
                    throw new ParserException("Error. token:" + this._tokenList.lookup(0).value, this._tokenList.lookup(0).beginLine, this._tokenList.lookup(0).beginColumn);
                }
                SqlSelectInto into = new SqlSelectInto();
                into.setIntoWord(orgValue);
                into.new_table = this._tokenList.lookup(0).value;
                into.setNewTableOrgName(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                select.into = into;
            }
        }

        private void orderBy(SqlSelectBase select)
        {
            if (this._tokenList.lookup(0).Equals(Token.OrderToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                select.setOrderByWord(orgValue);
                do
                {
                    SqlOrderByItem item;
                    this._tokenList.match();
                    string orgChineseOrderByType = null;
                    int chineseOrderByMode = -1;
                    SqlExpr expr = this.exprParser.expr();
                    if (this._tokenList.lookup(0).Equals(Token.PinYinToken))
                    {
                        orgChineseOrderByType = this._tokenList.lookup(0).GetOrgValue();
                        chineseOrderByMode = 2;
                        this._tokenList.match();
                    }
                    else if (this._tokenList.lookup(0).Equals(Token.StrokeToken))
                    {
                        orgChineseOrderByType = this._tokenList.lookup(0).GetOrgValue();
                        chineseOrderByMode = 3;
                        this._tokenList.match();
                    }
                    else if (this._tokenList.lookup(0).Equals(Token.RadicalToken))
                    {
                        orgChineseOrderByType = this._tokenList.lookup(0).GetOrgValue();
                        chineseOrderByMode = 4;
                        this._tokenList.match();
                    }
                    else
                    {
                        chineseOrderByMode = -1;
                    }
                    if (this._tokenList.lookup(0).Equals(Token.AscToken))
                    {
                        item = new SqlOrderByItem(expr, 0, chineseOrderByMode);
                        item.setOrgOrderByName(this._tokenList.lookup(0).GetOrgValue());
                        item.setOrgChineseOrderByType(orgChineseOrderByType);
                        this._tokenList.match();
                    }
                    else if (this._tokenList.lookup(0).Equals(Token.DescToken))
                    {
                        item = new SqlOrderByItem(expr, 1, chineseOrderByMode);
                        item.setOrgOrderByName(this._tokenList.lookup(0).GetOrgValue());
                        item.setOrgChineseOrderByType(orgChineseOrderByType);
                        this._tokenList.match();
                    }
                    else
                    {
                        item = new SqlOrderByItem(expr, 0, chineseOrderByMode);
                        item.setOrgOrderByName("");
                        item.setOrgChineseOrderByType(orgChineseOrderByType);
                    }
                    select.orderBy.Add(item);
                }
                while (this._tokenList.lookup(0).Equals(Token.CommaToken));
            }
        }

        public SqlSelectBase select()
        {
            return this.select(true);
        }

        public SqlSelectBase select(bool parseOrderBy)
        {
            SqlSelectBase base2;
            if (this._tokenList.lookup(0).Equals(Token.SelectToken))
            {
                SqlSelect select = new SqlSelect();
                select.setSelectWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(Token.SelectToken);
                this.selectHint(select);
                this.selectListOption(select);
                this.top(select);
                this.selectList(select.selectList);
                this.into(select);
                this.from(select);
                this.where(select);
                this.hierarchicalQueryClause(select);
                this.groupBy(select);
                base2 = this.selectRest(select);
            }
            else
            {
                if (!this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    throw new ParserException("not support token:" + this._tokenList.lookup(0));
                }
                this._tokenList.match();
                SqlSelectBase base3 = this.select();
                this._tokenList.match(Token.CloseBraceToken);
                if (parseOrderBy)
                {
                    this.orderBy(base3);
                }
                base2 = this.selectRest(base3);
            }
            if (parseOrderBy)
            {
                this.orderBy(base2);
            }
            if (!this._tokenList.lookup(0).Equals(Token.OptionToken))
            {
                return base2;
            }
            string str = null;
            string str2 = this._tokenList.lookup(0).value;
            this._tokenList.match();
            Hashtable hashtable = base2.getOptionMap();
            ArrayList list = base2.getOptionMapOrgWord();
            bool flag = false;
            Label_0155:
            str = this._tokenList.lookup(0).value;
            if (str.EqualsIgnoreCase(","))
            {
                this._tokenList.match();
            }
            if ("CONCAT".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match(3);
                string str3 = this._tokenList.lookup(0).value;
                this._tokenList.match(Token.UnionToken);
                hashtable.Add("CONCAT UNION", null);
                list.Add(str2 + " " + str + " " + str3);
                goto Label_0155;
            }
            if ("EXPAND".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str4 = this._tokenList.lookup(0).value;
                this._tokenList.match(8, "VIEWS", false);
                hashtable.Add("EXPAND VIEWS", null);
                list.Add(str2 + " " + str + " " + str4);
                goto Label_0155;
            }
            if ("FAST".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string s = this._tokenList.lookup(0).value;
                this._tokenList.match(8);
                int num = int.Parse(s);
                hashtable.Add("FAST", num);
                list.Add(str2 + " " + str);
                goto Label_0155;
            }
            if ("FASTFIRSTROW".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                hashtable.Add("FASTFIRSTROW", null);
                list.Add(str2 + " " + str);
                goto Label_0155;
            }
            if ("FOR".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str6 = this._tokenList.lookup(0).value;
                this._tokenList.match(Token.UpdateToken);
                hashtable.Add("FORCE UPDATE", null);
                list.Add(str2 + " " + str + " " + str6);
                goto Label_0155;
            }
            if ("FORCE".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str7 = this._tokenList.lookup(0).value;
                this._tokenList.match(Token.OrderToken);
                hashtable.Add("FORCE ORDER", null);
                list.Add(str2 + " " + str + " " + str7);
                goto Label_0155;
            }
            if ("HASH".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str8 = this._tokenList.lookup(0).value;
                if (this._tokenList.lookup(0).Equals(Token.GroupToken))
                {
                    this._tokenList.match();
                    hashtable.Add("HASH GROUP", null);
                }
                else if (this._tokenList.lookup(0).Equals(Token.UnionToken))
                {
                    this._tokenList.match();
                    hashtable.Add("HASH UNION", null);
                }
                else
                {
                    if (!this._tokenList.lookup(0).Equals(Token.JoinToken))
                    {
                        throw new ParserException("not support token:" + this._tokenList.lookup(0));
                    }
                    this._tokenList.match();
                    hashtable.Add("HASH JOIN", null);
                }
                list.Add(str2 + " " + str + " " + str8);
                goto Label_0155;
            }
            if ("KEEP".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str9 = this._tokenList.lookup(0).value;
                this._tokenList.match(Token.PlanToken);
                hashtable.Add("KEEP PLAN", null);
                list.Add(str2 + " " + str + " " + str9);
                goto Label_0155;
            }
            if ("KEEPFIXED".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str10 = this._tokenList.lookup(0).value;
                this._tokenList.match(Token.PlanToken);
                hashtable.Add("KEEPFIXED PLAN", null);
                list.Add(str2 + " " + str + " " + str10);
                goto Label_0155;
            }
            if ("MAXDOP".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str11 = this._tokenList.lookup(0).value;
                this._tokenList.match(8);
                int num2 = int.Parse(str11);
                hashtable.Add("MAXDOP", num2);
                list.Add(str2 + " " + str);
                goto Label_0155;
            }
            if ("MERGE".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str12 = this._tokenList.lookup(0).value;
                if (this._tokenList.lookup(0).Equals(Token.UnionToken))
                {
                    this._tokenList.match();
                    hashtable.Add("MERGE UNION", null);
                }
                else
                {
                    if (!this._tokenList.lookup(0).Equals(Token.JoinToken))
                    {
                        throw new ParserException("not support token:" + this._tokenList.lookup(0));
                    }
                    this._tokenList.match();
                    hashtable.Add("MERGE JOIN", null);
                }
                list.Add(str2 + " " + str + " " + str12);
                goto Label_0155;
            }
            if ("ORDER".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match(3);
                string str13 = this._tokenList.lookup(0).value;
                this._tokenList.match(Token.GroupToken);
                hashtable.Add("ORDER GROUP", null);
                list.Add(str2 + " " + str + " " + str13);
                goto Label_0155;
            }
            if ("LOOP".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str14 = this._tokenList.lookup(0).value;
                this._tokenList.match(Token.JoinToken);
                hashtable.Add("LOOP JOIN", null);
                list.Add(str2 + " " + str + " " + str14);
                goto Label_0155;
            }
            if ("ROBUST".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                string str15 = this._tokenList.lookup(0).value;
                this._tokenList.match(Token.PlanToken);
                hashtable.Add("ROBUST PLAN", null);
                list.Add(str2 + " " + str + " " + str15);
                goto Label_0155;
            }
            if ("ROWLOCK".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                hashtable.Add("ROWLOCK", null);
                list.Add(str2 + " " + str);
                goto Label_0155;
            }
            if ("TABLOCK".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                hashtable.Add("TABLOCK", null);
                list.Add(str2 + " " + str);
                goto Label_0155;
            }
            if ("UPDLOCK".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                hashtable.Add("UPDLOCK", null);
                list.Add(str2 + " " + str);
                goto Label_0155;
            }
            if ("XLOCK".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                hashtable.Add("XLOCK", null);
                list.Add(str2 + " " + str);
                goto Label_0155;
            }
            if ("RECOMPILE".EqualsIgnoreCase(str))
            {
                flag = true;
                this._tokenList.match();
                hashtable.Add("RECOMPILE", null);
                list.Add(str2 + " " + str);
                goto Label_0155;
            }
            if ("(".EqualsIgnoreCase(str))
            {
                this._tokenList.match();
                goto Label_0155;
            }
            if (!")".EqualsIgnoreCase(str))
            {
                goto Label_0155;
            }
            this._tokenList.match();
            if (!flag)
            {
                throw new ParserException("Syntax Error. Option hints ... , current token is : " + this._tokenList.lookup(0).toString());
            }
            return base2;
        }

        private void selectHint(SqlSelect select)
        {
            if (this._tokenList.lookup(0).type == 0x10)
            {
                string hints = this._tokenList.lookup(0).value;
                select.getHints().AddRange(HintsParser.parse(hints));
                this._tokenList.match();
            }
        }

        private void selectList(IList selectItemCol)
        {
            SqlExpr right = this.exprParser.expr();
            string alias = null;
            bool mark = false;
            if (right is SqlBinaryOpExpr)
            {
                SqlBinaryOpExpr expr2 = (SqlBinaryOpExpr)right;
                if (((expr2.Operator == 2) || (expr2.Operator == 10)) && (expr2.left is SqlIdentifierExpr))
                {
                    right = expr2.right;
                    alias = ((SqlIdentifierExpr)expr2.left).value;
                    mark = true;
                }
            }
            string[] sb = new string[2];
            if (alias == null)
            {
                alias = this.As(sb);
            }
            else
            {
                sb[0] = "";
                sb[1] = alias;
            }
            selectItemCol.Add(new SqlSelectItem(right, alias, sb[1], sb[0], mark));
            while (this._tokenList.lookup(0).Equals(Token.CommaToken))
            {
                this._tokenList.match();
                alias = null;
                right = this.exprParser.expr();
                mark = false;
                if (right is SqlBinaryOpExpr)
                {
                    SqlBinaryOpExpr expr3 = (SqlBinaryOpExpr)right;
                    if (((expr3.Operator == 2) || (expr3.Operator == 10)) && (expr3.left is SqlIdentifierExpr))
                    {
                        right = expr3.right;
                        alias = ((SqlIdentifierExpr)expr3.left).value;
                        mark = true;
                    }
                }
                sb = new string[2];
                if (alias == null)
                {
                    alias = this.As(sb);
                }
                else
                {
                    sb[0] = "";
                    sb[1] = alias;
                }
                selectItemCol.Add(new SqlSelectItem(right, alias, sb[1], sb[0], mark));
            }
        }

        private void selectListOption(SqlSelect select)
        {
            if (this._tokenList.lookup(0).Equals(Token.DistinctToken))
            {
                select.setDistinctWord(this._tokenList.lookup(0).GetOrgValue());
                select.distinct = 1;
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).Equals(Token.AllToken))
            {
                select.setDistinctWord(this._tokenList.lookup(0).GetOrgValue());
                select.distinct = 0;
                this._tokenList.match();
            }
        }

        private SqlSelectBase selectRest(SqlSelectBase select)
        {
            if (this._tokenList.lookup(0).Equals(Token.UnionToken))
            {
                this._tokenList.match();
                int union = SqlUnionSelect.Union;
                if (this._tokenList.lookup(0).Equals(Token.AllToken))
                {
                    this._tokenList.match();
                    union = SqlUnionSelect.UnionAll;
                }
                SqlSelectBase right = this.select(false);
                select = new SqlUnionSelect(select, right, union);
                select = this.selectRest(select);
            }
            return select;
        }

        private void TableFunctionParser(StringBuilder tableFunc, int openBraceCount)
        {
            this._tokenList.match();
            if ((this._tokenList.lookup(0).type == 5) && "(".Equals(this._tokenList.lookup(0).value))
            {
                openBraceCount++;
                tableFunc.Append(this._tokenList.lookup(0).value);
            }
            else if ((this._tokenList.lookup(0).type == 5) && ")".Equals(this._tokenList.lookup(0).value))
            {
                openBraceCount--;
                if (openBraceCount == 0)
                {
                    return;
                }
                tableFunc.Append(this._tokenList.lookup(0).value);
            }
            else if (this._tokenList.lookup(0).type == 6)
            {
                tableFunc.Append("'");
                tableFunc.Append(this._tokenList.lookup(0).value);
                tableFunc.Append("'");
            }
            this.TableFunctionParser(tableFunc, openBraceCount);
        }

        public SqlTableSourceBase tableSource()
        {
            SqlTableSourceBase tablesource = null;
            int num = 0;
            if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
            {
                this._tokenList.match();
                tablesource = this.tableSource();
                SqlSubQueryTableSource source = null;
                if (tablesource is SqlSubQueryTableSource)
                {
                    source = (SqlSubQueryTableSource)tablesource;
                    source.subQuery = this.selectRest(source.subQuery);
                }
                this._tokenList.match(Token.CloseBraceToken);
                string[] sb = new string[2];
                string str = this.As(sb);
                if ((str != null) && (str.Length != 0))
                {
                    source.alias = str;
                    source.setAsWord(sb[0]);
                    source.setOrgAlias(sb[1]);
                }
                return this.tableSourceRest(tablesource);
            }
            if (this._tokenList.lookup(0).Equals(Token.SelectToken))
            {
                SelectParser parser = new SelectParser(this._tokenList);
                tablesource = new SqlSubQueryTableSource(parser.select());
            }
            else if (this._tokenList.lookup(0).type == 1)
            {
                StringBuilder builder = new StringBuilder();
                StringBuilder builder2 = new StringBuilder();
                builder.Append(this._tokenList.lookup(0).value);
                builder2.Append(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(1);
                while (this._tokenList.lookup(0).Equals(Token.PeriodToken))
                {
                    builder.Append(".");
                    builder2.Append(".");
                    this._tokenList.match();
                    builder.Append(this._tokenList.lookup(0).value);
                    builder2.Append(this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match(1);
                }
                tablesource = new SqlTableSource(builder.ToString());
                ((SqlTableSource)tablesource).setOrgName(builder2.ToString());
            }
            else if (((this._tokenList.lookup(0).type == 3) && ((this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.USERTABLES.value) || this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.USERVIEWS.value)) || (this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.USERCOLUMNS.value) || this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.SYSINDEXES.value)))) || (this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.SYSCONSTRAINTS.value) || this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.TABLECOLUMNDEFAULTVALUE.value)))
            {
                StringBuilder builder3 = new StringBuilder();
                StringBuilder builder4 = new StringBuilder();
                builder3.Append(this._tokenList.lookup(0).value);
                builder4.Append(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(3);
                tablesource = new SqlTableSource(builder3.ToString());
                ((SqlTableSource)tablesource).setOrgName(builder4.ToString());
            }
            else if (this._tokenList.lookup(0).type == 0x12)
            {
                StringBuilder builder5 = new StringBuilder();
                StringBuilder builder6 = new StringBuilder();
                builder5.Append(this._tokenList.lookup(0).value);
                builder6.Append(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(0x12);
                tablesource = new SqlTableFunctionTableSource(builder5.ToString());
            }
            while (num != 0)
            {
                this._tokenList.match(Token.CloseBraceToken);
                num--;
            }
            return this.tableSourceRest(tablesource);
        }

        private SqlTableSourceBase tableSourceRest(SqlTableSourceBase tablesource)
        {
            if (this._tokenList.lookup(0).Equals(Token.AsToken))
            {
                tablesource.setAsWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                if (this._tokenList.lookup(0).type != 1)
                {
                    if (this._tokenList.lookup(0).type != 6)
                    {
                        throw new ParserException("Error");
                    }
                    if (tablesource.alias != null)
                    {
                        throw new ParserException("Error");
                    }
                    tablesource.setAlias("'" + this._tokenList.lookup(0).value + "'");
                    this._tokenList.match();
                    tablesource = this.tableSourceRest(tablesource);
                }
                else
                {
                    if (tablesource.alias != null)
                    {
                        throw new ParserException("Error");
                    }
                    tablesource.alias = this._tokenList.lookup(0).value;
                    tablesource.setOrgAlias(this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match();
                    tablesource = this.tableSourceRest(tablesource);
                }
            }
            else if (this._tokenList.lookup(0).type == 1)
            {
                if ((tablesource.alias != null) && (tablesource.alias.Length != 0))
                {
                    throw new ParserException("Error", 0, 0);
                }
                tablesource.alias = this._tokenList.lookup(0).value;
                tablesource.setOrgAlias(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                tablesource = this.tableSourceRest(tablesource);
            }
            else if (this._tokenList.lookup(0).type == 6)
            {
                if ((tablesource.alias != null) && (tablesource.alias.Length != 0))
                {
                    throw new ParserException("Error", 0, 0);
                }
                tablesource.setAlias("'" + this._tokenList.lookup(0).value + "'");
                this._tokenList.match();
                tablesource = this.tableSourceRest(tablesource);
            }
            else if (this._tokenList.lookup(0).type == 7)
            {
                if ((tablesource.alias != null) && (tablesource.alias.Length != 0))
                {
                    throw new ParserException("Error", 0, 0);
                }
                tablesource.alias = "N'" + this._tokenList.lookup(0).value + "'";
                tablesource.setOrgAlias(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                tablesource = this.tableSourceRest(tablesource);
            }
            else if (this._tokenList.lookup(0).Equals(Token.JoinToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                SqlTableSourceBase right = this.tableSource();
                string onWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.OnToken);
                SqlExpr condition = this.exprParser.expr();
                tablesource = new SqlJoinedTableSource(tablesource, right, 0, condition, orgValue, onWord);
            }
            else if (this._tokenList.lookup(0).Equals(Token.InnerToken))
            {
                string joinWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                joinWord = joinWord + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.JoinToken);
                SqlTableSourceBase base3 = this.tableSource();
                string str4 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.OnToken);
                SqlExpr expr2 = this.exprParser.expr();
                tablesource = new SqlJoinedTableSource(tablesource, base3, 0, expr2, joinWord, str4);
            }
            else if (this._tokenList.lookup(0).Equals(Token.LeftToken))
            {
                string str5 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.OuterToken))
                {
                    str5 = str5 + " " + this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(Token.OuterToken);
                }
                str5 = str5 + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.JoinToken);
                SqlTableSourceBase base4 = this.tableSource();
                string str6 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.OnToken);
                SqlExpr expr = this.exprParser.expr();
                checkOuterJoinCondition(expr);
                tablesource = new SqlJoinedTableSource(tablesource, base4, 1, expr, str5, str6);
            }
            else if (this._tokenList.lookup(0).Equals(Token.RightToken))
            {
                string str7 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.OuterToken))
                {
                    str7 = str7 + " " + this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(Token.OuterToken);
                }
                str7 = str7 + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.JoinToken);
                SqlTableSourceBase base5 = this.tableSource();
                string str8 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.OnToken);
                SqlExpr expr4 = this.exprParser.expr();
                tablesource = new SqlJoinedTableSource(tablesource, base5, 2, expr4, str7, str8);
            }
            else if (this._tokenList.lookup(0).Equals(Token.FullToken))
            {
                string str9 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.OuterToken))
                {
                    str9 = str9 + " " + this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(Token.OuterToken);
                }
                str9 = str9 + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.JoinToken);
                SqlTableSourceBase base6 = this.tableSource();
                string str10 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.OnToken);
                SqlExpr expr5 = this.exprParser.expr();
                tablesource = new SqlJoinedTableSource(tablesource, base6, 3, expr5, str9, str10);
            }
            else if (this._tokenList.lookup(0).Equals(Token.CrossToken))
            {
                string str11 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                str11 = str11 + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.JoinToken);
                SqlTableSourceBase base7 = this.tableSource();
                tablesource = new SqlJoinedTableSource(tablesource, base7, 4, null, str11, "");
            }
            else if (this._tokenList.lookup(0).Equals(Token.CommaToken))
            {
                this._tokenList.match();
                SqlTableSourceBase base8 = this.tableSource();
                tablesource = new SqlJoinedTableSource(tablesource, base8, 4, null, ",", "");
            }
            if (((this._tokenList.lookup(0).Equals(Token.CommaToken) || this._tokenList.lookup(0).Equals(Token.InnerToken)) || (this._tokenList.lookup(0).Equals(Token.LeftToken) || this._tokenList.lookup(0).Equals(Token.RightToken))) || (this._tokenList.lookup(0).Equals(Token.FullToken) || this._tokenList.lookup(0).Equals(Token.JoinToken)))
            {
                tablesource = this.tableSourceRest(tablesource);
            }
            if (tablesource is SqlTableSource)
            {
                SqlTableSource source = (SqlTableSource)tablesource;
                if (!this._tokenList.lookup(0).Equals(Token.WithToken))
                {
                    return tablesource;
                }
                source.setWithWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                this._tokenList.match(Token.OpenBraceToken);
                string str12 = this._tokenList.lookup(0).value;
                if ((!str12.EqualsIgnoreCase("HOLDLOCK") && !str12.EqualsIgnoreCase("READPAST")) && ((!str12.EqualsIgnoreCase("NOLOCK") && !str12.EqualsIgnoreCase("PAGLOCK")) && !str12.EqualsIgnoreCase("READCOMMITTED")))
                {
                    throw new ParserException("Not support locking hints.");
                }
                source.setHintWord(str12);
                source.lockingHint = str12.ToUpper();
                this._tokenList.match();
                this._tokenList.match(Token.CloseBraceToken);
            }
            return tablesource;
        }

        private void top(SqlSelect select)
        {
            if (this._tokenList.lookup(0).Equals(Token.TopToken))
            {
                select.setTopWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                if (this._tokenList.lookup(0).type != 8)
                {
                    throw new ParserException("Error. ", this._tokenList.lookup(0).beginLine, this._tokenList.lookup(0).beginColumn);
                }
                SqlSelectLimit limit = new SqlSelectLimit(int.Parse(this._tokenList.lookup(0).value));
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.PercentToken))
                {
                    limit.type = 1;
                    this._tokenList.match();
                }
                select.limit = limit;
            }
        }

        private void where(SqlSelect select)
        {
            if (this._tokenList.lookup(0).Equals(Token.WhereToken))
            {
                select.setWhereWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                SqlExpr expr = this.exprParser.expr();
                select.condition = expr;
            }
        }

        private void withRollUp(SqlSelect select)
        {
            if (this._tokenList.lookup(0).Equals(Token.WithToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                if (this._tokenList.lookup(1).Equals(Token.RollUpToken))
                {
                    orgValue = orgValue + " " + this._tokenList.lookup(1).GetOrgValue();
                    select.setWithRollUpWord(orgValue);
                    select.hasWithRollUp = true;
                    this._tokenList.match();
                    this._tokenList.match();
                }
            }
        }
    }






}
