using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public class MergeParser : SqlParserBase
    {
        // Fields
        private TokenList _tokenList;

        // Methods
        public MergeParser(TokenList tokenList)
        {
            this._tokenList = tokenList;
        }

        private SqlExpr ExprParse()
        {
            if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
            {
                this._tokenList.match();
            }
            SqlExpr expr = new SqlExprParser(this._tokenList).expr();
            if (this._tokenList.lookup(0).Equals(Token.CloseBraceToken))
            {
                this._tokenList.match();
            }
            return expr;
        }

        public SqlMerge Merge()
        {
            SqlMerge merge2;
            try
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.MergeToken);
                SqlMerge merge = new SqlMerge();
                merge.SetMergeWord(orgValue);
                if (!this._tokenList.lookup(0).Equals(Token.IntoToken))
                {
                    throw new ParserException("Not support '" + this._tokenList.lookup(0).value.ToString() + "'");
                }
                string intoWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.IntoToken);
                merge.SetIntoWord(intoWord);
                this.updateTableSource(merge);
                if (this._tokenList.lookup(0).Equals(Token.UsingToken))
                {
                    merge.SetUsingWord(this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match(Token.UsingToken);
                    if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        merge.UsingExpr = this.ExprParse();
                    }
                    else if (this._tokenList.lookup(0).type == 1)
                    {
                        merge.UsingTable = new SqlTableSource(this._tokenList.lookup(0).value);
                        this._tokenList.match(1);
                    }
                }
                if (this._tokenList.lookup(0).Equals(Token.AsToken))
                {
                    this._tokenList.match();
                    merge.UsingTableAlias = this._tokenList.lookup(0).value;
                    this._tokenList.match(1);
                }
                else
                {
                    SelectParser parser = new SelectParser(this._tokenList);
                    SqlTableSource source = (SqlTableSource)parser.tableSource();
                    if (source != null)
                    {
                        merge.UsingTableAlias = source.name;
                    }
                }
                this.OnParse(merge);
                while (this._tokenList.lookup(0).type != 12)
                {
                    if (this._tokenList.lookup(0).Equals(Token.SemicolonToken))
                    {
                        this._tokenList.match(Token.SemicolonToken);
                    }
                    else
                    {
                        bool matched = this.WhenMatchedParse();
                        this.SqlStmtParse(merge, matched);
                    }
                }
                merge2 = merge;
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
            return merge2;
        }

        private void OnParse(SqlMerge merge)
        {
            if (!this._tokenList.lookup(0).Equals(Token.OnToken))
            {
                throw new ParserException(" ERROR： '" + this._tokenList.lookup(0).value.ToString() + "'");
            }
            merge.SetOnWord(this._tokenList.lookup(0).GetOrgValue());
            this._tokenList.match(Token.OnToken);
            merge.OnExpr = this.ExprParse();
        }

        private void SqlStmtParse(SqlMerge merge, bool matched)
        {
            if (matched)
            {
                SqlMergeMatched matched2 = new SqlMergeMatched();
                if (this._tokenList.lookup(0).Equals(Token.UpdateToken))
                {
                    this._tokenList.match(Token.UpdateToken);
                    if (!this._tokenList.lookup(0).Equals(Token.SetToken))
                    {
                        throw new ParserException(" ERROR： '" + this._tokenList.lookup(0).value.ToString() + "'.  merge into the update statement has not 'set'! ");
                    }
                    this._tokenList.match(Token.SetToken);
                    SqlExpr item = this.ExprParse();
                    matched2.SetClauses.Add(item);
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        this._tokenList.match();
                        item = this.ExprParse();
                        matched2.SetClauses.Add(item);
                    }
                    if (this._tokenList.lookup(0).Equals(Token.WhereToken))
                    {
                        this._tokenList.match();
                        SqlExpr expr2 = new SqlExprParser(this._tokenList).expr();
                        matched2.UpdateWhere = expr2;
                    }
                }
                if (this._tokenList.lookup(0).Equals(Token.DeleteToken))
                {
                    this._tokenList.match(Token.DeleteToken);
                    if (!this._tokenList.lookup(0).Equals(Token.WhereToken))
                    {
                        throw new ParserException(" ERROR： '" + this._tokenList.lookup(0).value.ToString() + "'.  merge into the delete statement has not 'where'! ");
                    }
                    this._tokenList.match(Token.WhereToken);
                    SqlExpr expr3 = new SqlExprParser(this._tokenList).expr();
                    matched2.DeleteWhere = expr3;
                }
                merge.MatchedSql = matched2;
            }
            else
            {
                SqlMergeNotMatched matched3 = new SqlMergeNotMatched();
                if (this._tokenList.lookup(0).Equals(Token.InsertToken))
                {
                    this._tokenList.match(Token.InsertToken);
                    if (!this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        throw new ParserException(" ERROR： '" + this._tokenList.lookup(0).value.ToString() + "'.  merge into the insert statement has not '('! ");
                    }
                    this._tokenList.match();
                    string str = this._tokenList.lookup(0).value;
                    string orgValue = this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(1);
                    SqlIdentifierExpr expr4 = new SqlIdentifierExpr(str, orgValue);
                    matched3.InsertColumns.Add(expr4);
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        this._tokenList.match();
                        str = this._tokenList.lookup(0).value;
                        orgValue = this._tokenList.lookup(0).GetOrgValue();
                        this._tokenList.match(1);
                        expr4 = new SqlIdentifierExpr(str, orgValue);
                        matched3.InsertColumns.Add(expr4);
                    }
                    this._tokenList.match(Token.CloseBraceToken);
                    if (!this._tokenList.lookup(0).Equals(Token.ValuesToken))
                    {
                        throw new ParserException(" ERROR： '" + this._tokenList.lookup(0).value.ToString() + "'.  merge into the insert statement has not value! ");
                    }
                    this._tokenList.match();
                    this._tokenList.match(Token.OpenBraceToken);
                    SqlExprParser parser3 = new SqlExprParser(this._tokenList);
                    SqlExpr expr5 = parser3.expr();
                    matched3.InsertValues.Add(expr5);
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        this._tokenList.match();
                        expr5 = parser3.expr();
                        matched3.InsertValues.Add(expr5);
                    }
                    this._tokenList.match(Token.CloseBraceToken);
                    if (this._tokenList.lookup(0).Equals(Token.WhereToken))
                    {
                        this._tokenList.match(Token.WhereToken);
                        SqlExpr expr6 = new SqlExprParser(this._tokenList).expr();
                        matched3.InsertWhere = expr6;
                    }
                }
                merge.NotMatchedSql = matched3;
            }
        }

        private void updateTableSource(SqlMerge merge)
        {
            if (this._tokenList.lookup(0).type == 1)
            {
                merge.UpdateTable = new SqlTableSource(this._tokenList.lookup(0).value);
                this._tokenList.match(1);
                if (this._tokenList.lookup(0).Equals(Token.AsToken))
                {
                    this._tokenList.match();
                    merge.UpdateTable.alias = this._tokenList.lookup(0).value;
                    this._tokenList.match(1);
                }
                else
                {
                    SelectParser parser = new SelectParser(this._tokenList);
                    SqlTableSource source = (SqlTableSource)parser.tableSource();
                    if (source != null)
                    {
                        merge.UpdateTable.alias = source.name;
                    }
                }
            }
        }

        private bool WhenMatchedParse()
        {
            bool flag = true;
            if (!this._tokenList.lookup(0).Equals(Token.WhenToken))
            {
                throw new ParserException(" ERROR： '" + this._tokenList.lookup(0).value.ToString() + "'");
            }
            this._tokenList.match(Token.WhenToken);
            if (this._tokenList.lookup(0).Equals(Token.NotToken))
            {
                this._tokenList.match(Token.NotToken);
                flag = false;
            }
            if (!this._tokenList.lookup(0).Equals(Token.MatchedToken))
            {
                throw new ParserException(" ERROR： '" + this._tokenList.lookup(0).value.ToString() + "'");
            }
            this._tokenList.match(Token.MatchedToken);
            if (!this._tokenList.lookup(0).Equals(Token.ThenToken))
            {
                throw new ParserException(" ERROR： '" + this._tokenList.lookup(0).value.ToString() + "'");
            }
            this._tokenList.match(Token.ThenToken);
            return flag;
        }
    }





}
