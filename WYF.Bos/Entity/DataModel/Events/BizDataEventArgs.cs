
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel.Events
{
    public class BizDataEventArgs
    {
   

        public object DataEntity { get; set; }
        public bool IsExecuteRule { get; set; }
        public bool FireAfterCreateNewData { get; set; } = true;
    }
}
