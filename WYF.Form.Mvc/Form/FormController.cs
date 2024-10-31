using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Entity.DataModel;
using WYF.Form;
using WYF.Form.plugin;

namespace WYF.Mvc.Form
{
    public class FormController : IFormController
    {
        protected IFormView _view;
        protected bool bRegListner = false;
        public FormController(IFormView view)
        {
            this._view = view;
        }
        public IFormView View
        {
            get
            {
                return _view;
            }
        }

        public void Destory()
        {

            ((FormView)View).IsDestory = true;

        }

        public void LoadData()
        {
            IDataModel model = this.GetModel();
            model.BeginInit();
            this.InitModelContextVariable(this._view.FormShowParameter, model);
            this.CreateModelData(model);
            ((FormDataModel)model).IsCacheExpireAfter = true;
            this._view.UpdateView();
            model.EndInit();

        }
        private IDataModel GetModel()
        {
            return this._view.GetService<IDataModel>();
        }
        protected void CreateModelData(IDataModel model)
        {
            model.CreateNewData();
        }
        protected void InitModelContextVariable(FormShowParameter parameter, IDataModel model)
        {
            String appId = parameter.AppId;
            if (!string.IsNullOrEmpty(appId))
            {
                model.PutContextVariable("CUR_APP_ID", appId);
                model.PutContextVariable("CUR_ENTITY_TYPE_ID", parameter.FormId);
            }

        }
        public void PostData(List<Dictionary<string, object>> postDatas)
        {
            if (postDatas != null && postDatas.Count > 0)
            {

            }
        }
        protected FormViewPluginProxy GetPluginProxy()
        {
            return this._view.GetService<FormViewPluginProxy>();
        }
        public void RegisterListener()
        {
            Console.WriteLine("FormController.RegisterListener()");
            if (!this.bRegListner)
            {
                GetPluginProxy().FireRegisterListener(new EventObject(this.View));
                this.bRegListner = true;
            }
        }
    }
}
