using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree
{
    public interface IAstStatementVisitor<R, C>
    {
        R VisitNode(Node node, C context);
        R VisitRelation(Relation relation, C context);
        R VisitQueryBody(QueryBody queryBody, C context);
        R VisitSelect(Select select, C context);

    }
}
