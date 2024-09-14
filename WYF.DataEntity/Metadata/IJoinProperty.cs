using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{
    public interface IJoinProperty
    {
        public IDataEntityProperty FKProperty { get; }

        public IDataEntityProperty JoinProperty { get; }
    }
}
