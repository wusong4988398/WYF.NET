using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Entity.DataModel.Events;
using WYF.Form.control.Events;
using WYF.Form.Events;


namespace WYF.Form.plugin
{
    /// <summary>
    /// 表单界面插件基本接口，提供了访问IFormView的方法，据此控制界面；提供了基本的表单插件事件，如绑定数据事件；
    /// </summary>
    public interface IFormPlugin : ClickListener, ItemClickListener, IDataModelListener, IDataModelChangeListener, ICloseCallBack
    {
        public string PluginName { get; }

        /// <summary>
        /// 初始化事件
        /// </summary>
        void Initialize();
        /// <summary>
        /// 设置表单视图
        /// </summary>
        /// <param name="formView"></param>
        void SetView(IFormView formView);

        /// <summary>
        /// 获取插件控件
        /// </summary>
        /// <param name="e"></param>
        void OnGetControl(OnGetControlArgs e);

        /// <summary>
        /// 控件事件监听器注册事件
        /// </summary>
        /// <param name="e"></param>
        void RegisterListener(EventObject e) { }

        /// <summary>
        /// 自定义事件
        /// </summary>
        /// <param name="e"></param>
        void CustomEvent(CustomEventArgs e);
    }
}
