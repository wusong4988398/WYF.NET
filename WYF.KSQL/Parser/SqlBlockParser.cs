using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public class SqlBlockParser
    {
        // Fields
        private TokenList _tokenList;

        // Methods
        public SqlBlockParser(TokenList tokenList)
        {
            this._tokenList = tokenList;
        }

        public SqlBlockParser(string sql)
        {
            Lexer l = new Lexer(sql);
            this._tokenList = new TokenList(l);
        }

        public SqlBlockStmt block()
        {
            SqlBlockStmt stmt = new SqlBlockStmt();
            if (stmt.declItemList == null)
            {
                stmt.declItemList = new ArrayList();
            }
            if (this._tokenList.lookup(0).Equals(Token.DeclareToken))
            {
                this.decl(stmt.declItemList);
            }
            this._tokenList.match(Token.BeginToken);
            if (stmt.stmtList == null)
            {
                stmt.stmtList = new ArrayList();
            }
            new SqlStmtParser(this._tokenList).stmtList(stmt.stmtList, Token.EndToken);
            this._tokenList.match(Token.EndToken);
            this._tokenList.match(Token.SemicolonToken);
            return stmt;
        }

        public void decl(IList descList)
        {
            if (this._tokenList.lookup(0).Equals(Token.DeclareToken))
            {
                this._tokenList.match();
                SqlExprParser parser = new SqlExprParser(this._tokenList);
                while (!this._tokenList.lookup(0).Equals(Token.BeginToken))
                {
                    if (this._tokenList.lookup(0).type == 2)
                    {
                        SqlBlockStmt.DeclVarItem item = new SqlBlockStmt.DeclVarItem
                        {
                            name = this._tokenList.lookup(0).value
                        };
                        this._tokenList.match();
                        string str = this._tokenList.lookup(0).value.ToUpper();
                        item.dataType = str;
                        this._tokenList.match();
                        if (str.Equals("INT") || str.Equals("INTEGER"))
                        {
                            item.dataType = "INT";
                            item.length = 4;
                        }
                        else if (str.Equals("SMALLINT"))
                        {
                            item.length = 2;
                        }
                        else if ((str.Equals("DECIMAL") || str.Equals("NUMBER")) || str.Equals("NUMERIC"))
                        {
                            item.dataType = "DECIMAL";
                            item.length = 9;
                            if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                            {
                                this._tokenList.match();
                                item.precision = int.Parse(this._tokenList.lookup(0).value);
                                this._tokenList.match(8);
                                if (this._tokenList.lookup(0).Equals(Token.CommaToken))
                                {
                                    this._tokenList.match();
                                    item.scale = int.Parse(this._tokenList.lookup(0).value);
                                    this._tokenList.match(8);
                                }
                                this._tokenList.match(Token.CloseBraceToken);
                            }
                        }
                        else if (str.Equals("CHAR"))
                        {
                            this._tokenList.match(Token.OpenBraceToken);
                            item.length = int.Parse(this._tokenList.lookup(0).value);
                            if (item.length > 0xfe)
                            {
                                throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'char' is 254, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                            }
                            this._tokenList.match(8);
                            this._tokenList.match(Token.CloseBraceToken);
                        }
                        else if (str.Equals("NCHAR"))
                        {
                            this._tokenList.match(Token.OpenBraceToken);
                            item.length = int.Parse(this._tokenList.lookup(0).value);
                            if (item.length > 0xfe)
                            {
                                throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'nchar' is 254, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                            }
                            this._tokenList.match(8);
                            this._tokenList.match(Token.CloseBraceToken);
                        }
                        else if (str.Equals("VARCHAR"))
                        {
                            this._tokenList.match(Token.OpenBraceToken);
                            item.length = int.Parse(this._tokenList.lookup(0).value);
                            if (item.length > 0xfa0)
                            {
                                throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'varchar' is 4000, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                            }
                            this._tokenList.match(8);
                            this._tokenList.match(Token.CloseBraceToken);
                        }
                        else if (str.Equals("NVARCHAR"))
                        {
                            this._tokenList.match(Token.OpenBraceToken);
                            item.length = int.Parse(this._tokenList.lookup(0).value);
                            if (item.length > 0x7d0)
                            {
                                throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'nvarchar' is 2000, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                            }
                            this._tokenList.match(8);
                            this._tokenList.match(Token.CloseBraceToken);
                        }
                        else if (!str.Equals("DATETIME"))
                        {
                            if (str.Equals("TIMESTAMP"))
                            {
                                item.dataType = "DATETIME";
                            }
                            else if (str.Equals("BINARY"))
                            {
                                this._tokenList.match(Token.OpenBraceToken);
                                item.length = int.Parse(this._tokenList.lookup(0).value);
                                if (item.length > 0x7d0)
                                {
                                    throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'binary' is 2000, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                                }
                                this._tokenList.match(8);
                                this._tokenList.match(Token.CloseBraceToken);
                            }
                            else if (str.Equals("VARBINARY"))
                            {
                                this._tokenList.match(Token.OpenBraceToken);
                                item.length = int.Parse(this._tokenList.lookup(0).value);
                                if (item.length > 0x7d0)
                                {
                                    throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'varbinary' is 2000, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                                }
                                this._tokenList.match(8);
                                this._tokenList.match(Token.CloseBraceToken);
                            }
                            else if (str.Equals("BLOG"))
                            {
                                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                                {
                                    this._tokenList.match(Token.OpenBraceToken);
                                    item.length = int.Parse(this._tokenList.lookup(0).value);
                                    this._tokenList.match(8);
                                    this._tokenList.match(Token.CloseBraceToken);
                                }
                            }
                            else if (str.Equals("CLOB"))
                            {
                                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                                {
                                    this._tokenList.match(Token.OpenBraceToken);
                                    item.length = int.Parse(this._tokenList.lookup(0).value);
                                    if (item.length > 0x40000000)
                                    {
                                        throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'CLOB' is 1073741824, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                                    }
                                    this._tokenList.match(8);
                                    this._tokenList.match(Token.CloseBraceToken);
                                }
                            }
                            else if (str.Equals("NCLOB"))
                            {
                                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                                {
                                    this._tokenList.match(Token.OpenBraceToken);
                                    item.length = int.Parse(this._tokenList.lookup(0).value);
                                    this._tokenList.match(8);
                                    this._tokenList.match(Token.CloseBraceToken);
                                }
                            }
                            else if (str.Equals("SMALLINT"))
                            {
                                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                                {
                                    this._tokenList.match(Token.OpenBraceToken);
                                    item.length = 1;
                                    this._tokenList.match(8);
                                    this._tokenList.match(Token.CloseBraceToken);
                                }
                            }
                            else if (str.Equals("BLOB"))
                            {
                                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                                {
                                    this._tokenList.match(Token.OpenBraceToken);
                                    item.length = int.Parse(this._tokenList.lookup(0).value);
                                    this._tokenList.match(8);
                                    this._tokenList.match(Token.CloseBraceToken);
                                }
                            }
                            else
                            {
                                if (!str.Equals("XMLTYPE"))
                                {
                                    throw new ParserException(string.Concat(new object[] { "NOT SUPPORT DATA TYPE '", str, "', at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                                }
                                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                                {
                                    this._tokenList.match(Token.OpenBraceToken);
                                    item.length = int.Parse(this._tokenList.lookup(0).value);
                                    this._tokenList.match(8);
                                    this._tokenList.match(Token.CloseBraceToken);
                                }
                            }
                        }
                        if (this._tokenList.lookup(0).Equals(Token.EqualToken))
                        {
                            this._tokenList.match();
                            item.defaultValueExpr = parser.expr();
                        }
                        this._tokenList.match(Token.SemicolonToken);
                        descList.Add(item);
                    }
                    else
                    {
                        if (!this._tokenList.lookup(0).Equals(Token.CursorToken))
                        {
                            throw new ParserException("TODO, " + this._tokenList.lookup(0).value);
                        }
                        this._tokenList.match();
                        SqlBlockStmt.DeclCurItem item2 = new SqlBlockStmt.DeclCurItem
                        {
                            name = this._tokenList.lookup(0).value
                        };
                        this._tokenList.match();
                        this._tokenList.match(Token.IsToken);
                        item2.select = new SelectParser(this._tokenList).select();
                        this._tokenList.match(Token.SemicolonToken);
                        descList.Add(item2);
                        continue;
                    }
                }
            }
        }
    }






}
