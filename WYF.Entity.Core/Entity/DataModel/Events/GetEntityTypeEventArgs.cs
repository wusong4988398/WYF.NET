using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Entity.DataModel.Events
{
    public class GetEntityTypeEventArgs
    {
        public GetEntityTypeEventArgs(MainEntityType mainEntityType)
        {
            this.OriginalEntityType = mainEntityType;
        }
        public MainEntityType OriginalEntityType { get; set; }
        public MainEntityType NewEntityType { get; set; }
    }
}
