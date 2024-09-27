using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Entity.Format;
using WYF.Entity.List.Column;

namespace WYF.Entity.Property
{
    public class DateProp : DateTimeProp
    {
        private static readonly long serialVersionUID = 1593941437706854539L;

        public DateProp()
        {
            RegionType = (int)FormatTypes.Date;
        }

 
        public override int DbType => 91;

        //protected override ColumnDesc CreateColumnDesc(ListField col)
        //{
        //    return (ColumnDesc)new DateColumnDesc(col.Key, this, col.FieldProp);
        //}

        public override string ClientType => "date";

        public string DateFormatString => "yyyy-MM-dd";

        public override  string FilterControlType => "date";

        public DateTimeFormatInfo DateFormat
        {
            get
            {
                var format = new DateTimeFormatInfo();
                format.ShortDatePattern = DateFormatString;
                return format;
            }
        }
    }
}
