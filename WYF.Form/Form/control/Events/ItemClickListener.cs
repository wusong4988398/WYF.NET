using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Form.control.Events
{
    public interface ItemClickListener
    {
        void BeforeItemClick(BeforeItemClickEvent evt);

        void ItemClick(ItemClickEvent evt);
    }
}
