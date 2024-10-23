using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Cache
{

    public interface IDistributeSessionlessCache: ISessionlessCache<string>
    {
        int AddList(string key, params string[] values);
    }
}
