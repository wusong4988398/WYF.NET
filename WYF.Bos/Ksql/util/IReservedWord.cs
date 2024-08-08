using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql.util
{
    public  interface IReservedWord
    {
        ReservedWordInfo IsReservedWord(string word);

    }
}
