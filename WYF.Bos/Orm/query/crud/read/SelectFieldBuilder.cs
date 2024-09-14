using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Clr;
using WYF.Bos.Orm.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.crud.read
{
    public class SelectFieldBuilder
    {
        private IDataEntityType entityType;
        public SelectFieldBuilder(string entityName, Dictionary<string, IDataEntityType> entityTypeCache)
        {
            this.entityType = ORMConfiguration.InnerGetDataEntityType(entityName, entityTypeCache);
        }

        public string BuildSelectFields(bool loadRefDatabase)
        {
            List<string> list = new List<string>();
            CollectFields("", this.entityType, loadRefDatabase, list, false, false);
            StringBuilder sb = new StringBuilder(list.Count * 10);
            for (int i = 0, n = list.Count; i < n; i++)
            {
                if (i != 0)
                    sb.Append(',');
                sb.Append(list[i]);
            }
            return sb.ToString();
        }
        private void CollectFields(string fullObjName, IDataEntityType entityType, bool loadRefDatabase, List<string> list, bool isLocaleEntity, bool onlyPK)
        {
            if (entityType == null)
                return;
            string prefix = string.IsNullOrEmpty(fullObjName) ? fullObjName : (fullObjName + ".");
            DataEntityPropertyCollection ps = entityType.Properties;
            foreach (IDataEntityProperty dp in ps)
            {
                if (dp is ICollectionProperty){
                    string subFullObjName = prefix + dp.Name;
                    CollectFields(subFullObjName, ((ICollectionProperty)dp).ItemType, loadRefDatabase, list, false, onlyPK);
                    continue;
                }
                if (dp is IComplexProperty){
                    if (ORMUtil.IsDbIgnoreRefBaseData(dp))
                        continue;
                    if (loadRefDatabase)
                    {
                        CollectFields(prefix + dp.Name, ((IComplexProperty)dp).ComplexType, false, list, false, onlyPK);
                        continue;
                    }
                    list.Add(prefix + dp.Name + '.' + ((IComplexProperty)dp).ComplexType.PrimaryKey.Name);
                    continue;
                }
                if (onlyPK)
                {
                    if (dp != entityType.PrimaryKey) continue;

                }
                else
                {
                    if (ORMUtil.IsDbIgnore(dp))
                        continue;
                    string name = dp.Name.ToLower();
                    if (isLocaleEntity && ("pkid"== name || "localeid"== name))
                        continue;
                    if (name.EndsWith("_id") && ps.ContainsKey(name.Substring(0, name.Length - "_id".Length)))
                        continue;

                }
                list.Add(prefix + dp.Name);
            }


        }

    }
}
