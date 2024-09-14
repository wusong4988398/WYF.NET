using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    public interface IDataEntityCacheManager
    {
        IDataEntityType GetDataEntityType();

        Dictionary<object, object> Get(object[] pkArray);
    }
}
