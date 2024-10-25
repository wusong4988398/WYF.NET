using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Form.control.Events;

namespace WYF.Form.control.events
{
    public interface ISuportClick
    {
        void AddClickListener(ClickListener listener);

        void AddItemClickListener(ItemClickListener listener);
    }
}
