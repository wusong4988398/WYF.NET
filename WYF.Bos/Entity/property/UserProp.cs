using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    /// <summary>
    /// 用户字段
    /// </summary>
    public class UserProp:BasedataProp
    {

        public UserProp()
        {
            this.CompareGroupID = "0,1,2,6";
            this.BaseEntityId = "bos_user";
            this.FilterControlType = "user";
        }
        [SimpleProperty]
        [DefaultValue("bos_user")]
        public override  string BaseEntityId => base.BaseEntityId;
        /// <summary>
        /// 用户字段的查询风格  0（缺省）、1（树形+列表）、2（列表）、3（树形（受控）+列表）
        /// </summary>
        [SimpleProperty]
        public int F7Style { get; set; }

        
    }
}
