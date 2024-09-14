using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Clr;
using WYF.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{
    /// <summary>
    /// 定义实体类型的描述
    /// </summary>
    public interface IDataEntityType: IMetadata
    {
        /// <summary>
        /// 创建此实体类型的实例
        /// </summary>
        /// <returns></returns>
        Object CreateInstance();

        Object CreateInstance(bool isQueryObj);


        Object DataEntityReferenceSchema { get; set; }  

        /// <summary>
        /// 返回实体名称带引用属性
        /// </summary>
        /// <returns></returns>
        String getExtendName => Name;
       
        /// <summary>
        /// 返回当前实体的主键属性
        /// </summary>
        /// <returns></returns>
        ISimpleProperty PrimaryKey { get; }
        /// <summary>
        /// 返回当前实体的排序属性集合
        /// </summary>
        /// <returns></returns>
        List<ISimpleProperty> SortProperties { get; }

        /// <summary>
        /// 实体版本
        /// </summary>
        string Version => "";
        /// <summary>
        /// 实体缓存的类型
        /// </summary>
        DataEntityCacheType CacheType { get; }

        /// <summary>
        /// 获取实体快照对象
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        PkSnapshotSet GetPkSnapshot(object dataEntity);

        /// <summary>
        /// 分库后实体对应得数据库路由Key
        /// </summary>
        String DBRouteKey { get; set; }

        /// <summary>
        /// 返回此实体类型特征，可选值包括：Class(缺省)、Abstract、Sealed、Interface
        /// </summary>
        /// <returns></returns>
        DataEntityTypeFlag Flag { get; }


        void SetDirty(object dataEntity, bool newValue);


        /// <summary>
        /// 返回所有的属性集合
        /// </summary>
        /// <returns></returns>
        public DataEntityPropertyCollection Properties { get; }
        /// <summary>
        /// 返回某个实体数据是否已经发生了变更
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        bool IsDirty(Object dataEntity);
        /// <summary>
        /// 根据实体数据返回是否为空
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        bool IsEmpty(Object dataEntity);

        IEnumerable<IDataEntityProperty> GetDirtyProperties(object dataEntity, bool includehasDefault);

        IEnumerable<IDataEntityProperty> GetDirtyProperties(object dataEntity);

        void SetPkSnapshot(object dataEntity, PkSnapshotSet pkSnapshotSet);

        /// <summary>
        /// 设置一个实体是从数据库加载而来，并且清除脏标志，当读取或保存完毕后，调用此方法
        /// </summary>
        /// <param name="dataEntity"></param>
        void SetFromDatabase(Object dataEntity);
        /// <summary>
        /// 设置一个实体是从数据库加载而来，但是不清除脏标志
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="clearDirty"></param>
        void SetFromDatabase(Object dataEntity, bool clearDirty);

        /// <summary>
        /// 检查实体类型是否可修改,用于给属性动态增加，删除属性时的校验，主要用于DynamicPropertyCollection
        /// </summary>
        void CheckUnmodifiable() { }
        /// <summary>
        /// 获取该实体类型对应的父实体类型
        /// </summary>
        /// <returns></returns>
        public IDataEntityType Parent { get; set; }
        /// <summary>
        /// 获取该实体对应的父实体
        /// </summary>
        /// <param name="currentObject"></param>
        /// <returns></returns>
        Object GetParent(Object currentObject);

        HashSet<string> FullIndexFields { get; set; }


        List<IDataEntityProperty> GetJsonSerializerProperties();
    }
}
