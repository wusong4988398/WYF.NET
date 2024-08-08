using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata
{
    public interface IJoinProperty
    {
        public IDataEntityProperty FKProperty { get; }

        public IDataEntityProperty JoinProperty { get; }
    }
}
