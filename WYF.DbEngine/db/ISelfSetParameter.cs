using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WYF.DbEngine.db
{
    public interface ISelfSetParameter
    {
        void SetValue(SqlCommand command, int paramIndex);
        object GetValue();
    }
}
