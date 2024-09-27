using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.Impl
{
    public class ORMConfiguration
    {
        private static Type _entryEntityTypeCls;

        private static Type _subEntryEntityTypeCls;

        private static Type _baseDataEntityTypeCls;
        private static Type _entryPropCls;
        private static IORMEntityInvoker ormEntityInvoker = null;
        static ORMConfiguration()
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom("WYF.Entity.dll");
                //_multiLangTextPropType = Type.GetType("kd.bos.entity.property.MuliLangTextProp");
                //_entryPropType = Type.GetType("kd.bos.entity.property.EntryProp");
                //_entryEntityType = Type.GetType("kd.bos.entity.EntryType");
                //_subEntryEntityType = Type.GetType("kd.bos.entity.SubEntryType");
                //_baseDataEntityType = Type.GetType("kd.bos.entity.BasedataEntityType");
                //_refEntityType = Type.GetType("kd.bos.entity.RefEntityType");
           
                Type type = assembly.GetType("WYF.Entity.ORMEntityInvokerImpl",true);

                ormEntityInvoker = (IORMEntityInvoker)Activator.CreateInstance(type);
                _entryEntityTypeCls = assembly.GetType("WYF.Entity.EntryType", true);
                _subEntryEntityTypeCls = assembly.GetType("WYF.Entity.SubEntryType", true);
                _entryPropCls = assembly.GetType("WYF.Entity.Property.EntryProp", true);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to init ORMConfiguration: {e.Message}", e);
            }


        }

    
        //private static IORMEntityInvoker ormEntityInvoker = null;
        public static IDataEntityType InnerGetDataEntityType(string entityNamePath, Dictionary<string, IDataEntityType> entityTypeCache)
        {
            return GetDataEntityType(entityNamePath, entityTypeCache, false);
        }

        private static IDataEntityType? GetDataEntityType(string entityNamePath, Dictionary<string, IDataEntityType> entityTypeCache, bool onlyAtCache)
        {
            if (entityNamePath.IndexOf('.') == -1)
            {
                if (onlyAtCache) return (entityTypeCache == null) ? null : entityTypeCache[entityNamePath.ToLower()];
                return DoGetDataEntityType(entityNamePath, entityTypeCache);

            }
            string[] entityNames = entityNamePath.Split(".");
            IDataEntityType root = DoGetDataEntityType(entityNames[0], entityTypeCache);
            IDataEntityType ret = root;
            for (int i = 1; i < entityNames.Length; i++)
            {
                string entityName = entityNames[i].Trim();
                IDataEntityProperty dp = ret.Properties[entityName];
                if (dp is ICollectionProperty)
                {
                    ret = ((ICollectionProperty)dp).ItemType;
                }
                else if (dp is IComplexProperty)
                {
                    ret = ((IComplexProperty)dp).ComplexType;
                    if (onlyAtCache)
                        return (entityTypeCache == null) ? null : entityTypeCache[ret.Name.ToLower()];
                    ret = DoGetDataEntityType(ret.Name, entityTypeCache);
                }
                else
                {
                    throw new ArgumentException(entityName + " is not an entity.");
                }
            }
            return ret;

        }
        private static IDataEntityType DoGetDataEntityType(string entityName, Dictionary<string, IDataEntityType> entityTypeCache)
        {
            if (entityTypeCache != null)
            {
                string key = entityName.ToLower();
                IDataEntityType type = entityTypeCache[key];
                if (type == null)
                {
                    type = ormEntityInvoker.GetDataEntityType(entityName);
                    entityTypeCache[key] = type;
                }
                return type;
            }

            return ormEntityInvoker.GetDataEntityType(entityName);
        }
        public static bool IsMulBasedata(IDataEntityType entityType)
        {
            IDataEntityType mulbdMainEntityType = entityType.Parent;
            if (mulbdMainEntityType != null)
            {
                IDataEntityProperty fkProperty = (IDataEntityProperty)mulbdMainEntityType.Properties[entityType.Name];
                return IsMulBasedataProp(fkProperty);
            }
            return false;
        }

        public static bool IsMultiLangDataEntityType(IDataEntityType dt)
        {
            if (dt != null)
                return (dt.Name.Equals("locale") || (dt
                  .Alias != null && dt.Alias.EndsWith("_L") &&

                  !dt.Alias.ToUpper().StartsWith("TEMP_")));
            return false;
        }


        public static bool IsMulBasedataProp(IDataEntityProperty propertyType)
        {
            return (ormEntityInvoker.GetMulBasedataPropDataEntityType(propertyType) != null);
        }

        public static bool IsBasedata(IDataEntityType entityType)
        {
            return _baseDataEntityTypeCls.IsAssignableFrom(entityType.GetType());
        }
        public static bool IsEntryProPropertyType(IDataEntityProperty peropertyType)
        {
            return _entryPropCls.IsAssignableFrom(peropertyType.GetType());
        }
        public static IDataEntityType InnerGetBaseDataEntityType(IComplexProperty basedataProperty, Dictionary<string, IDataEntityType> entityTypeCache)
        {
            string entityName = ormEntityInvoker.GetBaseDataEntityName(basedataProperty);
            if (entityName == null)
                throw new Exception($"{basedataProperty.Parent}的属性{basedataProperty.Name}所引用的对象名为空");

            IDataEntityType ret = GetDataEntityType(entityName, entityTypeCache, true);
            if (ret == null)
                if (basedataProperty.ComplexType != null)
                {
                    ret = basedataProperty.ComplexType;
                }
                else
                {
                    ret = GetDataEntityType(entityName, entityTypeCache, false);
                }
            return ret;
        }

        public static bool IsEntryEntityType(IDataEntityType entityType)
        {
            return (_entryEntityTypeCls.IsAssignableFrom(entityType.GetType()) || _subEntryEntityTypeCls
              .IsAssignableFrom(entityType.GetType()));
        }
    }
}
