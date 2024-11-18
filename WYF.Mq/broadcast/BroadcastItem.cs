using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Common;

namespace WYF.Mq.broadcast
{
    public class BroadcastItem
    {
        public string ClassName { get; set; }
        public string MethodName {  get; set; } 
        public object[] Params { get; set; }
        public string InstanceId =>  Instance.GetInstanceId();
        public string[] ParamsTypes {  get; set; }  
       

    }
}
