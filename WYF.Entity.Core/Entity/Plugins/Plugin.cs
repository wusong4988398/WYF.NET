using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;

namespace WYF.Entity.Plugins
{
    public class Plugin
    {
        /// <summary>
        /// 类名
        /// </summary>
        [SimpleProperty]
        public string ClassName { get; set; }


        [SimpleProperty]
        public string Description { get; set; }

        [SimpleProperty]
        public int RowKey { get; set; }

        [SimpleProperty]
        public int Type { get; set; } = 0;

        [SimpleProperty(Name = "Enabled")]
        [DefaultValue("true")]
        public bool IsEnabled { get; set; }


        [DefaultValue("false")]
        public bool IsDynamicPlugin { get; set; }
    }
}
