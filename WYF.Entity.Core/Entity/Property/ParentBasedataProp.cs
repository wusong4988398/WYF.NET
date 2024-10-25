using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;

namespace WYF.Entity.Property
{
    public class ParentBasedataProp: GroupProp
    {

        public string GroupTableName =>this.Parent.Alias;

        [SimpleProperty(Name = "ShowTreeNow")]
        [DefaultValue("true")]
        public bool IsShowTreeNow {  get; set; }=true;
        [SimpleProperty(Name = "LongNumberDLM")]
        [DefaultValue(".")]
        public string LongNumberDLM { get; set; } = ".";
    }
}
