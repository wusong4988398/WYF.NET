using WYF.Bos.DataEntity.Entity;
using WYF.Bos.Form.control;
using WYF.Bos.Form.control.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.container
{
    /// <summary>
    /// 容器控件基类；表单也是从此控件中派生的。
    /// 实现了ISuportClick接口，标记控件支持点击处理，并绑定实现了接口ClickListener的插件，触发插件事件；
    /// </summary>
    public class Container : TipsSupport, ISuportClick
    {

        List<Control> items = new List<Control>();



        protected List<ClickListener> clickListeners = new List<ClickListener>();

        [CollectionProperty(CollectionItemPropertyType =typeof(Control))]
        public List<Control> Items { get { return items; } }
        /// <summary>
        /// 操作代码
        /// </summary>
        [SimpleProperty]
        public string OperationKey { get; set; } = "";
        /// <summary>
        /// 控制容器内折叠摘要字段
        /// </summary>
        [SimpleProperty]
        public List<string> CollapseFields { get; set; }=new List<string>() { };



        public void AddClickListener(ClickListener listener)
        {
            this.clickListeners.Add(listener);
        }

        public void AddItemClickListener(ItemClickListener listener)
        {
           
        }

        public void BindData(BindingContext bctx)
        {
            foreach (Control ctl in this.items)
            {

                ctl.BindData(bctx);
            }
        }

        public void Click()
        {
            throw new NotImplementedException();
        }
    }
}
