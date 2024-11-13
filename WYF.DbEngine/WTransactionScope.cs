using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace WYF.DbEngine
{
    public class WTransactionScope : IDisposable
    {
 
        private TransactionScope ts;

  
        public WTransactionScope(TransactionScopeOption option)
        {
            if ((option != TransactionScopeOption.Required) && (option != TransactionScopeOption.RequiresNew))
            {
                this.ts = new TransactionScope(option);
            }
            else
            {
                TransactionOptions transactionOptions = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = new TimeSpan(1, 0, 0)
                };
                this.ts = new TransactionScope(option, transactionOptions);
            }
        }

        public void Complete()
        {
            this.ts.Complete();
        }

        public void Dispose()
        {
            this.ts.Dispose();
        }
    }
}
