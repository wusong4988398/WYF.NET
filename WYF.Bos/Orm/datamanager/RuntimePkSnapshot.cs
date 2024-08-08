﻿using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
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
