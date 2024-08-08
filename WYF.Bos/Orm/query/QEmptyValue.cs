using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    [Serializable]
    public  class QEmptyValue  
    {


    public static  QEmptyValue value = new QEmptyValue();

    public override string ToString()
    {
        return "EMPTY";
    }
}
}
