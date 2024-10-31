using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.Entity.DataModel;
using WYF.Form;
using WYF.Form.container;

namespace WYF.Mvc.Form
{
    public class FormDataBinder : IFormDataBinder
    {
        IFormView _view;

        private IDataModel _model;

        private string _firstFocusControl;

        public FormDataBinder(IFormView view)
        {
            this._view = view;
            this._model = (IDataModel)this._view.GetService<IDataModel>();


        }

        public void UpdateView()
        {
            DynamicObject dataEntity = this._model.GetDataEntity();
            if (dataEntity == null)
                throw new Exception();
            List<RowDataEntity> objs = new List<RowDataEntity>();
            objs.Add(new RowDataEntity(0, dataEntity));
            FormRoot rootControl = (FormRoot)this._view.GetRootControl();
            PutFmtInfo();

            rootControl.BindData(new BindingContext(dataEntity, 0));
        }

        private void PutFmtInfo()
        {

        }

        public void UpdateView(string key)
        {
            throw new NotImplementedException();
        }
    }
}
