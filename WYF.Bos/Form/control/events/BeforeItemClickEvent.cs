using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.control.events
{
    public class BeforeItemClickEvent : ItemClickEvent
    {
        public bool IsCancel { get; set; }

        public BeforeItemClickEvent(object source, Dictionary<string, object> paramsMap) : base(source, paramsMap)
        {
        }

        public BeforeItemClickEvent(object source, string itemKey, string operationKey) : base(source, itemKey, operationKey)
        {
        }
    }
}
