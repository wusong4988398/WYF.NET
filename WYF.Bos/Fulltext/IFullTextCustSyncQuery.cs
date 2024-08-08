
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.fulltext
{
    public interface IFullTextCustSyncQuery
    {
        bool IsEnable();

        bool IsConfigFullText(string entityName);

        string[] Query(string entityName, FTFilter filter);
    }
}
