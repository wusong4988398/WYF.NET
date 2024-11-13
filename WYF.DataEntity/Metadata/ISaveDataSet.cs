using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Collections;

namespace WYF.DataEntity.Metadata
{
    public interface ISaveDataSet
    {
        KeyedCollectionBase<string, ISaveDataTable> Tables { get; }

    }
}
