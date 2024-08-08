using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.fulltext.custsync
{
    public class FullTextCustSyncQuery : IFullTextCustSyncQuery
    {
        public bool IsConfigFullText(string entityName)
        {
            throw new NotImplementedException();
        }

        public bool IsEnable()
        {
            throw new NotImplementedException();
        }

        public string[] Query(string entityName, FTFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
