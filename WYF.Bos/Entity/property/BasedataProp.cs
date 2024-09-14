using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.dynamicobject;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Bos.Entity.DataModel;
using WYF.Bos.Entity.validate;
using WYF.Bos.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    /// <summary>
    /// 基础资料字段
    /// </summary>
    public class BasedataProp : DynamicComplexProperty, IValidatorHanlder, IBasedataField, ICompareTypeConfig
    {
        /// <summary>
        /// 关联的基础资料内码实体属性
        /// 基础资料字段，在实体上会注册两个属性对象：
        /// 1. 基础资料属性对象：复合型，存储基础资料数据包
        /// 2. 基础资料内码属性对象：长整型，存储基础资料数据内码
        /// </summary>
        public IDataEntityProperty RefIdProp { get; set; }
        

        public int DbType 
        { 
            get {

                return ((DynamicSimpleProperty)this.RefIdProp).DbType;
            } 
        }
        /// <summary>
        /// 字段默认值配置，默认返回为null，即未配置默认值
        /// </summary>
        [SimpleProperty]
        public object? DefValue { get; set; }
        /// <summary>
        /// 绑定基础资料标识
        /// </summary>
        [SimpleProperty]
        public virtual  string BaseEntityId { get; set; }
        /// <summary>
        /// 是否必录
        /// </summary>
        [SimpleProperty(Name = "MustInput")]
        public bool IsMustInput { get; set; }
        /// <summary>
        /// 基础资料的功能控制选项值
        /// </summary>
        [SimpleProperty]
        public int Features { get; set; }

        /// <summary>
        /// 比较符组标识
        /// </summary>
        public string CompareGroupID { get; set; } = "0,1,2";
        /// <summary>
        /// 列表常用过滤条件选择了一个值时使用的比较符
        /// </summary>
        public string DefaultCompareTypeId { get; set; } = "67";

        /// <summary>
        /// 过滤控件类型
        /// </summary>
        public string FilterControlType { get; set; } = "basedata";
        /// <summary>
        /// 基础资料显示属性
        /// </summary>
        [SimpleProperty]
        [DefaultValue("name")]
        public string DisplayProp { get; set; } = "name";
        /// <summary>
        /// 基础资料实体类型
        /// </summary>
        public override Type PropertyType
        {
            get
            {
              Type type=  ((DynamicObjectType)base.ComplexType).ClrType;
              return type;
            }
        }
        /// <summary>
        /// 基础资料编辑显示属性
        /// </summary>
        [SimpleProperty]
        [DefaultValue("number")]
        public string EditSearchProp { get; set; } = "number";
        /// <summary>
        /// 基础资料对应使用组织的属性名(即字段标识)
        /// </summary>
        [SimpleProperty]
        public string OrgProp { get; set; } = "";
        /// <summary>
        /// 是否只显示启用
        /// </summary>
        [SimpleProperty(Name = "ShowUsed")]
        [DefaultValue("true")]
        public bool IsShowUsed { get; set; }
        /// <summary>
        /// 基础资料数据删除操作时，不检查本单有没有引用被删的数据(暂时没有在设计器放开属性配置，可以直接修改XML设置值)
        /// </summary>
        [SimpleProperty]
        public bool IgnoreRefCheck { get; set; }
        /// <summary>
        /// 是否集团控制
        /// </summary>
        [SimpleProperty(Name = "GroupControl")]
        public bool IsGroupControl {  get; set; }
        /// <summary>
        /// 是否关联物理表
        /// </summary>
        [SimpleProperty(Name = "DbIgnore")]
        [DefaultValue("true")]
        public new bool IsDbIgnore { get; set; }
        /// <summary>
        /// 是否是系统字段
        /// </summary>
        public bool IsSysField { get; set; } = false;
        /// <summary>
        /// 计算基础资料字段默认值
        /// 基于默认值配置(可内置函数)以及环境上下文，计算出基础资料字段的真实默认值
        /// </summary>
        /// <param name="model">数据模型</param>
        /// <param name="dataEntity">需改动字段值的数据包</param>
        /// <param name="rowIndex">行索引</param>
        public virtual void ApplyDefaultValue(IDataModel model, DynamicObject dataEntity, int rowIndex)
        {
            Object value = model.GetContextVariable(this.Name);
            if (value == null)
            {
                IDefValueProvider defValueProvider = model.GetService<IDefValueProvider>();
                if (defValueProvider == null)
                {
                    value = IFieldHandle.GetFieldDefaultValue2(model, dataEntity, new DefaultValueCalculator(), (DynamicProperty)this);

                }
                else
                {
                    object defValue = defValueProvider.GetDefValue(this);
                    //FieldDefValue defValue2 = defValueProvider.getDefValue2(this);
                }

            }

            if (value != null) SetFieldValue(model, dataEntity, value);



           
        }

        public void SetFieldValue(IDataModel model, object dataEntity, object value)
        {
            Object pkValue = null;
            if (value == null || (value is string && ((String)value).IsEmpty())){
                SetValueFast(dataEntity, null);
            }
            else {
                //DynamicObjectType dt = (DynamicObjectType)GetComplexType(dataEntity);
                DynamicObject dynamicObject = null;
            }
        }
    }
}
