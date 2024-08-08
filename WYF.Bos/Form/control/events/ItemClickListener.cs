using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.control.events
{
    public interface ItemClickListener
    {
        void BeforeItemClick(BeforeItemClickEvent evt);

        void ItemClick(ItemClickEvent evt);
    }
}
