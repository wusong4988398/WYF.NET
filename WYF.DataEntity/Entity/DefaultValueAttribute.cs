using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultValueAttribute : Attribute
    {
        public string value { get; set; } = "";
        public DefaultValueAttribute(string value)
        {
            this.value = value;
        }
    }

}
