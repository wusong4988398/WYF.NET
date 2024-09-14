using WYF.DataEntity.Metadata;
using WYF.Bos.Orm.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query
{
    public class WithEntityEntryDistinctable : IDistinctable
    {
        public static WithEntityEntryDistinctable Instance = new WithEntityEntryDistinctable();

   
        public bool Distinct(IDataEntityType entityType, Dictionary<string, bool> joinEntitySelectFieldMap)
        {
            string entityName = entityType.Name;
            int len = entityName.Length;
            foreach (KeyValuePair<string, bool> entry in joinEntitySelectFieldMap)
            {
                string entryFullName = entry.Key;
                if (entryFullName.Length <= len)
                    continue;
                string entryName = entryFullName.Substring(len + 1);
                if (!entryName.Contains("."))
                {
                    IDataEntityProperty dp = entityType.Properties[entryName];
                    if (dp is ICollectionProperty)
                    {
                        IDataEntityType type = ((ICollectionProperty)dp).ItemType;
                        if (ORMConfiguration.IsEntryEntityType(type))
                            if (!entry.Value)
                                return true;
                    }
                }
            }
            return false;
        }
    }
}
