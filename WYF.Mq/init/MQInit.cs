using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Mq.broadcast;

namespace WYF.Mq.init
{
    public class MQInit
    {
        public static void Init()
        {
            BroadcastService.Start();
        }
    }
}
