using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Entity.Property;

namespace WYF.Entity.DataModel
{
    public interface IDefValueProvider
    {
        object GetDefValue(IFieldHandle fieldProp);
    }
}
