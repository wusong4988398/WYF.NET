using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.OrmEngine.Query;
using WYF.ServiceHelper;

namespace ConsoleApp2
{
    public class Test1
    {
        public static void TestMethod1()
        {
            QFilter bizappFilter = new QFilter("id", "=", "2");
            //DynamicObjectCollection dynamicObjects= QueryServiceHelper.Query("bos_user", "id,phone", null, "", -1);

            DynamicObjectCollection dynamicObjects = QueryServiceHelper.Query("bos_user", "id", [bizappFilter]);


            Console.WriteLine("执行完毕");
        }
    }
}
