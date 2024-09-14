using WYF.Bos.Entity.DataModel;
using WYF.Bos.Form;
using WYF.Bos.Form.container;
using WYF.Bos.Form.control;
using WYF.Bos.Form.control.events;
using WYF.Bos.Form.Events;
using WYF.Bos.Form.plugin;
using WYF.Bos.Mvc.cache;

using JNPF.Form;
using WYF.DataEntity.Serialization;
using WYF;


namespace WYF.Bos.Mvc.Form
{
    public class FormView : AbstractFormView
    {
        private FormRoot formRoot;

        protected IFormView _parentView;

        private Dictionary<String, IFormView> mapFormViews;
        protected Dictionary<String, Control> _cacheControls = new Dictionary<String, Control>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 表单实体标识
        /// </summary>
        public string EntityId 
        {
            get { return this.FormShowParameter.FormConfig.EntityTypeId; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDestory { get;set; }

        /// <summary>
        /// 页面是否初始化插件
        /// </summary>
        public bool IgnorePlugin { get; set; } = false;


        public String FormId { get { return this.FormShowParameter.FormId; } }


        public IFormView ParentView 
        {
            get
            {
                if (this._parentView==null)
                {
                    this._parentView = GetView(this.FormShowParameter.ParentPageId);
                }

                return this._parentView;
            }
            set { this._parentView = value; }
        }


        public FormViewPluginProxy PluginProxy
        {
            get { return GetService<FormViewPluginProxy>(); }
        }



    public override void Initialize(FormShowParameter showParameter)
        {
            this.FormShowParameter = showParameter;

            this.InitiService();
        }

        protected void InitiService()
        {
            this.AddService(typeof(IFormController), this.CreateFormController());
            this.AddService(typeof(IDataModel), this.CreateDataModel());
            this.AddService(typeof(IClientViewProxy), new ClientViewProxy(GetService<IPageCache>()));
            this.AddService(typeof(IFormDataBinder), this.CreateFormDataBinder());
            this.InitiPluginProxy();
            this.FireInitialize();

        }


        protected void FireInitialize()
        {
            this.PluginProxy.FireInitialize();
       
        }

        private void InitiPluginProxy()
        {
            FormViewPluginProxy pluginProxy = CreatePluginProxy();
            AddService(typeof(FormViewPluginProxy), pluginProxy);


            if (!this.IgnorePlugin)
            {
                pluginProxy.RegisterPlugins(this.FormShowParameter.CreatePlugin());

            }

            pluginProxy.SetView((IFormView)this);

        }

        protected FormDataBinder CreateFormDataBinder()
        {
            return new FormDataBinder((IFormView)this);
        }

        protected FormViewPluginProxy CreatePluginProxy()
        {
            return new FormViewPluginProxy();
        }
        protected IDataModel CreateDataModel()
        {
            return new FormDataModel(this.EntityId, this.PageId, this.services, this.FormShowParameter.AppId, "47150e89000000ac");
        }

        protected FormController CreateFormController()
        {
            return new FormController(this);
        }

        public override List<object> GetActionResult()
        {
            IClientViewProxy service = GetService<IClientViewProxy>();
            if (service != null)
            {
                FormDataModel dataModel = (FormDataModel)GetService<IDataModel>();
                //dataModel.UpdateCache();
                List<Object> actionResult = service.GetActionResult();
                this.PageCache.SaveChanges();
                return actionResult;
            }

            return new List<object>();
        }

        public override T GetControl<T>(string key) 
        {
            Control ctl = this._cacheControls.GetValueOrDefault(key);
            if (ctl != null) return (T)ctl;

            if (this.PluginProxy != null)
            {
                OnGetControlArgs e = new OnGetControlArgs(this, key);
                this.PluginProxy.FireOnGetControl(e);
                ctl = e.Control;
            }
            if (ctl==null)
            {
                GetRootControl();
                return (T)this.cacheControls[key];
            }

            this.cacheControls[key] = ctl;
            return (T)ctl;

        }

     
        public override FormRoot GetRootControl()
        {
            if (this.formRoot == null)
            {
                this.formRoot = FormMetadataCache.GetRootControl(this.FormId);
                this.cacheControls.Clear();
                CreateControlIndex(this.formRoot.Items);
            }


            return formRoot;
        }

        private void CreateControlIndex(List<Control> items)
        {
            foreach (var ctl in items)
            {
                ctl.SetView((IFormView)this);
                this.cacheControls[ctl.Key]=ctl;
                if (ctl is Container)
                {
                    CreateControlIndex(((Container)ctl).Items);
                }
                
            }
        }

        public override void UpdateView()
        {
            GetService<IFormDataBinder>().UpdateView();
        }

        public override void ShowForm(FormShowParameter parameter)
        {
            this.ReturnData = "";
            if (parameter != null)
            {
                if (string.IsNullOrEmpty(parameter.ParentPageId))
                {
                    parameter.ParentPageId = this.PageId;
                }
                if (string.IsNullOrEmpty(parameter.AppId))
                {
                    parameter.AppId = this.FormShowParameter.AppId;
                }
                if (parameter.PageId!= parameter.RootPageId)
                {
                    parameter.RootPageId = this.FormShowParameter.RootPageId;
                }
                if (string.IsNullOrEmpty(parameter.ParentFormId))
                {
                    parameter.ParentFormId = this.FormId;
                }
                if (parameter.OpenStyle.ShowType== ShowType.MainNewTabPage)
                {
                    IFormView mainview = GetMainView();
                    if (mainview == null)
                    {
                        if (this.FormShowParameter.OpenStyle.ShowType == ShowType.Modal)
                        {
                            parameter.OpenStyle.ShowType= ShowType.Modal;
                        }
                        else
                        {
                            parameter.OpenStyle.ShowType = ShowType.NonModal;

                        }
                        SendShowFormAction(parameter);
                    }
                    else
                    {
                        IFormView appView = SessionManager.GetCurrent().GetViewNoPlugin(parameter.AppId + mainview.PageId);
                        ShowType appViewShowType = ShowType.Default;
                        if (appView!=null)
                        {
                            appViewShowType = appView.FormShowParameter.OpenStyle.ShowType;
                            if (parameter.OpenStyle.ShowType != ShowType.Modal)
                            {
                                Control c = appView.GetControl<Control>("_submaintab_");
                                if (c is Tab)
                                {
                                    parameter.OpenStyle.TargetKey = "_submaintab_";
                                    mainview = appView;

                                }else if (c == null)
                                {
                                    IPageCache pageCache = (IPageCache)mainview.GetService<IPageCache>();
                                    string _submaintab_count_ = pageCache.Get("_submaintab_count_");
                                    int count = (_submaintab_count_ == null) ? 3 : _submaintab_count_.ToInt();
                                    if (count >= 8)
                                    {

                                    }
                                    pageCache.Add("_submaintab_count_", ++count + "");
                                }

                                parameter.OpenStyle.ShowType= ShowType.NewTabPage;

                            }

                            parameter.CustomParams["mainPageId"] = mainview.PageId;
                            IFormView view = this.GetViewNoPlugin(parameter.PageId);
                            if (view!=null)
                            {
                                Activate(parameter.PageId);
                                return;
                            }
                            if (parameter.OpenStyle.ShowType == ShowType.Modal)
                            {

                            }
                            else
                            {
                                mainview.ShowForm(parameter);
                                SendFormAction(mainview);
                            }


                        }
                    }

                }
                else
                {
                    SendShowFormAction(parameter);
                }

            }     
        }
        private  void Activate(String pageId)
        {
            this.GetService<IClientViewProxy>().AddAction("activate", pageId);
        }

        public override void Activate()
        {
            throw new NotImplementedException();
        }
        private void SendShowFormAction(FormShowParameter param)
        {
            if (param.IssendToClient)
            {
                this.GetService<IClientViewProxy>().AddAction("showFormByClient", FormShowParameter.ToJsonString(param));
                return;
            }
            Dictionary<string, object> config = FormConfigFactory.CreateConfig(param);

            if (param.OpenStyle.ShowType== ShowType.NewWindow || param.OpenStyle.ShowType== ShowType.IFrame)
            {
                PageCache pageCache = new PageCache(param.PageId);
                pageCache.Add("formconfig", SerializationUtils.ToJsonString(config));
                if (param.OpenStyle.ShowType == ShowType.NewWindow)
                {
                    this.GetService<IClientViewProxy>().AddAction("openWindow", config);
                }
                else
                {
                    this.GetService<IClientViewProxy>().AddAction("showForm", config);

                }
            }
            else
            {
                this.GetService<IClientViewProxy>().AddAction("showForm", config);

            }



        }


        public override IFormView GetMainView()
        {
            var result= SessionManager.GetCurrent().GetMainView(this.FormShowParameter.RootPageId);

            return result;
           
        }

        public override IFormView GetViewNoPlugin(string pageId)
        {
            IFormView view = SessionManager.GetCurrent().GetViewNoPlugin(pageId);
            if (view!=null&&this.PageId==view.FormShowParameter.ParentFormId)
            {
                ((FormView)view).ParentView = (IFormView)this;
            }
            return view;
        }

        public override IFormView GetView(string pageId)
        {
            IFormView view = null;
            if (this.mapFormViews != null)
            {
                view = this.mapFormViews.GetValueOrDefault(pageId,null);
                if (view != null)
                    return view;
            }
            else
            {
                this.mapFormViews = new Dictionary<string, IFormView> ();
            }
            view = SessionManager.GetCurrent().GetView(pageId);
            if (view!=null)
            {
                if (this.PageId== view.FormShowParameter.ParentPageId)
                {
                    ((FormView)view).ParentView= (IFormView)this;
                }
            }
            return view;
        }

     
    }
}
