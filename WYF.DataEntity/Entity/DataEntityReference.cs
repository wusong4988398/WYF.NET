
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    public class DataEntityReference
    {
        public Object Oid { get;set; }

        public Object DataEntity { get; set; }

        public bool IsDataEntityLoaded { get; set; }


        public DataEntityReference(Object oid)
        {
            this.Oid = oid;
        }
        public void SetDataEntity(Object value)
        {
            this.DataEntity = value;
            this.IsDataEntityLoaded = true;
        }


    }
}
