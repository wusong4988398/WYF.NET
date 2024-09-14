using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public class ColumnListParser : SqlParserBase
    {
        // Fields
        private TokenList _tokenList;
        private SqlExprParser exprParser;

        // Methods
        public ColumnListParser(string sql)
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

        public SqlSelect selectList()
        {
            SqlSelect select = new SqlSelect();
            IList selectList = select.selectList;
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
            selectList.Add(new SqlSelectItem(right, alias, sb[1], sb[0], mark));
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
                selectList.Add(new SqlSelectItem(right, alias, sb[1], sb[0], mark));
            }
            return select;
        }
    }


 



}
