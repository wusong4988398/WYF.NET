using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Entity.DataModel;

namespace WYF.Entity.Property
{
    /// <summary>
    /// 运行时字段接口
    /// FieldProp实现此接口，实现字段必须的属性
    /// </summary>
    public interface IFieldHandle
    {
        bool IsSysField { get; set; }

        /// <summary>
        /// 获取数据库字段类型
        /// </summary>
        int DbType { get; }

        object? DefValue { get; set; }

        /// <summary>
        /// 设置字段值
        /// </summary>
        /// <param name="model">数据模型</param>
        /// <param name="dataEntity">需改动字段值的数据包</param>
        /// <param name="Value">字段值</param>
        void SetFieldValue(IDataModel model, Object dataEntity, Object Value);
        /// <summary>
        /// 设置该字段默认值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataEntity">数据包</param>
        /// <param name="rowIndex"></param>
        void ApplyDefaultValue(IDataModel model, DynamicObject dataEntity, int rowIndex);




        /// <summary>
        /// 输出字段的默认值，高级版，可以进行函数、公式计算
        /// </summary>
        /// <param name="model">数据模型，如果传null，不能进行含字段变量的公式运算</param>
        /// <param name="dataEntity">字段当前行数据包</param>
        /// <param name="defaultValueCalculator">默认值解析器：据此解析字段的默认值配置； 如果传入null，则在内部自动创建DefaultValueCalculator实例</param>
        /// <param name="fieldProp">字段属性对象</param>
        /// <returns></returns>
        static object GetFieldDefaultValue2(IDataModel model, DynamicObject dataEntity, DefaultValueCalculator defaultValueCalculator, DynamicProperty fieldProp)
        {
            if (fieldProp == null) return null;
            if (!(fieldProp is IFieldHandle))
            {
                return fieldProp.DefaultValue;
            }
            object defvalue = ((IFieldHandle)fieldProp).DefValue;
            DefaultValueCalculator calculator = (defaultValueCalculator != null) ? defaultValueCalculator : new DefaultValueCalculator();
            if (defvalue != null)
            {
                return calculator.GetValue((IDataEntityProperty)fieldProp, defvalue);
            }



            return fieldProp.DefaultValue;
        }
    }
}