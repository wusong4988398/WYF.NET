using System;

namespace WYF.KSQL.Formater
{
  
    public class Oracle9iHints : OracleHints
    {
        
        public static Oracle9iHints GetInstance()
        {
            return Oracle9iHints.instance;
        }

   
        private Oracle9iHints()
        {
            base.NewHint("NO_PARALLEL", "NOPARALLEL", 1, 0, -1);
        }

       
        private static Oracle9iHints instance = new Oracle9iHints();
    }
}
