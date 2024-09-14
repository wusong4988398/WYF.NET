using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Util
{
    public interface IReservedWord
    {
        // Methods
        ReservedWordInfo isReservedWord(string word);
    }





}
