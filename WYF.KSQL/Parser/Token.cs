using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Parser
{
    public class Token
    {
        // Fields
        public static Token AddToken = new Token("ADD", 3, 0, 0);
        public static Token AllToken = new Token("ALL", 3, 0, 0);
        public static Token AlterToken = new Token("ALTER", 3, 0, 0);
        public static Token AndToken = new Token("AND", 3, 0, 0);
        public static Token AnyToken = new Token("ANY", 3, 0, 0);
        public static Token AscToken = new Token("ASC", 3, 0, 0);
        public static Token AsToken = new Token("AS", 3, 0, 0);
        public static Token AuthorizationToken = new Token("AUTHORIZATION", 3, 0, 0);
        public static Token BackupToken = new Token("BACKUP", 3, 0, 0);
        public int beginColumn;
        public int beginLine;
        public static Token BeginToken = new Token("BEGIN", 3, 0, 0);
        public static Token BetweenToken = new Token("BETWEEN", 3, 0, 0);
        public static Token BitwiseAndToken = new Token("&", 4, 0, 0);
        public static Token BitwiseOrToken = new Token("|", 4, 0, 0);
        public static Token BreakToken = new Token("BREAK", 3, 0, 0);
        public static Token BrowseToken = new Token("BROWSE", 3, 0, 0);
        public static Token BulkToken = new Token("BULK", 3, 0, 0);
        public static Token ByToken = new Token("BY", 3, 0, 0);
        public static Token CallToken = new Token("CALL", 3, 0, 0);
        public static Token CascadeToken = new Token("CASCADE", 3, 0, 0);
        public static Token CaseToken = new Token("CASE", 3, 0, 0);
        public static Token CheckpointToken = new Token("CHECKPOINT", 3, 0, 0);
        public static Token CheckToken = new Token("CHECK", 3, 0, 0);
        public static Token CloseBraceToken = new Token(")", 5, 0, 0);
        public static Token CloseCurlyBraceToken = new Token("}", 5, 0, 0);
        public static Token CloseSquareBraceToken = new Token("]", 5, 0, 0);
        public static Token CloseToken = new Token("CLOSE", 3, 0, 0);
        public static Token ClusteredToken = new Token("CLUSTERED", 3, 0, 0);
        public static Token CoalesceToken = new Token("COALESCE", 3, 0, 0);
        public static Token CollateToken = new Token("COLLATE", 3, 0, 0);
        public static Token ColonToken = new Token(":", 5, 0, 0);
        public static Token ColumnToken = new Token("COLUMN", 3, 0, 0);
        public static Token CommaToken = new Token(",", 5, 0, 0);
        public static Token CommitToken = new Token("COMMIT", 3, 0, 0);
        public static Token ComputeToken = new Token("COMPUTE", 3, 0, 0);
        public static Token ConcatOpToken = new Token("||", 4, 0, 0);
        public static Token ConnectToken = new Token("CONNECT", 3, 0, 0);
        public static Token ConstraintToken = new Token("CONSTRAINT", 3, 0, 0);
        public static Token ContainstableToken = new Token("CONTAINSTABLE", 3, 0, 0);
        public static Token ContainsToken = new Token("CONTAINS", 3, 0, 0);
        public static Token ContinueToken = new Token("CONTINUE", 3, 0, 0);
        public static Token ConvertToken = new Token("CONVERT", 3, 0, 0);
        public static Token CreateToken = new Token("CREATE", 3, 0, 0);
        public static Token CrossToken = new Token("CROSS", 3, 0, 0);
        public static Token Current_DateToken = new Token("CURRENT_DATE", 3, 0, 0);
        public static Token Current_TimestampToken = new Token("CURRENT_TIMESTAMP", 3, 0, 0);
        public static Token Current_TimeToken = new Token("CURRENT_TIME", 3, 0, 0);
        public static Token Current_UserToken = new Token("CURRENT_USER", 3, 0, 0);
        public static Token CurrentToken = new Token("CURRENT", 3, 0, 0);
        public static Token CursorLoopToken = new Token("CURSOR_LOOP", 3, 0, 0);
        public static Token CursorToken = new Token("CURSOR", 3, 0, 0);
        public static Token DatabaseToken = new Token("DATABASE", 3, 0, 0);
        public static Token DateToken = new Token("DATE", 3, 0, 0);
        public static Token DBCCToken = new Token("DBCC", 3, 0, 0);
        public static Token DeallocateToken = new Token("DEALLOCATE", 3, 0, 0);
        public static Token DeclareToken = new Token("DECLARE", 3, 0, 0);
        public static Token DefaultToken = new Token("DEFAULT", 3, 0, 0);
        public static Token DeleteToken = new Token("DELETE", 3, 0, 0);
        public static Token DenyToken = new Token("DENY", 3, 0, 0);
        public static Token DescToken = new Token("DESC", 3, 0, 0);
        public static Token DiskToken = new Token("DISK", 3, 0, 0);
        public static Token DistinctToken = new Token("DISTINCT", 3, 0, 0);
        public static Token DistributedToken = new Token("DISTRIBUTED", 3, 0, 0);
        public static Token DivToken = new Token("/", 4, 0, 0);
        public static Token DoToken = new Token("DO", 3, 0, 0);
        public static Token DoubleToken = new Token("DOUBLE", 3, 0, 0);
        public static Token DropToken = new Token("DROP", 3, 0, 0);
        public static Token DummyToken = new Token("DUMMY", 3, 0, 0);
        public static Token DumpToken = new Token("DUMP", 3, 0, 0);
        public static Token ElseToken = new Token("ELSE", 3, 0, 0);
        public static Token EmptyToken = new Token("EMPTY", 3, 0, 0);
        public int endColumn;
        public int endLine;
        public static Token EndToken = new Token("END", 3, 0, 0);
        public static Token EOFToken = new Token("", 12, 0, 0);
        public static Token EqualToken = new Token("=", 4, 0, 0);
        public static Token ErrLvlToken = new Token("ERRLVL", 3, 0, 0);
        public static Token EscapeToken = new Token("ESCAPE", 3, 0, 0);
        public static Token ExceptToken = new Token("EXCEPT", 3, 0, 0);
        public static Token ExecToken = new Token("EXEC", 3, 0, 0);
        public static Token ExecuteToken = new Token("EXECUTE", 3, 0, 0);
        public static Token ExistsToken = new Token("EXISTS", 3, 0, 0);
        public static Token ExitToken = new Token("EXIT", 3, 0, 0);
        public static Token FalseToken = new Token("FALSE", 3, 0, 0);
        public static Token FetchToken = new Token("FETCH", 3, 0, 0);
        public static Token FileToken = new Token("FILE", 3, 0, 0);
        public static Token FillFactorToken = new Token("FILLFACTOR", 3, 0, 0);
        public static Token ForeignToken = new Token("FOREIGN", 3, 0, 0);
        public static Token ForToken = new Token("FOR", 3, 0, 0);
        public static Token FreeTextTableToken = new Token("FREETEXTTABLE", 3, 0, 0);
        public static Token FreeTextToken = new Token("FREETEXT", 3, 0, 0);
        public static Token FromToken = new Token("FROM", 3, 0, 0);
        public static Token FullToken = new Token("FULL", 3, 0, 0);
        public static Token FunctionToken = new Token("FUNCTION", 3, 0, 0);
        public static Token GotoToken = new Token("GOTO", 3, 0, 0);
        public static Token GrantToken = new Token("GRANT", 3, 0, 0);
        public static Token GreaterThanOrEqualToken = new Token(">=", 4, 0, 0);
        public static Token GreaterThanToken = new Token(">", 4, 0, 0);
        public static Token GroupToken = new Token("GROUP", 3, 0, 0);
        public static Token HavingToken = new Token("HAVING", 3, 0, 0);
        public static Token HoldLockToken = new Token("HOLDLOCK", 3, 0, 0);
        public static Token Identity_InsertToken = new Token("IDENTITY_INSERT", 3, 0, 0);
        public static Token IdentityColToken = new Token("IDENTITYCOL", 3, 0, 0);
        public static Token IdentityToken = new Token("IDENTITY", 3, 0, 0);
        public static Token IfToken = new Token("IF", 3, 0, 0);
        public static Token INDCOLUMNS = new Token("KSQL_INDCOLUMNS", 3, 0, 0);
        public static Token IndexToken = new Token("INDEX", 3, 0, 0);
        public static Token INDNAME = new Token("KSQL_INDNAME", 3, 0, 0);
        public static Token InnerToken = new Token("INNER", 3, 0, 0);
        public static Token InsertToken = new Token("INSERT", 3, 0, 0);
        public static Token IntersectToken = new Token("INTERSECT", 3, 0, 0);
        public static Token InToken = new Token("IN", 3, 0, 0);
        public static Token IntoToken = new Token("INTO", 3, 0, 0);
        public static Token IsToken = new Token("IS", 3, 0, 0);
        public static Token JoinToken = new Token("JOIN", 3, 0, 0);
        public static Token KeyToken = new Token("KEY", 3, 0, 0);
        public static Token KillToken = new Token("KILL", 3, 0, 0);
        public static Token KSQL_COL_DEFAULT = new Token("KSQL_COL_DEFAULT", 3, 0, 0);
        public static Token KSQL_COL_NAME = new Token("KSQL_COL_NAME", 3, 0, 0);
        public static Token KSQL_COL_NULLABLE = new Token("KSQL_COL_NULLABLE", 3, 0, 0);
        public static Token KSQL_COL_TABNAME = new Token("KSQL_COL_TABNAME", 3, 0, 0);
        public static Token KSQL_CONS_NAME = new Token("KSQL_CONS_NAME", 3, 0, 0);
        public static Token KSQL_CONS_TABNAME = new Token("KSQL_CONS_TABNAME", 3, 0, 0);
        public static Token KSQL_CONS_TYPE = new Token("KSQL_CONS_TYPE", 3, 0, 0);
        public static Token KSQL_CT_C = new Token("KSQL_CT_C", 3, 0, 0);
        public static Token KSQL_CT_F = new Token("KSQL_CT_F", 3, 0, 0);
        public static Token KSQL_CT_P = new Token("KSQL_CT_P", 3, 0, 0);
        public static Token KSQL_CT_U = new Token("KSQL_CT_U", 3, 0, 0);
        public static Token KSqlBlockToken = new Token("KSQL_BLOCK", 3, 0, 0);
        public static Token KSqlCursorLoopToken = new Token("KSQL_CURSOR_LOOP", 3, 0, 0);
        public static Token KSqlFetchToken = new Token("KSQL_FETCH", 3, 0, 0);
        public static Token LabelToken = new Token("LABEL", 3, 0, 0);
        public static Token LeftToken = new Token("LEFT", 3, 0, 0);
        public static Token LessThanOrEqualToken = new Token("<=", 4, 0, 0);
        public static Token LessThanOrGreaterThanToken = new Token("<>", 4, 0, 0);
        public static Token LessThanToken = new Token("<", 4, 0, 0);
        public static Token LikeToken = new Token("LIKE", 3, 0, 0);
        public static Token LineNoToken = new Token("LINENO", 3, 0, 0);
        public static Token LoadToken = new Token("LOAD", 3, 0, 0);
        public static Token LoopToken = new Token("LOOP", 3, 0, 0);
        public static Token MatchedToken = new Token("MATCHED", 3, 0, 0);
        public static Token MergeToken = new Token("MERGE", 3, 0, 0);
        public static Token MinusToken = new Token("-", 4, 0, 0);
        public static Token ModToken = new Token("%", 4, 0, 0);
        public static Token MulToken = new Token("*", 4, 0, 0);
        public static Token NationalToken = new Token("NATIONAL", 3, 0, 0);
        public static Token New = new Token("NEW", 3, 0, 0);
        public static Token NoCheckToken = new Token("NOCHECK", 3, 0, 0);
        public static Token NonClusteredToken = new Token("NONCLUSTERED", 3, 0, 0);
        public static Token NotEqualToken = new Token("!=", 4, 0, 0);
        public static Token NotGreaterThanToken = new Token("!>", 4, 0, 0);
        public static Token NotLessThanToken = new Token("!<", 4, 0, 0);
        public static Token NotToken = new Token("NOT", 3, 0, 0);
        public static Token NullIfToken = new Token("NULLIF", 3, 0, 0);
        public static Token NullToken = new Token("NULL", 3, 0, 0);
        public static Token OffSetsToken = new Token("OFFSETS", 3, 0, 0);
        public static Token OffToken = new Token("OFF", 3, 0, 0);
        public static Token OfToken = new Token("OF", 3, 0, 0);
        public static Token OnToken = new Token("ON", 3, 0, 0);
        public static Token OpenBraceToken = new Token("(", 5, 0, 0);
        public static Token OpenCurlyBraceToken = new Token("{", 5, 0, 0);
        public static Token OpenDataSourceToken = new Token("OPENDATASOURCE", 3, 0, 0);
        public static Token OpenQueryToken = new Token("OPENQUERY", 3, 0, 0);
        public static Token OpenRowSetToken = new Token("OPENROWSET", 3, 0, 0);
        public static Token OpenSquareBraceToken = new Token("[", 5, 0, 0);
        public static Token OpenToken = new Token("OPEN", 3, 0, 0);
        public static Token OpenXmlToken = new Token("OPENXML", 3, 0, 0);
        public static Token OptionToken = new Token("OPTION", 3, 0, 0);
        public static Token OrderToken = new Token("ORDER", 3, 0, 0);
        private string orgValue;
        public static Token OrToken = new Token("OR", 3, 0, 0);
        public static Token OuterToken = new Token("OUTER", 3, 0, 0);
        public static Token OverToken = new Token("OVER", 3, 0, 0);
        public static Token PartitionToken = new Token("PARTITION", 3, 0, 0);
        public static Token PercentToken = new Token("PERCENT", 3, 0, 0);
        public static Token PeriodToken = new Token(".", 5, 0, 0);
        public static Token PERSISTEDToken = new Token("PERSISTED", 3, 0, 0);
        public static Token PinYinToken = new Token("SCHINESE_PINYIN", 3, 0, 0);
        public static Token PlanToken = new Token("PLAN", 3, 0, 0);
        public static Token PlusToken = new Token("+", 4, 0, 0);
        public int position;
        public static Token PrecisionToken = new Token("PRECISION", 3, 0, 0);
        public static Token PrimaryToken = new Token("PRIMARY", 3, 0, 0);
        public static Token PrintToken = new Token("PRINT", 3, 0, 0);
        public static Token PriorToken = new Token("PRIOR", 3, 0, 0);
        public static Token ProcedureToken = new Token("PROCEDURE", 3, 0, 0);
        public static Token ProcToken = new Token("PROC", 3, 0, 0);
        public static Token PublicToken = new Token("PUBLIC", 3, 0, 0);
        public static Token RadicalToken = new Token("SCHINESE_RADICAL", 3, 0, 0);
        public static Token RaisErrorToken = new Token("RAISERROR", 3, 0, 0);
        public static Token ReadPastToken = new Token("READPAST", 3, 0, 0);
        public static Token ReadTextToken = new Token("READTEXT", 3, 0, 0);
        public static Token ReadToken = new Token("READ", 3, 0, 0);
        public static Token ReConfigureToken = new Token("RECONFIGURE", 3, 0, 0);
        public static Token ReferencesToken = new Token("REFERENCES", 3, 0, 0);
        public static Token ReplicationToken = new Token("REPLICATION", 3, 0, 0);
        public static Token RestoreToken = new Token("RESTORE", 3, 0, 0);
        public static Token ReturnToken = new Token("RETURN", 3, 0, 0);
        public static Token RevokeToken = new Token("REVOKE", 3, 0, 0);
        public static Token RightToken = new Token("RIGHT", 3, 0, 0);
        public static Token RollBackToken = new Token("ROLLBACK", 3, 0, 0);
        public static Token RollUpToken = new Token("ROLLUP", 3, 0, 0);
        public static Token RowCountToken = new Token("ROWCOUNT", 3, 0, 0);
        public static Token RowGuidColToken = new Token("ROWGUIDCOL", 3, 0, 0);
        public static Token RuleToken = new Token("RULE", 3, 0, 0);
        public static Token SaveToken = new Token("SAVE", 3, 0, 0);
        public static Token SchemaToken = new Token("SCHEMA", 3, 0, 0);
        public static Token SelectIntoToken = new Token("INTO", 3, 0, 0);
        public static Token SelectToken = new Token("SELECT", 3, 0, 0);
        public static Token SemicolonToken = new Token(";", 5, 0, 0);
        public static Token Session_User = new Token("SESSION_USER", 3, 0, 0);
        public static Token SetToken = new Token("SET", 3, 0, 0);
        public static Token SetUserToken = new Token("SETUSER", 3, 0, 0);
        public static Token ShutdownToken = new Token("SHUTDOWN", 3, 0, 0);
        public static Token SomeToken = new Token("SOME", 3, 0, 0);
        public static Token StartToken = new Token("START", 3, 0, 0);
        public static Token StatisticsToken = new Token("STATISTICS", 3, 0, 0);
        public static Token StrokeToken = new Token("SCHINESE_STROKE", 3, 0, 0);
        public static Token SYSCONSTRAINTS = new Token("KSQL_CONSTRAINTS", 3, 0, 0);
        public static Token SYSINDEXES = new Token("KSQL_INDEXES", 3, 0, 0);
        public static Token System_UserToken = new Token("SYSTEM_USER", 3, 0, 0);
        public static Token TABLECOLUMNDEFAULTVALUE = new Token("KSQL_TABLECOLUMNDEFAULTVALUE", 3, 0, 0);
        public static Token TableToken = new Token("TABLE", 3, 0, 0);
        public static Token TABNAME = new Token("KSQL_TABNAME", 3, 0, 0);
        public static Token TextsizeToken = new Token("TEXTSIZE", 3, 0, 0);
        public static Token ThenToken = new Token("THEN", 3, 0, 0);
        public static Token TimeStampToken = new Token("TIMESTAMP", 3, 0, 0);
        public static Token TimeToken = new Token("TIME", 3, 0, 0);
        public static Token TopToken = new Token("TOP", 3, 0, 0);
        public static Token ToToken = new Token("TO", 3, 0, 0);
        public static Token TransactionToken = new Token("TRANSACTION", 3, 0, 0);
        public static Token TranToken = new Token("TRAN", 3, 0, 0);
        public static Token TriggerToken = new Token("TRIGGER", 3, 0, 0);
        public static Token TrueToken = new Token("TRUE", 3, 0, 0);
        public static Token TruncateToken = new Token("TRUNCATE", 3, 0, 0);
        public static Token TsequalToken = new Token("TSEQUAL", 3, 0, 0);
        public int type;
        public static Token UnionToken = new Token("UNION", 3, 0, 0);
        public static Token UniqueToken = new Token("UNIQUE", 3, 0, 0);
        public static Token UpdateTextToken = new Token("UPDATETEXT", 3, 0, 0);
        public static Token UpdateToken = new Token("UPDATE", 3, 0, 0);
        public static Token USERCOLUMNS = new Token("KSQL_USERCOLUMNS", 3, 0, 0);
        public static Token USERTABLES = new Token("KSQL_USERTABLES", 3, 0, 0);
        public static Token UserToken = new Token("USER", 3, 0, 0);
        public static Token USERVIEWS = new Token("KSQL_USERVIEWS", 3, 0, 0);
        public static Token UseToken = new Token("USE", 3, 0, 0);
        public static Token UsingToken = new Token("USING", 3, 0, 0);
        public string value;
        public static Token ValuesToken = new Token("VALUES", 3, 0, 0);
        public static Token VaryingToken = new Token("VARYING", 3, 0, 0);
        public static Token VIEWNAME = new Token("KSQL_VIEWNAME", 3, 0, 0);
        public static Token ViewToken = new Token("VIEW", 3, 0, 0);
        public static Token WaitForToken = new Token("WAITFOR", 3, 0, 0);
        public static Token WhenToken = new Token("WHEN", 3, 0, 0);
        public static Token WhereToken = new Token("WHERE", 3, 0, 0);
        public static Token WhileToken = new Token("WHILE", 3, 0, 0);
        public static Token WithToken = new Token("WITH", 3, 0, 0);
        public static Token WriteTextToken = new Token("WRITETEXT", 3, 0, 0);
        private string v1;
        private char c;
        private int v2;
        private int _line;
        private int _col;
        private int _ptr;

        // Methods
        public Token()
        {
        }

        public Token(int type, string value)
        {
            this.value = value;
            this.type = type;
        }

        public Token(string value, int type, int line, int col)
        {
            this.value = value;
            this.type = type;
            this.beginLine = line;
            this.beginColumn = col;
            this.SetOrgValue(value);
        }

        public Token(string value, int type, int line, int col, int ptr)
        {
            this.value = value;
            this.type = type;
            this.beginLine = line;
            this.beginColumn = col;
            this.position = ptr;
            this.SetOrgValue(value);
        }

        public Token(string value, string ordValue, int type, int line, int col, int ptr)
        {
            this.value = value;
            this.SetOrgValue(ordValue);
            this.type = type;
            this.beginLine = line;
            this.beginColumn = col;
            this.position = ptr;
        }

        public Token(string v1, char c, int v2, int line, int col, int ptr)
        {
            this.v1 = v1;
            this.c = c;
            this.v2 = v2;
            _line = line;
            _col = col;
            _ptr = ptr;
        }

        public bool Equals(Token tok)
        {
            if (tok != null)
            {
                if (tok.type != this.type)
                {
                    return false;
                }
                switch (tok.type)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 0x11:
                        return (string.Compare(this.value, tok.value, StringComparison.CurrentCultureIgnoreCase) == 0);

                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        return (this.value == tok.value);

                    case 12:
                        return true;

                    case 13:
                        return false;
                }
            }
            return false;
        }

        public string GetOrgValue()
        {
            return this.orgValue;
        }

        public void Output(StringBuilder buff)
        {
            if (this.type == 6)
            {
                buff.Append('\'');
                buff.Append(this.value);
                buff.Append('\'');
            }
            else if (this.type == 7)
            {
                buff.Append("N'");
                buff.Append(this.value);
                buff.Append('\'');
            }
            else
            {
                buff.Append(this.value);
            }
        }

        public void SetOrgValue(string orgValue)
        {
            this.orgValue = orgValue;
        }

        public string toString()
        {
            StringBuilder buff = new StringBuilder();
            this.Output(buff);
            return buff.ToString();
        }

        public string Typename()
        {
            return TokenType.Typename(this.type);
        }
    }





}
