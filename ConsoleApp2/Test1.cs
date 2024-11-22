using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WYF.Cache;
using WYF.Data;
using WYF.DataEntity.Entity;
using WYF.DbEngine;
using WYF.Mq.init;
using WYF.OrmEngine.Query;
using WYF.ServiceHelper;
using static IronPython.Modules._ast;

namespace ConsoleApp2
{
    public class Test1
    {
        static Test1()
        {
            MQInit.Init();
        }
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

            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            DynamicObjectCollection dynamicObjects1 = QueryServiceHelper.Query("bos_user", "phone", null, "", 1);//单表查询一条记录
            stopwatch1.Stop();
            TimeSpan executionTime2 = stopwatch1.Elapsed;
            Console.WriteLine($"第二次方法执行耗时：{executionTime2.TotalSeconds:F2} 秒");
        }

        internal static void TestCache()
        {
            IDistributeSessionlessCache cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache("customRegion");
            cache.Put("K", "3");
            Console.WriteLine("执行完毕");
        }

        /// <summary>
        /// 数据库相关测试(事务)
        /// </summary>
        internal static void TestDataBase()
        {



            using (WTransactionScope tran = new WTransactionScope(TransactionScopeOption.Required))
            {
                int i = 0;

                BusinessDataWriter.SaveTest1();
                //SugarBase.Db.Ado.ExecuteCommand("insert into Test1 (FName) values('1')");

                decimal kkk = 1.0M / i;
                SugarBase.Db.Ado.ExecuteCommand("insert into Test1 (FName) values('2')");
   
                tran.Complete();
            }
        }

        /// <summary>
        /// 查询并更新
        /// </summary>
        internal static void TestDataBase2() {
            DynamicObjectCollection dynamicObjects1 = QueryServiceHelper.Query("bos_user", "password,bos_org.name", [new QFilter("id", "=", 2)]);//多表查询
            Console.WriteLine("11");
            DynamicObject[] dynamicObjects = BusinessDataServiceHelper.Load("bos_user", "password,fyzjorgid1", [new QFilter("id", "=", 2)]);
            if (dynamicObjects == null || dynamicObjects.Length == 0)
            {
                throw new Exception("用户不存在！");
            }
            DynamicObject dynamicObject = dynamicObjects[0];
            dynamicObject["password"] = "wsl4988398";
            BusinessDataServiceHelper.Save(dynamicObject.GetDataEntityType(), [dynamicObject]);

        }
    }
}
