
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    public class DataEntityReference
    {
        public object Oid { get;set; }

        public virtual object DataEntity { get; set; }

        public bool IsDataEntityLoaded { get; set; }


        public DataEntityReference(object oid)
        {
            this.Oid = oid;
        }
        public void SetDataEntity(object value)
        {
            this.DataEntity = value;
            this.IsDataEntityLoaded = true;
        }


    }
}
