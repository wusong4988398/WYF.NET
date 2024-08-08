using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.calc
{
    public class ConstantCalc : Calc
    {
        private object _value;
        public object Value { get { return _value; } }

       public ConstantCalc(object value)
        {
            this._value = value;
        }
        public object Execute(IRowFeature row1, IRowFeature row2)
        {
            return this._value;
        }
    }
}
