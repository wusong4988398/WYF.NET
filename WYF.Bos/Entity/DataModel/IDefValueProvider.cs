using WYF.Bos.Entity.property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel
{
    public interface IDefValueProvider
    {
        object GetDefValue(IFieldHandle fieldProp);
    }
}
