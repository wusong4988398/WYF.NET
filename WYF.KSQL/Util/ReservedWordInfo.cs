using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Util
{
    public class ReservedWordInfo
    {
        // Fields
        private string reservedWord;
        private int standard;

        // Methods
        public ReservedWordInfo(string reservedWord, int standard)
        {
            this.reservedWord = reservedWord;
            this.standard = standard;
        }

        public string getReservedWord()
        {
            return this.reservedWord;
        }

        public int getStandard()
        {
            return this.standard;
        }
    }



}
