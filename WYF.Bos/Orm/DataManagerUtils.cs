using WYF.Bos.Collections.Generic;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.Orm.dataentity;
using WYF.Bos.Orm.DataManager;
using WYF.Bos.Orm.Drivers;
using WYF.Bos.Orm.Exceptions;
using JNPF.Form.DataEntity;
using System.Collections;


namespace WYF.Bos.Orm
{
    public static class DataManagerUtils
    {
        public static bool GetAlwaysReturnArray(this OperateOption option)
        {
            return ((option == null) || option.GetVariableValue<bool>("AlwaysReturnArray", true));
        }

        public static int GetBatchSize(this OperateOption option)
        {
            if (option == null)
            {
                return 0x1388;
            }
            int variableValue = option.GetVariableValue<int>("BatchSize", 0x1388);
            if (variableValue <= 0)
            {
                variableValue = 0x1388;
            }
            return variableValue;
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
            return ((option == null) || option.GetVariableValue<bool>("CacheMetadata", true));
        }

        public static IDataManager<DT, ID> GetDataManager<DT, ID>()
        {
            throw new NotImplementedException();
        }

        public static IDataManager GetDataManager(IDataEntityType dt)
        {
            if (dt == null)
                throw new ArgumentException("创建一个数据管理器DataManager失败,实体类型IDataEntityType不能为空！");

            IDataManager manager = null;
            try
            {
                manager = new DataManagerImplement();
                //manager = (IDataManager)Activator.CreateInstance(Type.GetType("ws.bos.orm.dataentity.DataManagerImplement"));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
       
            manager.SetDataEntityType(dt);
            return manager;
        }
        public static bool GetCacheMetadata()
        {
            return true;
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
