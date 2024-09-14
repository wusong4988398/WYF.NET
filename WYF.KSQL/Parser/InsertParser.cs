using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Parser
{
    public class InsertParser
    {
        // Fields
        private TokenList _tokenList;

        // Methods
        public InsertParser(TokenList tokenList)
        {
            this._tokenList = tokenList;
        }

        public InsertParser(string sql)
        {
            Lexer l = new Lexer(sql);
            this._tokenList = new TokenList(l);
        }

        public void columnList(IList exprCol)
        {
            if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
            {
                this._tokenList.match();
                string str = this._tokenList.lookup(0).value;
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(1);
                SqlIdentifierExpr expr = new SqlIdentifierExpr(str, orgValue);
                exprCol.Add(expr);
                while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                {
                    this._tokenList.match();
                    str = this._tokenList.lookup(0).value;
                    orgValue = this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(1);
                    expr = new SqlIdentifierExpr(str, orgValue);
                    exprCol.Add(expr);
                }
                this._tokenList.match(Token.CloseBraceToken);
            }
        }

        public SqlInsert insert()
        {
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.InsertToken);
            string hints = null;
            if (this._tokenList.lookup(0).type == 0x10)
            {
                hints = this._tokenList.lookup(0).value;
                this._tokenList.match();
            }
            string intoWord = "";
            if (this._tokenList.lookup(0).Equals(Token.IntoToken))
            {
                intoWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
            }
            string tableName = this._tokenList.lookup(0).value;
            SqlInsert insert = new SqlInsert(tableName);
            insert.setInsertWord(orgValue);
            insert.setIntoWord(intoWord);
            if (hints != null)
            {
                insert.getHints().AddRange(HintsParser.parse(hints));
            }
            this._tokenList.match(1);
            if (this._tokenList.lookup(0).Equals(Token.PeriodToken))
            {
                this._tokenList.match();
                tableName = tableName + '.' + this._tokenList.lookup(0).value;
                this._tokenList.match(1);
            }
            this.columnList(insert.columnList);
            if (this._tokenList.lookup(0).Equals(Token.ValuesToken))
            {
                insert.setValuesWord(this._tokenList.lookup(0).GetOrgValue());
            }
            if (!this.valueList(insert.valueList))
            {
                insert.subQuery = new SelectParser(this._tokenList).select();
            }
            return insert;
        }

        public bool valueList(IList exprCol)
        {
            if (!this._tokenList.lookup(0).Equals(Token.ValuesToken))
            {
                return false;
            }
            this._tokenList.match();
            this._tokenList.match(Token.OpenBraceToken);
            SqlExprParser parser = new SqlExprParser(this._tokenList);
            SqlExpr expr = parser.expr();
            exprCol.Add(expr);
            while (this._tokenList.lookup(0).Equals(Token.CommaToken))
            {
                this._tokenList.match();
                expr = parser.expr();
                exprCol.Add(expr);
            }
            this._tokenList.match(Token.CloseBraceToken);
            return true;
        }
    }





}
