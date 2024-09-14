using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using WYF.OrmEngine.Impl;
using WYF.OrmEngine.Query.Multi;

namespace WYF.OrmEngine.Query.Optimize
{
    internal class QueryTreeNode
    {
        public SingleQuery SingleQuery {  get; private set; }
        public List<QueryTreeNode> Children {  get; set; }=new List<QueryTreeNode>();
        public QueryTreeNode Parent {  get; set; }


        private QueryTreeNode(SingleQuery query)
        {
            SetQuery(query);
        }
        void SetQuery(SingleQuery query)
        {
            this.SingleQuery = query;
            //是否可以直接对基础数据的成本进行某种操作而不必经过过滤
        }

        public static QueryTreeNode Create(SingleQuery[] queries)
        {
            QueryTreeNode root = new QueryTreeNode(queries[0]);
            for (int i = 1, n = queries.Length; i < n; i++)
                root.AddNode(new QueryTreeNode(queries[i]));
            return root;
        }

        private void AddNode(QueryTreeNode node)
        {

            QContext allCtx = node.SingleQuery.AllCtx;
            if (!AddNode(node, this, allCtx))
                throw new Exception($"当前节点{node.SingleQuery.FullObjName},找不到父节点");
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
            foreach (var sub in root.Children)
            {
                if (AddNode(node, sub, allCtx))
                    return true;
            }
            return false;
        }
    }
}
