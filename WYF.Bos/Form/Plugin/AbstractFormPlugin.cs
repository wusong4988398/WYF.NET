using WYF.Bos.Entity.DataModel;
using WYF.Bos.Entity.DataModel.Events;
using WYF.Bos.Entity.plugin;
using WYF.Bos.Form.control;
using WYF.Bos.Form.control.events;
using WYF.Bos.Form.Events;
using WYF.Bos.Mvc.Form;

namespace WYF.Bos.Form.plugin
{
    /// <summary>
    /// 表单视图层插件基类
    /// 继承自表单模型层插件基类AbstractDataModelPlugin，实现了视图层插件事件接口IFormPlugin：既能处理表单数据改变事件，也能处理表单视图事件；
    /// </summary>
    public abstract class AbstractFormPlugin : IDataModelListener, IDataModelChangeListener,IFormPlugin
    {

        private IFormView _formView;

        private IDataModel _dataModel;

        private IPageCache _pageCache;

        public void SetView(IFormView formView)
        {
            this._formView = formView;
            this._dataModel =this._formView.GetService<IDataModel>();
            this._pageCache = this._formView.GetService<IPageCache>();
            this._dataModel.AddDataModelListener(this);
            this._dataModel.AddDataModelChangeListener(this);
        }
        public string PluginName { get => this.GetType().Name; }
        public IFormView FormView { get => _formView; set => _formView = value; }
        public IDataModel DataModel { get => _dataModel; set => _dataModel = value; }
        public IPageCache PageCache { get => _pageCache; set => _pageCache = value; }


        /// <summary>
        /// 控件事件监听器注册事件
        /// 在表单页面请求开始时触发，通知插件注册控件的事件监听器
        /// </summary>
        /// <param name="e"></param>
        public virtual void RegisterListener(EventObject e) { }

        //public abstract void AfterCreateNewData(EventObject e);

 

      


        public void AddClickListeners()
        {
           
        }
    
        //public abstract void AfterCreateNewData(EventObject e);

        public T GetControl<T>(string key) where T : Control
        {
            return (T)this.FormView.GetControl<Control>(key);
        }

        public virtual void OnGetControl(OnGetControlArgs e) { }
        

        public virtual void AfterCreateNewData(EventObject e) { }

        /// <summary>
        /// 表单页面请求初始化事件
        /// </summary>
        public virtual void Initialize() { }
        /// <summary>
        /// 自定义事件
        /// </summary>
        /// <param name="e"></param>
        public virtual void CustomEvent(CustomEventArgs e) { }

        /// <summary>
        /// 子界面关闭后的回调处理接口
        /// 父界面的插件可以实现此接口，在打开的子界面关闭时，处理子界面关闭回调逻辑
        /// </summary>
        /// <param name="closedCallBackEvent"></param>
        public virtual void ClosedCallBack(ClosedCallBackEvent closedCallBackEvent) { }

        /// <summary>
        /// 控件点击前事件
        /// </summary>
        /// <param name="evt"></param>
        public virtual void BeforeClick(BeforeClickEvent evt) { }
        
        /// <summary>
        /// 控件点击事件
        /// </summary>
        /// <param name="evt"></param>
        public virtual void Click(EventObject evt) { }
        /// <summary>
        /// 控件子项点击前事件
        /// </summary>
        /// <param name="evt"></param>
        public virtual void BeforeItemClick(BeforeItemClickEvent evt) { }

        /// <summary>
        /// 控件子项点击事件
        /// </summary>
        /// <param name="evt"></param>
        public virtual void ItemClick(ItemClickEvent evt) { }
        
    }
}
