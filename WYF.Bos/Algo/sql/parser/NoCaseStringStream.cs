using Antlr4.Runtime;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.parser
{
    public class NoCaseStringStream: AntlrInputStream
    {
    public NoCaseStringStream(String input):base(input)
    {
       
    }

        //public override int LA(int i)
        //{
        //    int la = base.LA(i);
        //    if (la == 0 || la == -1)
        //        return la;
        //    "11".ToUpper();
        //    return Character.toUpperCase(la);
        //}
        public override int LA(int i)
        {
            int la = base.LA(i);
            if (la == 0 || la == -1)
                return la;
            return Char.ToUpper((char)la);
        }

    }
}
