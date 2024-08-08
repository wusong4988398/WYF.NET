using WYF.Bos.DataEntity.Entity;
using WYF.Bos.Mvc.Form;
using JNPF.Form;
using JNPF.Form.DataEntity;
using System;
using System.Buffers;
using System.Reflection;
using System.Text;
using WYF;
using OperationStatus = WYF.Bos.Bill.OperationStatus;

namespace WYF.Bos.Form
{
    /// <summary>
    /// 表单显示参数
    /// 使用IFormView#showForm(FormShowParameter)打开动态表单，传入表单显示参数
    /// 必需填写表单标识参数，#setFormId(String)，其他参数可选
    /// </summary>
    public class FormShowParameter
    {
        protected static readonly String FORM_SETTING = "FormSetting";
        private static readonly String NUMBER = "number";
        private static readonly String PHONE = "phone";
        private static readonly String PARAMKEY_CAPTION = "caption";
        private static readonly string CONSTANT_FORMID = "formId";
        private static readonly string CONSTANT_APPID = "appId";
        private static readonly string MAINPAGEID = "mainPageId";
        private static readonly string PATCHVERSION = "patchVersion";
        private static readonly string COSMIC_BIZ = "cosmic_biz";
        private static readonly string COSMIC_BOS = "cosmic_bos";

        private DateTime _cacheExpireTime;
        private bool _isInitialized;
        private bool _issendToClient;
        private static Dictionary<string, Type> ptypes = new Dictionary<string, Type>();

        private string _pageId;

        #region 方法

        /// <summary>
        /// 
        /// </summary>
        static FormShowParameter()
        {
            ptypes.Add("form", typeof(FormShowParameter));
            //ptypes.Add("form", typeof(BillShowParameter));
        }

        public FormShowParameter()
        {
            SetCustomParams(new Dictionary<string, object>());
            SetOpenStyle(new OpenStyle());
            this.ClientParams = new Dictionary<string, object>();
        }

        public bool IssendToClient
        {
            get { return _issendToClient; }
            set 
            {
  //              if ("true".equals(System.getProperty("allowformsendToClient")) || "true"
  //.equals(System.getProperty("audit.enable")))
                    _issendToClient = value; 
            }
        }



    

        public DateTime CacheExpireTime 
        {
            get { return _cacheExpireTime; } 
            set { _cacheExpireTime = value; } 
        }

        public void SetCustomParams(Dictionary<string, object> value)
        {
            if (this._isInitialized)
                throw new Exception("不允许设置自定义参数对象，请使用 setCustomParam方法");
            this.CustomParams = value;
        }

        public static FormShowParameter FromJsonString(String str)
        {
            return (FormShowParameter)ControlTypes.FromJsonStringToObj(str);
        }

        public List<IFormPlugin> CreatePlugin()
        {
            return this.FormConfig.CreatePlugin(this.FormId);
        }

