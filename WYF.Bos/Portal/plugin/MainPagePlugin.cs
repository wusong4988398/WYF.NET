using WYF.Bos.Form;
using WYF.Bos.Form.control;
using WYF.Bos.Form.control.events;
using WYF.Bos.Form.plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace WYF.Bos.Portal.plugin
{
    public class MainPagePlugin: AbstractFormPlugin
    {

        public override void RegisterListener(EventObject e)
        {
            Image icon = GetControl<Image>("usericon");
            if (icon != null) icon.AddClickListener(this);

            base.RegisterListener(e);
        }

        public override   void AfterCreateNewData(EventObject e)
        {
            ShowBizAppPage();

        }

        public override void Initialize()
        {
            
        }

        private void ShowBizAppPage()
        {
            FormShowParameter fsp = new FormShowParameter();
            fsp.FormId = "tenant_myapp";
            fsp.CustomParams["_noloadsetting_"] = true;
            fsp.OpenStyle.TargetKey = "flexapp";
            fsp.OpenStyle.ShowType = ShowType.InContainer;
            this.FormView.ShowForm(fsp);
            this.PageCache.Add("appbetaopened", "true");
   
        }

 
        public void Click(EventObject evt)
        {
            Control control = (Control)evt.GetSource();
            string key = control.Key;
            IClientViewProxy proxy =this.FormView.GetService<IClientViewProxy>();
            JSONObject o = new JSONObject();
            switch (key)
            {
                case "usericon":
                    o.Put("formId", "bos_portal_personalinfo");
                    proxy.AddAction("showSlideBill", o);
                    break;
                default:
                    break;
            }

        }

}
}
