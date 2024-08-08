using WYF.Bos.DataEntity.Metadata.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata
{
    public interface ISaveDataTable
    {
        IDeleteMetaRow[] DeleteRows { get; }
        ISaveMetaRow[] SaveRows { get; }
        DbMetadataTable Schema { get; }
    }
}
