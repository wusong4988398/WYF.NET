using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree;
using WYF.OrmEngine.Query.Multi;
using WYF.OrmEngine.Query.OQL.G.Visitor;

namespace WYF.OrmEngine.Query.OQL.G.expr
{
    public abstract class OQLExpr : OQLable
    {
        private PropertySegExpress pse;
        protected Expr expr;

        public OQLExpr(Expr expr)
        {
            this.expr = expr;
        }

        public Expr GetExpr()
        {
            return this.expr;
        }

        public override string ToString()
        {
            return this.expr.ToString();
        }

        public PropertySegExpress ToExpress()
        {
            if (this.pse == null)
            {
                ParsePropertyVisitor ppv = new ParsePropertyVisitor();
                this.expr.Accept(ppv, null);
                this.pse = ppv.PropertySegExpress;
            }
            return this.pse;
        }

        public string GetPropertyObjName(List<string> fullPropertyNames, string rootObjName)
        {
            string fullObjectName = null;
            if (fullPropertyNames.Count > 0)
            {
                int len = 0;
                foreach (var fullPropertyName in fullPropertyNames)
                {
                    int dot = fullPropertyName.LastIndexOf('.');
                    if (dot != -1)
                    {
                        string sameParentObj = fullPropertyName.Substring(0, dot);
                        if (fullObjectName == null || len > sameParentObj.Length)
                        {
                            fullObjectName = sameParentObj;
                            len = fullObjectName.Length;
                        }
                    }
                }
                if (fullObjectName == null)
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
    }
}
