
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.validate
{
    public interface IScopeCheck
    {
        bool CheckScope(object value);

        string GetDataScopeMessage(object value);
    }
}
