using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Common
{
    public class Instance
    {
        private static string clusterName;
        static Instance() {
            Instance.clusterName = "wyf";
        }
        public static string GetClusterName()
        {
            return Instance.clusterName;
        }
    }
}
