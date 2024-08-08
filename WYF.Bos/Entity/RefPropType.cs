using WYF.Bos.DataEntity.Entity;
using WYF.Bos.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity
{
    /// <summary>
    /// 引用属性
    /// </summary>
    [Serializable]
    public class RefPropType
    {
        /// <summary>
        /// ID
        /// </summary>
        [SimpleProperty]
        public string Id { get; set; }
        /// <summary>
        /// 引用属性
        /// </summary>
        [DefaultValue("")]
        [SimpleProperty]
        public string Props { get; set; }
        /// <summary>
        /// 是否关联到主资料，对于主资料属性，需要支持第三层嵌套
        /// </summary>
        [SimpleProperty(Name = "Master")]
        public bool IsMaster { get; set; }


        public HashSet<string> GetPropSet()
        {
            if (string.IsNullOrEmpty(this.Props))
                return new HashSet<string>();
            string[] arrProps = this.Props.Split(",");
            HashSet<string> setProps = new HashSet<string>();
            foreach (string prop in arrProps)
            {
                setProps.Add(prop);
            }
            return setProps;
        }
    }
}
