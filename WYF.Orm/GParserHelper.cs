using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Orm.Query.G.Visitor;


namespace WYF.SqlParser
{
    public class GParserHelper
    {
        public static PropertySegExpress Parse(string s)
        {
            TSqlParser sqlParser = new TSqlParser();
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
    }
}
