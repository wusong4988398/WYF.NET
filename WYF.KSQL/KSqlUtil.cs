
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Exception;
using WYF.KSQL.Formater;
using WYF.KSQL.Parser;
using WYF.KSQL.Schema;
using WYF.KSQL.Shell.Trace;

namespace WYF.KSQL
{
    public class KSqlUtil
    {
      
        private const string BASE_KEY = "VUVvSjFBWUF0RjFaMEYyQXBFb0IyRllCMEVvSjBGWTMyRW5aMEJIRW1CMTYr";
        public const int DEFAULT_LIFECYCLE = 0x6ddd00;
        public const int FUNCTION_NAME = 0;
        public const int KEY_WORD = 1;
       // private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Hashtable methodTypeMap = new Hashtable();
        public const int NOT_KW_FN = -1;
        public const int OPTIMIZE_MODE_DISABLED = -1;
        public const int OPTIMIZE_MODE_INNER_JOIN = 2;
        public const int OPTIMIZE_MODE_OUTER_JOIN = 1;
        public const int OPTIMIZE_MODE_UNUSED = 0;
        private static int optimizeMode = 1;
        public const string OPTIONS_BINDPORT = "bindport";
        public const string OPTIONS_DBSCHEMA = "dbSchema";
        public const string OPTIONS_DBTYPE = "dbtype";
        public const string OPTIONS_FILTER = "filter";
        public const string OPTIONS_NOLOGGING = "noLogging";
        public const string OPTIONS_OPTIMIZE = "optimize";
        public const string OPTIONS_TABLESCHEMA = "tableSchema";
        public const string OPTIONS_TEMPTABLESPACE = "temptablespace";
        public const string OPTIONS_TRACE = "trace";
        public const string OPTIONS_TRACEFILE = "file";
        public const string OPTIONS_TRANSLATE = "translate";

        // Methods
        static KSqlUtil()
        {
            methodTypeMap.Add("ABS", TypeCode.Decimal);
            methodTypeMap.Add("ACOS", TypeCode.Decimal);
            methodTypeMap.Add("ASIN", TypeCode.Decimal);
            methodTypeMap.Add("ATAN", TypeCode.Decimal);
            methodTypeMap.Add("ATN2", TypeCode.Decimal);
            methodTypeMap.Add("CEILING", TypeCode.Int16);
            methodTypeMap.Add("COS", TypeCode.Decimal);
            methodTypeMap.Add("EXP", TypeCode.Decimal);
            methodTypeMap.Add("FLOOR", TypeCode.Int16);
            methodTypeMap.Add("MOD", TypeCode.Int16);
            methodTypeMap.Add("LOG", TypeCode.Decimal);
            methodTypeMap.Add("POWER", TypeCode.Decimal);
            methodTypeMap.Add("ROUND", TypeCode.Decimal);
            methodTypeMap.Add("SIGN", TypeCode.Decimal);
            methodTypeMap.Add("SIN", TypeCode.Decimal);
            methodTypeMap.Add("SQRT", TypeCode.Decimal);
            methodTypeMap.Add("TAN", TypeCode.Decimal);
            methodTypeMap.Add("FN_GCD", TypeCode.Int32);
            methodTypeMap.Add("FN_LCM", TypeCode.Int32);
            methodTypeMap.Add("CURDATE", TypeCode.DateTime);
            methodTypeMap.Add("CURTIME", TypeCode.DateTime);
            methodTypeMap.Add("DATEADD", TypeCode.DateTime);
            methodTypeMap.Add("DATEDIFF", TypeCode.Int16);
            methodTypeMap.Add("DAYNAME", TypeCode.String);
            methodTypeMap.Add("DAYOFMONTH", TypeCode.Int16);
            methodTypeMap.Add("DAYOFWEEK", TypeCode.Int16);
            methodTypeMap.Add("DAYOFYEAR", TypeCode.Int16);
            methodTypeMap.Add("GETDATE", TypeCode.DateTime);
            methodTypeMap.Add("HOUR", TypeCode.Int16);
            methodTypeMap.Add("MINUTE", TypeCode.Int16);
            methodTypeMap.Add("MONTH", TypeCode.Int16);
            methodTypeMap.Add("MONTHNAME", TypeCode.String);
            methodTypeMap.Add("NOW", TypeCode.DateTime);
            methodTypeMap.Add("QUARTER", TypeCode.Int16);
            methodTypeMap.Add("SECOND", TypeCode.Int16);
            methodTypeMap.Add("WEEK", TypeCode.Int16);
            methodTypeMap.Add("YEAR", TypeCode.Int16);
            methodTypeMap.Add("TO_DATE", TypeCode.DateTime);
            methodTypeMap.Add("MONTHS_BETWEEN", TypeCode.Int16);
            methodTypeMap.Add("DAYS_BETWEEN", TypeCode.Int16);
            methodTypeMap.Add("ADD_MONTHS", TypeCode.DateTime);
            methodTypeMap.Add("ADD_YEARS", TypeCode.DateTime);
            methodTypeMap.Add("ADD_DAYS", TypeCode.DateTime);
            methodTypeMap.Add("ADD_HOURS", TypeCode.DateTime);
            methodTypeMap.Add("ADD_MINUTES", TypeCode.DateTime);
            methodTypeMap.Add("ADD_SECONDS", TypeCode.DateTime);
            methodTypeMap.Add("ASCII", TypeCode.Int16);
            methodTypeMap.Add("CHAR", TypeCode.String);
            methodTypeMap.Add("CHARINDEX", TypeCode.Int16);
            methodTypeMap.Add("CONCAT", TypeCode.String);
            methodTypeMap.Add("LEFT", TypeCode.String);
            methodTypeMap.Add("LEN", TypeCode.Int16);
            methodTypeMap.Add("LENGTH", TypeCode.Int16);
            methodTypeMap.Add("LOWER", TypeCode.String);
            methodTypeMap.Add("LCASE", TypeCode.String);
            methodTypeMap.Add("LTRIM", TypeCode.String);
            methodTypeMap.Add("REPLACE", TypeCode.String);
            methodTypeMap.Add("RIGHT", TypeCode.String);
            methodTypeMap.Add("RTRIM", TypeCode.String);
            methodTypeMap.Add("SOUNDEX", TypeCode.Int16);
            methodTypeMap.Add("SUBSTRING", TypeCode.String);
            methodTypeMap.Add("TRIM", TypeCode.String);
            methodTypeMap.Add("UCASE", TypeCode.String);
            methodTypeMap.Add("UPPER", TypeCode.String);
            methodTypeMap.Add("TOCHAR", TypeCode.String);
            methodTypeMap.Add("DATENAME", TypeCode.String);
            methodTypeMap.Add("TO_NUMBER", TypeCode.Decimal);
            methodTypeMap.Add("TO_INT", TypeCode.Int16);
            methodTypeMap.Add("NEWID", TypeCode.String);
            methodTypeMap.Add("COUNT", TypeCode.Int16);
            methodTypeMap.Add("AVG", TypeCode.Decimal);
            methodTypeMap.Add("SUM", TypeCode.Decimal);
            methodTypeMap.Add("MAX", TypeCode.Decimal);
            methodTypeMap.Add("MIN", TypeCode.Decimal);
        }

        public static Hashtable adjust(SqlSelect select)
        {
            return adjust(select, optimizeMode);
        }

