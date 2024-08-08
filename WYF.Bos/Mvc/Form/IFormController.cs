using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Mvc.Form
{
    public interface IFormController
    {

        void LoadData();
        void PostData(List<Dictionary<String, Object>> postDatas);

        void RegisterListener();

        void Destory();
    }
}
