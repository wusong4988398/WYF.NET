using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata
{
    public interface ISaveDataSet
    {
        KeyedCollection<string, ISaveDataTable> Tables { get; }

    }
}
