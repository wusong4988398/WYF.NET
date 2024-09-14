using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.database
{
    public class CollectionPropertyMap: PropertyMap<ICollectionProperty>
    {
        public DataEntityTypeMap CollectionItemPropertyTypeMap { get; set; }

        public DbMetadataColumn ParentColumn { get; set; }

    }
}
