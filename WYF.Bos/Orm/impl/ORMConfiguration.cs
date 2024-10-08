﻿using WYF.DataEntity.Metadata;
using WYF.Bos.Entity.query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using WYF.Bos.Entity;

namespace WYF.Bos.Orm.impl
{
    public class ORMConfiguration
    {
        //private static Type _multiLangTextPropType;
        private static Type _entryPropType;
        private static Type _entryEntityType;
        private static Type _subEntryEntityType;
        private static Type _baseDataEntityType;
        private static Type _refEntityType;
        private static IORMEntityInvoker ormEntityInvoker=new ORMEntityInvokerImpl();


        static ORMConfiguration()
        {
            try
            {
                //_multiLangTextPropType = Type.GetType("kd.bos.entity.property.MuliLangTextProp");
                _entryPropType = Type.GetType("kd.bos.entity.property.EntryProp");
                _entryEntityType = Type.GetType("kd.bos.entity.EntryType");
                _subEntryEntityType = Type.GetType("kd.bos.entity.SubEntryType");
                _baseDataEntityType = Type.GetType("kd.bos.entity.BasedataEntityType");
                _refEntityType = Type.GetType("kd.bos.entity.RefEntityType");

                ormEntityInvoker = (IORMEntityInvoker)Activator.CreateInstance(Type.GetType("kd.bos.entity.query.ORMEntityInvokerImpl"));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to init ORMConfiguration: {e.Message}", e);
            }

   
        }

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
                if (dp is ICollectionProperty){
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
                if (type==null)
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
            return _baseDataEntityType.IsAssignableFrom(entityType.GetType());
        }
        public static bool IsEntryProPropertyType(IDataEntityProperty peropertyType)
        {
            return _entryPropType.IsAssignableFrom(peropertyType.GetType());
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
            return (_entryEntityType.IsAssignableFrom(entityType.GetType()) || _subEntryEntityType
              .IsAssignableFrom(entityType.GetType()));
        }
    }
}
