
using WYF.Bos.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.Form;
public interface IFormMetadataProvide
{
    FormConfig GetFormConfig(string formId);
    FormRoot GetRootControl(string formId);
}
