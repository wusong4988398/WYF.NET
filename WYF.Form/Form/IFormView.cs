
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Entity.DataModel;
using WYF.Form.control;

namespace WYF.Form
{
    /// <summary>
    /// 表单视图控制接口
    /// 提供各种操作界面的方法，包括更新界面、获取控件、显示提示等。
    /// 表单服务、操作、插件等，可以通过此接口一站式获取和控制表单视图。
    /// 下级控件的控制，放到控件Control等相关类上去实现。
    /// 表单插件可以使用 this.getView()获取本接口实例。
    /// </summary>
    public interface IFormView
    {
        public string PageId { get; }

        public FormShowParameter FormShowParameter { get; set; }

        void Initialize(FormShowParameter showParameter);
        void AddService(Type type, object service);

        IDataModel Model { get; }
        T GetService<T>();
        T GetControl<T>(string key) where T : Control;


        void ShowForm(FormShowParameter showParameter);
        List<object> GetActionResult();
        /// <summary>
        /// 重新绑定数据到前端控件
        /// 此方法会重新刷新整个页面，插件推荐用 this.getView().updateView(key) 局部刷新指定控件
        /// </summary>
        void UpdateView();

        /// <summary>
        /// 获取表单根容器： FormRoot	
        /// </summary>
        /// <returns></returns>
        Control GetRootControl();

        /// <summary>
        /// 获取应用首页
        /// </summary>
        /// <returns></returns>
        IFormView GetMainView();

        IFormView GetView(string pageId);
        IFormView GetViewNoPlugin(string pageId);

        void Activate();

        void SendFormAction(IFormView view);
    }
}
