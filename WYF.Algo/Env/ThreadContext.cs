
using System.Collections.Generic;
using System;
using System.Threading;
using WYF.Algo.dataset.store;

namespace WYF.Algo.Env
{
    public class ThreadContext : IDisposable
    {
        private static readonly ThreadLocal<ThreadContext> tlContext = new ThreadLocal<ThreadContext>(() => new ThreadContext());
        private static bool autoReleaseWhenThreadDie = true;

        private readonly Dictionary<string, Environment> envMap = new Dictionary<string, Environment>();
        private readonly List<IDisposable> allCloseable = new List<IDisposable>();

        static ThreadContext()
        {
            //string str = Environment.GetEnvironmentVariable("algo.autoCloseDataSet");
            string str = "";
            if (!string.IsNullOrEmpty(str))
            {
                autoReleaseWhenThreadDie = bool.Parse(str);
            }
        }

        public void AddEnv(Environment env)
        {
            envMap[env.AlgoConfig.Key] = env;
        }

        private void RemoveEnv(Environment env)
        {
            envMap.Remove(env.AlgoConfig.Key);
        }

        public void Dispose()
        {
            if (autoReleaseWhenThreadDie)
            {
                var list = new List<Environment>(envMap.Values);
                foreach (var env in list)
                {
                    try
                    {
                        env.Dispose();
                    }
                    catch (Exception e)
                    {
                       // logger.Error(e, e.Message);
                    }
                }
                envMap.Clear();

                var toRemove = new List<IDisposable>(allCloseable);
                foreach (var c in toRemove)
                {
                    try
                    {
                        c.Dispose();
                    }
                    catch (Exception e)
                    {
                        //logger.Error(e, e.Message);
                    }
                }
                allCloseable.Clear();
            }
        }

        public Environment GetEnv(string key)
        {
            envMap.TryGetValue(key, out var env);
            return env;
        }

        public void AddStore(IStore store)
        {
            allCloseable.Add(store);
        }

        public void RemoveStore(IStore store)
        {
            allCloseable.Remove(store);
        }

        public static ThreadContext GetCurrent()
        {
            return tlContext.Value;
        }

        public static void AddEnvironment(Environment env)
        {
            tlContext.Value.AddEnv(env);
        }

        public static void RemoveEnvironment(Environment env)
        {
            tlContext.Value.RemoveEnv(env);
        }

        public static void Clear()
        {
            tlContext.Value.Dispose();
            tlContext.Value = null;
        }
    }
}