
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Form.control;

namespace WYF.Form.Events
{
    public class OnGetControlArgs : EventObject
    {

        public Control Control { get; set; }

        public String Key { get; private set; }

        public OnGetControlArgs(object source, string key) : base(source)
        {
            this.Key = key;
        }

    }
}
