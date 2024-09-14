using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.Entity.DataModel;
using WYF.Bos.Entity.validate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    /// <summary>
    /// 字段基类 表单运行时元数据-实体的下级属性对象
    /// </summary>
    public class FieldProp : DynamicSimpleProperty, IFieldHandle, IValidatorHanlder, ICompareTypeConfig
    {
        /// <summary>
        /// 比较符组标识
        /// </summary>
        public string CompareGroupID { get; set; } = "0,1,2";
        /// <summary>
        /// 列表常用过滤条件选择了一个值时使用的比较符
        /// </summary>
        public string DefaultCompareTypeId { get; set; } = "67";

        /// <summary>
        /// 列表快速搜索过滤条件或常用过滤条件选择了多个值时使用的比较符
        /// </summary>
        public string DefaultMultiCompareTypeId { get; set; } = "17";
        /// <summary>
        /// 过滤控件类型
        /// </summary>
        public string FilterControlType { get; set; } = "";
        /// <summary>
        /// 获取是否必录字段
        /// </summary>
        [SimpleProperty(Name = "MustInput")]
        public bool IsMustInput { get; set; }

        /// <summary>
        /// 是否系统属性，如内码、基础资料引用Id，多语言对应内部字段
        /// </summary>
        [SimpleProperty(Name = "Sys")]
        public bool IsSysField { get; set; }
        /// <summary>
        /// 字段默认值
        /// </summary>
        [SimpleProperty]
        public Object? DefValue { get; set; }
        /// <summary>
        /// 获取字段的功能控制选项值
        /// </summary>
        [SimpleProperty]
        public int Features { get; set; }
        /// <summary>
        /// 0是否显示
        /// </summary>
        [SimpleProperty(Name = "ZeroShow")]
        public bool IsZeroShow { get; set; }
        /// <summary>
        /// 获取前端控件类型
        /// 基础资料 ： basedata 多选基础资料：mulbasedata 组织：org 用户：user 下拉列表：combo
        /// 多选下拉列表：mulcombo 复选框：checkbox 文本：text 多行文本：textarea 大文本：largeText 日期：date
        /// 日期范围：daterange 长日期：datetime 数字：number 弹性域字段：flexfield 城市：city
        /// 多语言文本：localeText 图片：picture 密码：passwordbox 操作列：operate 合并列：combinedField
        ///其他：other(不给的时候)
        /// </summary>
        public virtual string ClientType 
        { 
            get
            {
                int length = this.GetType().Name.Length;
                if (this.GetType().Name.EndsWith("Prop"))
                {
                    return this.GetType().Name.Substring(0, length - 4).ToLower();
                }
                return this.GetType().Name.ToLower();
            } 
        }
        /// <summary>
        /// 创建字段列表格式化类
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        //protected ColumnDesc CreateColumnDesc(ListField col)
        //{
        //    return new ColumnDesc(col.getKey(), (IDataEntityProperty)this, col.getFieldProp());
        //}

        /// <summary>
        /// 获取基础资料在界面上展示的值
        /// </summary>
        /// <param name="baseObj">基础资料的动态对象</param>
        /// <returns>基础资料在界面上展示的值</returns>
        public Object GetBasePropDisplayValue(Object baseObj)
        {
            Object value = GetValueFast(baseObj);
            return value;
        }
        /// <summary>
        /// 获取列表字段元素，用于设计器根据字段动态列表列创建
        /// </summary>
        /// <param name="entityTreeNode"></param>
        /// <returns></returns>
        public virtual Dictionary<string, object> CreateEntityTreeNode(EntityTreeNode entityTreeNode)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            return dic;
        }

        /// <summary>
        /// 获取字段默认值，将默认值填充到字段中
        /// 如果没有注入单据类型的默认值，取DefaultValue2得到的默认值
        /// 否则（有单据类型）取DefaultValue1（强制DefaultValue2有默认值也返回NULL
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataEntity"></param>
        /// <param name="rowIndex"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void ApplyDefaultValue(IDataModel model, DynamicObject dataEntity, int rowIndex)
        {
            Object value = model.GetContextVariable(this.Name);
            if (value == null) {

                value = IFieldHandle.GetFieldDefaultValue2(model, dataEntity, new DefaultValueCalculator(), (DynamicProperty)this);

            }

            if (value!=null)
            {
                SetFieldValue(model, dataEntity, value);
            }
        }


       

        /// <summary>
        /// 设置字段值
        /// </summary>
        /// <param name="model">数据模型</param>
        /// <param name="dataEntity">需改动字段值的数据包</param>
        /// <param name="Value">字段值</param>
        public virtual void SetFieldValue(IDataModel model, object dataEntity, object value)
        {
            SetValueFast(dataEntity, value);
        }
    }
}
