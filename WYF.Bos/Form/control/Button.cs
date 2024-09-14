using WYF.DataEntity.Entity;
using WYF.Bos.Form.control.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.control
{
    public class Button : TipsSupport, ISuportClick
    {
        /// <summary>
        /// 操作代码
        /// </summary>
        [SimpleProperty]
        public string OperationKey { get; set; } = "";

        protected List<ClickListener> buttonClickListeners = new List<ClickListener>();

        protected List<ItemClickListener> itemClickListeners = new List<ItemClickListener>();


        public void AddClickListener(ClickListener listener)
        {
            this.buttonClickListeners.Add(listener);
        }

        public void AddItemClickListener(ItemClickListener listener)
        {
            this.itemClickListeners.Add(listener);
        }
    }
}
