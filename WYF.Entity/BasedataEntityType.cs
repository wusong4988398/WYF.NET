using WYF.DataEntity.Entity;
using WYF.Entity.Plugins;

namespace WYF.Entity
{
    /// <summary>
    /// 基础资料实体类型
    /// </summary>
    public class BasedataEntityType : BillEntityType
    {
        [SimpleProperty]
        [DefaultValue("number")]
        public string NumberProperty { get; set; }
        [SimpleProperty]
        public string NameProperty { get; set; }
        [SimpleProperty]
        public string FlexProperty { get; set; }

        public string MoblistFormId { get; set; }

        public int masteridType = 0;

        public string MasteridPropName { get; set; }


        public string CustomControllerProp { get; set; }

        public List<Plugin> BasedataControllersProp { get; set; } = new List<Plugin>();
    }
}