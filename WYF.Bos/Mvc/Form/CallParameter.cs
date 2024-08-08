using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Mvc.Form
{
    [Serializable]
    public class CallParameter
    {
        private String Key { get; set; }

        private String Methodname { get; set; }

        private Object[] Args { get; set; } 
    }
}
