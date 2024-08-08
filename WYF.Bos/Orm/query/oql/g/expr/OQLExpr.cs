using WYF.Bos.algo.sql.tree;
using WYF.Bos.Orm.query.multi;
using WYF.Bos.Orm.query.oql.g.visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.oql.g.expr
{
    public abstract class OQLExpr : OQLable
    {
        private PropertySegExpress pse;

        public Expr Expr { get; private set; }

        public OQLExpr(Expr expr)
        {
            this.Expr = expr;
        }

  

        public override string ToString()
        {
            return this.Expr.ToString();
        }
        public string GetPropertyObjName(List<string> fullPropertyNames, string rootObjName)
        {
            string fullObjectName = null;
            if (fullPropertyNames.Count() > 0)
            {
                int len = 0;
                foreach (string fullPropertyName in fullPropertyNames)
                {
                    int dot = fullPropertyName.LastIndexOf('.');
                    if (dot != -1)
                    {
                        string sameParentObj = fullPropertyName.Substring(0, dot);
                        if (string.IsNullOrEmpty(fullObjectName) || len > sameParentObj.Length)
                        {
                            fullObjectName = sameParentObj;
                            len = fullObjectName.Length;
                        }
                    }
                }
       
                if (string.IsNullOrEmpty(fullObjectName))
                {
                    fullObjectName = rootObjName;
                }
                else
                {
                    fullObjectName = rootObjName + "." + fullObjectName;
                }
            }
            else
            {
                fullObjectName = rootObjName;
            }
            return fullObjectName;
        }
        public PropertySegExpress ToExpress()
        {
            if (this.pse == null)
            {
                ParsePropertyVisitor ppv = new ParsePropertyVisitor();
                this.Expr.Accept(ppv, null);
                this.pse = ppv.PropertySegExpress;
            }
            return this.pse;
        }
    }
}
