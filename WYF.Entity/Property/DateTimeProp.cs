using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.Entity.DataModel;

namespace WYF.Entity.Property
{
    public class DateTimeProp : FieldProp
    {
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }
        [SimpleProperty(Name = "UseRegion")]
        [DefaultValue("true")]
        public bool IsUseRegion { get; set; } = true;
        [SimpleProperty]
        [DefaultValue("2")]
        public int RegionType { get; set; } = 2;
        [SimpleProperty]
        [DefaultValue("0")]
        public int TimeZoneTransType { get; set; } = 0;
        [SimpleProperty]
        public string RelateOrg { get; set; }

        public override void SetFieldValue(IDataModel model, object dataEntity, object value)
        {

            if (value is DateTime)
            {
                base.SetFieldValue(model, dataEntity, value);

            }
            else if (value.IsEmpty())
            {
                base.SetFieldValue(model, dataEntity, null);

            }
            else
            {
                base.SetFieldValue(model, dataEntity, value.ToDate());

            }
        }
    }
}
