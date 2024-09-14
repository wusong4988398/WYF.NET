using WYF.DataEntity.Metadata.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{
    public  interface IColumnValuePair
    {
        DbMetadataColumn Column { get; }
        object Value { get; set; }
    }
}
