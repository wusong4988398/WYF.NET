using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Form.control.Events
{
    public interface ClickListener
    {
        void BeforeClick(BeforeClickEvent evt);
        void Click(EventObject evt);
    }
}
