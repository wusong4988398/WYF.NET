using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Util
{
    public abstract class ReservedWord : IReservedWord
    {
        // Methods
        protected ReservedWord()
        {
        }

        public ReservedWordInfo isReservedWord(string word)
        {
            throw new NotImplementedException();
        }

        public int isReservedWord(string word, string[] reservedWords, int standard)
        {
            word = word.ToUpperInvariant();
            int num = 0;
            int num2 = reservedWords.Length - 1;
            while (num <= num2)
            {
                int index = (num + num2) >> 1;
                string strA = reservedWords[index];
                int num4 = string.Compare(strA, word, StringComparison.OrdinalIgnoreCase);
                if (num4 < 0)
                {
                    num = index + 1;
                }
                else
                {
                    if (num4 > 0)
                    {
                        num2 = index - 1;
                        continue;
                    }
                    return standard;
                }
            }
            return -1;
        }
    }






}
