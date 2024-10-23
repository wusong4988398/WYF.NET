using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WYF.Cache
{
    public class CacheConfigInfo
    {
        public int Timeout {  get; set; }
        public int MaxMemSize { get; set; }

        public int MaxItemSize {  get; set; }

        public bool IsTimeToLive {  get; set; }
    }
}
