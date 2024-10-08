﻿using WYF.DataEntity.Entity;
using WYF.Bos.Entity.DataModel;
using WYF.Bos.Form;

namespace WYF.Bos.Mvc.Form
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