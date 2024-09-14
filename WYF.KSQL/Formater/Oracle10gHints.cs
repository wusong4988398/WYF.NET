using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Formater
{
    public class Oracle10gHints : OracleHints
    {
        // Fields
        private static Oracle10gHints instance = new Oracle10gHints();

        // Methods
        private Oracle10gHints()
        {
            base.NewHint("NO_PARALLEL", 0, -1);
        }

        public static Oracle10gHints GetInstance()
        {
            return instance;
        }
    }






}
