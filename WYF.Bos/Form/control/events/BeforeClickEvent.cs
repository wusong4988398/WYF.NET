using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.control.events
{
    public class BeforeClickEvent : ClickEvent
    {
        public bool IsCancel { get; set; }=false;

        public BeforeClickEvent(object source) : base(source)
        {
        }
        public BeforeClickEvent(Object source, Dictionary<String, Object> paramsDic) : base(source, paramsDic)
        {
        }
        

    }
}
