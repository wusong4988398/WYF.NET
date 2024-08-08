using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql.util
{
    public class KSQLReservedWord : ReservedWord
    {
        protected string[] reservedWords;
        protected readonly int standard = 0;

        public KSQLReservedWord()
        {
            this.reservedWords = new string[] { "AT", "LEAVE", "NUMBER", "POSITION", "RESULT", "ROLE", "ROLES", "TEMP", "TO", "TYPE", "VALUE" };
        }
        public override ReservedWordInfo IsReservedWord(string word)
        {
            int result = this.IsReservedWord(word, this.reservedWords, 0);
            if (result > -1)
            {
                return new ReservedWordInfo(word, 0);
            }
            return null;
        }
    }
}
