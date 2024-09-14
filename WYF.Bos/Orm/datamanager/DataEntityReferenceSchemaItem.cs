using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    public class DataEntityReferenceSchemaItem
    {
        public String PropertyPath { get; set; }

        public ISimpleProperty ReferenceOidProperty { get; set; }

        public String ReferenceTo { get; set; }

        public IComplexProperty ReferenceObjectProperty { get; set; }
    }
}
