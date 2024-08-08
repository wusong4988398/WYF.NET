
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WYF.Bos.ksql.parser
{
    public  class KeyWord
    {
        private static readonly String[] KSQL_KEYWORDS = new String[] { "ADD", "ALL", "ALTER", "AND", "ANY", "AS", "ASC", "AUTHORIZATION", "BACKUP", "BEGIN", "BETWEEN", "BREAK", "BROWSE", "BULK", "BY", "CALL", "CASCADE", "CASE", "CHECK", "CHECKPOINT", "CLOSE", "CLUSTERED", "COALESCE", "COLLATE", "COLUMN", "COMMIT", "COMPUTE", "CONNECT", "CONSTRAINT", "CONTAINS", "CONTAINSTABLE", "CONTINUE", "CONVERT", "CREATE", "CROSS", "CURRENT", "CURRENT_DATE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER", "CURSOR", "CURSOR_LOOP", "DATABASE", "DATE", "DBCC", "DEALLOCATE", "DECLARE", "DEFAULT", "DELETE", "DENY", "DESC", "DISK", "DISTINCT", "DISTRIBUTED", "DO", "DOUBLE", "DROP", "DUMMY", "DUMP", "ELSE", "EMPTY", "END", "ERRLVL", "ESCAPE", "EXCEPT", "EXEC", "EXECUTE", "EXISTS", "EXIT", "FALSE", "FAST", "FETCH", "FILE", "FILLFACTOR", "FOR", "FOREIGN", "FREETEXT", "FREETEXTTABLE", "FROM", "FULL", "FUNCTION", "GOTO", "GRANT", "GROUP", "HAVING", "HOLDLOCK", "IDENTITY", "IDENTITYCOL", "IDENTITY_INSERT", "IF", "IN", "INDEX", "INNER", "INSERT", "INTERSECT", "INTO", "IS", "JOIN", "KEY", "KILL", "KSQL_BLOCK", "KSQL_COLNAME", "KSQL_CONSTNAME", "KSQL_CONSTRAINTS", "KSQL_CURSOR_LOOP", "KSQL_DEFAULT", "KSQL_FETCH", "KSQL_INDEXES", "KSQL_INDNAME", "KSQL_NULLABLE", "KSQL_TABNAME", "KSQL_USERCOLUMNS", "KSQL_USERTABLES", "LABEL", "LEFT", "LIKE", "LIMIT", "LINENO", "LOAD", "LOOP", "MATCH", "NATIONAL", "NEW", "NOCHECK", "NONCLUSTERED", "NOT", "NULL", "NULLIF", "OF", "OFF", "OFFSETS", "ON", "OPEN", "OPENDATASOURCE", "OPENQUERY", "OPENROWSET", "OPENXML", "OPTION", "OR", "ORDER", "OUTER", "OVER", "PERCENT", "PLAN", "PRECISION", "PRIMARY", "PRINT", "PRIOR", "PROC", "PROCEDURE", "PUBLIC", "RAISERROR", "READ", "READPAST", "READTEXT", "RECONFIGURE", "REFERENCES", "REPLICATION", "RESTORE", "RETURN", "REVOKE", "RIGHT", "ROLLBACK", "ROLLUP", "ROWCOUNT", "ROWGUIDCOL", "RULE", "SAVE", "SCHEMA", "SCHINESE_PINYIN", "SCHINESE_RADICAL", "SCHINESE_STROKE", "SELECT", "SESSION_USER", "SET", "SETUSER", "SHUTDOWN", "SOME", "START", "STATISTICS", "SYSTEM_USER", "TABLE", "TEXTSIZE", "THEN", "TIME", "TIMESTAMP", "TO", "TOP", "TRAN", "TRANSACTION", "TRIGGER", "TRUE", "TRUNCATE", "TSEQUAL", "UNION", "UNIQUE", "UPDATE", "UPDATETEXT", "USE", "USER", "VALUES", "VARYING", "VIEW", "WAITFOR", "WHEN", "WHERE", "WHILE", "WITH", "WRITETEXT", "XIN" };
        private static readonly HashSet<String> aggregateFunctions = new HashSet<String>() { "AVG", "COUNT", "MAX", "MIN", "SUM", "STDDEV", "GROUPING" };
        private static readonly HashSet<String> rankingFunctions = new HashSet<String>() { "RANK", "ROW_NUMBER", "NTILE", "DENSE_RANK" };

        public static readonly KeyWord instance = new KeyWord();
        private readonly HashSet<String> keyWordSet;

        public KeyWord(): this(KeyWord.KSQL_KEYWORDS)
        {
            
        }

        public KeyWord(string[] keywords)
        {
            this.keyWordSet = new HashSet<String>((int)(keywords.Length / 0.75f));
            foreach (string key in keywords)
            {
                this.keyWordSet.Add(key.ToUpper());
            }
        }

        public bool IsKeyWord(string word)
        {
            return this.keyWordSet.Contains(word.ToUpper());
        }

    }
}
