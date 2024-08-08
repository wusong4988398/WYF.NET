using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    /// <summary>
    /// 多选下拉框
    /// </summary>
    public class MulComboProp: ComboProp
    {
        public override string ClientType => "mulcombo";

        public override Dictionary<string, object> CreateEntityTreeNode(EntityTreeNode entityTreeNode)
        {
            Dictionary<string, object> col = base.CreateEntityTreeNode(entityTreeNode);
            col["Type"] = "MulComboListColumnAp";
            return col;
        }
    }
}
