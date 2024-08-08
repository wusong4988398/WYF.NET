using WYF.Bos.Entity.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.property
{
    public class IntegerProp: DecimalProp
    {
        public override int DbType { get =>4; set => base.DbType = value; }
        public override Type PropertyType => typeof(int);

        public override void SetFieldValue(IDataModel model, object dataEntity, object value)
        {
            SetValueFast(dataEntity, value);
        }

    }
}
