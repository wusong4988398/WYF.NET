using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public class UpdateParser : SqlParserBase
    {
        // Fields
        private TokenList _tokenList;
        private SqlExprParser exprParser;

        // Methods
        public UpdateParser(TokenList tokenList)
        {
            this._tokenList = tokenList;
            this.exprParser = new SqlExprParser(this._tokenList);
        }

        public UpdateParser(string sql)
        {
            try
            {
                Lexer l = new Lexer(sql);
                this._tokenList = new TokenList(l);
                this.exprParser = new SqlExprParser(this._tokenList);
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }

        public void from(SqlUpdate update)
        {
            try
            {
                if (this._tokenList.lookup(0).Equals(Token.FromToken))
                {
                    update.setFromWord(this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match();
                    update.tableSource = new SelectParser(this._tokenList).tableSource();
                }
            }
            catch (System.Exception)
            {
                throw new ParserException();
            }
        }

        public SqlUpdate update()
        {
            SqlUpdate update2;
            try
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.UpdateToken);
                SqlUpdate update = new SqlUpdate();
                update.setUpdateWord(orgValue);
                string hints = null;
                if (this._tokenList.lookup(0).type == 0x10)
                {
                    hints = this._tokenList.lookup(0).value;
                    this._tokenList.match();
                }
                if (hints != null)
                {
                    update.getHints().AddRange(HintsParser.parse(hints));
                }
                this.updateTableSource(update);
                this.updateList(update);
                this.from(update);
                this.where(update);
                update2 = update;
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
            return update2;
        }

        public void updateList(SqlUpdate sqlUpdate)
        {
            string str;
            SqlUpdateItem item;
            if (this._tokenList.lookup(0).Equals(Token.SetToken))
            {
                sqlUpdate.setSetWord(this._tokenList.lookup(0).GetOrgValue());
            }
            this._tokenList.match(Token.SetToken);
            if (this._tokenList.lookup(0).type == 1)
            {
                SqlExpr expr;
                str = this._tokenList.lookup(0).value;
                this._tokenList.match();
                this._tokenList.match(Token.EqualToken);
                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken) && this._tokenList.lookup(1).Equals(Token.SelectToken))
                {
                    this._tokenList.match();
                    SelectParser parser = new SelectParser(this._tokenList);
                    expr = new QueryExpr(parser.select());
                    this._tokenList.match(Token.CloseBraceToken);
                }
                else
                {
                    expr = this.exprParser.expr();
                }
                item = new SqlUpdateItem(str, expr);
                sqlUpdate.updateList.Add(item);
            }
            else
            {
                if (!this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    throw new ParserException("Error", 0, 0);
                }
                this._tokenList.match();
                SubQueryUpdateItem item2 = new SubQueryUpdateItem();
                string str2 = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                item2.columnList.Add(str2);
                while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                {
                    this._tokenList.match();
                    str2 = this._tokenList.lookup(0).value;
                    this._tokenList.match(1);
                    item2.columnList.Add(str2);
                }
                this._tokenList.match(Token.CloseBraceToken);
                this._tokenList.match(Token.EqualToken);
                this._tokenList.match(Token.OpenBraceToken);
                SqlSelectBase base3 = new SelectParser(this._tokenList).select();
                item2.subQuery = base3;
                this._tokenList.match(Token.CloseBraceToken);
                if (((sqlUpdate != null) && (sqlUpdate.updateTable != null)) && ((sqlUpdate.updateTable.alias != null) && !sqlUpdate.updateTable.alias.Trim().Equals("")))
                {
                    item2.addExtAttr("tableSourceAlias", sqlUpdate.updateTable.alias);
                }
                sqlUpdate.updateList.Add(item2);
            }
            while (this._tokenList.lookup(0).Equals(Token.CommaToken))
            {
                this._tokenList.match();
                if (this._tokenList.lookup(0).type == 1)
                {
                    str = this._tokenList.lookup(0).value;
                    this._tokenList.match();
                    this._tokenList.match(Token.EqualToken);
                    SqlExpr expr2 = this.exprParser.expr();
                    item = new SqlUpdateItem(str, expr2);
                    sqlUpdate.updateList.Add(item);
                }
                else
                {
                    if (!this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        throw new ParserException("Error", 0, 0);
                    }
                    this._tokenList.match();
                    SubQueryUpdateItem item3 = new SubQueryUpdateItem();
                    string str3 = this._tokenList.lookup(0).value;
                    this._tokenList.match(1);
                    item3.columnList.Add(str3);
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        this._tokenList.match();
                        str3 = this._tokenList.lookup(0).value;
                        this._tokenList.match(1);
                        item3.columnList.Add(str3);
                    }
                    this._tokenList.match(Token.CloseBraceToken);
                    this._tokenList.match(Token.EqualToken);
                    this._tokenList.match(Token.OpenBraceToken);
                    SqlSelectBase base4 = new SelectParser(this._tokenList).select();
                    item3.subQuery = base4;
                    this._tokenList.match(Token.CloseBraceToken);
                    sqlUpdate.updateList.Add(item3);
                }
            }
        }

        private void updateTableSource(SqlUpdate update)
        {
            try
            {
                if (this._tokenList.lookup(0).type == 1)
                {
                    update.updateTable = new SqlTableSource(this._tokenList.lookup(0).value);
                    this._tokenList.match(1);
                    if (this._tokenList.lookup(0).Equals(Token.AsToken))
                    {
                        update.setAsWord(this._tokenList.lookup(0).GetOrgValue());
                        this._tokenList.match();
                        update.updateTable.alias = this._tokenList.lookup(0).value;
                        this._tokenList.match(1);
                    }
                    update.tableSource = new SelectParser(this._tokenList).tableSource();
                }
            }
            catch (System.Exception)
            {
                throw new ParserException();
            }
        }

        public void where(SqlUpdate update)
        {
            try
            {
                if (this._tokenList.lookup(0).Equals(Token.WhereToken))
                {
                    update.setWhereWord(this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match();
                    SqlExpr expr = new SqlExprParser(this._tokenList).expr();
                    update.condition = expr;
                }
            }
            catch (System.Exception)
            {
                throw new ParserException();
            }
        }
    }






}
