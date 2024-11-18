using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Clr;
using WYF.DbEngine;
using WYF.DbEngine.db;
using WYF.Form.DataEntity;
using WYF.OrmEngine;
using WYF.OrmEngine.dataManager;
using WYF.OrmEngine.Drivers;
using WYF.OrmEngine.sequence;

namespace WYF.Data
{
    public  class BusinessDataWriter
    {

        public static object[] Save(IDataEntityType type,object[] dataEntities, OperateOption option = null)
        {
            return Save(type,dataEntities, null, option);
        }
        public static void SaveTest1()
        {
            using (WTransactionScope tran2 = new WTransactionScope(TransactionScopeOption.Required))
            {
                SugarBase.Db.Ado.ExecuteCommand("insert into Test1 (FName) values('3')");
                tran2.Complete();
            }
           

        }
        public static void SaveTest2()
        {
            SugarBase.Db.Ado.ExecuteCommand("insert into Test1 (FName) values('2')");

        }
        public static object[] Save(IDataEntityType type, object[] dataEntities, IOrmTransaction tran, OperateOption option)
        {
            if (dataEntities == null)
                throw new ArgumentNullException(nameof(dataEntities));
            if (dataEntities.Length == 0)
                return dataEntities;
            if (string.IsNullOrWhiteSpace(type.Alias))
                throw new Exception($"实体{type.Name}物理表名为空，无法保存数据入库");

            var dbRoute = new DBRoute(type.DBRouteKey);
            var seqReader = new SequenceReader(dbRoute);
            seqReader.AutoSetPrimaryKey(dataEntities, type);
            var updatePks = GetUpdatePks(type, dataEntities);
            IDbDriver driver = new OLEDbDriver(new Context());

            var dataManager = DataManagerUtils.GetDataManager(type, driver);
            dataManager.Save(dataEntities,tran);
            var cacheManager = new DataEntityCacheManager(type);
            if (updatePks.Length > 0)
            {
                cacheManager.RemoveByPrimaryKey(updatePks);
            }
            else
            {
                cacheManager.RemoveByFilterDt();
            }
            return dataEntities;
        }


        /// <summary>
        /// 获取更新的主键值。
        /// </summary>
        /// <param name="dt">数据实体类型</param>
        /// <param name="dataEntities">数据实体数组</param>
        /// <returns>包含主键值的数组</returns>
        private static object[] GetUpdatePks(IDataEntityType dt, object[] dataEntities)
        {
            List<object> pks = new List<object>();

            for (int rowIndex = 0; rowIndex < dataEntities.Length; rowIndex++)
            {
                PkSnapshotSet snapshotSetTemp = dt.GetPkSnapshot(dataEntities[rowIndex]);
                if (snapshotSetTemp != null)
                {
                    foreach (PkSnapshot item in snapshotSetTemp.Snapshots)
                    {
                        if (item.TableName.Equals(dt.Alias, StringComparison.OrdinalIgnoreCase))
                        {
                            if (item.Oids != null)
                            {
                                foreach (object id in item.Oids)
                                {
                                    pks.Add(id);
                                }
                            }
                            break;
                        }
                    }
                }
            }

            return pks.ToArray();
        }

    }
}
