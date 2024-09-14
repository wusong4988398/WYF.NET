using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal sealed class DeleteRow : IDeleteMetaRow
    {

        public object Oid { get; internal set; }
    }
}
