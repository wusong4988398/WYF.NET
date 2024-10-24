using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WYF.Cache;
using WYF.DataEntity.Metadata.Dynamicobject;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WYF.Entity
{
    /// <summary>
    /// 实体元数据缓存服务类,用于获取实体元数据的各种元素信息
    /// </summary>
    public class EntityMetadataCache
    {
        private static IEntityMetaDataProvider _provider;
        public EntityMetadataCache()
        {
        }

        public static IEntityMetaDataProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = new EntityMetadataProvider();
                }
                return _provider;
            }
        }
        /// <summary>
        /// 返回实体元数据
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static MainEntityType? GetDataEntityType(string entityName)
        {
            long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            MainEntityType dt = EntityMetadataLocalCache.GetDataEntityType(entityName);
            if (dt==null)
            {
                GetRuntimeMetadataVersion(entityName);
                dt = Provider.GetDataEntityType(entityName);
                MakeUnmodifiable(dt);
                EntityMetadataLocalCache.PutDataEntityType(dt);
                dt.CheckVersionTimeMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            if (startTime - dt.CheckVersionTimeMillis > 900000L)
            {
                if (!GetRuntimeMetadataVersion(entityName).Equals(dt.Version))
                {
                    dt = Provider.GetDataEntityType(entityName);
                    MakeUnmodifiable(dt);
                    EntityMetadataLocalCache.PutDataEntityType(dt);
                }
                dt.CheckVersionTimeMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            }
            return dt;
        }
        /// <summary>
        /// 通过反射机制将 DynamicObjectType 类中的一个私有字段 unmodifiable 设置为 true，从而将 MainEntityType 实例标记为不可修改
        /// 这种做法通常用于确保某些对象在初始化后不能被修改，以保证数据的一致性和安全性。
        /// </summary>
        /// <param name="dt"></param>
        private static void MakeUnmodifiable(MainEntityType dt)
        {
            try
            {
                // 获取类型信息
                Type dynamicObjectType = typeof(DynamicObjectType);

                // 获取字段信息
                FieldInfo field = dynamicObjectType.GetField("unmodifiable", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                {
                    throw new InvalidOperationException("Field 'unmodifiable' not found in DynamicObjectType.");
                }
                // 设置字段可访问性
                field.SetValue(dt, true);
            }
            catch (Exception e)
            {
               
            }
        }

        private static string GetRuntimeMetadataVersion(string entityName)
        {
            string cacheKey = GetThreadCacheKey("ver", entityName);
            // 从缓存中获取或加载版本
            string version = ThreadCache.Get(cacheKey, () =>
            {
                return EntityMetadataCache.Provider.GetRuntimeMetadataVersion(entityName);
            });
            return version;
        }



        private static string GetThreadCacheKey(params string[] keys)
        {
     
            StringBuilder sb = new StringBuilder("EntityMetadata");
            foreach (string key in keys)
            {
                sb.Append('.').Append(key);
            }
            return sb.ToString();
        }


        public static DynamicObjectType GetSubDataEntityType(string entityNumber, ICollection<string> properties)
        {
            return GetDataEntityType(entityNumber).GetSubEntityType(properties);
        }
    }
}
