using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form
{
    public interface IClientViewProxy
    {
        List<Object> GetActionResult();

        void AddAction(string actionName, object param);

        void SetFieldProperty(String key, String property, Object v);
    }
}
