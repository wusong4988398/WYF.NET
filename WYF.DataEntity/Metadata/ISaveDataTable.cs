using WYF.DataEntity.Metadata.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{
    public interface ISaveDataTable
    {
        IDeleteMetaRow[] DeleteRows { get; }
        ISaveMetaRow[] SaveRows { get; }
        DbMetadataTable Schema { get; set; }
        List<Tuple<object, object, int>> ChangeRows { get; }
    }
}
