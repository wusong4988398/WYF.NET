using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Form.container;

namespace WYF.Form
{
    public interface IFormMetadataProvide
    {
        FormConfig GetFormConfig(string formId);
        FormRoot GetRootControl(string formId);
    }
}
