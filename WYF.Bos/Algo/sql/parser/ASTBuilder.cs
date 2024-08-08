using WYF.Bos.algo.sql.g;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.parser
{
    public class ASTBuilder: GBaseVisitor<Object>
    {
        private String originText;
        public ASTBuilder(String originText)
        {
            this.originText = originText;
        }

        public override object VisitSingleExpression(GParser.SingleExpressionContext ctx)
        {
            
            return base.VisitSingleExpression(ctx);
        }

    }
}
