using WYF.Bos.Form.container;
using WYF.Bos.Form.control.events;
using WYF.DataEntity.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.List
{
    public class BizCustomList: Container
    {
        protected List<ItemClickListener> customItemClickListeners = new List<ItemClickListener>();
        public void AddCustomListListener(ItemClickListener listener)
        {
            this.customItemClickListeners.Add(listener);
        }



        public void ItemClick(object id, string operationKey)
        {
            BeforeItemClickEvent evt = new BeforeItemClickEvent(this, SerializationUtils.ToJsonString(id), operationKey);
            FireBeforeItemClick(evt);
            if (!evt.IsCancel)
            {
                ItemClickEvent evt1 = new ItemClickEvent(this, SerializationUtils.ToJsonString(id), operationKey);
                FireItemClick(evt1);
            }
        }

        private void FireBeforeItemClick(BeforeItemClickEvent e)
        {
            foreach (var l in this.customItemClickListeners)
            {
                l.BeforeItemClick(e);
            }
        
        }

        private void FireItemClick(ItemClickEvent e)
        {
            foreach (var l in this.customItemClickListeners)
            {
                l.ItemClick(e);
            }
        }

    }
}
