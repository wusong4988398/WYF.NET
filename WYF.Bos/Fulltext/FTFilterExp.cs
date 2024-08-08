using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.fulltext
{
    public class FTFilterExp
    {
        public string Exp { get; private set; }

        public FTValue[] Values { get; private set; }

        public FTFilterExp(string exp, params FTValue[] values)
        {
            Exp = exp;
            Values = values;
        }



        public override string ToString()
        {
            string param = "";
            if (Values != null)
                param = Values.ToList().ToString();
            return Exp + ':' + param;
        }
    }
}
