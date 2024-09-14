using WYF.DataEntity.Entity;
using WYF.Bos.Entity.DataModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace WYF.Bos.Form.control
{
    /// <summary>
    /// 控件基类运行期各种控件抽象基类，衍生出容器、表单、单据体表格、按钮控件、字段编辑控件等 持有表单视图对象，可以访问并控制界面各种控件
    /// </summary>
    public class Control : ICloneable
    {
        private IDataModel model;
        protected IFormView view;
        protected IClientViewProxy clientViewProxy;

        public IDictionary<String, Object> CustomProperties { get; set; } = new ConcurrentDictionary<String, Object>();

        /// <summary>
        /// 控件标识
        /// </summary>
        [SimpleProperty]
        public string Key { get; set; }

        /// <summary>
        ///自定义样式
        /// </summary>
        [SimpleProperty]
        public string CustomeStyles { get; set; }

        public string GetOpKey(string methodName, JSONArray args)
        {
            return this.Key;
        }

        public void SetView(IFormView view)
        {
            this.view = view;
            this.model = view.Model;
            this.clientViewProxy = view.GetService<IClientViewProxy>();

            //this.clientViewProxy = (IClientViewProxy)view.getService(IClientViewProxy.class);
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public virtual void BindData(BindingContext bctx)
        {
            
        }
    }
}
