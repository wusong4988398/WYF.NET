using System;

namespace WYF.Algo.Env
{
    public class AlgoConfig
    {
    

        public AlgoConfig(string algoKey)
        {
            this.Key = algoKey;
        }

        public string Key { get; internal set; }

        internal AlgoEnv GetEnv(AlgoEnv jvm)
        {
            return AlgoEnv.Jvm;
        }
    }
}