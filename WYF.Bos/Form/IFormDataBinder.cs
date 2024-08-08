using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form
{
    public interface IFormDataBinder
    {
        void UpdateView();

        void UpdateView(string key);
    }
}
