using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Cache;
using WYF.DataEntity.Entity;
using WYF.OrmEngine.Query;
using WYF.ServiceHelper;

namespace ConsoleApp2
{
    public class Test1
    {
        public static void TestMethod1()
        {
            QFilter bizappFilter = new QFilter("id", "=", "2005892292086334464");
            //DynamicObjectCollection dynamicObjects= QueryServiceHelper.Query("bos_user", "id,phone", null, "", -1);//单表查询
            //DynamicObjectCollection dynamicObjects = QueryServiceHelper.Query("bos_user", "id", [bizappFilter]);//参数查询
            //DynamicObjectCollection dynamicObjects = QueryServiceHelper.Query("bos_user", "id,useropenid,entryentity.dpt.fyzjorgid fyzjorgid", [bizappFilter]);//多表查询
            //DynamicObjectCollection dynamicObjects = QueryServiceHelper.Query("bos_user", "id,useropenid,entryentity.dpt.fyzjorgid fyzjorgid", [bizappFilter],"id asc");//多表查询并排序
            DynamicObjectCollection dynamicObjects = QueryServiceHelper.Query("bos_user", "id,phone", null, "",1);//单表查询一条记录
            Console.WriteLine("执行完毕");
        }

        internal static void TestCache()
        {
            IDistributeSessionlessCache cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache("customRegion");
            cache.Put("K", "3");
            Console.WriteLine("执行完毕");
        }
    }
}
