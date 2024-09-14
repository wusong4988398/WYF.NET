using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Util
{
    public class KSQLReservedWord : ReservedWord
    {
        // Fields
        protected string[] reservedWords = new string[] { "AT", "LEAVE", "NUMBER", "POSITION", "RESULT", "ROLE", "ROLES", "TEMP", "TO", "TYPE", "VALUE" };
        protected const int standard = 0;

        // Methods
        public ReservedWordInfo isReservedWord(string word)
        {
            if (base.isReservedWord(word, this.reservedWords, 0) > -1)
            {
                return new ReservedWordInfo(word, 0);
            }
            return null;
        }
    }






}
