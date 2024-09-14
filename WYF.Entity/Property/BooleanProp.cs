using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Entity.Property
{
    /// <summary>
    /// 复选框字段
    /// </summary>
    public class BooleanProp : FieldProp
    {
        /// <summary>
        /// 数据库字段类型
        /// </summary>
        public override int DbType { get { return 1; } }

        /// <summary>
        /// 字段值类型
        /// </summary>
        public override Type PropertyType { get { return typeof(bool); } }

        public BooleanProp()
        {
            this._defaultValue = false;
        }
        public void SetDefValue(Object defValue)
        {
            base.DefValue = defValue;
            this.DefaultValue = defValue;

        }
        /// <summary>
        /// 前端控件类型
        /// </summary>
        public override string ClientType { get { return "checkbox"; } }
        /// <summary>
        /// 获取基础资料在界面上展示的值
        /// </summary>
        /// <param name="basedataObj"></param>
        /// <returns></returns>
        public new Object GetBasePropDisplayValue(Object basedataObj)
        {
            bool boolValue = (bool)GetValueFast(basedataObj);
            return boolValue ? "是" : "否";
        }

        /// <summary>
        /// 创建列表字段元素，用于设计器根据字段动态列表列创建
        /// </summary>
        /// <param name="parentEntityTreeNode"></param>
        /// <returns></returns>
        public override Dictionary<String, Object> CreateEntityTreeNode(EntityTreeNode parentEntityTreeNode)
        {
            Dictionary<String, Object> col = base.CreateEntityTreeNode(parentEntityTreeNode);
            //col.put("Type", "CheckBoxListColumnAp");
            //col.put("CommonFilterApType", "CommonCheckBoxFilterColumnAp");
            //col.put("IsMulti", Boolean.valueOf(true));
            //col.put("Custom", Boolean.valueOf(false));
            return col;
        }

    }
}
