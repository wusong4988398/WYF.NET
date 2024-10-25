using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Form.container;

namespace WYF.Form
{
    public class FormMetadataCache
    {
        //private readonly IFormMetadataProvide _formMetadataProvide;
        private static IFormMetadataProvide _provide;

        public static IFormMetadataProvide GetFormMetadataProvide()
        {
            if (_provide == null)
                _provide = new FormMetadataProvide();
            return _provide;
        }

        //public FormMetadataCache(IFormMetadataProvide formMetadataProvide)
        //{
        //    this._formMetadataProvide= formMetadataProvide;
        //}

        public static FormConfig GetFormConfig(string formId)
        {
            return GetFormMetadataProvide().GetFormConfig(formId);
        }


        public static FormRoot GetRootControl(string formId)
        {
            return GetFormMetadataProvide().GetRootControl(formId.ToLower());
        }
    }
}
