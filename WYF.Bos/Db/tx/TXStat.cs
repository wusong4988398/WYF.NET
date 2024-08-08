using WYF.Bos.trace;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db.tx
{
    public  class TXStat
    {
        private LinkedList<bool> isRootTXStack = new LinkedList<bool>();
        private LinkedList<ITraceSpan> rootTraceSpanStack = new LinkedList<ITraceSpan>();
        private int rootTXCount;
        private int _currentTXDeep;
        private int maxRootTXDeep;
        public void Enter(long txId, bool isRootTX)
        {
            this.isRootTXStack.AddLast(isRootTX);
            if (isRootTX)
            {
                ITraceSpan ts = Tracer.Create("TX", "enter-exit");

                this.rootTraceSpanStack.AddLast(ts);
                this.rootTXCount++;
                this._currentTXDeep++;
                if (this.maxRootTXDeep < this._currentTXDeep)
                    this.maxRootTXDeep = this._currentTXDeep;
            }
        }
    }
}
