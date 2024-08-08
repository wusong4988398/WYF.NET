using WYF.Bos.Form;
using WYF.Bos.Form.control.events;
using WYF.Bos.Form.Events;
using WYF.Bos.Form.plugin;
using WYF.Bos.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace WYF.Bos.Portal.plugin
{
    public abstract class MyAppAbstract : AbstractFormPlugin
    {
        public override void RegisterListener(EventObject e)
        {
            Console.WriteLine("12222");
            
            BizCustomList appStoreList = this.FormView.GetControl<BizCustomList>("bizcustomlistap");
            appStoreList.AddCustomListListener((ItemClickListener)this);
        }

        public override void AfterCreateNewData(EventObject e)
        {
            base.AfterCreateNewData(e);

        }

        public override void CustomEvent(CustomEventArgs e)
        {
            
        }
        /// <summary>
        /// 控件子项点击事件
        /// </summary>
        /// <param name="evt"></param>
        public override void ItemClick(ItemClickEvent evt)
        {
            string key = evt.OperationKey;
            switch (key)
            {
                case "app_store_list":
                    AppStoreList();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void AppStoreList()
        {
            JSONArray apps = GetResultApps(true);
            this.FormView.GetService<IClientViewProxy>().AddAction("APPSTORELIST", apps);
        }

        private JSONArray GetResultApps(bool runtime)
        {
            throw new NotImplementedException();
        }
    }
}
