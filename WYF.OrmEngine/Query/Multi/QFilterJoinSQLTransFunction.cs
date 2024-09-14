using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query.Multi
{
    public class QFilterJoinSQLTransFunction
    {

        public QFilter SQLTransFunction(QFilter filter)
        {
            if (filter.IsJoinSQLFilter())
            {
                QParameter qp = new QParameter(filter.Property, (Object[])filter.Value);
                qp.SetSQLBuilder((sql, ctx) =>
                {
                    string joinEntity = filter.JoinEntityPath;
                    string joinEntityAlias = ctx.GetSimpleEntityAlias(joinEntity);
                    return ReplaceEntityNameAsAlias(sql, joinEntity.ToLower(), joinEntityAlias);
                });
                filter.SelfDefinedQParameter = qp;

            }
            return filter;
        }


        private string ReplaceEntityNameAsAlias(string sql, string entityName, string alias)
        {
            entityName += ".";
            alias += ".";
            int entityNameLen = entityName.Length;
            char[] lowerEntityName = entityName.ToLower().ToCharArray();
            char[] upperEntityName = entityName.ToUpper().ToCharArray();
            StringBuilder ret = new StringBuilder(sql.Length);
            StringBuilder segName = new StringBuilder(entityNameLen);
            bool inQuote = false;
            int segI = 0;
            char[] chs = sql.ToCharArray();
            bool preIsLetterOrDigit = false;

            for (int i = 0, n = chs.Length; i < n; i++)
            {
                char ch = chs[i];

                if (ch == '\'')
                {
                    if (segI != 0)
                    {
                        segI = 0;
                        ret.Append(segName);
                        segName.Length = 0;
                    }
                    inQuote = !inQuote;
                    ret.Append(ch);
                    preIsLetterOrDigit = false;
                }
                else
                {
                    if (inQuote || (segI == 0 && preIsLetterOrDigit))
                    {
                        ret.Append(ch);
                    }
                    else if (ch == lowerEntityName[segI] || ch == upperEntityName[segI])
                    {
                        segName.Append(ch);
                        segI++;

                        if (segI == entityNameLen)
                        {
                            ret.Append(alias);
                            segName.Length = 0;
                            segI = 0;
                        }
                    }
                    else
                    {
                        if (segI != 0)
                        {
                            ret.Append(segName);
                            segName.Length = 0;
                            segI = 0;
                        }
                        ret.Append(ch);
                    }
                    preIsLetterOrDigit = char.IsLetterOrDigit(ch);
                }
            }

            if (segName.Length > 0)
                ret.Append(segName);

            return ret.ToString();
        }

        //public  Func<QFilter, QFilter> sQLTransFunction(QFilter filter)
        //{
        //    if (filter.IsJoinSQLFilter())
        //    {
        //        QParameter qp = new QParameter(filter.Property, (Object[])filter.Value);
        //        qp.SetSQLBuilder((sql, ctx) =>
        //        {
        //            string joinEntity = filter.JoinEntityPath;
        //            string joinEntityAlias = ctx.GetSimpleEntityAlias(joinEntity);
        //            return ReplaceEntityNameAsAlias(sql, joinEntity.ToLower(), joinEntityAlias);
        //        });
        //        filter.__setSelfDefinedQParameter(qp);
        //    }

        //    return filter =>
        //{
        //    return filter;
        //};
        //}





    }

}
