using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Formater
{
    public class Oracle10SQLFormater : Oracle9SQLFormater
    {
        // Methods
        public Oracle10SQLFormater() : base(null)
        {
        }

        public Oracle10SQLFormater(StringBuilder sb) : base(sb)
        {
        }
    }






}
