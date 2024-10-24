using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch stopwatch = new Stopwatch();

            // 开始计时
            stopwatch.Start();
            QFilter bizappFilter = new QFilter("id", "=", "2005892292086334464");
            //DynamicObjectCollection dynamicObjects= QueryServiceHelper.Query("bos_user", "id,phone", null, "", -1);//单表查询
            //DynamicObjectCollection dynamicObjects = QueryServiceHelper.Query("bos_user", "id", [bizappFilter]);//参数查询
            //DynamicObjectCollection dynamicObjects = QueryServiceHelper.Query("bos_user", "id,useropenid,entryentity.dpt.fyzjorgid fyzjorgid", [bizappFilter]);//多表查询
            //DynamicObjectCollection dynamicObjects = QueryServiceHelper.Query("bos_user", "id,useropenid,entryentity.dpt.fyzjorgid fyzjorgid", [bizappFilter],"id asc");//多表查询并排序
            DynamicObjectCollection dynamicObjects = QueryServiceHelper.Query("bos_user", "id,phone", null, "",1);//单表查询一条记录
            //DynamicObjectCollection dynamicObjects1 = QueryServiceHelper.Query("bos_user", "id,phone", null, "", 1);//单表查询一条记录

            Console.WriteLine("执行完毕");
            // 停止计时
            stopwatch.Stop();

            // 输出结果
            TimeSpan executionTime = stopwatch.Elapsed;
            Console.WriteLine($"第一次方法执行耗时：{executionTime.TotalSeconds:F2} 秒");

            //Stopwatch stopwatch1 = new Stopwatch();
            //stopwatch1.Start();
            //DynamicObjectCollection dynamicObjects1 = QueryServiceHelper.Query("bos_user", "phone", null, "", 1);//单表查询一条记录
            //stopwatch1.Stop();
            //TimeSpan executionTime2 = stopwatch1.Elapsed;
            //Console.WriteLine($"第二次方法执行耗时：{executionTime2.TotalSeconds:F2} 秒");
        }

        internal static void TestCache()
        {
            IDistributeSessionlessCache cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache("customRegion");
            cache.Put("K", "3");
            Console.WriteLine("执行完毕");
        }
    }
}
