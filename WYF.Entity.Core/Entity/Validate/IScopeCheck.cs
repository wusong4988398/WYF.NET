using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Entity.Validate
{
    public interface IScopeCheck
    {
        bool CheckScope(object value);

        string GetDataScopeMessage(object value);
    }
}
