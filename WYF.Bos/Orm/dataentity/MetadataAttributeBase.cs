
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.dataentity
{
    public abstract class MetadataAttributeBase : Attribute
    {

        protected MetadataAttributeBase()
        {
        }


        [DataMember(Name = "Alias", EmitDefaultValue = false)]
        public virtual string Alias { get; set; }

        [DataMember(Name = "Description", EmitDefaultValue = false)]
        public virtual string Description { get; set; }

        [DataMember(Name = "DisplayName", EmitDefaultValue = false)]
        public virtual string DisplayName { get; set; }
    }
}
