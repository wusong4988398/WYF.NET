using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.database
{
    public abstract class DbMetadataBase
    {
        
        public string Name { get; set; }
        public DbMetadataBase() { }
        public DbMetadataBase(string name)
        {
            this.Name = name;
        }

    }
}
