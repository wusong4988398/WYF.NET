using WYF.DataEntity.Entity;

namespace WYF.Entity
{
    /// <summary>
    /// 单据实体类型
    /// </summary>
    public class BillEntityType : MainEntityType
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
        public string BillType { get; set; }
        [SimpleProperty]
        public string BillTypePara { get; set; }
        [SimpleProperty]
        public string ForbidStatus { get; set; }

        [SimpleProperty]
        public string BillParameter { get; set; }

        public static readonly string PKPropName = "id";
        [SimpleProperty]
        public string MobFormId { get; set; }
        [SimpleProperty]
        public string EntityTypeId { get; set; }


    }
}