        /// <summary>
        /// 创建界面配置参数(给前端用)
        /// </summary>
        /// <param name="settting"></param>
        /// <returns></returns>
        public Dictionary<String, Object> CreateClientConfig(Dictionary<String, Object> settting)
        {
            if (FormConfig == null) this.GetFormConfigFromMeta();


            Dictionary<String, Object> config = new Dictionary<String, Object>();
            List<IFormPlugin> plugins = this.FormConfig.CreatePlugin(this.FormId);
            //bool notShowForm = this.CouldnotShowForm(plugins, config);
            config.Add("parentPageId", this.ParentPageId);
            config.Add("pageId", this.PageId);
            config.Add("appId", "bos");
            object mainPageId= this.CustomParams.GetValueOrDefault("mainPageId", null);
            if (mainPageId != null) config.Add("mainPageId", mainPageId);
            if (!string.IsNullOrEmpty(this.Caption))
            {
                config.Add("caption", this.Caption);
            }
            else
            {
                config.Add("caption", this.FormConfig.Caption);
            }
            if (this.Status.Equals(OperationStatus.ADDNEW))
            {
                config.Add("status", (int)this.Status);
            }


            if (this.AnimationType != AnimationType.none) config.Add("animation", this.AnimationType);

            if (this.OpenStyle.InlineStyleCss != null && !string.IsNullOrEmpty(this.OpenStyle.InlineStyleCss.Width))
            {
                config.Add("width", this.OpenStyle.InlineStyleCss.Width);
            }
            else if (!string.IsNullOrEmpty(this.FormConfig.Width))
            {
                config.Add("width", this.FormConfig.Width);
            }

            if (this.OpenStyle.InlineStyleCss != null &&  !string.IsNullOrEmpty(this.OpenStyle.InlineStyleCss.Height))
            {
                config.Add("height", this.OpenStyle.InlineStyleCss.Height);
            }
            else if (!string.IsNullOrEmpty(this.FormConfig.Height))
            {
                config.Add("height", this.FormConfig.Height);
            }

            if (!this.FormConfig.IsShowTitle)
            {
                config["isShowTitle"] = this.FormConfig.IsShowTitle;
            }

            if (!this.IsShowTitle)
            {
                config["isShowTitle"] = this.IsShowTitle;
            }
            config.Add("formId", this.FormId);
            config.Add("version", this.FormConfig.Version);
            config.Add("type", "kdform");
            //this.LoadCustomControlMetas(plugins, config);
            config.Add("openStyle", this.OpenStyle.GetConfig());
            if (this.ClientParams.Count > 0)
                config.Add("clientParams", this.ClientParams);
           
            return config;
        }
        private void GetFormConfigFromMeta()
        {
            FormConfig fConfig = FormMetadataCache.GetFormConfig(this.FormId);
            this.FormConfig= fConfig;
        }

        public void SetOpenStyle(OpenStyle value)
        {
            this.OpenStyle = value;
        }
        public void BeginInit()
        {
            this._isInitialized = false;
        }

        public void EndInit()
        {
            this._isInitialized = true;
        }

        #endregion

        public string GetFormName()
        {
            if (!string.IsNullOrEmpty(this.Caption))
                return this.Caption;
            return (this.FormConfig.Caption == null) ? "" : this.FormConfig.Caption;
        }

        public bool IsInitialized { get { return this._isInitialized; } }

