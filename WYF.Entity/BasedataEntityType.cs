using WYF.DataEntity.Entity;
using WYF.Entity.Plugins;

namespace WYF.Entity
{
    /// <summary>
    /// 基础资料实体类型
    /// </summary>
    public class BasedataEntityType : BillEntityType
    {

        public BasedataEntityType()
        {
            this.BasedataControllersProp = new List<Plugin>();
            this.NameProperty = "name";
            this.NumberProperty = "number";
            this.BillNo = "number";
        }
        [SimpleProperty]
        [DefaultValue("number")]
        public string NumberProperty { get; set; }
        [DefaultValue("name")]
        [SimpleProperty]
        public string NameProperty { get; set; }
        [SimpleProperty]
        public string FlexProperty { get; set; }
        [SimpleProperty]
        public string MoblistFormId { get; set; }
        [SimpleProperty]
        public int MasteridType { get; set; }
        [SimpleProperty]
        public string MasteridPropName { get; set; }

        [SimpleProperty]
        public string CustomControllerProp { get; set; }
        [CollectionProperty(CollectionItemPropertyType = typeof(Plugin))]
        public List<Plugin> BasedataControllersProp { get; set; } = new List<Plugin>();
    }
}