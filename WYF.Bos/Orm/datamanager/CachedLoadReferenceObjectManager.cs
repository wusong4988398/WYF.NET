using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    public class CachedLoadReferenceObjectManager : LoadReferenceObjectManager
    {
        public CachedLoadReferenceObjectManager(IDataEntityType dt, bool onlyDbProperty) : base(dt, onlyDbProperty)
        {

        }

        public  void Load(Object[] dataEntities)
        {
            DoTasks(GetTasks(dataEntities));
        }

    }
}
