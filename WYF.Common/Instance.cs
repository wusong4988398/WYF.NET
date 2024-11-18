using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Common
{
    public class Instance
    {
        private static readonly Random RandomGenerator = new Random();
        private static string clusterName;
        private static string instanceId;
        private static string appName;
        static Instance() {
            Instance.clusterName = "wyf";
            appName = "hr";
            Instance.instanceId = CreateInstanceId(Instance.appName);
        }
        public static string GetClusterName()
        {
            return Instance.clusterName;
        }

        public static string GetInstanceId()
        {
            return Instance.instanceId;
        }

        public static bool IsLightWeightDeploy()
        {
            return true;
        }
        public static string CreateInstanceId(string appName)
        {
            return $"{appName}-{RandomGenerator.Next(9)}{DateTime.UtcNow.Ticks % 1000000000L}";
        }
    }
}
