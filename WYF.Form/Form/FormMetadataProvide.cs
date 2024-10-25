using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Entity.Service;
using WYF.Form.container;
using WYF.Service.Metadata;

namespace WYF.Form
{
    public class FormMetadataProvide : IFormMetadataProvide
    {
        private IMetadataService _metadataService = ServiceFactory.GetService<IMetadataService>();

        // private readonly IMetadataService _metadataService = App.GetService<IMetadataService>();

        //private readonly IMetadataService _metadataService = new MetadataService();

        public FormMetadataProvide()
        {

        }
        //public FormMetadataProvide(IMetadataService metadataService)
        //{
        //    this._metadataService = metadataService;
        //}


        public FormConfig GetFormConfig(string formId)
        {
            string configStr = _metadataService.LoadFormConfig(formId);

            if (string.IsNullOrEmpty(configStr))
            {
                throw new Exception("运行期元数据不存在");
            }
            FormConfig formConfig = ToFormConfig(configStr);

            return formConfig;
        }

        public FormRoot GetRootControl(string formId)
        {
            string formMetaStr = this._metadataService.LoadFormRuntimeMeta(formId, (int)RuntimeMetaType.Form, formId);
            if (string.IsNullOrEmpty(formMetaStr))
            {
                throw new Exception("运行期元数据不存在");
            }
            FormRoot formRoot = ControlTypes.FromJsonString<FormRoot>(formMetaStr);

            return formRoot;
        }

        public FormConfig ToFormConfig(string strConfig)
        {
            FormConfig? formConfig = null;
            try
            {
                formConfig = (FormConfig)ControlTypes.FromJsonStringToObj(strConfig);
            }
            catch (Exception ex)
            {
                //throw JNPFException.Oh(ex.Message);
                //formConfig = SerializationUtils.FromJsonString<FormConfig>(strConfig);
            }

            return formConfig;
        }


    }
}
