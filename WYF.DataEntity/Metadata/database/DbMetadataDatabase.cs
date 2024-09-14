using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.database
{
    public class DbMetadataDatabase : DbMetadataBase
    {

        public DbMetadataDatabase()
        {
            this.Tables = new DbMetadataTableCollection();
        }
        public DbMetadataTableCollection Tables { get; set; }


    }
}
