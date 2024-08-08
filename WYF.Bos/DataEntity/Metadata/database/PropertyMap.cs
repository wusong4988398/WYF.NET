using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.database
{
    public abstract class PropertyMap<T> : DataEntityMetadataMapBase<T> where T : IDataEntityProperty
    {
  
        protected PropertyMap()
        {
        }

    
        public T DataEntityProperty
        {
            get
            {
                return base.MapSource;
            }
            internal set
            {
                base.MapSource = value;
            }
        }
    }
}
