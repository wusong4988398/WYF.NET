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
    public class SqlStmtParser : SqlParserBase
    {
        // Fields
        private TokenList _tokenList;

        // Methods
        public SqlStmtParser(TokenList tokenList)
        {
            this._tokenList = tokenList;
        }

        public SqlStmtParser(string sql)
        {
            Lexer l = new Lexer(sql);
            this._tokenList = new TokenList(l);
        }

        private SqlStmt drop()
        {
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match();
            if (this._tokenList.lookup(0).Equals(Token.TableToken))
            {
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                string tableName = this._tokenList.lookup(0).value;
                string orgTableName = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(1);
                if (this._tokenList.lookup(0).Equals(Token.PeriodToken))
                {
                    this._tokenList.match();
                    tableName = tableName + '.' + this._tokenList.lookup(0).value;
                    orgTableName = orgTableName + '.' + this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(1);
                }
                return new SqlDropTableStmt(tableName, orgTableName, orgValue);
            }
            if (this._tokenList.lookup(0).Equals(Token.ViewToken))
            {
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                string viewName = this._tokenList.lookup(0).value;
                string orgViewName = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(1);
                if (this._tokenList.lookup(0).Equals(Token.PeriodToken))
                {
                    this._tokenList.match();
                    viewName = viewName + '.' + this._tokenList.lookup(0).value;
                    orgViewName = orgViewName + '.' + this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(1);
                }
                return new SqlDropViewStmt(viewName, orgViewName, orgValue);
            }
            if (this._tokenList.lookup(0).Equals(Token.TriggerToken))
            {
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                string trggerName = this._tokenList.lookup(0).value;
                string orgTrggerName = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(1);
                return new SqlDropTriggerStmt(trggerName, orgTrggerName, orgValue);
            }
            if (this._tokenList.lookup(0).Equals(Token.IndexToken))
            {
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                string str8 = this._tokenList.lookup(0).value;
                string str9 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(1);
                if (!this._tokenList.lookup(0).Equals(Token.PeriodToken))
                {
                    throw new ParserException("ERROR");
                }
                this._tokenList.match();
                string indexName = this._tokenList.lookup(0).value;
                string orgIndexName = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(1);
                SqlDropIndexStmt stmt = new SqlDropIndexStmt(str8, indexName);
                stmt.SetDropWord(orgValue);
                stmt.SetOrgIndexName(orgIndexName);
                stmt.SetOrgTableName(str9);
                return stmt;
            }
            if (!this._tokenList.lookup(0).Equals(Token.FunctionToken))
            {
                throw new ParserException("Not support '" + this._tokenList.lookup(0).value.ToString() + "'");
            }
            orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match();
            string functionName = this._tokenList.lookup(0).value;
            string orgFunctionName = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(1);
            SqlDropFunctionStmt stmt2 = new SqlDropFunctionStmt(functionName);
            stmt2.setDropWord(orgValue);
            stmt2.SetOrgFunctionName(orgFunctionName);
            return stmt2;
        }

        private SqlStmt ksql_cursor_loop()
        {
            this._tokenList.match();
            SqlCursorLoopStmt stmt = new SqlCursorLoopStmt
            {
                curName = this._tokenList.lookup(0).value
            };
            this._tokenList.match();
            SqlExprParser parser = new SqlExprParser(this._tokenList);
            if (stmt.fieldList == null)
            {
                stmt.fieldList = new ArrayList();
            }
            this._tokenList.match(Token.OpenBraceToken);
            parser.exprList(stmt.fieldList);
            this._tokenList.match(Token.CloseBraceToken);
            this._tokenList.match(Token.IntoToken);
            if (stmt.intoList == null)
            {
                stmt.intoList = new ArrayList();
            }
            this._tokenList.match(Token.OpenBraceToken);
            parser.exprList(stmt.intoList);
            this._tokenList.match(Token.CloseBraceToken);
            this._tokenList.match(Token.DoToken);
            if (stmt.stmtList == null)
            {
                stmt.stmtList = new ArrayList();
            }
            this.stmtList(stmt.stmtList, Token.EndToken);
            this._tokenList.match();
            this._tokenList.match(Token.CursorLoopToken);
            this._tokenList.match(Token.SemicolonToken);
            return stmt;
        }

        private SqlStmt ksql_fetch()
        {
            this._tokenList.match();
            SqlFetchStmt stmt = new SqlFetchStmt
            {
                curName = this._tokenList.lookup(0).value
            };
            this._tokenList.match();
            SqlExprParser parser = new SqlExprParser(this._tokenList);
            if (stmt.fieldList == null)
            {
                stmt.fieldList = new ArrayList();
            }
            this._tokenList.match(Token.OpenBraceToken);
            parser.exprList(stmt.fieldList);
            this._tokenList.match(Token.CloseBraceToken);
            this._tokenList.match(Token.IntoToken);
            if (stmt.intoList == null)
            {
                stmt.intoList = new ArrayList();
            }
            this._tokenList.match(Token.OpenBraceToken);
            parser.exprList(stmt.intoList);
            this._tokenList.match(Token.CloseBraceToken);
            return stmt;
        }

        public SqlStmt parseCreate()
        {
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.CreateToken);
            if (this._tokenList.lookup(0).Equals(Token.TableToken))
            {
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                SqlCreateTableStmt stmt = new SqlCreateTableParser(this._tokenList).parse();
                stmt.SetCreateTableWord(orgValue);
                return stmt;
            }
            if (this._tokenList.lookup(0).Equals(Token.ViewToken))
            {
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                SqlCreateViewStmt stmt2 = new SqlCreateViewStmt();
                stmt2.SetCreateViewWord(orgValue);
                string str2 = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                stmt2.name = str2;
                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    this._tokenList.match();
                    string str3 = this._tokenList.lookup(0).value;
                    this._tokenList.match(1);
                    stmt2.columnList.Add(str3);
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        this._tokenList.match();
                        str3 = this._tokenList.lookup(0).value;
                        this._tokenList.match(1);
                        stmt2.columnList.Add(str3);
                    }
                    this._tokenList.match(Token.CloseBraceToken);
                }
                stmt2.setAsWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(Token.AsToken);
                stmt2.select = new SelectParser(this._tokenList).select();
                return stmt2;
            }
            if (this._tokenList.lookup(0).Equals(Token.ProcedureToken))
            {
                this._tokenList.match();
                throw new ParserException("not support token:" + this._tokenList.lookup(0));
            }
            if (this._tokenList.lookup(0).Equals(Token.FunctionToken))
            {
                this._tokenList.match();
                throw new ParserException("not support token:" + this._tokenList.lookup(0));
            }
            if (this._tokenList.lookup(0).Equals(Token.IndexToken))
            {
                return this.parseCreateIndex(orgValue);
            }
            if (this._tokenList.lookup(0).Equals(Token.UniqueToken))
            {
                return this.parseCreateUniqueIndex(orgValue);
            }
            if (this._tokenList.lookup(0).Equals(Token.ClusteredToken))
            {
                return this.parseCreateClusteredIndex(orgValue);
            }
            if (this._tokenList.lookup(0).Equals(Token.TriggerToken))
            {
                this._tokenList.match();
                throw new ParserException("not support token:" + this._tokenList.lookup(0));
            }
            throw new ParserException("not support token:" + this._tokenList.lookup(0));
        }

        private SqlStmt parseCreateClusteredIndex()
        {
            return this.parseCreateClusteredIndex("CREATE");
        }

        private SqlStmt parseCreateClusteredIndex(string createWord)
        {
            createWord = createWord + " " + this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match();
            createWord = createWord + " " + this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.IndexToken);
            string indexName = this._tokenList.lookup(0).value;
            if ((indexName.Length > 30) && this._tokenList.getSqlParser().isThrowExWhenNameTooLong())
            {
                throw new ParserException(string.Concat(new object[] { "indexName name is too long. max length is 30, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
            }
            this._tokenList.match(1);
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.OnToken);
            string str3 = this._tokenList.lookup(0).value;
            this._tokenList.match(1);
            SqlCreateIndexStmt stmt = new SqlCreateIndexStmt(indexName, 1);
            stmt.SetCreateIndexWord(createWord);
            stmt.SetOnWord(orgValue);
            stmt.tableName = str3;
            SqlExprParser parser = new SqlExprParser(this._tokenList);
            do
            {
                this._tokenList.match();
                stmt.itemList.Add(this.parseOrderByMode(parser));
            }
            while (this._tokenList.lookup(0).Equals(Token.CommaToken));
            this._tokenList.match(Token.CloseBraceToken);
            return stmt;
        }

        private SqlStmt parseCreateIndex()
        {
            return this.parseCreateIndex("CREATE");
        }

        private SqlStmt parseCreateIndex(string createWord)
        {
            createWord = createWord + " " + this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match();
            string indexName = this._tokenList.lookup(0).value;
            if ((indexName.Length > 30) && this._tokenList.getSqlParser().isThrowExWhenNameTooLong())
            {
                throw new ParserException(string.Concat(new object[] { "indexName name is too long. max length is 30, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
            }
            this._tokenList.match(1);
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.OnToken);
            string str3 = this._tokenList.lookup(0).value;
            this._tokenList.match(1);
            SqlCreateIndexStmt stmt = new SqlCreateIndexStmt(indexName);
            stmt.SetCreateIndexWord(createWord);
            stmt.SetOnWord(orgValue);
            stmt.tableName = str3;
            SqlExprParser parser = new SqlExprParser(this._tokenList);
            do
            {
                this._tokenList.match();
                stmt.itemList.Add(this.parseOrderByMode(parser));
            }
            while (this._tokenList.lookup(0).Equals(Token.CommaToken));
            this._tokenList.match(Token.CloseBraceToken);
            return stmt;
        }

        private SqlStmt parseCreateUniqueIndex()
        {
            return this.parseCreateUniqueIndex("CREATE");
        }

        private SqlStmt parseCreateUniqueIndex(string createWord)
        {
            int indexStmtEnum = 0;
            createWord = createWord + " " + this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match();
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            if ("CLUSTERED".EqualsIgnoreCase(orgValue))
            {
                createWord = createWord + " " + orgValue;
                this._tokenList.match();
                indexStmtEnum = 10;
            }
            else if ("NONCLUSTERED".EqualsIgnoreCase(orgValue))
            {
                createWord = createWord + " " + orgValue;
                this._tokenList.match();
            }
            createWord = createWord + " " + this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.IndexToken);
            orgValue = this._tokenList.lookup(0).value;
            if ((orgValue.Length > 30) && this._tokenList.getSqlParser().isThrowExWhenNameTooLong())
            {
                throw new ParserException(string.Concat(new object[] { "indexName name is too long. max length is 30, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
            }
            this._tokenList.match(1);
            string onWord = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.OnToken);
            string str3 = this._tokenList.lookup(0).value;
            this._tokenList.match(1);
            SqlCreateIndexStmt stmt = new SqlCreateIndexStmt(orgValue, indexStmtEnum);
            stmt.SetCreateIndexWord(createWord);
            stmt.SetOnWord(onWord);
            stmt.tableName = str3;
            SqlExprParser parser = new SqlExprParser(this._tokenList);
            do
            {
                this._tokenList.match();
                stmt.itemList.Add(this.parseOrderByMode(parser));
            }
            while (this._tokenList.lookup(0).Equals(Token.CommaToken));
            this._tokenList.match(Token.CloseBraceToken);
            return stmt;
        }

        public SqlIfStmt parseIf()
        {
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match(Token.IfToken);
            SqlExprParser parser = new SqlExprParser(this._tokenList);
            SqlIfStmt stmt = new SqlIfStmt(parser.expr());
            stmt.setIfWord(orgValue);
            if (this._tokenList.lookup(0).Equals(Token.BeginToken))
            {
                stmt.setIfWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                this.stmtList(stmt.trueStmtList, Token.EndToken);
                stmt.setEndWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(Token.EndToken);
            }
            else
            {
                SqlStmt stmt2 = this.stmt();
                stmt.trueStmtList.Add(stmt2);
            }
            if (this._tokenList.lookup(0).Equals(Token.ElseToken))
            {
                stmt.setElseWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.BeginToken))
                {
                    stmt.setElseBeginWord(this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match();
                    this.stmtList(stmt.falseStmtList, Token.EndToken);
                    stmt.setElseEndWord(this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match(Token.EndToken);
                    return stmt;
                }
                SqlStmt stmt3 = this.stmt();
                stmt.falseStmtList.Add(stmt3);
            }
            return stmt;
        }

        private SqlOrderByItem parseOrderByMode(SqlExprParser parser)
        {
            SqlOrderByItem item;
            string orgChineseOrderByType = null;
            int chineseOrderByMode = -1;
            SqlExpr expr = null;
            expr = parser.expr();
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
                return item;
            }
            if (this._tokenList.lookup(0).Equals(Token.DescToken))
            {
                item = new SqlOrderByItem(expr, 1, chineseOrderByMode);
                item.setOrgOrderByName(this._tokenList.lookup(0).GetOrgValue());
                item.setOrgChineseOrderByType(orgChineseOrderByType);
                this._tokenList.match();
                return item;
            }
            item = new SqlOrderByItem(expr, 0, chineseOrderByMode);
            item.setOrgOrderByName("");
            item.setOrgChineseOrderByType(orgChineseOrderByType);
            return item;
        }

        public SqlStmt parseSet()
        {
            this._tokenList.match(Token.SetToken);
            if (this._tokenList.lookup(0).type != 2)
            {
                throw new ParserException("not support token:" + this._tokenList.lookup(0), 0, 0);
            }
            string text = this._tokenList.lookup(0).value;
            this._tokenList.match();
            SqlVarRefExpr variant = new SqlVarRefExpr(text);
            this._tokenList.match(Token.EqualToken);
            SqlExpr expr2 = new SqlExprParser(this._tokenList).expr();
            return new SqlSetLocalVariantStmt(variant, expr2);
        }

        public SqlWhileStmt parseWhile()
        {
            this._tokenList.match(Token.WhileToken);
            SqlExprParser parser = new SqlExprParser(this._tokenList);
            SqlWhileStmt stmt = new SqlWhileStmt(parser.expr());
            if (stmt.stmtList == null)
            {
                stmt.stmtList = new ArrayList();
            }
            if (this._tokenList.lookup(0).Equals(Token.BeginToken))
            {
                this._tokenList.match();
                this.stmtList(stmt.stmtList, Token.EndToken);
                this._tokenList.match(Token.EndToken);
                return stmt;
            }
            this._tokenList.match();
            SqlStmt stmt2 = this.stmt();
            stmt.stmtList.Add(stmt2);
            return stmt;
        }

        public SqlSelectBase select()
        {
            SelectParser parser = new SelectParser(this._tokenList);
            return parser.select();
        }

        public SqlStmt stmt()
        {
            if (this._tokenList.lookup(0).Equals(Token.SelectToken) || this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
            {
                return new SqlSelectStmt(this.select());
            }
            if (this._tokenList.lookup(0).Equals(Token.InsertToken))
            {
                InsertParser parser = new InsertParser(this._tokenList);
                return new SqlInsertStmt(parser.insert());
            }
            if (this._tokenList.lookup(0).Equals(Token.DeleteToken))
            {
                DeleteParser parser2 = new DeleteParser(this._tokenList);
                return new SqlDeleteStmt(parser2.parse_delete());
            }
            if (this._tokenList.lookup(0).Equals(Token.MergeToken))
            {
                MergeParser parser3 = new MergeParser(this._tokenList);
                return new SqlMergeStmt(parser3.Merge());
            }
            if (this._tokenList.lookup(0).Equals(Token.UpdateToken))
            {
                return new SqlUpdateStmt(this.update());
            }
            if (this._tokenList.lookup(0).Equals(Token.IfToken))
            {
                return this.parseIf();
            }
            if (this._tokenList.lookup(0).Equals(Token.WhileToken))
            {
                return this.parseWhile();
            }
            if (this._tokenList.lookup(0).Equals(Token.SetToken))
            {
                return this.parseSet();
            }
            if (this._tokenList.lookup(0).Equals(Token.CreateToken))
            {
                return this.parseCreate();
            }
            if (this._tokenList.lookup(0).Equals(Token.AlterToken))
            {
                if (this._tokenList.lookup(1).Equals(Token.TableToken))
                {
                    AlterTableParser parser4 = new AlterTableParser(this._tokenList);
                    return parser4.parse();
                }
                if (!this._tokenList.lookup(1).Equals(Token.ViewToken))
                {
                    throw new ParserException("not support token:" + this._tokenList.lookup(1));
                }
                this._tokenList.match();
                this._tokenList.match();
                string name = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                this._tokenList.match(Token.AsToken);
                SelectParser parser5 = new SelectParser(this._tokenList);
                return new SqlAlterViewStmt(parser5.select(), name);
            }
            if (this._tokenList.lookup(0).Equals(Token.CursorLoopToken))
            {
                return this.ksql_cursor_loop();
            }
            if (this._tokenList.lookup(0).Equals(Token.KSqlFetchToken))
            {
                return this.ksql_fetch();
            }
            if (this._tokenList.lookup(0).Equals(Token.DropToken))
            {
                return this.drop();
            }
            if (this._tokenList.lookup(0).Equals(Token.DeclareToken) || this._tokenList.lookup(0).Equals(Token.BeginToken))
            {
                SqlBlockParser parser6 = new SqlBlockParser(this._tokenList);
                return parser6.block();
            }
            if (this._tokenList.lookup(0).Equals(Token.KSqlBlockToken))
            {
                SqlBlockParser parser7 = new SqlBlockParser(this._tokenList);
                return parser7.block();
            }
            if (this._tokenList.lookup(0).Equals(Token.CloseToken))
            {
                this._tokenList.match();
                SqlCloseStmt stmt = new SqlCloseStmt
                {
                    curName = this._tokenList.lookup(0).value
                };
                this._tokenList.match();
                return stmt;
            }
            if (this._tokenList.lookup(0).Equals(Token.OpenToken))
            {
                this._tokenList.match();
                SqlOpenStmt stmt2 = new SqlOpenStmt
                {
                    curName = this._tokenList.lookup(0).value
                };
                this._tokenList.match();
                return stmt2;
            }
            if (this._tokenList.lookup(0).Equals(Token.DeallocateToken))
            {
                this._tokenList.match();
                SqlDeallocateStmt stmt3 = new SqlDeallocateStmt
                {
                    curName = this._tokenList.lookup(0).value
                };
                this._tokenList.match();
                return stmt3;
            }
            if (this._tokenList.lookup(0).Equals(Token.BreakToken))
            {
                this._tokenList.match();
                return new SqlBreakStmt();
            }
            if (this._tokenList.lookup(0).Equals(Token.ContinueToken))
            {
                this._tokenList.match();
                return new SqlContinueStmt();
            }
            if (this._tokenList.lookup(0).Equals(Token.GotoToken))
            {
                this._tokenList.match();
                SqlGotoStmt stmt4 = new SqlGotoStmt
                {
                    name = this._tokenList.lookup(0).value
                };
                this._tokenList.match();
                return stmt4;
            }
            if (this._tokenList.lookup(0).Equals(Token.LabelToken))
            {
                this._tokenList.match();
                SqlLabelStmt stmt5 = new SqlLabelStmt
                {
                    name = this._tokenList.lookup(0).value
                };
                this._tokenList.match();
                this._tokenList.match(Token.ColonToken);
                if (this._tokenList.lookup(0).Equals(Token.NullToken))
                {
                    this._tokenList.match();
                    this._tokenList.match(Token.SemicolonToken);
                    stmt5.isNullLable = true;
                }
                return stmt5;
            }
            if (this._tokenList.lookup(0).Equals(Token.ExecToken) || this._tokenList.lookup(0).Equals(Token.ExecuteToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                SqlExecStmt stmt6 = new SqlExecStmt
                {
                    processName = this._tokenList.lookup(0).value
                };
                stmt6.setOrgProcessName(this._tokenList.lookup(0).GetOrgValue());
                stmt6.setExecWord(orgValue);
                this._tokenList.match(1);
                if (this._tokenList.lookup(0).Equals(Token.PeriodToken))
                {
                    this._tokenList.match();
                    stmt6.processName = stmt6.processName + "." + this._tokenList.lookup(0).value;
                    stmt6.setOrgProcessName(stmt6.getOrgProcessName() + "." + this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match(1);
                }
                new SqlExprParser(this._tokenList).exprList(stmt6.paramList);
                return stmt6;
            }
            if (this._tokenList.lookup(0).Equals(Token.TruncateToken))
            {
                this._tokenList.match();
                this._tokenList.match(Token.TableToken);
                StringBuilder builder = new StringBuilder();
                builder.Append(this._tokenList.lookup(0).value);
                this._tokenList.match(1);
                while (this._tokenList.lookup(0).Equals(Token.PeriodToken))
                {
                    this._tokenList.match();
                    builder.Append(this._tokenList.lookup(0).value);
                    this._tokenList.match(1);
                }
                return new SqlTrancateTableStmt(builder.ToString());
            }
            if (this._tokenList.lookup(0).Equals(Token.OpenCurlyBraceToken))
            {
                this._tokenList.match();
                CallStmt stmt7 = new CallStmt();
                if (this._tokenList.lookup(0).type == 2)
                {
                    if (!this._tokenList.lookup(0).value.Equals("?"))
                    {
                        throw new ParserException("not support token:" + this._tokenList.lookup(0));
                    }
                    stmt7.returnExpr = new SqlVarRefExpr("?");
                    this._tokenList.match();
                    this._tokenList.match(Token.EqualToken);
                }
                this._tokenList.match(Token.CallToken);
                string str3 = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                if (this._tokenList.lookup(0).Equals(Token.PeriodToken))
                {
                    this._tokenList.match();
                    str3 = str3 + '.' + this._tokenList.lookup(0).value;
                    this._tokenList.match(1);
                }
                stmt7.procName = str3;
                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    this._tokenList.match();
                    new SqlExprParser(this._tokenList).exprList(stmt7.paramList);
                    this._tokenList.match(Token.CloseBraceToken);
                }
                this._tokenList.match(Token.CloseCurlyBraceToken);
                return stmt7;
            }
            if ((this._tokenList.lookup(0).type == 1) && "show".EqualsIgnoreCase(this._tokenList.lookup(0).value))
            {
                this._tokenList.match();
                if ((this._tokenList.lookup(0).type == 1) && "tables".EqualsIgnoreCase(this._tokenList.lookup(0).value))
                {
                    this._tokenList.match();
                    return new SqlShowTablesStmt();
                }
                if ((this._tokenList.lookup(0).type == 1) && "columns".EqualsIgnoreCase(this._tokenList.lookup(0).value))
                {
                    this._tokenList.match();
                    this._tokenList.match(Token.FromToken);
                    if (((this._tokenList.lookup(0).type == 1) || (this._tokenList.lookup(0).type == 6)) || (this._tokenList.lookup(0).type == 7))
                    {
                        string tableName = this._tokenList.lookup(0).value;
                        this._tokenList.match();
                        return new SqlShowColumnsStmt(tableName);
                    }
                    if (this._tokenList.lookup(0).Equals(Token.NullToken))
                    {
                        this._tokenList.match();
                        return new SqlShowColumnsStmt(null);
                    }
                }
                else
                {
                    int type = this._tokenList.lookup(0).type;
                }
            }
            throw new ParserException(string.Concat(new object[] { "unexcept token. token is : '", this._tokenList.lookup(0).value, "', at line ", this._tokenList.lookup(0).beginLine, " column ", this._tokenList.lookup(0).beginColumn, ", token type is '", this._tokenList.lookup(0).Typename(), "'" }));
        }

        public void stmtList(IList stmtCol, Token tok)
        {
            SqlStmt stmt = this.stmt();
            if (stmt != null)
            {
                stmtCol.Add(stmt);
            }
            if (this._tokenList.lookup(0).Equals(Token.SemicolonToken))
            {
                this._tokenList.match();
            }
            while (!this._tokenList.lookup(0).Equals(tok) && (this._tokenList.lookup(0).type != 12))
            {
                stmt = this.stmt();
                if (this._tokenList.lookup(0).Equals(Token.SemicolonToken))
                {
                    this._tokenList.match();
                }
                if (stmt == null)
                {
                    throw new ParserException("Error", 0, 0);
                }
                stmtCol.Add(stmt);
            }
        }

        public SqlUpdate update()
        {
            UpdateParser parser = new UpdateParser(this._tokenList);
            return parser.update();
        }
    }





}
