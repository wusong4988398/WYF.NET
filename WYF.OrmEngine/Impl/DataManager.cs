using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.database;
using WYF.DataEntity.Metadata;
using WYF.OrmEngine.dataManager;
using WYF.DbEngine.db;
using WYF.OrmEngine.DataEntity;
using WYF.OrmEngine.Drivers;
using WYF.Form.DataEntity;

namespace WYF.OrmEngine.Impl
{
    public class DataManager : IDataManager
    {
        private IDataManager impl;

        public DataManager(IDataEntityType dataEntityType)
            : this(dataEntityType, new DBRoute(dataEntityType.DBRouteKey))
        {
        }

        public DataManager(IDataEntityType dataEntityType, DBRoute dbRoute)
        {
            impl = new DataManagerImplement(dataEntityType, dbRoute);
        }
        public IDataEntityType DataEntityType
        {
            get => impl.DataEntityType;
            set => impl.DataEntityType = value;
        }

        public object Read(object pk)
        {
            return impl.Read(pk);
        }

        public object[] Read(List<object> pks)
        {
            return (object[])impl.Read(pks);
        }

        public object[] Read(object[] pks)
        {
            return impl.Read(pks);
        }

        public object[] Read(ReadWhere where)
        {
            return impl.Read(where);
        }

        //public List<object> Select(string where)
        //{
        //    return impl.Select(where);
        //}

        //public List<object> Select(List<object> pks)
        //{
        //    return impl.Select(pks);
        //}

        public ISaveDataSet GetSaveDataSet(object[] dataEntities, bool includeDefaultValue)
        {
            return impl.GetSaveDataSet(dataEntities, includeDefaultValue);
        }

        public void Save(object dataEntity)
        {
            Save(new object[] { dataEntity });
        }

        public void Save(object[] dataEntities)
        {
            //var updatePks = GetUpdatePks(impl.DataEntityType, dataEntities);
            //impl.Save(dataEntities);
            //var cacheManager = new DataEntityCacheManager(impl.DataEntityType);
            //if (updatePks.Length > 0)
            //{
            //    cacheManager.RemoveByPrimaryKey(updatePks);
            //}
            //else
            //{
            //    cacheManager.RemoveByFilterDt();
            //}
        }

        //public void CommitSnapData(object[] dataEntities)
        //{
        //    impl.CommitSnapData(dataEntities);
        //}

        //public bool Delete(object pk)
        //{
        //    return impl.Delete(pk);
        //}

        //public int Delete(List<object> pks)
        //{
        //    return impl.Delete(pks);
        //}

        //public int Delete(object[] pks)
        //{
        //    int count = impl.Delete(pks);
        //    var cacheManager = new DataEntityCacheManager(impl.DataEntityType);
        //    cacheManager.RemoveByPrimaryKey(pks);
        //    return count;
        //}

        //public bool TryGetTableMapping(string path, out DbMetadataTable table, out string errorMessage)
        //{
        //    return impl.TryGetTableMapping(path, out table, out errorMessage);
        //}

        //public bool TryGetColumnMapping(string path, out DbMetadataColumn column, out string errorMessage)
        //{
        //    return impl.TryGetColumnMapping(path, out column, out errorMessage);
        //}

        public DataEntityTypeMap DataEntityTypeMap
        {
            get => impl.DataEntityTypeMap;
        }
        public bool IsSelectHeadOnly { get; set; }

        public void SetStartRowIndex(int start) { }

        public void SetPageSize(int? pageSize) { }

        public void Update(object[] dataEntities)
        {
            //var updatePks = GetUpdatePks(impl.DataEntityType, dataEntities);
            //impl.Update(dataEntities);
            //var cacheManager = new DataEntityCacheManager(impl.DataEntityType);
            //if (updatePks.Length > 0)
            //{
            //    cacheManager.RemoveByPrimaryKey(updatePks);
            //}
            //else
            //{
            //    cacheManager.RemoveByFilterDt();
            //}
        }

        private object[] GetUpdatePks(IDataEntityType dt, object[] dataEntities)
        {
            var pks = new List<object>();
            for (int rowIndex = 0; rowIndex < dataEntities.Length; rowIndex++)
            {
                PkSnapshotSet snapshotSetTemp = dt.GetPkSnapshot(dataEntities[rowIndex]);
                if (snapshotSetTemp != null)
                {
                    foreach (var item in snapshotSetTemp.Snapshots)
                    {
                        if (item.TableName.Equals(dt.Alias, StringComparison.OrdinalIgnoreCase))
                        {
                            if (item.Oids != null)
                            {
                                pks.AddRange(item.Oids);
                            }
                            break;
                        }
                    }
                }
            }
            return pks.ToArray();
        }

        public void Save(object[] dataEntity, IOrmTransaction ormTransaction = null, OperateOption option = null)
        {
            throw new NotImplementedException();
        }
    }
}