        public static Hashtable adjust(SqlSelect select, int mode)
        {
            Hashtable exprAliasMap = new Hashtable();
            IEnumerator enumerator = select.selectList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SqlSelectItem current = (SqlSelectItem)enumerator.Current;
                if ((current.alias != null) && (current.alias.Length > 0))
                {
                    exprAliasMap.Add(current.alias, current.expr);
                }
            }
            select.condition = adjust_replace(exprAliasMap, select.condition);
            IEnumerator enumerator2 = select.orderBy.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                SqlOrderByItem item2 = (SqlOrderByItem)enumerator2.Current;
                item2.expr = adjust_replace(exprAliasMap, item2.expr);
            }
            Hashtable joinConditionExprCanReplaceMap = new Hashtable();
            computeJoinConditionCanReplaceExprMap(joinConditionExprCanReplaceMap, select.tableSource, mode);
            int num = 0;
            int count = select.selectList.Count;
            while (num < count)
            {
                SqlSelectItem item3 = (SqlSelectItem)select.selectList[num];
                item3.expr = replace_for_join_condition(joinConditionExprCanReplaceMap, item3.expr);
                num++;
            }
            int index = 0;
            int num4 = select.groupBy.Count;
            while (index < num4)
            {
                SqlExpr expr = (SqlExpr)select.groupBy[index];
                expr = replace_for_join_condition(joinConditionExprCanReplaceMap, expr);
                select.groupBy.SetRange(index, (ICollection)expr);
                index++;
            }
            select.having = replace_for_join_condition(joinConditionExprCanReplaceMap, select.having);
            replaceTableSourceJoinCondition(joinConditionExprCanReplaceMap, select.tableSource);
            select.condition = replace_for_Select_condition(joinConditionExprCanReplaceMap, select.condition);
            IEnumerator enumerator3 = select.orderBy.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                SqlOrderByItem item4 = (SqlOrderByItem)enumerator3.Current;
                item4.expr = replace_for_join_condition(joinConditionExprCanReplaceMap, item4.expr);
                item4.expr = replace_for_join_condition(exprAliasMap, item4.expr);
            }
            return exprAliasMap;
        }

        private static SqlExpr adjust_replace(Hashtable exprAliasMap, SqlExpr expr)
        {
            if (expr == null)
            {
                return null;
            }
            if (!(expr is SqlAllColumnExpr) && (expr is SqlIdentifierExpr))
            {
                string key = ((SqlIdentifierExpr)expr).value;
                if (exprAliasMap.ContainsKey(key))
                {
                    return (SqlExpr)exprAliasMap[key];
                }
            }
            return expr;
        }

        public static string buildKSqlDateString(DateTime date)
        {
            return date.ToString("{yyyy-MM-dd HH:mm:ss}");
        }

        public static SqlTableSourceBase cleanTableSource(Hashtable usedTableAliasSet, Hashtable joinConditionUsedAliasMap, SqlTableSourceBase tableSource, int mode)
        {
            if (tableSource is SqlJoinedTableSource)
            {
                SqlJoinedTableSource source = (SqlJoinedTableSource)tableSource;
                if (source.joinType == 1)
                {
                    string str;
                    string alias = source.right.alias;
                    if (((alias != null) && (alias.Length != 0)) && (alias.CharAt(0) == '"'))
                    {
                        str = alias.substring(1, source.right.alias.Length - 1);
                    }
                    else
                    {
                        str = alias;
                    }
                    int num = 0;
                    if (str != null)
                    {
                        str = str.ToUpper();
                        if (joinConditionUsedAliasMap[str] == null)
                        {
                            num = 0;
                        }
                        else
                        {
                            num = (int)joinConditionUsedAliasMap[str];
                        }
                    }
                    else if (source.right is SqlTableSource)
                    {
                        str = ((SqlTableSource)source.right).name.ToUpper();
                        if (joinConditionUsedAliasMap[str] == null)
                        {
                            num = 0;
                        }
                        else
                        {
                            num = (int)joinConditionUsedAliasMap[str];
                        }
                    }
                    source.left = cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.left, mode);
                    if (source.right is SqlTableSource)
                    {
                        if (!usedTableAliasSet.Contains(str) && (num <= 1))
                        {
                            return cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.left, mode);
                        }
                        return tableSource;
                    }
                    if (source.right is SqlSubQueryTableSource)
                    {
                        if (!usedTableAliasSet.Contains(str) && (num <= 1))
                        {
                            return cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.left, mode);
                        }
                        return tableSource;
                    }
                    if (source.right is SqlJoinedTableSource)
                    {
                        source.right = cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.right, mode);
                    }
                    return tableSource;
                }
                if ((source.joinType == 0) && (mode > 1))
                {
                    string str3;
                    string str4 = source.right.alias;
                    if (((str4 != null) && (str4.Length != 0)) && (str4.CharAt(0) == '"'))
                    {
                        str3 = str4.substring(1, source.right.alias.Length - 1);
                    }
                    else
                    {
                        str3 = str4;
                    }
                    int num4 = 0;
                    if (str3 != null)
                    {
                        str3 = str3.ToUpper();
                        if (joinConditionUsedAliasMap[str3] == null)
                        {
                            num4 = 0;
                        }
                        else
                        {
                            num4 = (int)joinConditionUsedAliasMap[str3];
                        }
                    }
                    else if (source.right is SqlTableSource)
                    {
                        str3 = ((SqlTableSource)source.right).name.ToUpper();
                        if (joinConditionUsedAliasMap[str3] == null)
                        {
                            num4 = 0;
                        }
                        else
                        {
                            num4 = (int)joinConditionUsedAliasMap[str3];
                        }
                    }
                    source.left = cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.left, mode);
                    if (source.right is SqlTableSource)
                    {
                        if (!usedTableAliasSet.Contains(str3) && (num4 <= 1))
                        {
                            return cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.left, mode);
                        }
                        return tableSource;
                    }
                    if (source.right is SqlSubQueryTableSource)
                    {
                        if (!usedTableAliasSet.Contains(str3) && (num4 <= 1))
                        {
                            return cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.left, mode);
                        }
                        return tableSource;
                    }
                    if (source.right is SqlJoinedTableSource)
                    {
                        source.right = cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.right, mode);
                    }
                    return tableSource;
                }
                source.left = cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.left, mode);
                source.right = cleanTableSource(usedTableAliasSet, joinConditionUsedAliasMap, source.right, mode);
            }
            return tableSource;
        }

        public static void cleanUp(IDbCommand stmt, IDataReader rs)
        {
            try
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }
            finally
            {
                if (stmt != null)
                {
                    stmt.Connection.Close();
                }
            }
        }

        public static void cleanUp(IDbConnection conn, IDbCommand stmt)
        {
            try
            {
                if (stmt != null)
                {
                    stmt.Connection.Close();
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public static void cleanUp(IDbConnection conn, IDbCommand stmt, IDataReader rs)
        {
            try
            {
                if (rs != null)
                {
                    rs.Close();
                }
            }
            finally
            {
                try
                {
                    if (stmt != null)
                    {
                        stmt.Connection.Close();
                    }
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
        }

        private static void computeInnerJoinConditionCanReplaceExprMap(Hashtable joinConditionExprCanReplaceMap, SqlJoinedTableSource joinTabSrc)
        {
            if (isSimpleJoinCondition(joinTabSrc.condition))
            {
                string str3;
                string str4;
                SqlExpr left = ((SqlBinaryOpExpr)joinTabSrc.condition).left;
                SqlExpr right = ((SqlBinaryOpExpr)joinTabSrc.condition).right;
                string alias = joinTabSrc.right.alias;
                if ((alias == null) && (joinTabSrc.right is SqlTableSource))
                {
                    alias = ((SqlTableSource)joinTabSrc.right).alias;
                    if (alias == null)
                    {
                        alias = ((SqlTableSource)joinTabSrc.right).name;
                    }
                }
                string str2 = getProperOwnerString(right);
                if ((str2 != null) && str2.EqualsIgnoreCase(alias))
                {
                    str3 = right.toString();
                    str4 = left.toString();
                }
                else
                {
                    str3 = left.toString();
                    str4 = right.toString();
                }
                if (joinConditionExprCanReplaceMap.ContainsKey(str4))
                {
                    str4 = (string)joinConditionExprCanReplaceMap[str4];
                }
                Hashtable hashtable = new Hashtable();
                IEnumerator enumerator = joinConditionExprCanReplaceMap.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DictionaryEntry current = (DictionaryEntry)enumerator.Current;
                    string key = current.Key.ToString();
                    if (current.Value.ToString().EqualsIgnoreCase(str3))
                    {
                        hashtable.Add(key.ToUpper(), str4);
                        joinConditionExprCanReplaceMap.Remove(key);
                    }
                }
                joinConditionExprCanReplaceMap.Add(str3.ToUpper(), str4);
                joinConditionExprCanReplaceMap = hashtable;
            }
        }

        private static void computeJoinConditionCanReplaceExprMap(Hashtable joinConditionExprCanReplaceMap, SqlTableSourceBase tabSrcBase, int mode)
        {
            if (tabSrcBase is SqlJoinedTableSource)
            {
                SqlJoinedTableSource joinTabSrc = (SqlJoinedTableSource)tabSrcBase;
                if (joinTabSrc.joinType == 1)
                {
                    computeLeftJoinConditionCanReplaceExprMap(joinConditionExprCanReplaceMap, joinTabSrc);
                }
                if ((joinTabSrc.joinType == 0) && (mode > 1))
                {
                    computeInnerJoinConditionCanReplaceExprMap(joinConditionExprCanReplaceMap, joinTabSrc);
                }
                computeJoinConditionCanReplaceExprMap(joinConditionExprCanReplaceMap, joinTabSrc.left, mode);
                computeJoinConditionCanReplaceExprMap(joinConditionExprCanReplaceMap, joinTabSrc.right, mode);
            }
        }

        public static void computeJoinConditionUsedAlias(Hashtable usedAliasMap, SqlTableSourceBase tableSource)
        {
            if ((tableSource != null) && (tableSource is SqlJoinedTableSource))
            {
                SqlJoinedTableSource source = (SqlJoinedTableSource)tableSource;
                Hashtable usedTableAliasSet = new Hashtable();
                computeUsedTableAlias(usedTableAliasSet, source.condition);
                IEnumerator enumerator = usedTableAliasSet.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    if (usedAliasMap[current] != null)
                    {
                        int num = (int)usedAliasMap[current];
                        num++;
                        usedAliasMap.Add(current, num);
                    }
                    else
                    {
                        usedAliasMap.Add(current, 1);
                    }
                }
                computeJoinConditionUsedAlias(usedAliasMap, source.left);
                computeJoinConditionUsedAlias(usedAliasMap, source.right);
            }
        }

        private static void computeLeftJoinConditionCanReplaceExprMap(Hashtable joinConditionExprCanReplaceMap, SqlJoinedTableSource joinTabSrc)
        {
            if (isSimpleJoinCondition(joinTabSrc.condition))
            {
                string str3;
                string str4;
                SqlExpr left = ((SqlBinaryOpExpr)joinTabSrc.condition).left;
                SqlExpr right = ((SqlBinaryOpExpr)joinTabSrc.condition).right;
                string alias = joinTabSrc.right.alias;
                if ((alias == null) && (joinTabSrc.right is SqlTableSource))
                {
                    alias = ((SqlTableSource)joinTabSrc.right).alias;
                    if (alias == null)
                    {
                        alias = ((SqlTableSource)joinTabSrc.right).name;
                    }
                }
                string str2 = getProperOwnerString(right);
                if ((str2 != null) && str2.EqualsIgnoreCase(alias))
                {
                    str3 = right.toString();
                    str4 = left.toString();
                }
                else
                {
                    str3 = left.toString();
                    str4 = right.toString();
                }
                if (joinConditionExprCanReplaceMap.ContainsKey(str4))
                {
                    str4 = (string)joinConditionExprCanReplaceMap[str4];
                }
                Hashtable hashtable = new Hashtable();
                IEnumerator enumerator = joinConditionExprCanReplaceMap.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DictionaryEntry current = (DictionaryEntry)enumerator.Current;
                    string key = current.Key.ToString();
                    if (current.Value.ToString().EqualsIgnoreCase(str3))
                    {
                        hashtable.Add(key.ToUpper(), str4);
                        joinConditionExprCanReplaceMap.Remove(key);
                    }
                }
                joinConditionExprCanReplaceMap.Add(str3.ToUpper(), str4);
                joinConditionExprCanReplaceMap = hashtable;
            }
        }

        public static bool computeUsedTableAlias(Hashtable usedTableAliasSet, SqlExpr expr)
        {
            if (expr != null)
            {
                if (expr is SqlAllColumnExpr)
                {
                    return true;
                }
                if (expr is SqlIdentifierExpr)
                {
                    return true;
                }
                if (expr is SqlBinaryOpExpr)
                {
                    SqlBinaryOpExpr expr2 = (SqlBinaryOpExpr)expr;
                    if (expr2.Operator != 20)
                    {
                        return (computeUsedTableAlias(usedTableAliasSet, expr2.left) || computeUsedTableAlias(usedTableAliasSet, expr2.right));
                    }
                    string str = ((SqlIdentifierExpr)getRootExpr(expr2)).value;
                    if ((str != null) && (str.Length != 0))
                    {
                        string str2;
                        if (str.CharAt(0) == '"')
                        {
                            str2 = str.substring(1, str.Length - 1);
                        }
                        else
                        {
                            str2 = str;
                        }
                        usedTableAliasSet.Add(str2.ToUpper(), "");
                    }
                }
                else if (expr is SqlMethodInvokeExpr)
                {
                    SqlMethodInvokeExpr expr4 = (SqlMethodInvokeExpr)expr;
                    IEnumerator enumerator = expr4.parameters.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        SqlExpr current = (SqlExpr)enumerator.Current;
                        if (computeUsedTableAlias(usedTableAliasSet, current))
                        {
                            return true;
                        }
                    }
                }
                else if (expr is SqlAggregateExpr)
                {
                    SqlAggregateExpr expr6 = (SqlAggregateExpr)expr;
                    IEnumerator enumerator2 = expr6.paramList.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        SqlExpr expr7 = (SqlExpr)enumerator2.Current;
                        if (computeUsedTableAlias(usedTableAliasSet, expr7))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (expr is SqlBetweenExpr)
                    {
                        SqlBetweenExpr expr8 = (SqlBetweenExpr)expr;
                        return (computeUsedTableAlias(usedTableAliasSet, expr8.testExpr) || (computeUsedTableAlias(usedTableAliasSet, expr8.beginExpr) || computeUsedTableAlias(usedTableAliasSet, expr8.endExpr)));
                    }
                    if (expr is SqlCaseExpr)
                    {
                        SqlCaseExpr expr9 = (SqlCaseExpr)expr;
                        IEnumerator enumerator3 = expr9.itemList.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            SqlCaseItem item = (SqlCaseItem)enumerator3.Current;
                            if (computeUsedTableAlias(usedTableAliasSet, item.conditionExpr))
                            {
                                return true;
                            }
                            if (computeUsedTableAlias(usedTableAliasSet, item.valueExpr))
                            {
                                return true;
                            }
                        }
                        computeUsedTableAlias(usedTableAliasSet, expr9.valueExpr);
                        computeUsedTableAlias(usedTableAliasSet, expr9.elseExpr);
                    }
                    else if (expr is SqlInListExpr)
                    {
                        SqlInListExpr expr10 = (SqlInListExpr)expr;
                        if (computeUsedTableAlias(usedTableAliasSet, expr10.expr))
                        {
                            return true;
                        }
                        IEnumerator enumerator4 = expr10.targetList.GetEnumerator();
                        while (enumerator4.MoveNext())
                        {
                            SqlExpr expr11 = (SqlExpr)enumerator4.Current;
                            if (computeUsedTableAlias(usedTableAliasSet, expr11))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (expr is SqlInSubQueryExpr)
                        {
                            SqlInSubQueryExpr expr12 = (SqlInSubQueryExpr)expr;
                            return computeUsedTableAlias(usedTableAliasSet, expr12.expr);
                        }
                        if (expr is SqlNotExpr)
                        {
                            SqlNotExpr expr13 = (SqlNotExpr)expr;
                            return computeUsedTableAlias(usedTableAliasSet, expr13.expr);
                        }
                    }
                }
            }
            return false;
        }

        public static string decodePassword(string src)
        {
            string str2;
            if (src == null)
            {
                return null;
            }
            if (!src.StartsWith("ksqle:"))
            {
                return src;
            }
            string str = src.Substring("ksqle:".Length);
            try
            {
                str2 = str;
            }
            catch (System.Exception exception)
            {
                //logger.Error(exception.Message, exception);
                throw new ArgumentException(exception.Message);
            }
            return str2;
        }

        public static string decodeUrl(string url)
        {
            if (url == null)
            {
                throw new ArgumentException("url is null");
            }
            if (url.StartsWith("jdbc:ksql:"))
            {
                return url;
            }
            if (!url.StartsWith("jdbc:ksqle:"))
            {
                throw new ArgumentException("invalid encoded ksql driver url");
            }
            string str = url.Substring("jdbc:ksqle:".Length);
            string str2 = "";
            try
            {
                str2 = str;
            }
            catch (System.Exception exception)
            {
                //logger.Error(exception.Message, exception);
                throw new ArgumentException(exception.Message);
            }
            return ("jdbc:ksql:" + str2);
        }

        public static string encodePassword(string src)
        {
            string str;
            if (src == null)
            {
                return null;
            }
            if (src.StartsWith("ksqle:"))
            {
                return src;
            }
            try
            {
                str = src;
            }
            catch (System.Exception exception)
            {
                //logger.Error(exception.Message, exception);
                throw new ArgumentException(exception.Message);
            }
            return str;
        }

        public static string encodeUrl(string url)
        {
            if (url == null)
            {
                throw new ArgumentException("url is null");
            }
            if (url.StartsWith("jdbc:ksqle:"))
            {
                return url;
            }
            if (!url.StartsWith("jdbc:ksql:"))
            {
                throw new ArgumentException("invalid ksql driver url");
            }
            string str = url.Substring("jdbc:ksql:".Length);
            string str2 = "";
            try
            {
                str2 = str;
            }
            catch (System.Exception exception)
            {
               // logger.Error(exception.Message, exception);
                throw new ArgumentException(exception.Message);
            }
            return ("jdbc:ksqle:" + str2);
        }

        public static string GenerateTableSQL(IDbConnection conn, ArrayList tableNames, KSqlExportOption expOpt)
        {
            SQLFormater formater;
            if (expOpt.TargetDBType == 0)
            {
                expOpt.TargetDBType = expOpt.SourceDBType;
            }
            if ((expOpt.TargetDBType.Equals(2) || expOpt.TargetDBType.Equals(8)) || expOpt.TargetDBType.Equals(7))
            {
                formater = new Oracle10SQLFormater();
            }
            else
            {
                formater = new MSTransactSQLFormater();
            }
            string path = "";
            int num = 3;
            if (num.Equals(expOpt.SourceDBType))
            {
                if (tableNames.Count < 1)
                {
                    tableNames = Sql2kUtil.getUserTableNames((SqlConnection)conn);
                }
                if (expOpt.ExpTableSql)
                {
                    if (expOpt.ExpTableFileName.Trim() == "")
                    {
                        path = Environment.CurrentDirectory + @"\SP_CreateTable.sql";
                    }
                    else
                    {
                        path = expOpt.ExpTableFileName.Trim();
                    }
                    using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
                    {
                        writer.WriteLine("/******************SP_MakaluTable*****************");
                        writer.WriteLine("*                                                *");
                        writer.WriteLine("*                                                *");
                        writer.WriteLine("*************************************************/");
                        writer.WriteLine("");
                        writer.WriteLine("");
                        foreach (string str2 in tableNames)
                        {
                            writer.WriteLine("/******************Create  Table*****************");
                            writer.WriteLine("*  TableName" + str2 + "                        *");
                            writer.WriteLine("*************************************************/");
                            writer.WriteLine("");
                            writer.Write(Sql2kUtil.GenerateTableSQL((SqlConnection)conn, str2, formater, expOpt.CheckTable));
                            formater.SetBuffer("");
                            writer.WriteLine("");
                            if (expOpt.TargetDBType.Equals(3))
                            {
                                writer.WriteLine("GO");
                            }
                            writer.WriteLine("");
                        }
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer.WriteLine("GO");
                        }
                        writer.Close();
                    }
                }
                bool flag = false;
                if (expOpt.ExpTableFileName.Trim() == "")
                {
                    path = Environment.CurrentDirectory + @"\SP_Constraint.sql";
                }
                else
                {
                    path = expOpt.ExpTableFileName.Trim();
                }
                if (expOpt.ExpPrimaryKeySql)
                {
                    using (StreamWriter writer2 = new StreamWriter(path, flag, Encoding.Unicode))
                    {
                        flag = true;
                        writer2.WriteLine("/******************SP_MakaluConstraint*****************");
                        writer2.WriteLine("*                                                *");
                        writer2.WriteLine("*                                                *");
                        writer2.WriteLine("*************************************************/");
                        writer2.WriteLine("");
                        writer2.WriteLine("");
                        foreach (string str3 in tableNames)
                        {
                            writer2.WriteLine("/*************Create  PrimaryKey*****************");
                            writer2.WriteLine("*  TableName" + str3 + "                        *");
                            writer2.WriteLine("**************************************************/");
                            writer2.WriteLine("");
                            writer2.Write(Sql2kUtil.GeneratePKSQL((SqlConnection)conn, str3, formater, expOpt.CheckPrimaryKey));
                            formater.SetBuffer("");
                            writer2.WriteLine("");
                            if (expOpt.TargetDBType.Equals(3))
                            {
                                writer2.WriteLine("GO");
                            }
                            writer2.WriteLine("");
                        }
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer2.WriteLine("GO");
                        }
                        writer2.Close();
                    }
                }
                if (expOpt.ExpUniqueSql)
                {
                    using (StreamWriter writer3 = new StreamWriter(path, flag, Encoding.Unicode))
                    {
                        if (!flag)
                        {
                            flag = true;
                            writer3.WriteLine("/******************SP_MakaluConstraint*****************");
                            writer3.WriteLine("*                                                *");
                            writer3.WriteLine("*                                                *");
                            writer3.WriteLine("*************************************************/");
                            writer3.WriteLine("");
                            writer3.WriteLine("");
                        }
                        foreach (string str4 in tableNames)
                        {
                            writer3.WriteLine("/*************Create  Unique**********************");
                            writer3.WriteLine("*  TableName" + str4 + "                        *");
                            writer3.WriteLine("**************************************************/");
                            writer3.WriteLine("");
                            writer3.Write(Sql2kUtil.GenerateUniqueSQL((SqlConnection)conn, str4, formater, expOpt.CheckUnique));
                            formater.SetBuffer("");
                            writer3.WriteLine("");
                            if (expOpt.TargetDBType.Equals(3))
                            {
                                writer3.WriteLine("GO");
                            }
                            writer3.WriteLine("");
                        }
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer3.WriteLine("GO");
                        }
                        writer3.Close();
                    }
                }
                if (expOpt.ExpForeignKeySql)
                {
                    using (StreamWriter writer4 = new StreamWriter(path, flag, Encoding.Unicode))
                    {
                        if (!flag)
                        {
                            flag = true;
                            writer4.WriteLine("/******************SP_MakaluConstraint*****************");
                            writer4.WriteLine("*                                                *");
                            writer4.WriteLine("*                                                *");
                            writer4.WriteLine("*************************************************/");
                            writer4.WriteLine("");
                            writer4.WriteLine("");
                        }
                        foreach (string str5 in tableNames)
                        {
                            writer4.WriteLine("/*************Create  Foreign Key**********************");
                            writer4.WriteLine("*  TableName" + str5 + "                        *");
                            writer4.WriteLine("**************************************************/");
                            writer4.WriteLine("");
                            writer4.Write(Sql2kUtil.GenerateFKSQL((SqlConnection)conn, str5, formater, expOpt.CheckForeignKey));
                            formater.SetBuffer("");
                            writer4.WriteLine("");
                            if (expOpt.TargetDBType.Equals(3))
                            {
                                writer4.WriteLine("GO");
                            }
                            writer4.WriteLine("");
                        }
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer4.WriteLine("GO");
                        }
                        writer4.Close();
                    }
                }
                if (expOpt.ExpCheckSql)
                {
                    using (StreamWriter writer5 = new StreamWriter(path, flag, Encoding.Unicode))
                    {
                        if (!flag)
                        {
                            flag = true;
                            writer5.WriteLine("/******************SP_MakaluConstraint*****************");
                            writer5.WriteLine("*                                                *");
                            writer5.WriteLine("*                                                *");
                            writer5.WriteLine("*************************************************/");
                            writer5.WriteLine("");
                            writer5.WriteLine("");
                        }
                        foreach (string str6 in tableNames)
                        {
                            writer5.WriteLine("/*************Create  Check**********************");
                            writer5.WriteLine("*  TableName" + str6 + "                        *");
                            writer5.WriteLine("**************************************************/");
                            writer5.WriteLine("");
                            writer5.Write(Sql2kUtil.GenerateCheckSQL((SqlConnection)conn, str6, formater, expOpt.CheckCheckRule));
                            formater.SetBuffer("");
                            writer5.WriteLine("");
                            if (expOpt.TargetDBType.Equals(3))
                            {
                                writer5.WriteLine("GO");
                            }
                            writer5.WriteLine("");
                        }
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer5.WriteLine("GO");
                        }
                        writer5.Close();
                    }
                }
                if (expOpt.ExpIndexSql)
                {
                    using (StreamWriter writer6 = new StreamWriter(path, flag, Encoding.Unicode))
                    {
                        if (!flag)
                        {
                            flag = true;
                            writer6.WriteLine("/******************SP_MakaluConstraint*****************");
                            writer6.WriteLine("*                                                *");
                            writer6.WriteLine("*                                                *");
                            writer6.WriteLine("*************************************************/");
                            writer6.WriteLine("");
                            writer6.WriteLine("");
                        }
                        foreach (string str7 in tableNames)
                        {
                            writer6.WriteLine("/*************Create Index**********************");
                            writer6.WriteLine("*  TableName" + str7 + "                        *");
                            writer6.WriteLine("**************************************************/");
                            writer6.WriteLine("");
                            writer6.Write(Sql2kUtil.GenerateIndexSQL((SqlConnection)conn, str7, formater, expOpt.CheckIndex));
                            formater.SetBuffer("");
                            writer6.WriteLine("");
                            if (expOpt.TargetDBType.Equals(3))
                            {
                                writer6.WriteLine("GO");
                            }
                            writer6.WriteLine("");
                        }
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer6.WriteLine("GO");
                        }
                        writer6.Close();
                    }
                }
                if (expOpt.ExpDataSql)
                {
                    if (expOpt.ExpTableFileName.Trim() == "")
                    {
                        path = Environment.CurrentDirectory + @"\SP_Data.sql";
                    }
                    else
                    {
                        path = expOpt.ExpTableFileName.Trim();
                    }
                    using (StreamWriter writer7 = new StreamWriter(path, false, Encoding.Unicode))
                    {
                        writer7.WriteLine("/******************SP_MakaluData*****************");
                        writer7.WriteLine("*                                                *");
                        writer7.WriteLine("*                                                *");
                        writer7.WriteLine("*************************************************/");
                        writer7.WriteLine("");
                        writer7.WriteLine("");
                        foreach (string str8 in tableNames)
                        {
                            writer7.WriteLine("/*************Create  Data**********************");
                            writer7.WriteLine("*  TableName" + str8 + "                        *");
                            writer7.WriteLine("**************************************************/");
                            writer7.WriteLine("");
                            Sql2kUtil.GenerateDataSQL((SqlConnection)conn, str8, formater, expOpt.CheckData, writer7);
                            formater.SetBuffer("");
                            writer7.WriteLine("");
                            if (expOpt.TargetDBType.Equals(3))
                            {
                                writer7.WriteLine("GO");
                            }
                            writer7.WriteLine("");
                        }
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer7.WriteLine("GO");
                        }
                        writer7.Close();
                    }
                }
                return "";
            }
            int num2 = 2;
            if (!num2.Equals(expOpt.SourceDBType))
            {
                int num3 = 8;
                if (!num3.Equals(expOpt.SourceDBType))
                {
                    int num4 = 7;
                    if (!num4.Equals(expOpt.SourceDBType))
                    {
                        return "";
                    }
                }
            }
            if (tableNames.Count < 1)
            {
                tableNames = Oralce8iUtil.getUserTableNames(conn);
            }
            if (expOpt.ExpTableSql)
            {
                if (expOpt.ExpTableFileName.Trim() == "")
                {
                    path = Environment.CurrentDirectory + @"\SP_CreateTable.sql";
                }
                else
                {
                    path = expOpt.ExpTableFileName.Trim();
                }
                using (StreamWriter writer8 = new StreamWriter(path, false, Encoding.Unicode))
                {
                    writer8.WriteLine("/******************SP_MakaluTable*****************");
                    writer8.WriteLine("*                                                *");
                    writer8.WriteLine("*                                                *");
                    writer8.WriteLine("*************************************************/");
                    writer8.WriteLine("");
                    writer8.WriteLine("");
                    foreach (string str9 in tableNames)
                    {
                        writer8.WriteLine("/******************Create  Table*****************");
                        writer8.WriteLine("*  TableName" + str9 + "                        *");
                        writer8.WriteLine("*************************************************/");
                        writer8.WriteLine("");
                        writer8.Write(Oralce8iUtil.GenerateTableSQL(conn, str9, formater, expOpt.CheckTable));
                        formater.SetBuffer("");
                        writer8.WriteLine("");
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer8.WriteLine("GO");
                        }
                        writer8.WriteLine("");
                    }
                    if (expOpt.TargetDBType.Equals(3))
                    {
                        writer8.WriteLine("GO");
                    }
                    writer8.Close();
                }
            }
            bool append = false;
            if (expOpt.ExpTableFileName.Trim() == "")
            {
                path = Environment.CurrentDirectory + @"\SP_Constraint.sql";
            }
            else
            {
                path = expOpt.ExpTableFileName.Trim();
            }
            if (expOpt.ExpPrimaryKeySql)
            {
                using (StreamWriter writer9 = new StreamWriter(path, append, Encoding.Unicode))
                {
                    if (!append)
                    {
                        append = true;
                        writer9.WriteLine("/******************SP_MakaluConstraint*****************");
                        writer9.WriteLine("*                                                *");
                        writer9.WriteLine("*                                                *");
                        writer9.WriteLine("*************************************************/");
                        writer9.WriteLine("");
                        writer9.WriteLine("");
                    }
                    foreach (string str10 in tableNames)
                    {
                        writer9.WriteLine("/*************Create  PrimaryKey*****************");
                        writer9.WriteLine("*  TableName" + str10 + "                        *");
                        writer9.WriteLine("**************************************************/");
                        writer9.WriteLine("");
                        writer9.Write(Oralce8iUtil.GeneratePKSQL(conn, str10, formater, expOpt.CheckPrimaryKey));
                        formater.SetBuffer("");
                        writer9.WriteLine("");
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer9.WriteLine("GO");
                        }
                        writer9.WriteLine("");
                    }
                    if (expOpt.TargetDBType.Equals(3))
                    {
                        writer9.WriteLine("GO");
                    }
                    writer9.Close();
                }
            }
            if (expOpt.ExpUniqueSql)
            {
                using (StreamWriter writer10 = new StreamWriter(path, append, Encoding.Unicode))
                {
                    if (!append)
                    {
                        append = false;
                        writer10.WriteLine("/******************SP_MakaluConstraint*****************");
                        writer10.WriteLine("*                                                *");
                        writer10.WriteLine("*                                                *");
                        writer10.WriteLine("*************************************************/");
                        writer10.WriteLine("");
                        writer10.WriteLine("");
                    }
                    foreach (string str11 in tableNames)
                    {
                        writer10.WriteLine("/*************Create  Unique**********************");
                        writer10.WriteLine("*  TableName" + str11 + "                        *");
                        writer10.WriteLine("**************************************************/");
                        writer10.WriteLine("");
                        writer10.Write(Oralce8iUtil.GenerateUniqueSQL(conn, str11, formater, expOpt.CheckUnique));
                        formater.SetBuffer("");
                        writer10.WriteLine("");
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer10.WriteLine("GO");
                        }
                        writer10.WriteLine("");
                    }
                    if (expOpt.TargetDBType.Equals(3))
                    {
                        writer10.WriteLine("GO");
                    }
                    writer10.Close();
                }
            }
            if (expOpt.ExpForeignKeySql)
            {
                using (StreamWriter writer11 = new StreamWriter(path, append, Encoding.Unicode))
                {
                    if (!append)
                    {
                        append = false;
                        writer11.WriteLine("/******************SP_MakaluConstraint*****************");
                        writer11.WriteLine("*                                                *");
                        writer11.WriteLine("*                                                *");
                        writer11.WriteLine("*************************************************/");
                        writer11.WriteLine("");
                        writer11.WriteLine("");
                    }
                    foreach (string str12 in tableNames)
                    {
                        writer11.WriteLine("/*************Create  Foreign Key**********************");
                        writer11.WriteLine("*  TableName" + str12 + "                        *");
                        writer11.WriteLine("**************************************************/");
                        writer11.WriteLine("");
                        writer11.Write(Oralce8iUtil.GenerateFKSQL(conn, str12, formater, expOpt.CheckForeignKey));
                        formater.SetBuffer("");
                        writer11.WriteLine("");
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer11.WriteLine("GO");
                        }
                        writer11.WriteLine("");
                    }
                    if (expOpt.TargetDBType.Equals(3))
                    {
                        writer11.WriteLine("GO");
                    }
                    writer11.Close();
                }
            }
            if (expOpt.ExpIndexSql)
            {
                using (StreamWriter writer12 = new StreamWriter(path, append, Encoding.Unicode))
                {
                    if (!append)
                    {
                        append = false;
                        writer12.WriteLine("/******************SP_MakaluConstraint*****************");
                        writer12.WriteLine("*                                                *");
                        writer12.WriteLine("*                                                *");
                        writer12.WriteLine("*************************************************/");
                        writer12.WriteLine("");
                        writer12.WriteLine("");
                    }
                    foreach (string str13 in tableNames)
                    {
                        writer12.WriteLine("/*************Create Index**********************");
                        writer12.WriteLine("*  TableName" + str13 + "                        *");
                        writer12.WriteLine("**************************************************/");
                        writer12.WriteLine("");
                        writer12.Write(Oralce8iUtil.GenerateIndexSQL(conn, str13, formater, expOpt.CheckIndex));
                        formater.SetBuffer("");
                        writer12.WriteLine("");
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer12.WriteLine("GO");
                        }
                        writer12.WriteLine("");
                    }
                    if (expOpt.TargetDBType.Equals(3))
                    {
                        writer12.WriteLine("GO");
                    }
                    writer12.Close();
                }
            }
            if (expOpt.ExpDataSql)
            {
                if (expOpt.ExpTableFileName.Trim() == "")
                {
                    path = Environment.CurrentDirectory + @"\SP_Data.sql";
                }
                else
                {
                    path = expOpt.ExpTableFileName.Trim();
                }
                using (StreamWriter writer13 = new StreamWriter(path, false, Encoding.Unicode))
                {
                    writer13.WriteLine("/******************SP_MakaluData*****************");
                    writer13.WriteLine("*                                                *");
                    writer13.WriteLine("*                                                *");
                    writer13.WriteLine("*************************************************/");
                    writer13.WriteLine("");
                    writer13.WriteLine("");
                    foreach (string str14 in tableNames)
                    {
                        writer13.WriteLine("/*************Create  Data**********************");
                        writer13.WriteLine("*  TableName" + str14 + "                        *");
                        writer13.WriteLine("**************************************************/");
                        writer13.WriteLine("");
                        Oralce8iUtil.GenerateDataSQL(conn, str14, formater, expOpt.CheckData, writer13);
                        formater.SetBuffer("");
                        writer13.WriteLine("");
                        if (expOpt.TargetDBType.Equals(3))
                        {
                            writer13.WriteLine("GO");
                        }
                        writer13.WriteLine("");
                    }
                    if (expOpt.TargetDBType.Equals(3))
                    {
                        writer13.WriteLine("GO");
                    }
                    writer13.Close();
                }
            }
            return "";
        }

        private static string[] getArrayOption(string url, string name, string separator)
        {
            string str = getOption(url, name);
            if (str == null)
            {
                return null;
            }
            return str.Split(separator.ToCharArray());
        }

        private static bool getBoolOption(string url, string name, bool defValue)
        {
            string str = getOption(url, name);
            if (str == null)
            {
                return defValue;
            }
            return "true".Equals(str);
        }

        public static string getConnectStackTrace(IDbConnection conn)
        {
            string str;
            IDbCommand stmt = null;
            IDataReader reader = null;
            stmt = conn.CreateCommand();
            try
            {
                stmt.CommandText = "/*ksql_internal*/SELECT CONNECT_STACKTRACE FROM KSQL_ENV";
                reader = stmt.ExecuteReader();
                reader.Read();
                str = (string)reader[1];
            }
            finally
            {
                cleanUp(null, stmt);
            }
            return str;
        }

        public static int getDbType(IDbConnection conn)
        {
            int num;
            IDbCommand stmt = null;
            IDataReader reader = null;
            stmt = conn.CreateCommand();
            try
            {
                stmt.CommandText = "/*ksql_internal*/SELECT DBTYPE FROM KSQL_ENV";
                reader = stmt.ExecuteReader();
                reader.Read();
                num = (int)reader[1];
            }
            finally
            {
                cleanUp(null, stmt);
            }
            return num;
        }

        public static string getDbTypeName(IDbConnection conn)
        {
            string str;
            IDbCommand stmt = null;
            IDataReader reader = null;
            stmt = conn.CreateCommand();
            try
            {
                stmt.CommandText = "/*ksql_internal*/SELECT DBTYPE_NAME FROM KSQL_ENV";
                reader = stmt.ExecuteReader();
                reader.Read();
                str = (string)reader[1];
            }
            finally
            {
                cleanUp(null, stmt);
            }
            return str;
        }

        private static int getIntOption(string url, string name, int defValue)
        {
            string s = getOption(url, name);
            if (s == null)
            {
                return defValue;
            }
            try
            {
                return int.Parse(s);
            }
            catch (System.Exception)
            {
                return defValue;
            }
        }

        public static int getNameType(string name)
        {
            if (name == null)
            {
                throw new ArgumentException("methodName is null");
            }
            name = name.ToUpper();
            if (methodTypeMap[name] != null)
            {
                int num1 = (int)methodTypeMap[name];
                return 0;
            }
            if (KeyWord.instance.IsKeyWord(name))
            {
                return 1;
            }
            return -1;
        }

        public static IList GetObjectCheckStmt(KSqlCheckObjType objType, string objName, string tableName)
        {
            string str;
            if (objName == "")
            {
                return null;
            }
            switch (objType)
            {
                case KSqlCheckObjType.Table:
                    return TransUtil.getStmtList(("IF EXISTS (SELECT 1 FROM KSQL_USERTABLES WHERE KSQL_TABNAME = '" + objName + "')\n") + "\tDROP TABLE " + objName + ";");

                case KSqlCheckObjType.PrimaryKey:
                case KSqlCheckObjType.Unique:
                case KSqlCheckObjType.ForeignKey:
                case KSqlCheckObjType.Check:
                    str = "IF EXISTS (SELECT 1 FROM KSQL_CONSTRAINTS WHERE KSQL_CONS_NAME = '" + objName + "' AND KSQL_CONS_TABNAME = '" + tableName + "')\n";
                    return TransUtil.getStmtList(str + "\tALTER TABLE " + tableName + " DROP CONSTRAINT " + objName + " ;");

                case KSqlCheckObjType.Index:
                    str = "IF EXISTS (SELECT 1 FROM KSQL_INDEXES WHERE KSQL_INDNAME = '" + objName + "')\n";
                    return TransUtil.getStmtList(str + "\tDROP INDEX " + tableName + "." + objName + " ;");

                case KSqlCheckObjType.AllData:
                    return TransUtil.getStmtList("TRUNCATE TABLE " + tableName + ";\n");
            }
            throw new ArgumentException("dbType, value is " + objType);
        }

        public static int getOptimizeMode()
        {
            return optimizeMode;
        }

        private static string getOption(string oUrl, string name)
        {
            if ((name == null) || (oUrl == null))
            {
                return null;
            }
            string str = oUrl.ToLower();
            name = name.ToLower();
            int index = str.IndexOf(":" + name + "=");
            if (index == -1)
            {
                return null;
            }
            index += name.Length + 2;
            int end = str.IndexOf(":", index);
            if (end == -1)
            {
                return null;
            }
            return oUrl.substring(index, end).Replace(@"\|", ":");
        }

        public static string getOracleCurrentUser(IDbConnection conn)
        {
            string str = null;
            string str2 = "SELECT TOP 1 B.OWNER FROM USER_TABLES A INNER JOIN ALL_TABLES B ON A.TABLE_NAME = B.TABLE_NAME WHERE B.OWNER != 'SYS'";
            IDbCommand stmt = conn.CreateCommand();
            IDataReader rs = null;
            try
            {
                stmt.CommandText = str2;
                rs = stmt.ExecuteReader();
                if (rs.Read())
                {
                    str = (string)rs[1];
                }
            }
            finally
            {
                cleanUp(stmt, rs);
            }
            return str;
        }

        private static string getProperOwnerString(SqlExpr expr)
        {
            if (expr is SqlBinaryOpExpr)
            {
                SqlBinaryOpExpr expr2 = (SqlBinaryOpExpr)expr;
                if ((expr2.Operator == 20) && (expr2.left is SqlIdentifierExpr))
                {
                    SqlIdentifierExpr left = (SqlIdentifierExpr)expr2.left;
                    return left.value;
                }
            }
            return null;
        }

        public static SqlExpr getRootExpr(SqlExpr expr)
        {
            if (expr is SqlBinaryOpExpr)
            {
                SqlBinaryOpExpr expr2 = (SqlBinaryOpExpr)expr;
                if (expr2.Operator == 20)
                {
                    return getRootExpr(expr2.left);
                }
            }
            return expr;
        }

        public static string getSID(string ip, string port)
        {
            string[] strArray = ip.Split(@"\.".ToCharArray());
            if (strArray.Length == 4)
            {
                bool flag = false;
                int l = 0;
                try
                {
                    l = int.Parse(port);
                    flag = true;
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
                if (flag)
                {
                    int num2 = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        int num4 = (4 - i) * 8;
                        num2 = (int.Parse(strArray[i]) << num4) | num2;
                    }
                    return (normalize(num2) + normalize(l));
                }
            }
            return normalize((ip + ":" + port).GetHashCode());
        }

        public static int getSqlType(string methodName)
        {
            if (methodName == null)
            {
                throw new ArgumentException("methodName is null");
            }
            methodName = methodName.ToUpper();
            return (int)methodTypeMap[methodName];
        }

        public static string getURL(IDbConnection conn)
        {
            string str;
            IDbCommand stmt = null;
            IDataReader reader = null;
            stmt = conn.CreateCommand();
            try
            {
                stmt.CommandText = "/*ksql_internal*/SELECT URL FROM KSQL_ENV";
                reader = stmt.ExecuteReader();
                reader.Read();
                str = (string)reader[1];
            }
            finally
            {
                cleanUp(null, stmt);
            }
            return str;
        }

        private static bool isPropExpr(SqlExpr expr)
        {
            if (expr is SqlBinaryOpExpr)
            {
                SqlBinaryOpExpr expr2 = (SqlBinaryOpExpr)expr;
                if (expr2.Operator == 20)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool isSimpleJoinCondition(SqlExpr expr)
        {
            if (expr is SqlBinaryOpExpr)
            {
                SqlBinaryOpExpr expr2 = (SqlBinaryOpExpr)expr;
                if (expr2.Operator == 10)
                {
                    SqlExpr left = expr2.left;
                    SqlExpr right = expr2.right;
                    return (isPropExpr(left) && isPropExpr(right));
                }
            }
            return false;
        }

        private static string normalize(int l)
        {
            return l.ToString("35").ToUpper().Replace("-", "Z");
        }

        public static void optimize(SqlSelect select)
        {
            optimize(select, optimizeMode);
        }

        public static string optimize(object sqlObject)
        {
            return optimize((string)sqlObject);
        }

        public static string optimize(string sql)
        {
            return optimize(sql, optimizeMode);
        }

        private static void optimize(SqlSelect select, int mode)
        {
            Hashtable hashtable = null;
            try
            {
                hashtable = adjust(select, mode);
                optimize_table_source(select.tableSource, mode);
            }
            catch (ParserException exception)
            {
               // logger.Warn(exception.Message);
            }
            Hashtable usedTableAliasSet = new Hashtable();
            IEnumerator enumerator = select.selectList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SqlSelectItem current = (SqlSelectItem)enumerator.Current;
                if (computeUsedTableAlias(usedTableAliasSet, current.expr))
                {
                    return;
                }
            }
            if (!computeUsedTableAlias(usedTableAliasSet, select.condition))
            {
                enumerator = select.groupBy.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SqlExpr expr = (SqlExpr)enumerator.Current;
                    if (computeUsedTableAlias(usedTableAliasSet, expr))
                    {
                        return;
                    }
                }
                if (!computeUsedTableAlias(usedTableAliasSet, select.having))
                {
                    enumerator = select.orderBy.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        SqlOrderByItem item2 = (SqlOrderByItem)enumerator.Current;
                        if (computeUsedTableAlias(usedTableAliasSet, item2.expr))
                        {
                            return;
                        }
                    }
                    SqlTableSourceBase tableSource = select.tableSource;
                    Hashtable usedAliasMap = new Hashtable();
                    computeJoinConditionUsedAlias(usedAliasMap, tableSource);
                    tableSource = cleanTableSource(usedTableAliasSet, usedAliasMap, tableSource, mode);
                    select.tableSource = tableSource;
                    if (hashtable != null)
                    {
                        IEnumerator enumerator2 = select.orderBy.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            SqlOrderByItem item3 = (SqlOrderByItem)enumerator2.Current;
                            string str = item3.expr.toString();
                            IEnumerator enumerator3 = hashtable.GetEnumerator();
                            while (enumerator3.MoveNext())
                            {
                                object obj2 = enumerator3.Current;
                                object obj3 = hashtable[obj2];
                                if (str.EqualsIgnoreCase(obj3.ToString()))
                                {
                                    try
                                    {
                                        item3.expr = new SqlExprParser(obj2.ToString()).expr();
                                        continue;
                                    }
                                    catch (System.Exception exception2)
                                    {
                                        //logger.Warn(exception2.Message);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string optimize(object sqlObject, int optimizeMode)
        {
            return optimize((string)sqlObject, optimizeMode);
        }

        public static string optimize(string sql, int optimizeMode)
        {
            string str = singleOptimize(sql, optimizeMode);
            string str2 = singleOptimize(str, optimizeMode);
            while (!str2.Equals(str))
            {
                str = str2;
                str2 = singleOptimize(str, optimizeMode);
            }
            return str2;
        }

        private static void optimize_selectbase(SqlSelectBase selectBase, int mode)
        {
            if (selectBase is SqlSelect)
            {
                optimize((SqlSelect)selectBase, mode);
            }
            else if (selectBase is SqlUnionSelect)
            {
                SqlUnionSelect select = (SqlUnionSelect)selectBase;
                optimize_selectbase(select.left, mode);
                optimize_selectbase(select.right, mode);
            }
        }

        public static void optimize_table_source(SqlTableSourceBase tableSource, int mode)
        {
            if (tableSource is SqlJoinedTableSource)
            {
                SqlJoinedTableSource source = (SqlJoinedTableSource)tableSource;
                optimize_table_source(source.left, mode);
                optimize_table_source(source.right, mode);
            }
            if (tableSource is SqlSubQueryTableSource)
            {
                SqlSubQueryTableSource source2 = (SqlSubQueryTableSource)tableSource;
                optimize_selectbase(source2.subQuery, mode);
            }
        }

        public static TraceInfo parseURL(string url)
        {
            return parseURL(url, 0x6ddd00);
        }

        public static TraceInfo parseURL(string url, int lifecycle)
        {
            string[] strArray = new Regex(url).Split(";");
            if (!strArray[0].EqualsIgnoreCase("jdbc"))
            {
                throw new SystemException("invalid url");
            }
            if (!strArray[1].EqualsIgnoreCase("ksql"))
            {
                throw new SystemException("invalid url");
            }
            string driverName = strArray[2];
            string name = getOption(url, "dbtype");
            if (name == null)
            {
                throw new SystemException("you must set dbtype");
            }
            int dbType = DatabaseType.getValue(name);
            int bindPort = getIntOption(url, "bindport", -1);
            int optimizeMode = getIntOption(url, "optimize", 1);
            bool translate = getBoolOption(url, "translate", true);
            bool nologging = getBoolOption(url, "noLogging", false);
            string dbSchema = getOption(url, "dbSchema");
            string str4 = getOption(url, "trace");
            string filter = getOption(url, "filter");
            string logFileUrl = getOption(url, "file");
            string[] tmpTblSpcs = getArrayOption(url, "temptablespace", ",");
            string originalUrl = url.Substring(url.IndexOf(":jdbc:") + 1);
            TraceInfo info = new TraceInfo(driverName, dbType, originalUrl, "on".EqualsIgnoreCase(str4), logFileUrl, filter, translate, bindPort, optimizeMode, tmpTblSpcs, url, (long)lifecycle);
            FormatOptions formatOptions = new FormatOptions();
            formatOptions.SetDbSchema(dbSchema);
            formatOptions.SetNologging(nologging);
            formatOptions.SetTableSchema(getOption(url, "tableSchema"));
            formatOptions.SetTempTableSpaces(tmpTblSpcs);
            info.setFormatOptions(formatOptions);
            return info;
        }

        public static void printResultSet(IDataReader rs)
        {
            DataTable schemaTable = rs.GetSchemaTable();
            int count = schemaTable.Columns.Count;
            for (int i = 1; i <= count; i++)
            {
                string columnName = schemaTable.Columns[i].ColumnName;
                if (i != 1)
                {
                    Console.Write("\t");
                }
                Console.Write(columnName);
            }
            Console.WriteLine("");
            while (rs.Read())
            {
                for (int j = 1; j <= count; j++)
                {
                    Type type = schemaTable.Columns[j].GetType();
                    object obj2 = rs[j];
                    if (j != 1)
                    {
                        Console.Write("\t");
                    }
                    if (type.Equals(Type.GetType("DateTime")))
                    {
                        Console.Write(((DateTime)rs[j]).GetDateTimeFormats());
                    }
                    else
                    {
                        Console.Write(obj2);
                    }
                }
                Console.WriteLine("");
            }
        }

        private static SqlExpr replace_for_join_condition(Hashtable joinConditionExprCanReplaceMap, SqlExpr expr)
        {
            if (expr == null)
            {
                return null;
            }
            if (!(expr is SqlAllColumnExpr))
            {
                if (expr is SqlIdentifierExpr)
                {
                    string str = expr.toString();
                    object obj2 = joinConditionExprCanReplaceMap[str.ToUpper()];
                    if (obj2 != null)
                    {
                        return new SqlExprParser(obj2.ToString()).expr();
                    }
                    return expr;
                }
                if (isPropExpr(expr))
                {
                    string str2 = expr.toString();
                    object obj3 = joinConditionExprCanReplaceMap[str2.ToUpper()];
                    if (obj3 != null)
                    {
                        return new SqlExprParser(obj3.ToString()).expr();
                    }
                    return expr;
                }
                if (expr is SqlBinaryOpExpr)
                {
                    SqlBinaryOpExpr expr2 = (SqlBinaryOpExpr)expr;
                    if (expr2.Operator != 20)
                    {
                        string str3 = expr2.left.toString();
                        string str4 = expr2.right.toString();
                        if (!str3.EqualsIgnoreCase(((string)joinConditionExprCanReplaceMap[str4.ToUpper()])) && !str4.EqualsIgnoreCase(((string)joinConditionExprCanReplaceMap[str3.ToUpper()])))
                        {
                            expr2.left = replace_for_join_condition(joinConditionExprCanReplaceMap, expr2.left);
                        }
                    }
                    return expr;
                }
                if (expr is SqlMethodInvokeExpr)
                {
                    SqlMethodInvokeExpr expr3 = (SqlMethodInvokeExpr)expr;
                    int num = 0;
                    int count = expr3.parameters.Count;
                    while (num < count)
                    {
                        SqlExpr expr4 = (SqlExpr)expr3.parameters[num];
                        expr4 = replace_for_join_condition(joinConditionExprCanReplaceMap, expr4);
                        expr3.parameters[num] = expr4;
                        num++;
                    }
                    return expr;
                }
                if (expr is SqlAggregateExpr)
                {
                    SqlAggregateExpr expr5 = (SqlAggregateExpr)expr;
                    int num3 = 0;
                    int num4 = expr5.paramList.Count;
                    while (num3 < num4)
                    {
                        SqlExpr expr6 = (SqlExpr)expr5.paramList[num3];
                        expr6 = replace_for_join_condition(joinConditionExprCanReplaceMap, expr6);
                        expr5.paramList[num3] = expr6;
                        num3++;
                    }
                    return expr;
                }
                if (expr is SqlBetweenExpr)
                {
                    SqlBetweenExpr expr7 = (SqlBetweenExpr)expr;
                    expr7.testExpr = replace_for_join_condition(joinConditionExprCanReplaceMap, expr7.testExpr);
                    expr7.beginExpr = replace_for_join_condition(joinConditionExprCanReplaceMap, expr7.beginExpr);
                    expr7.endExpr = replace_for_join_condition(joinConditionExprCanReplaceMap, expr7.endExpr);
                    return expr;
                }
                if (expr is SqlCaseExpr)
                {
                    SqlCaseExpr expr8 = (SqlCaseExpr)expr;
                    int num5 = 0;
                    int num6 = expr8.itemList.Count;
                    while (num5 < num6)
                    {
                        SqlCaseItem item = (SqlCaseItem)expr8.itemList[num5];
                        item.conditionExpr = replace_for_join_condition(joinConditionExprCanReplaceMap, item.conditionExpr);
                        item.valueExpr = replace_for_join_condition(joinConditionExprCanReplaceMap, item.valueExpr);
                        num5++;
                    }
                    expr8.valueExpr = replace_for_join_condition(joinConditionExprCanReplaceMap, expr8.valueExpr);
                    expr8.elseExpr = replace_for_join_condition(joinConditionExprCanReplaceMap, expr8.elseExpr);
                    return expr;
                }
                if (expr is SqlInListExpr)
                {
                    SqlInListExpr expr9 = (SqlInListExpr)expr;
                    expr9.expr = replace_for_join_condition(joinConditionExprCanReplaceMap, expr9.expr);
                    int index = 0;
                    int num8 = expr9.targetList.Count;
                    while (index < num8)
                    {
                        SqlExpr expr10 = (SqlExpr)expr9.targetList[index];
                        expr10 = replace_for_join_condition(joinConditionExprCanReplaceMap, expr10);
                        expr9.targetList.SetRange(index, (ICollection)expr10);
                        index++;
                    }
                    return expr;
                }
                if (expr is SqlInSubQueryExpr)
                {
                    SqlInSubQueryExpr expr11 = (SqlInSubQueryExpr)expr;
                    expr11.expr = replace_for_join_condition(joinConditionExprCanReplaceMap, expr11.expr);
                    return expr;
                }
                if (expr is SqlNotExpr)
                {
                    SqlNotExpr expr12 = (SqlNotExpr)expr;
                    expr12.expr = replace_for_join_condition(joinConditionExprCanReplaceMap, expr12.expr);
                }
            }
            return expr;
        }

        private static SqlExpr replace_for_Select_condition(Hashtable conditionExprCanReplaceMap, SqlExpr expr)
        {
            if (expr == null)
            {
                return null;
            }
            if (!(expr is SqlAllColumnExpr))
            {
                if (expr is SqlIdentifierExpr)
                {
                    string str = expr.toString();
                    string text = (string)conditionExprCanReplaceMap[str.ToUpper()];
                    if (text != null)
                    {
                        return new SqlExprParser(text).expr();
                    }
                    return expr;
                }
                if (isPropExpr(expr))
                {
                    string str3 = expr.toString();
                    string str4 = (string)conditionExprCanReplaceMap[str3.ToUpper()];
                    if (str4 != null)
                    {
                        return new SqlExprParser(str4).expr();
                    }
                    return expr;
                }
                if (expr is SqlBinaryOpExpr)
                {
                    SqlBinaryOpExpr expr2 = (SqlBinaryOpExpr)expr;
                    List<SqlExpr> list = new List<SqlExpr> {
                    expr2
                };
                    while (list != null)
                    {
                        SqlExpr expr3 = list[0];
                        list.Remove(expr3);
                        if (expr3 is SqlBinaryOpExpr)
                        {
                            SqlBinaryOpExpr expr4 = (SqlBinaryOpExpr)expr3;
                            if (((expr4.Operator != 13) && (expr4.Operator != 0x29)) && (expr4.Operator != 20))
                            {
                                string str5 = expr4.left.toString();
                                string str6 = expr4.right.toString();
                                if (!str5.EqualsIgnoreCase(((string)conditionExprCanReplaceMap[str6.ToUpper()])) && !str6.EqualsIgnoreCase(((string)conditionExprCanReplaceMap[str5.ToUpper()])))
                                {
                                    if (!(expr4.left is SqlBinaryOpExpr) || isPropExpr(expr4.left))
                                    {
                                        expr4.left = replace_for_Select_condition(conditionExprCanReplaceMap, expr4.left);
                                    }
                                    else
                                    {
                                        list.Add(expr4.left);
                                    }
                                    if (!(expr4.right is SqlBinaryOpExpr) || isPropExpr(expr4.right))
                                    {
                                        expr4.right = replace_for_Select_condition(conditionExprCanReplaceMap, expr4.right);
                                    }
                                    else
                                    {
                                        list.Add(expr4.right);
                                    }
                                }
                            }
                        }
                    }
                    return expr;
                }
                if (expr is SqlMethodInvokeExpr)
                {
                    SqlMethodInvokeExpr expr5 = (SqlMethodInvokeExpr)expr;
                    int index = 0;
                    int count = expr5.parameters.Count;
                    while (index < count)
                    {
                        SqlExpr expr6 = (SqlExpr)expr5.parameters[index];
                        expr6 = replace_for_Select_condition(conditionExprCanReplaceMap, expr6);
                        expr5.parameters.SetRange(index, (ICollection)expr6);
                        index++;
                    }
                    return expr;
                }
                if (expr is SqlAggregateExpr)
                {
                    SqlAggregateExpr expr7 = (SqlAggregateExpr)expr;
                    int num3 = 0;
                    int num4 = expr7.paramList.Count;
                    while (num3 < num4)
                    {
                        SqlExpr expr8 = (SqlExpr)expr7.paramList[num3];
                        expr8 = replace_for_Select_condition(conditionExprCanReplaceMap, expr8);
                        expr7.paramList.SetRange(num3, (ICollection)expr8);
                        num3++;
                    }
                    return expr;
                }
                if (expr is SqlBetweenExpr)
                {
                    SqlBetweenExpr expr9 = (SqlBetweenExpr)expr;
                    expr9.testExpr = replace_for_Select_condition(conditionExprCanReplaceMap, expr9.testExpr);
                    expr9.beginExpr = replace_for_Select_condition(conditionExprCanReplaceMap, expr9.beginExpr);
                    expr9.endExpr = replace_for_Select_condition(conditionExprCanReplaceMap, expr9.endExpr);
                    return expr;
                }
                if (expr is SqlCaseExpr)
                {
                    SqlCaseExpr expr10 = (SqlCaseExpr)expr;
                    int num5 = 0;
                    int num6 = expr10.itemList.Count;
                    while (num5 < num6)
                    {
                        SqlCaseItem item = (SqlCaseItem)expr10.itemList[num5];
                        item.conditionExpr = replace_for_Select_condition(conditionExprCanReplaceMap, item.conditionExpr);
                        item.valueExpr = replace_for_Select_condition(conditionExprCanReplaceMap, item.valueExpr);
                        num5++;
                    }
                    expr10.valueExpr = replace_for_Select_condition(conditionExprCanReplaceMap, expr10.valueExpr);
                    expr10.elseExpr = replace_for_Select_condition(conditionExprCanReplaceMap, expr10.elseExpr);
                    return expr;
                }
                if (expr is SqlInListExpr)
                {
                    SqlInListExpr expr11 = (SqlInListExpr)expr;
                    expr11.expr = replace_for_Select_condition(conditionExprCanReplaceMap, expr11.expr);
                    int num7 = 0;
                    int num8 = expr11.targetList.Count;
                    while (num7 < num8)
                    {
                        SqlExpr expr12 = (SqlExpr)expr11.targetList[num7];
                        expr12 = replace_for_Select_condition(conditionExprCanReplaceMap, expr12);
                        expr11.targetList.SetRange(num7, (ICollection)expr12);
                        num7++;
                    }
                    return expr;
                }
                if (expr is SqlInSubQueryExpr)
                {
                    SqlInSubQueryExpr expr13 = (SqlInSubQueryExpr)expr;
                    expr13.expr = replace_for_Select_condition(conditionExprCanReplaceMap, expr13.expr);
                    return expr;
                }
                if (expr is SqlNotExpr)
                {
                    SqlNotExpr expr14 = (SqlNotExpr)expr;
                    expr14.expr = replace_for_Select_condition(conditionExprCanReplaceMap, expr14.expr);
                }
            }
            return expr;
        }

        private static void replaceTableSourceJoinCondition(Hashtable joinConditionExprMap, SqlTableSourceBase tabSrcBase)
        {
            if (tabSrcBase is SqlJoinedTableSource)
            {
                SqlJoinedTableSource source = (SqlJoinedTableSource)tabSrcBase;
                source.condition = replace_for_join_condition(joinConditionExprMap, source.condition);
                replaceTableSourceJoinCondition(joinConditionExprMap, source.left);
                replaceTableSourceJoinCondition(joinConditionExprMap, source.right);
            }
        }

        public static void setOptimizeMode(int mode)
        {
        }

        private static string singleOptimize(string sql, int mode)
        {
            if (mode == -1)
            {
                return sql;
            }
            SqlSelectBase selectBase = new SelectParser(sql).select();
            optimize_selectbase(selectBase, mode);
            StringBuilder sb = new StringBuilder();
            new DrSQLFormater(sb).FormatSelectBase(selectBase);
            return sb.ToString();
        }
    }





}
