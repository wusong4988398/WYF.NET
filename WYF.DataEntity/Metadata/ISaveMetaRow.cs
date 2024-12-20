﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{
    public interface ISaveMetaRow
    {
  
        ForWriteList<IColumnValuePair> DirtyValues { get; set; }
        IColumnValuePair Oid { get; set; }
        RowOperateType Operate { get; set; }
        List<IColumnValuePair> OutputValues { get; set; }
        IColumnValuePair ParentOid { get; set; }
        IColumnValuePair Version { get; set; }


    }
}
