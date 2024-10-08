using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Env
{
    public abstract class Environment : Algo, IDisposable
    {

        private AlgoConfig config;
        private List<IListener> listeners;

        protected Environment(AlgoConfig config)
        {
            this.listeners = new List<IListener>();
            this.config = config;
            ThreadContext.AddEnvironment(this);
        }
        public AlgoConfig AlgoConfig { get { return config; } }
 
        // 实现IDisposable接口的方法
        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }


        public void AddListener(IListener listener)
        {
            this.listeners.Add(listener);
        }

        public void FireClosed()
        {
            foreach (var listener in this.listeners)
            {
                listener.Closed();
            }
        }
        public abstract void RegisterDataSet(IDataSet dataSet);


        // 由于继承了抽象方法close()，这里也需要定义一个抽象方法Close()
        public abstract void Close();

  

        // 定义监听器接口
        public interface IListener
        {
            void Closed();
        }
    }
}
