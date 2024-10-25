using WYF.DataEntity.Entity;
using WYF.Entity.DataModel;
using WYF.Entity.Validate;

namespace WYF.Entity.Property
{
    /// <summary>
    /// 小数字段
    /// </summary>
    public class DecimalProp : FieldProp, IScopeCheck
    {
        /// <summary>
        /// 是否用区域设置
        /// </summary>
        public bool useRegion = true;
        /// <summary>
        /// 整体精度,数值的总位数：10位~38位
        /// </summary>
        [SimpleProperty]
        [DefaultValue("23")]
        public int Precision { get; set; } = 23;
        /// <summary>
        /// 小数精度,数值的小数位数：0~20
        /// </summary>
        [SimpleProperty]
        [DefaultValue("10")]
        public int Scale { get; set; } = 10;
        /// <summary>
        /// 数据范围设定
        /// </summary>
        [SimpleProperty]
        public string DataScope { get; set; }
        /// <summary>
        /// 超出数值设定范围提示信息
        /// </summary>
        public string DataScopeMessage { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        [SimpleProperty]
        public decimal Min { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        [SimpleProperty]
        public decimal Max { get; set; }
        /// <summary>
        /// 范围是否包含边界值，如[1,100)包含1；而(1,100)则不包含1
        /// </summary>
        [SimpleProperty(Name = "InclMin")]
        public bool IsInclMin { get; set; }
        /// <summary>
        /// 范围是否包含边界值，如(1,100]包含100；而(1,100)则不包含100
        /// </summary>
        [SimpleProperty(Name = "InclMax")]
        public bool IsInclMax { get; set; }

        /// <summary>
        /// 精度控制字段属性名
        /// </summary>
        [SimpleProperty]
        public string ControlPropName { get; set; }

        public override int DbType { get => 3; set => base.DbType = value; }
        public override Type PropertyType => typeof(decimal);

        public override void SetFieldValue(IDataModel model, object dataEntity, object value)
        {
            base.SetFieldValue(model, dataEntity, value.ToDecimal());
        }

        /// <summary>
        /// 检查值的范围是否合法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckScope(object value)
        {
            throw new NotImplementedException();
        }

        public string GetDataScopeMessage(object value)
        {
            throw new NotImplementedException();
        }
    }
}