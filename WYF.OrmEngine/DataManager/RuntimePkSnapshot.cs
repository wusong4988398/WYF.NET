using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;

namespace WYF.OrmEngine.dataManager
{
    internal sealed class RuntimePkSnapshot
    {

        public List<object> Oids;
        public readonly PkSnapshot Snapshot;

        public List<Object> ParentIds { get; set; }
        public RuntimePkSnapshot(PkSnapshot snapshot)
        {
            this.Snapshot = snapshot;
            this.Oids = new List<object>();
        }

        public void Complete()
        {
            this.Snapshot.Oids = this.Oids.ToArray();
        }
    }
}
