using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.SqlParser
{
    public class DefaultTraversalVisitor<R, C> : IAstVisitor<R, C>
    {
        
        public override R VisitNode(Node node, C context)
        {
            
            foreach (Node child in node.GetChildren())
            {
                Process(child, context);
            }
            return default;
        }
    }
}
