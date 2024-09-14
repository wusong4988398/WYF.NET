using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.multi
{
    public enum JoinTableTypeEnum
    {
        [Description("分录表")]
        entry,
        [Description("扩展表")]
        extend,
        [Description("多语言表")]
        multi_lang,
        [Description("基础资料表")]
        basedata
    }
}
