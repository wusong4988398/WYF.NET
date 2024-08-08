using WYF.Bos.Form.control.events;
using WYF.Bos.Form.Events;
using WYF.Bos.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.plugin
{
    public class FormViewPluginProxy
    {
        protected List<IFormPlugin> _plugIns;

        public List<IFormPlugin> PlugIns
        {
            get { return _plugIns; }
        }
        public FormViewPluginProxy()
        {
            this._plugIns = new List<IFormPlugin>();
        }
        public void FireOnGetControl(OnGetControlArgs e)
        {
            foreach (IFormPlugin plugin in _plugIns)
            {
                plugin.OnGetControl(e);
            }
         
        }


        public void FireInitialize()
        {
            foreach (IFormPlugin plugin in _plugIns)
            {
                plugin.Initialize();
            }
       

        }


        public void SetView(IFormView formView)
        {
            List<IFormPlugin> plugins = new List<IFormPlugin>(this.PlugIns);
            foreach (var plugin in plugins)
            {
                plugin.SetView(formView);
            }
          
        }

        public  void RegisterPlugins(List<IFormPlugin> list)
        {
            this._plugIns.AddRange(list);
        }

        public void FireRegisterListener(EventObject e)
        {

            foreach (IFormPlugin plugin in _plugIns)
            {
                plugin.RegisterListener(e);
            }
        }
    }
}
