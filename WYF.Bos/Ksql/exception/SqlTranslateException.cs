using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql.exception
{
    [Serializable]
    public class SqlTranslateException:Exception
    {
        public SqlTranslateException()
        {
        }

        public SqlTranslateException(string message):base(message)
        {
            
        }

        public SqlTranslateException(string message,  Exception e): base(message)
        {
            
        }
    }
}
