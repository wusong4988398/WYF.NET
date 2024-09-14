using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Util
{
    public interface SqlBinaryOpExprProcessor
    {
   
        void process(SqlBinaryOpExpr expr, int leftBracket, int rightBracket, Hashtable options);
    }





}
