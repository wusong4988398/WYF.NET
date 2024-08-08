using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql.util
{
    public class ReservedWordInfo
    {
        public string ReservedWord { get; set; }
        public int Standard { get; set; }

        public ReservedWordInfo(string reservedWord,int standard)
        {
            this.ReservedWord = reservedWord;
            this.Standard = standard;
        }
    }
}
