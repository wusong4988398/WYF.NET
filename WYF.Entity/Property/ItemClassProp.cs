using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;

namespace WYF.Entity.Property
{
    /// <summary>
    /// 多类别基础资料字段
    /// </summary>
    public class ItemClassProp : BasedataProp
    {
        public static readonly string NumberPropName = "number";

        public static readonly string NamePropName = "name";
        /// <summary>
        /// 基础资料类型名称
        /// </summary>
        [SimpleProperty]
        public string TypePropName { get; set; }

        public override Type PropertyType => typeof(DynamicObject);

    }
}