using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.database
{
    public enum DataSourceSchemaType
    {
        [EnumMember(Value = "Table")]
        Table = 0,
        [EnumMember(Value = "View")]
        View = 1
    }
}
