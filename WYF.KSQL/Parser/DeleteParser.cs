using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Parser
{
    public class DeleteParser
    {
        // Fields
        private TokenList _tokenList;

        // Methods
        public DeleteParser(TokenList tokenList)
        {
            this._tokenList = tokenList;
        }

        public DeleteParser(string sql)
        {
            Lexer l = new Lexer(sql);
            this._tokenList = new TokenList(l);
        }

        public void from(SqlDelete delete)
        {
            if (this._tokenList.lookup(0).Equals(Token.FromToken))
            {
                delete.setFromWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                delete.tableSource = new SelectParser(this._tokenList).tableSource();
            }
        }

        public SqlDelete parse_delete()
        {
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.DeleteToken);
            SqlDelete delete = new SqlDelete();
            delete.setDeleteWord(orgValue);
            string hints = null;
            if (this._tokenList.lookup(0).type == 0x10)
            {
                hints = this._tokenList.lookup(0).value;
                this._tokenList.match();
            }
            if (hints != null)
            {
                delete.getHints().AddRange(HintsParser.parse(hints));
            }
            if (this._tokenList.lookup(0).type == 1)
            {
                delete.tableName = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
            }
            this.from(delete);
            this.where(delete);
            return delete;
        }

        public void where(SqlDelete delete)
        {
            if (this._tokenList.lookup(0).Equals(Token.WhereToken))
            {
                delete.setWhereWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                SqlExpr expr = new SqlExprParser(this._tokenList).expr();
                delete.condition = expr;
            }
        }
    }


 



}
