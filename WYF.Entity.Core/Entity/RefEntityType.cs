using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.Dynamicobject;

namespace WYF.Entity
{
    /// <summary>
    /// 引用实体
    /// </summary>
    public class RefEntityType : BasedataEntityType
    {
        public string RefBaseEntityIds { get; set; }
        public override void EndInit()
        {
            Console.WriteLine("");

        }
        public void FillRefEntityTypes(Dictionary<string, DynamicObjectType> types)
        {
            FillRefType(this, types);
            this.RefPropTypes = null;
        }
    }
}
