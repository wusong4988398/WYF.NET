using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query
{
    public class QFilterUtil
    {
        public static QParameter GetInQParameter(Object value)
        {
            if (value == null)
                return null;
            Object[] paramss = GetWithoutDuplicateInValues(value);
            int c = paramss.Length;
            if (c > 0)
            {
                String sql = MultiParamsSQL(c);
                return new QParameter(sql, paramss);
            }
            return null;
        }

        public static string MultiParamsSQL(int c)
        {
            StringBuilder s = new StringBuilder(c * 2);
            for (int i = 0; i < c; i++)
            {
                if (i > 0)
                    s.Append(','); s.Append('?');
            }
            return s.ToString();
        }
        public static object[]? GetWithoutDuplicateInValues(object values)
        {
            if (values == null) return null;
            object[]? paramss = null;
            HashSet<object> set = new HashSet<object>();
            IEnumerable enumerable = values as IEnumerable;
            if (enumerable != null)
            {
                foreach (object element in enumerable)
                {
                    set.Add(element);
                }

                paramss = set.ToArray();
            }
            else
            {
                paramss = new Object[] { values };

            }
            return paramss;
        }

    }
}
