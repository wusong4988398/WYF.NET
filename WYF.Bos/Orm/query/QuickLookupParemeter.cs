using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace WYF.Bos.Orm.query
{
    [Serializable]
    public class QuickLookupParemeter //: QueryBuilderParemeter
    {

        public string KeyWord { get; set; }

        public int? limit { get; set; }

        public JSONObject LookupConfig { get; set; }

        public int? start { get; set; }
    }
}
