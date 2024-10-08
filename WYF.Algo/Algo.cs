using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Env;
using Environment = WYF.Algo.Env.Environment;

namespace WYF.Algo
{
    public abstract class Algo
    {
        public abstract IDataSet CreateDataSet(IDataReader rs, RowMeta rowMeta);
        public static Algo Create(string algoKey)
        {
            ThreadContext ctx = ThreadContext.GetCurrent();
            if (ctx != null)
            {
                Environment env = ctx.GetEnv(algoKey);
                if (env != null)
                    return (Algo)env;
            }
            AlgoConfig config = new AlgoConfig(algoKey);
            return Create(config);
        }

        public static Algo Create(AlgoConfig config)
        {
            JvmEnvironment environment;
            AlgoEnv env = config.GetEnv(AlgoEnv.Jvm);
            switch (env)
            {
                case AlgoEnv.Jvm:
                    environment = new JvmEnvironment(config);
                    return (Algo)environment;
            }
            return null;
        }
    }
}
