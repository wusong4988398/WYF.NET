
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;
using WYF.Entity.DataModel;
using WYF.Form.control;

namespace WYF.Form
{
    public abstract class AbstractFormView:IFormView
    {
        protected Dictionary<Type, object> services = new Dictionary<Type, object>();
        protected FormShowParameter _formShowParameter;
        protected Dictionary<String, Control> cacheControls = new Dictionary<String, Control>(StringComparer.OrdinalIgnoreCase);
        public string PageId 
        {

            get
            {
                return this.FormShowParameter.PageId;
            } 
        }

        public FormShowParameter FormShowParameter 
        { 
            get { return _formShowParameter; } 
            set { _formShowParameter = value; }
        }

        public IPageCache PageCache
        {
            get
            {
                return GetService<IPageCache>();
            }
        }

        public IDataModel Model
        {
            get 
            { 
                IDataModel dataModel= this.GetService<IDataModel>();
                return dataModel;
            }
        }
        protected Object ReturnData { get; set; }

        /// <summary>
        /// 新增表单内置的服务接口实现类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="service"></param>
        public void AddService(Type type, object service)
        {
            services[type] = service;
        }

    
      
        public T GetService<T>()
        {
            return (T)this.services.GetValueOrDefault(typeof(T));
        }
        public abstract T GetControl<T>(string key) where T : Control;
        public abstract void Initialize(FormShowParameter showParameter);
        public abstract List<object> GetActionResult();
        public abstract void UpdateView();
        public abstract Control GetRootControl();
        public abstract void ShowForm(FormShowParameter showParameter);
        /// <summary>
        /// 获取应用首页
        /// </summary>
        /// <returns></returns>
        public abstract IFormView GetMainView();
        public abstract IFormView GetViewNoPlugin(string pageId);
        public abstract IFormView GetView(string pageId);
        public abstract void Activate();

        public void SendFormAction(IFormView view)
        {
            List<Object> acts = view.GetActionResult();
            SendFormAction(view.PageId, acts);
        }

        protected void SendFormAction(String pageId, List<Object> acts)
        {
            if (acts!=null&&acts.Count>0)
            {
                JSONObject arg=new JSONObject();
                arg.Add("pageId", pageId);
                arg.Add("actions", acts);
                GetClientProxy().AddAction("sendDynamicFormAction", arg);
            }
        }

        public IClientViewProxy GetClientProxy()
        {
            return GetService<IClientViewProxy>();
        }
}
}
