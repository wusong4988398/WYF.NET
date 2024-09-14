using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;

namespace WYF.Entity
{
    public class ValueMapItem
    {
        public ValueMapItem()
        {

        }
        public ValueMapItem(string imageKey, string value, string name)
        {
            this.ImageKey = imageKey;
            this.Value = value;
            this.Name = name;
        }
        [SimpleProperty]
        public string ImageKey { get; set; }
        [SimpleProperty]
        public string Value { get; set; }
        [SimpleProperty]
        public string Name { get; set; }

        public Dictionary<string, object> CreateComboItem()
        {
            Dictionary<string, object> comboItem = new Dictionary<string, object>();
            comboItem["caption"] = this.Name;
            comboItem["value"] = this.Value;
            comboItem["imageKey"] = this.ImageKey;
            return comboItem;
        }

    }
}
