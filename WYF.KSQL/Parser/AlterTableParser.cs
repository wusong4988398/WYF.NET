using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public class AlterTableParser : SqlParserBase
    {
        // Fields
        private TokenList _tokenList;
        private Hashtable parserContext;
        private const string TABLENAME = "TABLENAME";

        // Methods
        public AlterTableParser(TokenList tokenList)
        {
            this.parserContext = new Hashtable(10);
            this._tokenList = tokenList;
        }

        public AlterTableParser(string sql)
        {
            this.parserContext = new Hashtable(10);
            Lexer l = new Lexer(sql);
            this._tokenList = new TokenList(l);
        }

        public SqlAlterTableStmt parse()
        {
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.AlterToken);
            orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.TableToken);
            string tableName = this._tokenList.lookup(0).value;
            SqlAlterTableStmt stmt = new SqlAlterTableStmt(tableName);
            stmt.SetAlterWord(orgValue);
            this._tokenList.match(1);
            this.parserContext.Add("TABLENAME", tableName);
            this.parse_itemList(stmt);
            return stmt;
        }

        private void parse_itemList(SqlAlterTableStmt stmt)
        {
            bool flag = false;
            do
            {
                if (flag)
                {
                    this._tokenList.match();
                }
                flag = true;
                if (this._tokenList.lookup(0).Equals(Token.AddToken))
                {
                    this.parseAddTokenItemList(stmt);
                }
                else if (this._tokenList.lookup(0).Equals(Token.AlterToken))
                {
                    this.parseAlterTokenItemList(stmt);
                }
                else if (this._tokenList.lookup(0).Equals(Token.DropToken))
                {
                    this.parseDropTokenItemList(stmt);
                }
                else
                {
                    if (this._tokenList.lookup(0).Equals(Token.WithToken))
                    {
                        throw new ParserException("not support token:" + this._tokenList.lookup(0));
                    }
                    throw new NotSupportedException("unexpect token:" + this._tokenList.lookup(0));
                }
            }
            while (this._tokenList.lookup(0).Equals(Token.CommaToken));
            if (stmt.items.Count > 0)
            {
                stmt.item = (SqlAlterTableItem)stmt.items[0];
            }
        }

        private void parseAddTokenItemList(SqlAlterTableStmt stmt)
        {
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.AddToken);
            bool flag = false;
            if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
            {
                flag = true;
                this._tokenList.match();
            }
            if (this._tokenList.lookup(0).type != 1)
            {
                if (this._tokenList.lookup(0).value.EqualsIgnoreCase("DEFAULT"))
                {
                    string defaultWord = this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match();
                    SqlExpr defaultValue = null;
                    SqlExprParser parser2 = new SqlExprParser(this._tokenList);
                    defaultValue = parser2.expr();
                    string forWord = this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(3);
                    string columnName = this._tokenList.lookup(0).value;
                    this._tokenList.match(1);
                    SqlAlterTableAddDefaultItem item2 = new SqlAlterTableAddDefaultItem(defaultValue, columnName);
                    item2.setItemWord(orgValue);
                    item2.setDefaultWord(defaultWord);
                    item2.setOpenBrace(flag);
                    item2.setForWord(forWord);
                    stmt.items.Add(item2);
                    if (flag)
                    {
                        while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                        {
                            this._tokenList.match();
                            defaultValue = null;
                            defaultValue = new SqlExprParser(this._tokenList).expr();
                            forWord = this._tokenList.lookup(0).GetOrgValue();
                            this._tokenList.match(3);
                            columnName = this._tokenList.lookup(0).value;
                            this._tokenList.match(1);
                            item2 = new SqlAlterTableAddDefaultItem(defaultValue, columnName);
                            item2.setOpenBrace(flag);
                            item2.setForWord(forWord);
                            stmt.items.Add(item2);
                        }
                    }
                    item2.setOpenBrace(false);
                }
                else
                {
                    if (this._tokenList.lookup(0).Equals(Token.WithToken))
                    {
                        throw new ParserException("not support token:" + this._tokenList.lookup(0));
                    }
                    SqlAlterTableAddItem item3 = new SqlAlterTableAddItem();
                    SqlCreateTableParser parser3 = new SqlCreateTableParser(this._tokenList);
                    SqlTableConstraint constraint = parser3.tableConstraint();
                    item3.constraintItemList.Add(constraint);
                    item3.setItemWord(orgValue);
                    item3.setOpenBrace(flag);
                    if (flag)
                    {
                        while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                        {
                            constraint = new SqlCreateTableParser(this._tokenList).tableConstraint();
                            item3.constraintItemList.Add(constraint);
                        }
                    }
                    stmt.items.Add(item3);
                }
            }
            else
            {
                SqlCreateTableParser parser = new SqlCreateTableParser(this._tokenList);
                SqlAlterTableAddItem item = new SqlAlterTableAddItem();
                item.setItemWord(orgValue);
                item.setOpenBrace(flag);
                item.columnDefItemList.Add(parser.parseColumnDef());
                if (!flag)
                {
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        if (this._tokenList.lookup(1).type != 1)
                        {
                            break;
                        }
                        this._tokenList.match();
                        item.columnDefItemList.Add(parser.parseColumnDef());
                    }
                }
                else
                {
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        this._tokenList.match();
                        item.columnDefItemList.Add(parser.parseColumnDef());
                    }
                }
                stmt.items.Add(item);
            }
            if (flag)
            {
                this._tokenList.match(Token.CloseBraceToken);
            }
        }

        private void parseAlterTokenItemList(SqlAlterTableStmt stmt)
        {
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match();
            if (this._tokenList.lookup(0).Equals(Token.ColumnToken))
            {
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
            }
            SqlCreateTableParser parser = new SqlCreateTableParser(this._tokenList);
            SqlAlterTableAlterColumnItem item = new SqlAlterTableAlterColumnItem(parser.parseColumnDef());
            item.setItemWord(orgValue);
            stmt.items.Add(item);
        }

        private void parseDropTokenItemList(SqlAlterTableStmt stmt)
        {
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.DropToken);
            SqlAlterTableDropItem item = new SqlAlterTableDropItem();
            if (!this._tokenList.lookup(0).Equals(Token.ColumnToken))
            {
                if (!this._tokenList.lookup(0).Equals(Token.ConstraintToken))
                {
                    if (!this._tokenList.lookup(0).value.EqualsIgnoreCase("DEFAULT"))
                    {
                        throw new ParserException("not support token:" + this._tokenList.lookup(0));
                    }
                    orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match();
                    bool flag3 = false;
                    if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        flag3 = true;
                        this._tokenList.match();
                    }
                    string forWord = this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(3);
                    string columnName = this._tokenList.lookup(0).value;
                    this._tokenList.match(1);
                    SqlAlterTableDropDefaultItem item2 = new SqlAlterTableDropDefaultItem(columnName);
                    item2.setItemWord(orgValue);
                    item2.setForWord(forWord);
                    item2.setOpenBrace(flag3);
                    stmt.items.Add(item2);
                    if (flag3)
                    {
                        while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                        {
                            this._tokenList.match();
                            forWord = this._tokenList.lookup(0).GetOrgValue();
                            this._tokenList.match(3);
                            columnName = this._tokenList.lookup(0).value;
                            this._tokenList.match(1);
                            item2 = new SqlAlterTableDropDefaultItem(columnName);
                            item2.setForWord(forWord);
                            item2.setOpenBrace(flag3);
                            stmt.items.Add(item2);
                        }
                        item2.setOpenBrace(false);
                        this._tokenList.match(Token.CloseBraceToken);
                    }
                    return;
                }
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                item.setItemWord(orgValue);
                bool flag2 = false;
                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    flag2 = true;
                    this._tokenList.match();
                }
                item.setOpenBrace(flag2);
                string str3 = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                item.constraintItemList.Add(str3);
                if (!flag2)
                {
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        if (this._tokenList.lookup(1).type != 1)
                        {
                            break;
                        }
                        this._tokenList.match();
                        str3 = this._tokenList.lookup(0).value;
                        this._tokenList.match(1);
                        item.constraintItemList.Add(str3);
                    }
                }
                else
                {
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        this._tokenList.match();
                        str3 = this._tokenList.lookup(0).value;
                        this._tokenList.match(1);
                        item.constraintItemList.Add(str3);
                    }
                    this._tokenList.match(Token.CloseBraceToken);
                }
            }
            else
            {
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                item.setItemWord(orgValue);
                bool flag = false;
                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    flag = true;
                    this._tokenList.match();
                }
                item.setOpenBrace(flag);
                string str2 = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                item.columnDefItemList.Add(str2);
                if (!flag)
                {
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        if (this._tokenList.lookup(1).type != 1)
                        {
                            break;
                        }
                        this._tokenList.match();
                        str2 = this._tokenList.lookup(0).value;
                        this._tokenList.match(1);
                        item.columnDefItemList.Add(str2);
                    }
                }
                else
                {
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        this._tokenList.match();
                        str2 = this._tokenList.lookup(0).value;
                        this._tokenList.match(1);
                        item.columnDefItemList.Add(str2);
                    }
                    this._tokenList.match(Token.CloseBraceToken);
                }
                stmt.items.Add(item);
                return;
            }
            stmt.items.Add(item);
        }
    }





}
