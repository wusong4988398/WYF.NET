using WYF.Bos.Form.control.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.Events
{
    public class CustomEventArgs : EventObject
    {
        public string Key { get; set; }
        public string EventName { get; set; }
        public string EventArgs { get; set; }

        public CustomEventArgs(object source, string key, string eventName, string eventArgs):base(source)
        {
            
            this.Key = key;
            this.EventName = eventName;
            this.EventArgs = eventArgs;
        }


    }
}
