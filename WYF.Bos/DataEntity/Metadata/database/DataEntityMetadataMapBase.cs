using WYF.Bos.Orm.dataentity;
using WYF.Bos.Orm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.database
{
    public abstract class DataEntityMetadataMapBase<T> where T : IMetadata
    {

        protected T _source;

        public T MapSource 
        { 
            get 
            { 
                return this._source;
            }
            set { this._source = value;}
        }


        public string Name => this._source.Name;

        public string Alias=> this._source.Alias;




    }
}
