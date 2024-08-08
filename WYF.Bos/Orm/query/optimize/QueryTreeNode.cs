using Antlr4.Runtime.Misc;
using WYF.Bos.Orm.impl;
using WYF.Bos.Orm.query.multi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.optimize
{
    public  class QueryTreeNode
    {
        public SingleQuery SingleQuery { get; set; }

        public QueryTreeNode Parent { get; set; }

        public List<QueryTreeNode> Children { get; set; } = new List<QueryTreeNode>();

        private bool canRoleOnCostWithoutFilter;

        private bool pkRoleOn;

        private bool costRoleOn;

        // private PeekingDataSet peekingDataSet;


        public static QueryTreeNode Create(SingleQuery[] queries)
        {
            QueryTreeNode root = new QueryTreeNode(queries[0]);
            for (int i = 1, n = queries.Length; i < n; i++)
                root.AddNode(new QueryTreeNode(queries[i]));
            return root;
        }
        private QueryTreeNode(SingleQuery query)
        {
            SetQuery(query);
        }

        private void AddNode(QueryTreeNode node)
        {
            
            QContext allCtx = node.SingleQuery.AllCtx;
            if (!AddNode(node, this, allCtx))
                throw new Exception($"当前节点:{node.SingleQuery.FullObjName},找不到父节点");


        }

        private bool AddNode(QueryTreeNode node, QueryTreeNode root, QContext allCtx)
        {
            string rootName = root.SingleQuery.FullObjName;
            string name = node.SingleQuery.FullObjName;
            string pname = name;
            bool anotherRoot = false;
            while (true)
            {
                int dot = pname.LastIndexOf('.');
                if (dot != -1)
                {
                    pname = pname.Substring(0, dot);
                }
                else
                {
                    anotherRoot = true;
                }
                EntityItem ei = allCtx.GetEntityItem(pname);
                if (ei == null || ORMConfiguration.IsEntryEntityType(ei.EntityType))
                    continue;
                break;
            }
            if (rootName.Equals(pname) || (anotherRoot && allCtx.GetEntityItem(pname).JoinProperty.ParentEntityItem.FullObjectName.Equals(rootName)))
            {
                root.Children.Add(node);
                node.Parent = root;
                return true;
            }
            foreach (QueryTreeNode sub in root.Children)
            {
                if (AddNode(node, sub, allCtx))
                    return true;
            }
            return false;
        }
        void SetQuery(SingleQuery query)
        {
            this.SingleQuery = query;
            //this.canRoleOnCostWithoutFilter = ORMConfiguration.IsBasedata(query.GetDataEntityType());
        }
    }
}
