using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db.tx
{
    public sealed class TX
    {
        private static  ThreadLocal<TXContext> tx_holder = new ThreadLocal<TXContext>();
        private static  ThreadLocal<TXContext> out_tx_holder = new ThreadLocal<TXContext>();    
        public static IDbConnection __GetConnection(string routeKey, bool requestForReadOnly, string mainTable, params string[] useTables)
        {
            //RequestContextInfo rc = RequestContextInfo.Get();
            //string accountId = rc.AccountId;
            //TXContext ctx = GetCurrentOrCreateImplicitTXContext();
            //DelegateConnection con = ctx.GetConnection(rc.getTenantId(), routeKey, accountId, requestForReadOnly,
            //    ThreadReadWriteContext.getCurrentSplittingReadWriteMode(), dsProvider, mainTable, useTables);
            //try
            //{
            //    if (ctx.isInTX() && con.getAutoCommit())
            //        con.__setAutoCommit(false);
            //}
            //catch (SQLException e)
            //{
            //    throw ExceptionUtil.asRuntimeException(e);
            //}
            //return con;
            return null;
        }

        private static TXContext GetCurrentOrCreateImplicitTXContext()
        {
            TXContext ctx = tx_holder.Value;
            if (ctx == null)
            {
                ctx = out_tx_holder.Value;
                if (ctx == null)
                {
                    ctx = new TXContext(Propagation.NOT_SUPPORTED, null, "Implicit-transaction", true);
                    out_tx_holder.Value = ctx;
                }
            }
            return ctx;
        }
    }
}
