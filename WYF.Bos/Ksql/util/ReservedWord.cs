using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql.util
{
    public abstract class ReservedWord : IReservedWord
    {
        protected int IsReservedWord(string word, string[] reservedWords,  int standard)
        {
            word = word.ToUpper();
            int low = 0;
            int high = reservedWords.Length - 1;
            while (low <= high)
            {
                 int mid = low + high >> 1;
                 String midVal = reservedWords[mid];
                 int cmp = midVal.CompareTo(word);
                if (cmp < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    if (cmp <= 0)
                    {
                        return standard;
                    }
                    high = mid - 1;
                }
            }
            return -1;
        }

        public abstract ReservedWordInfo IsReservedWord(string word);
    }
}
