using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.dataManager
{
    public class DeleteRow : IDeleteMetaRow
    {
        public object Oid { get; set; }
    }
}
