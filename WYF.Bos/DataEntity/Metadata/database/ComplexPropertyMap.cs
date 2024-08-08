using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.database
{
    public class ComplexPropertyMap: PropertyMap<IComplexProperty>
    {
        public DataEntityTypeMap ComplexPropertyTypeMap { get; set; }

        public IDataEntityProperty RefIdProperty { get; set; }

        public DbMetadataColumn IsNull_DbColumn { get; internal set; }

    }
}
