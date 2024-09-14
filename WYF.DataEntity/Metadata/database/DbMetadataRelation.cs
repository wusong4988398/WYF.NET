using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.database
{
    public class DbMetadataRelation : DbMetadataBase

    {
        public DbMetadataColumn ChildColumn { get; set; }

        public DbMetadataTable ParentTable { get; set; }

    }
}
