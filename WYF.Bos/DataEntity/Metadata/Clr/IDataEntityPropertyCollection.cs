﻿using WYF.Bos.DataEntity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.Clr
{
    public interface IDataEntityPropertyCollection : IKeyedCollectionBase<string, IDataEntityProperty>, IEnumerable<IDataEntityProperty>, IEnumerable
    {

    }
}
