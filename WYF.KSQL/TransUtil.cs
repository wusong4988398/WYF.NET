using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Exception;
using WYF.KSQL.Formater;
using WYF.KSQL.Parser;
using WYF.KSQL.Shell.Trace;
using WYF.KSQL.Util;

namespace WYF.KSQL
{
    public class TransUtil
    {
       
        public const string Dialect_Prefix = "/*dialect*/";
        //private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static bool OleDBDriver = true;
        private static bool throwExWhenNameTooLong = false;


        private TransUtil()
        {
        }

        private static string format(string ksql, IList stmtCol, int targetDbType, FormatOptions options)
        {
            string str2;
            try
            {
                SQLFormater formater = FormaterFactory.GetFormater(targetDbType);
                if (options != null)
                {
                    formater.SetOptions(options);
                }
                formater.Format(stmtCol);
                str2 = formater.GetBuffer().ToString();
            }
            catch (FormaterException exception)
            {
                logFormatError(exception, ksql, targetDbType);
                throw new ParserException(exception, ksql, targetDbType);
            }
            catch (RuntimeException exception2)
            {
                logFormatError(exception2, ksql, targetDbType);
                throw exception2;
            }
            catch (System.Exception exception3)
            {
                logFormatError(exception3, ksql, targetDbType);
                throw exception3;
            }
            return str2;
        }

        public static IList getStmtList(string ksql)
        {
            IList list;
            try
            {
                Lexer lexer = new Lexer(ksql);
                SqlParser parser = new SqlParser(lexer);
                parser.setThrowExWhenNameTooLong(isThrowExWhenNameTooLong());
                list = parser.parseStmtList();
            }
            catch (ParserException exception)
            {
                logParseError(exception, ksql);
                throw new ParserException(exception, ksql);
            }
            catch (RuntimeException exception2)
            {
                logParseError(exception2, ksql);
                throw exception2;
            }
            catch (System.Exception exception3)
            {
                logParseError(exception3, ksql);
                throw exception3;
            }
            return list;
        }

        public static bool isThrowExWhenNameTooLong()
        {
            return throwExWhenNameTooLong;
        }

        private static void logFormatError(System.Exception ex, string kSql, int targetDbType)
        {
            //logger.Info("format sql error. target database is '" + DatabaseType.getName(targetDbType) + "' detail message is :\n" + ex.Message + "\nsource sql is : \n" + kSql, ex);
        }

        private static void logParseError(System.Exception ex, string kSql)
        {
           // logger.Info("parse error. detail message is :\n" + ex.Message + "\nsource sql is : \n" + kSql, ex);
        }

        public static void setThrowExWhenNameTooLong(bool throwExWhenNameTooLong)
        {
            TransUtil.throwExWhenNameTooLong = throwExWhenNameTooLong;
        }

        private static string translate(string ksql, IList stmtCol, int targetDbType, FormatOptions options)
        {
            if (((ksql != null) && (ksql.Length > 0)) && ((stmtCol != null) && (stmtCol.Count > 0)))
            {
                return format(ksql, stmtCol, targetDbType, options);
            }
            return "";
        }

        public static string Translate(string ksql, int targetDbType)
        {
            return Translate(ksql, targetDbType, new FormatOptions());
        }

        public static string Translate(StringBuilder sqlBuffer, int targetDbType)
        {
            return Translate(sqlBuffer.ToString(), targetDbType, new FormatOptions());
        }

        public static string Translate(string kSql, IList stmtCol, int targetDbType)
        {
            return translate(kSql, stmtCol, targetDbType, new FormatOptions());
        }

        public static string Translate(string kSql, int targetDbType, FormatOptions options)
        {
            if ((kSql == null) || (kSql.Length == 0))
            {
                return kSql;
            }
            if (kSql.StartsWith("/*dialect*/"))
            {
                while (kSql.StartsWith("/*dialect*/"))
                {
                    kSql = kSql.Substring("/*dialect*/".Length);
                }
                return kSql;
            }
            IList stmtCol = getStmtList(kSql);
            return format(kSql, stmtCol, targetDbType, options);
        }

        public static string Translate(string kSql, int targetDbType, TraceInfo traceInfo)
        {
            kSql = kSql.Trim();
            if ((kSql == null) || (kSql.Length == 0))
            {
                return kSql;
            }
            if (kSql.StartsWith("/*dialect*/"))
            {
                while (kSql.StartsWith("/*dialect*/"))
                {
                    kSql = kSql.Substring("/*dialect*/".Length);
                }
                return kSql;
            }
            IList stmtCol = getStmtList(kSql);
            if (traceInfo != null)
            {
                string[] tempTableSpaces = traceInfo.tempTableSpaces;
                if ((tempTableSpaces != null) && (tempTableSpaces.Length > 0))
                {
                    IEnumerator enumerator = stmtCol.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        if (current is SqlCreateTableStmt)
                        {
                            SqlCreateTableStmt stmt = (SqlCreateTableStmt)current;
                            if (((stmt.tableSpace == null) || (stmt.tableSpace.Length <= 0)) && UUTN.isTempTable(stmt.name))
                            {
                                stmt.tableSpace = traceInfo.randomTableSpace();
                            }
                        }
                    }
                }
            }
            return translate(kSql, stmtCol, targetDbType, traceInfo.getFormatOptions());
        }

        public static string Translate(string ksql, int targetDbType, bool throwExWhenNameTooLong)
        {
            bool flag = isThrowExWhenNameTooLong();
            setThrowExWhenNameTooLong(throwExWhenNameTooLong);
            string str = Translate(ksql, targetDbType);
            setThrowExWhenNameTooLong(flag);
            return str;
        }

        public static string Translate(StringBuilder sqlBuffer, int targetDbType, bool throwExWhenNameTooLong)
        {
            bool flag = isThrowExWhenNameTooLong();
            setThrowExWhenNameTooLong(throwExWhenNameTooLong);
            string str = Translate(sqlBuffer, targetDbType);
            setThrowExWhenNameTooLong(flag);
            return str;
        }

        public static string Translate(string kSql, IList stmtCol, int targetDbType, bool throwExWhenNameTooLong)
        {
            bool flag = isThrowExWhenNameTooLong();
            setThrowExWhenNameTooLong(throwExWhenNameTooLong);
            string str = Translate(kSql, stmtCol, targetDbType);
            setThrowExWhenNameTooLong(flag);
            return str;
        }

        public static string Translate(string kSql, int targetDbType, FormatOptions options, bool throwExWhenNameTooLong)
        {
            bool flag = isThrowExWhenNameTooLong();
            setThrowExWhenNameTooLong(throwExWhenNameTooLong);
            string str = Translate(kSql, targetDbType, options);
            setThrowExWhenNameTooLong(flag);
            return str;
        }

        public static string Translate(string kSql, int targetDbType, TraceInfo traceInfo, bool throwExWhenNameTooLong)
        {
            bool flag = isThrowExWhenNameTooLong();
            setThrowExWhenNameTooLong(throwExWhenNameTooLong);
            string str = Translate(kSql, targetDbType, traceInfo);
            setThrowExWhenNameTooLong(flag);
            return str;
        }
    }





}
