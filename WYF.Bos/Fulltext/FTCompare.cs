using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.fulltext
{
    public enum FTCompare
    {
        [Description("like")]
        LIKE,
        [Description("in")]
        IN,
        [Description("match")]
        MATCH,
        [Description("eq")]
        EQ,
        [Description("gt")]
        GT,
        [Description("lt")]
        LT,
        [Description("notnull")]
        NOTNULL
    }
}
