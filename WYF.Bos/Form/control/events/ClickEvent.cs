using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.control.events
{
    public class ClickEvent : EventObject
    {
        private Dictionary<String, Object> _paramsMap;
        public ClickEvent(object source) : base(source)
        {
            SetSource(source);
        }
        public ClickEvent(Object source, Dictionary<String, Object> paramsDic) : base(source)
        {
        
            this._paramsMap = paramsDic;
            SetSource(source);
        }

        public Dictionary<String, Object> ParamsDic
        {
            get { return _paramsMap; }
            set { _paramsMap = value; }
        }

        public void SetSource(Object source)
        {
            this.source = source;
        }
    }
}
