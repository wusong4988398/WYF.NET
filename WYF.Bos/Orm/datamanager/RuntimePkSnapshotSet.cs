using WYF.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal sealed class RuntimePkSnapshotSet
    {

        public PkSnapshotSet PkSnapshotSet;
        public RuntimePkSnapshot[] Tables;
        public void Complete()
        {
            int length = this.Tables.Length;
            for (int i = 0; i < length; i++)
            {
                this.Tables[i].Complete();
            }
        }
    }
}
