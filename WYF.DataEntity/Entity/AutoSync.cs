using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    public enum AutoSync
    {
        Never = 0,
        OnInsert = 1,
        OnUpdate = 2,
        Always = 3
   
    }
}
