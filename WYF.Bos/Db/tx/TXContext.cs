using WYF.Bos.db.splittingread;
using WYF.Bos.Threading;
using Newtonsoft.Json;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db.tx
{
    public  class TXContext
    {
        private readonly string tip;
        private readonly string accountId;
        private readonly long id;
        private static readonly AtomicLong idseq = new AtomicLong();
        private readonly Propagation propagation;
        private readonly TXContext parent;
        private readonly TXContext transRootCtx;
        private bool isImplicitTX;
        private readonly HashSet<string> forceMasterRouteSet;
        private string tag;
        private readonly TXStat txStat;
        private bool inTX;
        public TXContext(Propagation propagation, TXContext parent, string tag, bool isImplicitTX)
        {
            this.tip = "跨库写的SQL，不能在一个事务中执行，请用事务隔离或用MQ异步处理：";
            this.accountId = RequestContextInfo.Get().AccountId;
            this.id = idseq.Increment();
            if ((parent == null || parent.propagation == Propagation.NOT_SUPPORTED) && propagation == Propagation.REQUIRED)
                propagation = Propagation.REQUIRES_NEW;
            this.propagation = propagation;
            this.parent = parent;
            this.isImplicitTX = isImplicitTX;
            this.transRootCtx = PeekTMRoot();
            this.forceMasterRouteSet = (this.transRootCtx == this) ? new HashSet<string>() : this.transRootCtx.forceMasterRouteSet;
            this.tag = tag;
            this.txStat = (parent == null) ? new TXStat() : parent.txStat;
            this.txStat.Enter(this.id, (this.transRootCtx == this && !isImplicitTX));
            switch (propagation)
            {
                case Propagation.SUPPORTS:
                    this.inTX = (parent != null) ? parent.inTX : false;
                    break;
                case Propagation.REQUIRED:
                    this.inTX = true;
                    break;
                case Propagation.REQUIRES_NEW:
                    this.inTX = true;
                    break;
                case Propagation.NOT_SUPPORTED:
                    this.inTX = false;
                    break;
            }

        }

        //public DelegateConnection GetConnection(string tenantId, string routeKey, string accountId, bool requestForReadOnly, SplittingReadWriteMode threadRWMode, DataSourceInfoProvider connectionProvider, string mainTable, params string[] useTables)
        //{
        //    return null;
        //}


        private TXContext PeekTMRoot()
        {
            TXContext cur = this;
            while (cur.parent != null)
            {
                switch (cur.propagation)
                {
                    case Propagation.REQUIRES_NEW:
                    case Propagation.NOT_SUPPORTED:
                        return cur;
                }
                cur = cur.parent;
            }
            return cur;
        }
    }
}
