using WYF.Bos.algo.sql.tree;
using WYF.Bos.Orm.query.multi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.oql.g.visitor
{
    public class ParsePropertyVisitor : ExprVisitor<Object>
    {
        private PropertySegExpress _pse = new PropertySegExpress();

        public PropertySegExpress PropertySegExpress => this._pse;

        public override object DefaultVisit(Expr exp, Object context)
        {
            if (exp.ChildrenCount==0)
            {
                this._pse.AppendString(exp.Sql());
            }
            else
            {
                throw new NotSupportedException($"未处理表达式对象：{exp.GetType().Name}");
            }
            return exp.Sql();
        }
    }
}
