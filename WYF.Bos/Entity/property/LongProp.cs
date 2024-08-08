
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    public class LongProp: PKFieldProp
    {
        public LongProp()
        {
            this.DefaultValue =0L;
            this.CompareGroupID = "0,1,4";
            this.DefaultCompareTypeId = "67";
            this.DefaultMultiCompareTypeId = "17";
            this.FilterControlType = "text";
        }

        public LongProp(bool isrefId):base(isrefId) 
        {
            this.DefaultValue = 0L;
        }

        public  override Type PropertyType { get { return typeof(long); } }

        public override int DbType { get { return -5; } }

    }
}
