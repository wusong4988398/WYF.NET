using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query
{
    public interface QCP
    {
        public static readonly String equals = "=";

        public static readonly String large_than = ">";

        public static readonly String less_than = "<";

        public static readonly String large_equals = ">=";

        public static readonly String less_equals = "<=";

        public static readonly String not_equals = "!=";

        public static readonly String not_equals2 = "<>";

        public static readonly String like = "like";

        public static readonly String not_like = "not like";

        public static readonly String in_ = "in";

        public static readonly String not_in = "not in";

        public static readonly String is_null = "is null";

        public static readonly String is_notnull = "is not null";

        public static readonly String match = "match";

        public static readonly String ftlike = "ftlike";


    }
}
