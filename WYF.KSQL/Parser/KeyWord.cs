using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Parser
{
    public class KeyWord
    {
        // Token: 0x06000310 RID: 784 RVA: 0x0000E929 File Offset: 0x0000CB29
        public KeyWord() : this(KeyWord.KSQL_KEYWORDS, KeyWord.KSQL_AGGREGATEFUNCTIONS, KeyWord.KSQL_RANKINGFUNCTIONS)
        {
        }

        // Token: 0x06000311 RID: 785 RVA: 0x0000E940 File Offset: 0x0000CB40
        public KeyWord(string[] keywords, string[] aggregateFunctions, string[] rankingFunctions)
        {
            this._keywords = new HashSet<string>(keywords, StringComparer.OrdinalIgnoreCase);
            this._aggregateFunctions = new HashSet<string>(aggregateFunctions, StringComparer.OrdinalIgnoreCase);
            this._rankingFunctions = new HashSet<string>(rankingFunctions, StringComparer.OrdinalIgnoreCase);
        }

        // Token: 0x06000312 RID: 786 RVA: 0x0000E97B File Offset: 0x0000CB7B
        public bool IsKeyWord(string word)
        {
            return word != null && this._keywords.Contains(word);
        }

        // Token: 0x06000313 RID: 787 RVA: 0x0000E98E File Offset: 0x0000CB8E
        public bool IsAggreateFunction(string word)
        {
            return word != null && this._aggregateFunctions.Contains(word);
        }

        // Token: 0x06000314 RID: 788 RVA: 0x0000E9A1 File Offset: 0x0000CBA1
        public bool IsRankingFunction(string word)
        {
            return word != null && this._rankingFunctions.Contains(word);
        }

        // Token: 0x040001DD RID: 477
        private static string[] KSQL_KEYWORDS = new string[]
        {
            "ADD",
            "ALL",
            "ALTER",
            "AND",
            "ANY",
            "AS",
            "ASC",
            "AUTHORIZATION",
            "BACKUP",
            "BEGIN",
            "BETWEEN",
            "BREAK",
            "BROWSE",
            "BULK",
            "BY",
            "CALL",
            "CASCADE",
            "CASE",
            "CHECK",
            "CHECKPOINT",
            "CLOSE",
            "CLUSTERED",
            "COALESCE",
            "COLLATE",
            "COLUMN",
            "COMMIT",
            "COMPUTE",
            "CONNECT",
            "CONSTRAINT",
            "CONTAINS",
            "CONTAINSTABLE",
            "CONTINUE",
            "CONVERT",
            "CREATE",
            "CROSS",
            "CURRENT",
            "CURRENT_DATE",
            "CURRENT_TIME",
            "CURRENT_TIMESTAMP",
            "CURRENT_USER",
            "CURSOR",
            "CURSOR_LOOP",
            "DATABASE",
            "DATE",
            "DBCC",
            "DEALLOCATE",
            "DECLARE",
            "DEFAULT",
            "DELETE",
            "DENY",
            "DESC",
            "DISK",
            "DISTINCT",
            "DISTRIBUTED",
            "DO",
            "DOUBLE",
            "DROP",
            "DUMMY",
            "DUMP",
            "ELSE",
            "EMPTY",
            "END",
            "ERRLVL",
            "ESCAPE",
            "EXCEPT",
            "EXEC",
            "EXECUTE",
            "EXISTS",
            "EXIT",
            "FALSE",
            "FAST",
            "FETCH",
            "FILE",
            "FILLFACTOR",
            "FOR",
            "FOREIGN",
            "FREETEXT",
            "FREETEXTTABLE",
            "FROM",
            "FULL",
            "FUNCTION",
            "GOTO",
            "GRANT",
            "GROUP",
            "HAVING",
            "HOLDLOCK",
            "IDENTITY",
            "IDENTITYCOL",
            "IDENTITY_INSERT",
            "IF",
            "IN",
            "INDEX",
            "INNER",
            "INSERT",
            "INTERSECT",
            "INTO",
            "IS",
            "JOIN",
            "KEY",
            "KILL",
            "KSQL_BLOCK",
            "KSQL_COLNAME",
            "KSQL_CONSTNAME",
            "KSQL_CONSTRAINTS",
            "KSQL_CURSOR_LOOP",
            "KSQL_DEFAULT",
            "KSQL_FETCH",
            "KSQL_INDEXES",
            "KSQL_INDNAME",
            "KSQL_NULLABLE",
            "KSQL_TABNAME",
            "KSQL_USERCOLUMNS",
            "KSQL_TABLECOLUMNDEFAULTVALUE",
            "KSQL_USERTABLES",
            "LABEL",
            "KSQL_USERVIEWS",
            "LEFT",
            "LIKE",
            "LIMIT",
            "LINENO",
            "LOAD",
            "LOOP",
            "MERGE",
            "MATCHED",
            "NATIONAL",
            "NEW",
            "NOCHECK",
            "NONCLUSTERED",
            "NOT",
            "NULL",
            "NULLIF",
            "OF",
            "OFF",
            "OFFSETS",
            "ON",
            "OPEN",
            "OPENDATASOURCE",
            "OPENQUERY",
            "OPENROWSET",
            "OPENXML",
            "OPTION",
            "OR",
            "ORDER",
            "OUTER",
            "OVER",
            "PARTITION",
            "PERCENT",
            "PLAN",
            "PRECISION",
            "PRIMARY",
            "PRINT",
            "PRIOR",
            "PROC",
            "PROCEDURE",
            "PUBLIC",
            "RAISERROR",
            "READ",
            "READPAST",
            "READTEXT",
            "RECONFIGURE",
            "REFERENCES",
            "REPLICATION",
            "RESTORE",
            "RETURN",
            "REVOKE",
            "RIGHT",
            "ROLLBACK",
            "ROLLUP",
            "ROWCOUNT",
            "ROWGUIDCOL",
            "RULE",
            "SAVE",
            "SCHEMA",
            "SCHINESE_PINYIN",
            "SCHINESE_RADICAL",
            "SCHINESE_STROKE",
            "SELECT",
            "SESSION_USER",
            "SET",
            "SETUSER",
            "SHUTDOWN",
            "SOME",
            "START",
            "STATISTICS",
            "SYSTEM_USER",
            "TABLE",
            "TEXTSIZE",
            "THEN",
            "TIME",
            "TIMESTAMP",
            "TO",
            "TOP",
            "TRAN",
            "TRANSACTION",
            "TRIGGER",
            "TRUE",
            "TRUNCATE",
            "TSEQUAL",
            "UNION",
            "UNIQUE",
            "UPDATE",
            "UPDATETEXT",
            "USE",
            "USING",
            "USER",
            "VALUES",
            "VARYING",
            "VIEW",
            "WAITFOR",
            "WHEN",
            "WHERE",
            "WHILE",
            "WITH",
            "WRITETEXT",
            "PERSISTED"
        };

        // Token: 0x040001DE RID: 478
        private static string[] KSQL_AGGREGATEFUNCTIONS = new string[]
        {
            "AVG",
            "COUNT",
            "MAX",
            "MIN",
            "SUM",
            "STDDEV",
            "GROUPING"
        };

        // Token: 0x040001DF RID: 479
        private static string[] KSQL_RANKINGFUNCTIONS = new string[]
        {
            "RANK",
            "ROW_NUMBER",
            "NTILE",
            "DENSE_RANK"
        };

        // Token: 0x040001E0 RID: 480
        public static KeyWord instance = new KeyWord();

        // Token: 0x040001E1 RID: 481
        private HashSet<string> _keywords;

        // Token: 0x040001E2 RID: 482
        private HashSet<string> _aggregateFunctions;

        // Token: 0x040001E3 RID: 483
        private HashSet<string> _rankingFunctions;
    }




}
