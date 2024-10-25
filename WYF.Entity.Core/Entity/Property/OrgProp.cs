using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;

namespace WYF.Entity.Property
{
    /// <summary>
    /// 组织字段
    /// </summary>
    public class OrgProp : BasedataProp
    {
        public override string BaseEntityId { get => "bos_org"; set => base.BaseEntityId = value; }
        [SimpleProperty]
        public string OrgFunc { get; set; }
        [SimpleProperty]
        public int F7Style {  get; set; }



    }
}
