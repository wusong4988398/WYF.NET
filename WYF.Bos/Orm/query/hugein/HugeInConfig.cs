
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.hugein
{
    public class HugeInConfig
    {
        private static bool enableOptIn = true;

        private static OptType optType = OptType.temp_table;

        private static int inThreshold = 1000;

        private static int inMaxSize = 500000;

        public static bool IsEnableOpt()
        {
            return (enableOptIn && inThreshold > 0);
        }
        public static OptType GetOptType()
        {
            return optType;
        }
        public static int InThreshold()
        {
            return inThreshold;
        }
        public enum OptType
        {
            temp_table,
            split_in
        }
    }
}
