using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.Bos.DataEntity;
using WYF.OrmEngine.DataEntity;
using WYF.OrmEngine.dataManager;
using WYF.OrmEngine.Drivers;
using WYF.Form.DataEntity;

namespace WYF.OrmEngine
{
    public static class DataManagerUtils
    {
   
        public static bool GetAlwaysReturnArray(this OperateOption option)
        {
            return ((option == null) || option.GetVariableValue<bool>("AlwaysReturnArray", true));
        }

        public static int GetBatchSize(this OperateOption option)
        {

            return 5000;
            //if (option == null)
            //{
            //    return 0x1388;
            //}
            //int variableValue = option.GetVariableValue<int>("BatchSize", 0x1388);
            //if (variableValue <= 0)
            //{
            //    variableValue = 0x1388;
            //}
            //return variableValue;
        }

        public static bool GetBulkCopy(this OperateOption option)
        {
            if (option == null)
            {
                return false;
            }
            return option.GetVariableValue<bool>("BulkCopy", false);
        }

        public static bool GetCacheMetadata(this OperateOption option)
        {
            //return ((option == null) || option.GetVariableValue<bool>("CacheMetadata", true));
            return true;
        }

        //public static IDataManager<DT, ID> GetDataManager<DT, ID>()
        //{
        //    throw new NotImplementedException();
        //}


        public static IDataManager GetDataManager(IDataEntityType dt)
        {
            if (dt == null)
                throw new ORMArgInvalidException("10001","创建一个数据管理器DataManager失败,实体类型IDataEntityType不能为空！");
            IDataManager manager = null;
            manager = new DataManagerImplement();
            manager.DataEntityType = dt;
            //manager.SetDataEntityType(dt);
            return manager;
        }


            public static IDataManager GetDataManager(this IDataEntityType dt, IDbDriver driver, OperateOption option = null)
        {
            if (dt == null)
            {
                throw new ORMArgInvalidException("10001", "创建一个数据管理器DataManager失败,实体类型IDataEntityType不能为空！");
            }
            if (driver == null)
            {
                throw new ORMArgInvalidException("??????", "创建一个数据管理器DataManager失败,数据库驱动driver不能为空！");
            }
            DataManagerImplement implement = new DataManagerImplement();
            if (option != null)
            {
                implement.Option = option;
            }
            implement.DbDriver = driver;
            implement.DataEntityType = dt;
            return implement;
        }

        public static bool GetThrowExceptionWhenNotFind(this OperateOption option)
        {
            return ((option == null) || option.GetVariableValue<bool>("ThrowExceptionWhenNotFind", true));
        }

        public static T[] ToArray<T>(this IEnumerable col)
        {
            if (col == null)
            {
                return null;
            }
            T[] localArray = col as T[];
            if (localArray != null)
            {
                return localArray;
            }
            ICollection is2 = col as ICollection;
            if (is2 != null)
            {
                T[] array = new T[is2.Count];
                is2.CopyTo(array, 0);
                return array;
            }
            ForWriteList<T> list = new ForWriteList<T>();
            list.AddRange(col);
            return list.ToArray();
        }
    }
}
