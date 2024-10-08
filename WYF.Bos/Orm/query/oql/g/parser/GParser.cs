﻿using WYF.Bos.algo.sql.parser;
using WYF.Bos.algo.sql.tree;
using WYF.Bos.Orm.query.multi;
using WYF.Bos.Orm.query.oql.g.expr;
using WYF.Bos.Orm.query.oql.g.visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.oql.g.parser
{
    public  class GParser
    {
        public static PropertySegExpress Parse(string s)
        {
            SqlParser sqlParser = new SqlParser();
            Expr exp = sqlParser.ParseExpr(s);
            return Parse(exp);
        }

        public static PropertySegExpress Parse(Expr exp)
        {
            ParsePropertyVisitor ppv = new ParsePropertyVisitor();
            Object context = null;
            exp.Accept(ppv, context);
            return ppv.PropertySegExpress;
        }

        public static SelectFields ParseSelectFields(string s)
        {
            return (SelectFields)ParseSegment(s, SegmentType.select_fields);
        }

        public static WhereQFilters parseWhereFilters(string s)
        {
            return (WhereQFilters)ParseSegment(s, SegmentType.where_filters);
        }

        public static OrderBys ParseOrderBys(string s)
        {
            return (OrderBys)ParseSegment(s, SegmentType.orderbys);
        }

        public static GroupBys ParseGroupBys(string s)
        {
            return (GroupBys)ParseSegment(s, SegmentType.groupbys);
        }

        private static OQLable ParseSegment(string s, SegmentType st)
        {
            SqlParser sqlParser = new SqlParser();
            Expr expr = sqlParser.ParseExpr(s);
            switch (st)
            {
                case SegmentType.select_fields:
                    return (OQLable)new SelectFields(expr);
                case SegmentType.where_filters:
                    return (OQLable)new WhereQFilters(expr);
                case SegmentType.orderbys:
                    return (OQLable)new OrderBys(expr);
                case SegmentType.groupbys:
                    return (OQLable)new GroupBys(expr);
            }
            throw new NotSupportedException("Unsupported SegmentType: " + st);
        }
    }
}