using Antlr4.Runtime.Misc;
using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity
{
    /// <summary>
    /// 单据实体类型
    /// </summary>
    public class BillEntityType: MainEntityType
    {
        public BillEntityType()
        {
            this.BillNo = "billno";
        }
        [SimpleProperty]
        [DefaultValue("billno")]
        public string BillNo { get; set; }

        [SimpleProperty]
        public string BillStatus { get; set; }
        [SimpleProperty]
        public string BillType {  get; set; }
        [SimpleProperty]
        public string BillTypePara {  get; set; }
        [SimpleProperty]
        public string ForbidStatus {  get; set; }

        [SimpleProperty]
        public string BillParameter {  get; set; }  

        public static readonly string PKPropName = "id";
        [SimpleProperty]
        public string MobFormId {  get; set; }
        [SimpleProperty]
        public string EntityTypeId {  get; set; }


    }
}
