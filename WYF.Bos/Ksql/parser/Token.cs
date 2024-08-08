using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql.parser
{
    public class Token
    {
        public int type;
        public String value;
        public int beginLine;
        public int endLine;
        public int beginColumn;
        public int endColumn;
        public int position;
        public string OrgValue { get; set; }
     


        public Token()
        {
        }

        public Token( int type,  String value)
        {
            this.value = value;
            this.type = type;
        }

        public Token( String value,  int type,  int line,  int col)
        {
            this.value = value;
            this.type = type;
            this.beginLine = line;
            this.beginColumn = col;
            this.OrgValue = value;
        }

        public Token( String value,  int type,  int line,  int col,  int ptr)
        {
            this.value = value;
            this.type = type;
            this.beginLine = line;
            this.beginColumn = col;
            this.position = ptr;
            this.OrgValue = value;
        }

        public Token( String value,  String ordValue,  int type,  int line,  int col,  int ptr)
        {
            this.value = value;
            this.OrgValue = value;
            this.type = type;
            this.beginLine = line;
            this.beginColumn = col;
            this.position = ptr;
        }

     
      public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            this.Output(buff);
            return buff.ToString();
        }

        public  void Output(StringBuilder buff)
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

        public  bool Equals(Token tok)
        {
            if (tok == null)
            {
                return false;
            }
            if (tok.type != this.type)
            {
                return false;
            }
            switch (tok.type)
            {
                case 4:
                case 5:
                case 6:
                case 7:
                    {
                        return this.value == tok.value;
                    }
                case 0:
                case 1:
                case 2:
                case 3:
                    {
                        return this.value.CompareTo(tok.value) == 0;
                        //return this.value.CompareToIgnoreCase(tok.value) == 0;
                    }
                case 8:
                case 9:
                case 10:
                case 11:
                    {
                        //return this.value.compareToIgnoreCase(tok.value) == 0;
                        return this.value.CompareTo(tok.value) == 0;

                    }
                case 12:
                    {
                        return true;
                    }
                case 13:
                    {
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        public  string Typename()
        {
            return TokenType.Typename(this.type);
        }

    


        public static readonly Token EqualToken = new Token("=", 4, 0, 0);
        public static readonly Token NotEqualToken = new Token("!=", 4, 0, 0);
        public static readonly Token LessThanToken = new Token("<", 4, 0, 0);
        public static readonly Token LessThanOrEqualToken = new Token("<=", 4, 0, 0);
        public static readonly Token GreaterThanToken = new Token(">", 4, 0, 0);
        public static readonly Token GreaterThanOrEqualToken = new Token(">=", 4, 0, 0);
        public static readonly Token NotLessThanToken = new Token("!<", 4, 0, 0);
        public static readonly Token NotGreaterThanToken = new Token("!>", 4, 0, 0);
        public static readonly Token LessThanOrGreaterThanToken = new Token("<>", 4, 0, 0);
        public static readonly Token PlusToken = new Token("+", 4, 0, 0);
        public static readonly Token MinusToken = new Token("-", 4, 0, 0);
        public static readonly Token MulToken = new Token("*", 4, 0, 0);
        public static readonly Token DivToken = new Token("/", 4, 0, 0);
        public static readonly Token ModToken = new Token("%", 4, 0, 0);
        public static readonly Token BitwiseAndToken = new Token("&", 4, 0, 0);
        public static readonly Token BitwiseOrToken = new Token("|", 4, 0, 0);
        public static readonly Token ConcatOpToken = new Token("||", 4, 0, 0);
        public static readonly Token OpenCurlyBraceToken = new Token("{", 5, 0, 0);
        public static readonly Token CloseCurlyBraceToken = new Token("}", 5, 0, 0);
        public static readonly Token OpenBraceToken = new Token("(", 5, 0, 0);
        public static readonly Token CloseBraceToken = new Token(")", 5, 0, 0);
        public static readonly Token OpenSquareBraceToken = new Token("[", 5, 0, 0);
        public static readonly Token CloseSquareBraceToken = new Token("]", 5, 0, 0);
        public static readonly Token PeriodToken = new Token(".", 5, 0, 0);
        public static readonly Token CommaToken = new Token(",", 5, 0, 0);
        public static readonly Token ColonToken = new Token(":", 5, 0, 0);
        public static readonly Token SemicolonToken = new Token(";", 5, 0, 0);
        public static readonly Token EOFToken = new Token("", 12, 0, 0);
        public static readonly Token AddToken = new Token("ADD", 3, 0, 0);
        public static readonly Token AllToken = new Token("ALL", 3, 0, 0);
        public static readonly Token AlterToken = new Token("ALTER", 3, 0, 0);
        public static readonly Token AndToken = new Token("AND", 3, 0, 0);
        public static readonly Token AnyToken = new Token("ANY", 3, 0, 0);
        public static readonly Token AsToken = new Token("AS", 3, 0, 0);
        public static readonly Token AscToken = new Token("ASC", 3, 0, 0);
        public static readonly Token AuthorizationToken = new Token("AUTHORIZATION", 3, 0, 0);
        public static readonly Token BackupToken = new Token("BACKUP", 3, 0, 0);
        public static readonly Token BeginToken = new Token("BEGIN", 3, 0, 0);
        public static readonly Token BetweenToken = new Token("BETWEEN", 3, 0, 0);
        public static readonly Token BreakToken = new Token("BREAK", 3, 0, 0);
        public static readonly Token BrowseToken = new Token("BROWSE", 3, 0, 0);
        public static readonly Token BulkToken = new Token("BULK", 3, 0, 0);
        public static readonly Token ByToken = new Token("BY", 3, 0, 0);
        public static readonly Token CascadeToken = new Token("CASCADE", 3, 0, 0);
        public static readonly Token CallToken = new Token("CALL", 3, 0, 0);
        public static readonly Token CaseToken = new Token("CASE", 3, 0, 0);
        public static readonly Token CheckToken = new Token("CHECK", 3, 0, 0);
        public static readonly Token CheckpointToken = new Token("CHECKPOINT", 3, 0, 0);
        public static readonly Token CloseToken = new Token("CLOSE", 3, 0, 0);
        public static readonly Token ClusteredToken = new Token("CLUSTERED", 3, 0, 0);
        public static readonly Token CoalesceToken = new Token("COALESCE", 3, 0, 0);
        public static readonly Token CollateToken = new Token("COLLATE", 3, 0, 0);
        public static readonly Token ColumnToken = new Token("COLUMN", 3, 0, 0);
        public static readonly Token CommitToken = new Token("COMMIT", 3, 0, 0);
        public static readonly Token ComputeToken = new Token("COMPUTE", 3, 0, 0);
        public static readonly Token ConnectToken = new Token("CONNECT", 3, 0, 0);
        public static readonly Token ConstraintToken = new Token("CONSTRAINT", 3, 0, 0);
        public static readonly Token ContainsToken = new Token("CONTAINS", 3, 0, 0);
        public static readonly Token ContainstableToken = new Token("CONTAINSTABLE", 3, 0, 0);
        public static readonly Token ContinueToken = new Token("CONTINUE", 3, 0, 0);
        public static readonly Token ConvertToken = new Token("CONVERT", 3, 0, 0);
        public static readonly Token CreateToken = new Token("CREATE", 3, 0, 0);
        public static readonly Token CrossToken = new Token("CROSS", 3, 0, 0);
        public static readonly Token CurrentToken = new Token("CURRENT", 3, 0, 0);
        public static readonly Token Current_DateToken = new Token("CURRENT_DATE", 3, 0, 0);
        public static readonly Token Current_TimeToken = new Token("CURRENT_TIME", 3, 0, 0);
        public static readonly Token Current_TimestampToken = new Token("CURRENT_TIMESTAMP", 3, 0, 0);
        public static readonly Token Current_UserToken = new Token("CURRENT_USER", 3, 0, 0);
        public static readonly Token CursorToken = new Token("CURSOR", 3, 0, 0);
        public static readonly Token DatabaseToken = new Token("DATABASE", 3, 0, 0);
        public static readonly Token DateToken = new Token("DATE", 3, 0, 0);
        public static readonly Token DBCCToken = new Token("DBCC", 3, 0, 0);
        public static readonly Token DeallocateToken = new Token("DEALLOCATE", 3, 0, 0);
        public static readonly Token DeclareToken = new Token("DECLARE", 3, 0, 0);
        public static readonly Token DefaultToken = new Token("DEFAULT", 3, 0, 0);
        public static readonly Token DeleteToken = new Token("DELETE", 3, 0, 0);
        public static readonly Token DescToken = new Token("DESC", 3, 0, 0);
        public static readonly Token DenyToken = new Token("DENY", 3, 0, 0);
        public static readonly Token DiskToken = new Token("DISK", 3, 0, 0);
        public static readonly Token DistinctToken = new Token("DISTINCT", 3, 0, 0);
        public static readonly Token DistributedToken = new Token("DISTRIBUTED", 3, 0, 0);
        public static readonly Token DoubleToken = new Token("DOUBLE", 3, 0, 0);
        public static readonly Token DoToken = new Token("DO", 3, 0, 0);
        public static readonly Token DropToken = new Token("DROP", 3, 0, 0);
        public static readonly Token DummyToken = new Token("DUMMY", 3, 0, 0);
        public static readonly Token DumpToken = new Token("DUMP", 3, 0, 0);
        public static readonly Token ElseToken = new Token("ELSE", 3, 0, 0);
        public static readonly Token EndToken = new Token("END", 3, 0, 0);
        public static readonly Token ErrLvlToken = new Token("ERRLVL", 3, 0, 0);
        public static readonly Token EscapeToken = new Token("ESCAPE", 3, 0, 0);
        public static readonly Token ExceptToken = new Token("EXCEPT", 3, 0, 0);
        public static readonly Token ExecToken = new Token("EXEC", 3, 0, 0);
        public static readonly Token ExecuteToken = new Token("EXECUTE", 3, 0, 0);
        public static readonly Token ExistsToken = new Token("EXISTS", 3, 0, 0);
        public static readonly Token ExitToken = new Token("EXIT", 3, 0, 0);
        public static readonly Token FalseToken = new Token("FALSE", 3, 0, 0);
        public static readonly Token FetchToken = new Token("FETCH", 3, 0, 0);
        public static readonly Token FileToken = new Token("FILE", 3, 0, 0);
        public static readonly Token FillFactorToken = new Token("FILLFACTOR", 3, 0, 0);
        public static readonly Token ForToken = new Token("FOR", 3, 0, 0);
        public static readonly Token ForeignToken = new Token("FOREIGN", 3, 0, 0);
        public static readonly Token FreeTextToken = new Token("FREETEXT", 3, 0, 0);
        public static readonly Token FreeTextTableToken = new Token("FREETEXTTABLE", 3, 0, 0);
        public static readonly Token FromToken = new Token("FROM", 3, 0, 0);
        public static readonly Token FullToken = new Token("FULL", 3, 0, 0);
        public static readonly Token FunctionToken = new Token("FUNCTION", 3, 0, 0);
        public static readonly Token GotoToken = new Token("GOTO", 3, 0, 0);
        public static readonly Token LabelToken = new Token("LABEL", 3, 0, 0);
        public static readonly Token GrantToken = new Token("GRANT", 3, 0, 0);
        public static readonly Token GroupToken = new Token("GROUP", 3, 0, 0);
        public static readonly Token HavingToken = new Token("HAVING", 3, 0, 0);
        public static readonly Token HoldLockToken = new Token("HOLDLOCK", 3, 0, 0);
        public static readonly Token ReadPastToken = new Token("READPAST", 3, 0, 0);
        public static readonly Token IdentityToken = new Token("IDENTITY", 3, 0, 0);
        public static readonly Token Identity_InsertToken = new Token("IDENTITY_INSERT", 3, 0, 0);
        public static readonly Token IdentityColToken = new Token("IDENTITYCOL", 3, 0, 0);
        public static readonly Token IfToken = new Token("IF", 3, 0, 0);
        public static readonly Token InToken = new Token("IN", 3, 0, 0);
        public static readonly Token XinToken = new Token("XIN", 3, 0, 0);
        public static readonly Token SelectIntoToken = new Token("INTO", 3, 0, 0);
        public static readonly Token IndexToken = new Token("INDEX", 3, 0, 0);
        public static readonly Token InnerToken = new Token("INNER", 3, 0, 0);
        public static readonly Token InsertToken = new Token("INSERT", 3, 0, 0);
        public static readonly Token IntersectToken = new Token("INTERSECT", 3, 0, 0);
        public static readonly Token IsToken = new Token("IS", 3, 0, 0);
        public static readonly Token JoinToken = new Token("JOIN", 3, 0, 0);
        public static readonly Token KeyToken = new Token("KEY", 3, 0, 0);
        public static readonly Token KillToken = new Token("KILL", 3, 0, 0);
        public static readonly Token KSqlBlockToken = new Token("KSQL_BLOCK", 3, 0, 0);
        public static readonly Token KSqlFetchToken = new Token("KSQL_FETCH", 3, 0, 0);
        public static readonly Token KSqlCursorLoopToken = new Token("KSQL_CURSOR_LOOP", 3, 0, 0);
        public static readonly Token CursorLoopToken = new Token("CURSOR_LOOP", 3, 0, 0);
        public static readonly Token LeftToken = new Token("LEFT", 3, 0, 0);
        public static readonly Token LikeToken = new Token("LIKE", 3, 0, 0);
        public static readonly Token MatchToken = new Token("MATCH", 3, 0, 0);
        public static readonly Token LineNoToken = new Token("LINENO", 3, 0, 0);
        public static readonly Token LoopToken = new Token("LOOP", 3, 0, 0);
        public static readonly Token LoadToken = new Token("LOAD", 3, 0, 0);
        public static readonly Token NationalToken = new Token("NATIONAL", 3, 0, 0);
        public static readonly Token NoCheckToken = new Token("NOCHECK", 3, 0, 0);
        public static readonly Token NonClusteredToken = new Token("NONCLUSTERED", 3, 0, 0);
        public static readonly Token NotToken = new Token("NOT", 3, 0, 0);
        public static readonly Token NullToken = new Token("NULL", 3, 0, 0);
        public static readonly Token EmptyToken = new Token("EMPTY", 3, 0, 0);
        public static readonly Token NullIfToken = new Token("NULLIF", 3, 0, 0);
        public static readonly Token OfToken = new Token("OF", 3, 0, 0);
        public static readonly Token OffToken = new Token("OFF", 3, 0, 0);
        public static readonly Token OffSetsToken = new Token("OFFSETS", 3, 0, 0);
        public static readonly Token OnToken = new Token("ON", 3, 0, 0);
        public static readonly Token OpenToken = new Token("OPEN", 3, 0, 0);
        public static readonly Token OpenDataSourceToken = new Token("OPENDATASOURCE", 3, 0, 0);
        public static readonly Token OpenQueryToken = new Token("OPENQUERY", 3, 0, 0);
        public static readonly Token OpenRowSetToken = new Token("OPENROWSET", 3, 0, 0);
        public static readonly Token OpenXmlToken = new Token("OPENXML", 3, 0, 0);
        public static readonly Token OptionToken = new Token("OPTION", 3, 0, 0);
        public static readonly Token OrToken = new Token("OR", 3, 0, 0);
        public static readonly Token OrderToken = new Token("ORDER", 3, 0, 0);
        public static readonly Token OuterToken = new Token("OUTER", 3, 0, 0);
        public static readonly Token OverToken = new Token("OVER", 3, 0, 0);
        public static readonly Token PercentToken = new Token("PERCENT", 3, 0, 0);
        public static readonly Token PlanToken = new Token("PLAN", 3, 0, 0);
        public static readonly Token PrecisionToken = new Token("PRECISION", 3, 0, 0);
        public static readonly Token PrimaryToken = new Token("PRIMARY", 3, 0, 0);
        public static readonly Token PrintToken = new Token("PRINT", 3, 0, 0);
        public static readonly Token PriorToken = new Token("PRIOR", 3, 0, 0);
        public static readonly Token ProcToken = new Token("PROC", 3, 0, 0);
        public static readonly Token ProcedureToken = new Token("PROCEDURE", 3, 0, 0);
        public static readonly Token PublicToken = new Token("PUBLIC", 3, 0, 0);
        public static readonly Token RaisErrorToken = new Token("RAISERROR", 3, 0, 0);
        public static readonly Token ReadToken = new Token("READ", 3, 0, 0);
        public static readonly Token ReadTextToken = new Token("READTEXT", 3, 0, 0);
        public static readonly Token ReConfigureToken = new Token("RECONFIGURE", 3, 0, 0);
        public static readonly Token ReferencesToken = new Token("REFERENCES", 3, 0, 0);
        public static readonly Token ReplicationToken = new Token("REPLICATION", 3, 0, 0);
        public static readonly Token RestoreToken = new Token("RESTORE", 3, 0, 0);
        public static readonly Token ReturnToken = new Token("RETURN", 3, 0, 0);
        public static readonly Token RevokeToken = new Token("REVOKE", 3, 0, 0);
        public static readonly Token RightToken = new Token("RIGHT", 3, 0, 0);
        public static readonly Token RollBackToken = new Token("ROLLBACK", 3, 0, 0);
        public static readonly Token RowCountToken = new Token("ROWCOUNT", 3, 0, 0);
        public static readonly Token RowGuidColToken = new Token("ROWGUIDCOL", 3, 0, 0);
        public static readonly Token RuleToken = new Token("RULE", 3, 0, 0);
        public static readonly Token SaveToken = new Token("SAVE", 3, 0, 0);
        public static readonly Token SchemaToken = new Token("SCHEMA", 3, 0, 0);
        public static readonly Token SelectToken = new Token("SELECT", 3, 0, 0);
        public static readonly Token Session_User = new Token("SESSION_USER", 3, 0, 0);
        public static readonly Token SetToken = new Token("SET", 3, 0, 0);
        public static readonly Token SetUserToken = new Token("SETUSER", 3, 0, 0);
        public static readonly Token ShutdownToken = new Token("SHUTDOWN", 3, 0, 0);
        public static readonly Token SomeToken = new Token("SOME", 3, 0, 0);
        public static readonly Token StartToken = new Token("START", 3, 0, 0);
        public static readonly Token StatisticsToken = new Token("STATISTICS", 3, 0, 0);
        public static readonly Token System_UserToken = new Token("SYSTEM_USER", 3, 0, 0);
        public static readonly Token TableToken = new Token("TABLE", 3, 0, 0);
        public static readonly Token TextsizeToken = new Token("TEXTSIZE", 3, 0, 0);
        public static readonly Token ThenToken = new Token("THEN", 3, 0, 0);
        public static readonly Token TimeToken = new Token("TIME", 3, 0, 0);
        public static readonly Token TimeStampToken = new Token("TIMESTAMP", 3, 0, 0);
        public static readonly Token ToToken = new Token("TO", 3, 0, 0);
        public static readonly Token TopToken = new Token("TOP", 3, 0, 0);
        public static readonly Token TranToken = new Token("TRAN", 3, 0, 0);
        public static readonly Token TransactionToken = new Token("TRANSACTION", 3, 0, 0);
        public static readonly Token TriggerToken = new Token("TRIGGER", 3, 0, 0);
        public static readonly Token TrueToken = new Token("TRUE", 3, 0, 0);
        public static readonly Token TruncateToken = new Token("TRUNCATE", 3, 0, 0);
        public static readonly Token TsequalToken = new Token("TSEQUAL", 3, 0, 0);
        public static readonly Token UnionToken = new Token("UNION", 3, 0, 0);
        public static readonly Token UniqueToken = new Token("UNIQUE", 3, 0, 0);
        public static readonly Token UpdateToken = new Token("UPDATE", 3, 0, 0);
        public static readonly Token IntoToken = new Token("INTO", 3, 0, 0);
        public static readonly Token UpdateTextToken = new Token("UPDATETEXT", 3, 0, 0);
        public static readonly Token UseToken = new Token("USE", 3, 0, 0);
        public static readonly Token UserToken = new Token("USER", 3, 0, 0);
        public static readonly Token ValuesToken = new Token("VALUES", 3, 0, 0);
        public static readonly Token VaryingToken = new Token("VARYING", 3, 0, 0);
        public static readonly Token ViewToken = new Token("VIEW", 3, 0, 0);
        public static readonly Token WaitForToken = new Token("WAITFOR", 3, 0, 0);
        public static readonly Token WhenToken = new Token("WHEN", 3, 0, 0);
        public static readonly Token WhereToken = new Token("WHERE", 3, 0, 0);
        public static readonly Token WhileToken = new Token("WHILE", 3, 0, 0);
        public static readonly Token WithToken = new Token("WITH", 3, 0, 0);
        public static readonly Token WriteTextToken = new Token("WRITETEXT", 3, 0, 0);
        public static readonly Token New = new Token("NEW", 3, 0, 0);
        public static readonly Token MergeToken = new Token("MERGE", 3, 0, 0);
        public static readonly Token UsingToken = new Token("USING", 3, 0, 0);
        public static readonly Token MatchedToken = new Token("MATCHED", 3, 0, 0);
        public static readonly Token USERTABLES = new Token("KSQL_USERTABLES", 3, 0, 0);
        public static readonly Token USERVIEWS = new Token("KSQL_USERVIEWS", 3, 0, 0);
        public static readonly Token USERCOLUMNS = new Token("KSQL_USERCOLUMNS", 3, 0, 0);
        public static readonly Token TABLECOLUMNDEFAULTVALUE = new Token("KSQL_TABLECOLUMNDEFAULTVALUE", 3, 0, 0);
        public static readonly Token TABNAME = new Token("KSQL_TABNAME", 3, 0, 0);
        public static readonly Token VIEWNAME = new Token("KSQL_VIEWNAME", 3, 0, 0);
        public static readonly Token KSQL_CREATETIME = new Token("KSQL_CREATETIME", 3, 0, 0);
        public static readonly Token KSQL_COL_NAME = new Token("KSQL_COL_NAME", 3, 0, 0);
        public static readonly Token KSQL_COL_TABNAME = new Token("KSQL_COL_TABNAME", 3, 0, 0);
        public static readonly Token KSQL_COL_TYPE = new Token("KSQL_COL_TYPE", 3, 0, 0);
        public static readonly Token KSQL_COL_LENGTH = new Token("KSQL_COL_LENGTH", 3, 0, 0);
        public static readonly Token KSQL_COL_NULLABLE = new Token("KSQL_COL_NULLABLE", 3, 0, 0);
        public static readonly Token KSQL_COL_DEFAULT = new Token("KSQL_COL_DEFAULT", 3, 0, 0);
        public static readonly Token SYSINDEXES = new Token("KSQL_INDEXES", 3, 0, 0);
        public static readonly Token INDNAME = new Token("KSQL_INDNAME", 3, 0, 0);
        public static readonly Token INDCOLUMNS = new Token("KSQL_INDCOLUMNS", 3, 0, 0);
        public static readonly Token SYSCONSTRAINTS = new Token("KSQL_CONSTRAINTS", 3, 0, 0);
        public static readonly Token KSQL_CONS_NAME = new Token("KSQL_CONS_NAME", 3, 0, 0);
        public static readonly Token KSQL_CONS_TABNAME = new Token("KSQL_CONS_TABNAME", 3, 0, 0);
        public static readonly Token KSQL_CONS_TYPE = new Token("KSQL_CONS_TYPE", 3, 0, 0);
        public static readonly Token KSQL_CT_F = new Token("KSQL_CT_F", 3, 0, 0);
        public static readonly Token KSQL_CT_P = new Token("KSQL_CT_P", 3, 0, 0);
        public static readonly Token KSQL_CT_U = new Token("KSQL_CT_U", 3, 0, 0);
        public static readonly Token KSQL_CT_C = new Token("KSQL_CT_C", 3, 0, 0);
        public static readonly Token PinYinToken = new Token("SCHINESE_PINYIN", 3, 0, 0);
        public static readonly Token StrokeToken = new Token("SCHINESE_STROKE", 3, 0, 0);
        public static readonly Token RadicalToken = new Token("SCHINESE_RADICAL", 3, 0, 0);
        public static readonly Token RollUpToken = new Token("ROLLUP", 3, 0, 0);
        public static readonly Token PartitionToken = new Token("PARTITION", 3, 0, 0);
    }
}


