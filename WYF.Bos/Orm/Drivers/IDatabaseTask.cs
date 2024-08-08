using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.Drivers
{
    public interface IDatabaseTask : IDisposable
    {

        void Execute(IDbConnection con, IDbTransaction tran);


        int Level { get; }
    }
}
