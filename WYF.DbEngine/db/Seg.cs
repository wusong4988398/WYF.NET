using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DbEngine.db
{
    public class Seg
    {
        public bool IsIn { get; set; }

        public bool IsInGenned { get; set; }

        public string SqlPart { get; set; }

        public object[] Paramters { get; set; }

        public Seg() { }
    }
}