        /// <summary>
        /// 父表单标识
        /// </summary>
        [SimpleProperty]
        public string ParentFormId { get; set; }
        /// <summary>
        /// 显示的表单标识，必选参数
        /// </summary>
        [SimpleProperty]
        public string FormId { get; set; }
        /// <summary>
        /// 界面显示风格
        /// </summary>
        [ComplexProperty]
        public OpenStyle OpenStyle { get; set; }
        /// <summary>
        /// 父页面PageId
        /// 显示表单页面时，系统自动会把当前页面作为新显示页面的父页面，不需要调用代码指定
        /// </summary>
        [SimpleProperty]
        public string ParentPageId { get; set; }
        /// <summary>
        /// 新页面的PageId
        /// 显示表单时，默认系统会自动为表单产生一个随机页面PageId返回
        /// </summary>
        [SimpleProperty]
        public string PageId
        {
            get {
                
                if (string.IsNullOrEmpty(this._pageId)) {
                    
                    this._pageId = Guid.NewGuid().ToString().Replace("-", "");
                    if (this.OpenStyle.ShowType == ShowType.NewWindow|| this.OpenStyle.ShowType == ShowType.IFrame)
                    {
                        this._pageId = "root" + this._pageId;
                        this.RootPageId = this._pageId;
                    }
                }
                return _pageId;
            }
            set { _pageId = value; }
        }
        /// <summary>
        /// 单挂在哪个应用主页下
        /// 在应用的主页中打开表单时，系统默认都会把表单挂在本应用主页下，如在A应用首页中打开B应用下的表单，默认会挂在A应用主页下
        /// </summary>
        [SimpleProperty]
        public string AppId { get; set; }
        /// <summary>
        /// 首页pageId
        /// </summary>
        [SimpleProperty]
        public string RootPageId { get; set; }
        /// <summary>
        /// 其他非标准参数，可以任意添加
        /// </summary>
        [SimpleProperty]
        public Dictionary<string, object> CustomParams { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> ClientParams { get; set; }
        /// <summary>
        /// 页面标题
        /// </summary>
        public string Caption { get; set; }
        public string SubSystemId { get; set; }
        /// <summary>
        /// 前端页面打开动画类型
        /// </summary>
        public AnimationType AnimationType { get; set; }
        /// <summary>
        /// 界面配置参数类
        /// </summary>
        [ComplexProperty]
        public FormConfig FormConfig { get; set; }
        /// <summary>
        /// 需要打开的单据是否已经验权
        /// </summary>
        public bool IsHasRight { get; set; }
        /// <summary>
        /// 页面是否显示满屏按钮
        /// </summary>
        public bool IsShowFullScreen { get; set; }

        /// <summary>
        /// 是否显示标题
        /// </summary>
        public bool IsShowTitle { get; set; }




        /// <summary>
        /// 回调参数
        /// </summary>
        public CloseCallBack CloseCallBack { get; set; }


        /// <summary>
        /// 页面是否显示水印
        /// </summary>
        public bool IsShowWaterMark { get; set; }
        /// <summary>
        /// 界面打开时的操作状态
        /// </summary>
        protected OperationStatus Status { get; set; }


        [SimpleProperty]
        public int StatusValue
        {
            get { return (int)this.Status; }
            set { this.Status = (OperationStatus)value; }
        }

        public static Dictionary<string, Type> Ptypes { get; set; }
        public string SessionId { get; set; }



        public static FormShowParameter CreateFormShowParameter(Dictionary<string, object> openParameter)
        {
            
            string formId = openParameter.GetString("formId");
            string type = openParameter.GetString("type");
            if (string.IsNullOrEmpty(formId))
            {
                throw new Exception("formId不能为空");
            }
            FormConfig formConfig = FormMetadataCache.GetFormConfig(formId);
            type = formConfig.ModelType;
            if (type == null) type = "form";


            Type? clasz = ptypes.GetValueOrDefault(type,null);
            if (clasz==null)
            {
                
            }
            FormShowParameter showParameter =TypesContainer.CreateInstance<FormShowParameter>(clasz);
            showParameter.FormConfig = formConfig;

            //BeanUtils.copyProperties(showParameter, openParameter);
            //openParameter.Adapt<Dictionary<string, object>,FormShowParameter>(showParameter);
            // var c=  showParameter.Adapt(openParameter);

            //showParameter= AutoMapperExtensions.MapTo(openParameter, showParameter);
            //openParameter.MapTo<FormShowParameter>(showParameter);
            openParameter.CopyPropertiesTo(showParameter, OptionTyp.IsIgnoreCase);
            //BeanUtils.CopyProperties(openParameter, showParameter, OptionTyp.IsIgnoreCase);

            //showParameter = openParameter.Adapt(showParameter);

            //if (!string.IsNullOrEmpty(appId))
            //{
            //    showParameter.AppId=appId;
            //}
  
            if (openParameter!=null&& openParameter.Count>0)
            {
                PropertyInfo[] propertyInfos = typeof(FormShowParameter).GetProperties();
                List<PropertyInfo> notCanWriteProp= propertyInfos.Where(t => !t.CanWrite).ToList();
                foreach (var item in openParameter)
                {
                    string propName = item.Key;
                    if (notCanWriteProp.Any(t => t.Name == propName))
                    {
                        showParameter.CustomParams.Add(propName, openParameter.GetValueOrDefault(propName));
                    }
                    
                }
            }

            return showParameter;
        }

        public override string ToString()
        {
            return ToJsonString(this);
        }

        public static string ToJsonString(FormShowParameter type)
        {
            return ControlTypes.ToJsonString(type);
        }


        public IFormView CreateView()
        {
            IFormView formView = TypesContainer.CreateInstance<IFormView>(typeof(FormView));

            return formView;
        }

        internal string GetServiceAppId()
        {
            string serviceAppId = "";
            if (this.FormConfig != null)
            {
                serviceAppId = this.FormConfig.AppId;
                serviceAppId = (string.IsNullOrEmpty(serviceAppId)) ? "bos" : serviceAppId;
            }
            else
            {
                serviceAppId = FormMetadataCache.GetFormConfig(this.FormId).AppId;

            }
            return serviceAppId;
        }
    }
}
