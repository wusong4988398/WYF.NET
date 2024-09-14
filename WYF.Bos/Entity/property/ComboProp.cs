using WYF.DataEntity.Entity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    /// <summary>
    /// 下拉列表字段
    /// </summary>
    public class ComboProp: FieldProp
    {
        public ComboProp()
        {
            this.FilterControlType = "enum";
            this.CompareGroupID = "10,11";
            this.DefaultCompareTypeId = "105";
            this.DefaultMultiCompareTypeId = "17";
        }

        /// <summary>
        /// 显示风格 0（文字）、1（图标）、2（文字+图片）
        /// </summary>
        [SimpleProperty]
        public int ShowStyle { get; set; }
        /// <summary>
        /// 是否允许手工录入
        /// </summary>
        [SimpleProperty(Name = "Editable")]
        public bool IsEditable { get; set; }
        /// <summary>
        /// 前端的控件类型
        /// </summary>
        public override string ClientType => "combo";
        /// <summary>
        /// 数据库字段类型
        /// </summary>
        public override int DbType { get => 12;  }
        /// <summary>
        /// 字段值类型
        /// </summary>
        public override Type PropertyType => typeof(string);
        /// <summary>
        /// 下拉项集合
        /// </summary>
        [CollectionProperty(Name = "ComboItems",CollectionItemPropertyType =typeof(ValueMapItem))]
        public List<ValueMapItem> ComboItems { get; set; }=new List<ValueMapItem>();
        /// <summary>
        /// 是否存在预定义的枚举项？
        /// </summary>
        public bool IsEmptyItems =>this.ComboItems.Count == 0;
        /// <summary>
        /// 判断传入的枚举项是否预先定义的
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsExistItem(string name)
        {
            foreach (ValueMapItem item in this.ComboItems)
            {
                if (item.Value.Trim() == name.Trim()) return true;
                
            }
            return false;
        }
    }
}
