using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.dataentity
{
    [Serializable]
    public class DataEntityCollectionWithSnapshot<T> : DataEntityCollection<T>
    {
    
        private PkSnapshot _pkSnapshot;


        public DataEntityCollectionWithSnapshot()
        {
        }

        public DataEntityCollectionWithSnapshot(object parent) : base(parent)
        {
        }

        public DataEntityCollectionWithSnapshot(object parent, IList<T> list) : base(parent, list)
        {
        }


        public PkSnapshot PkSnapshot
        {
            get
            {
                return this._pkSnapshot;
            }
            set
            {
                this._pkSnapshot = value;
            }
        }
    }
}
