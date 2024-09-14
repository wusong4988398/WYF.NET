using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;
using WYF.DbEngine.db;
using WYF.OrmEngine.Impl;

namespace WYF.OrmEngine.Query.Fulltext
{
    public class QFullTextQuery
    {
        private Dictionary<String, IDataEntityType> entityTypeCache;
        private static Type _entryProp;
        private static bool enableFT = false;
        private QFullTextQuery(Dictionary<string, IDataEntityType> entityTypeCache)
        {
            this.entityTypeCache = entityTypeCache;
        }
        public static QFullTextQuery Db(Dictionary<string, IDataEntityType> entityTypeCache)
        {
            return new QFullTextQuery(entityTypeCache);
        }
        public static bool IsFullTextEnable()
        {
            return enableFT;
        }
        private string GetField(IDataEntityProperty dp)
        {
            if (_entryProp.IsAssignableFrom(dp.GetType()))
                return dp.Parent.PrimaryKey.Alias;
            return dp.Alias;
        }
        private string GetTable(IDataEntityProperty dp)
        {
            string table = dp.Parent.Alias;
            string tableGroup = dp.TableGroup;
            if (tableGroup != null && tableGroup.Length > 0)
                table = table + '_' + tableGroup;
            return table;
        }

        public Object[] QueryPropertyValueByPKs(string entityName, string propertyName, object[] values)
        {
            if (values == null || values.Length == 0)
                return new Object[0];
            IDataEntityType dt = ORMConfiguration.InnerGetDataEntityType(entityName, this.entityTypeCache);
            IDataEntityType billRootDT = dt;
            while (ORMConfiguration.IsEntryEntityType(billRootDT))
                billRootDT = billRootDT.Parent;
            DBRoute dbRoute = DBRoute.Of(billRootDT.DBRouteKey);
            IDataEntityProperty dp = (IDataEntityProperty)dt.Properties[propertyName];
            string field = GetField(dp);
            StringBuilder sql = new StringBuilder(128 + values.Length * 2);
            sql.Append("/*ORM*/ ").Append("/*dialect*/").Append("SELECT ").Append(field)
              .Append(" FROM ").Append(GetTable(dp)).Append(" WHERE ").Append(dt.PrimaryKey.Alias)
              .Append(" IN (");
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                    sql.Append(',');
                sql.Append('?');
            }
            sql.Append(") ");
            //return (Object[])DB.query(dbRoute, sql.ToString(), values, rs->parseIds(rs, (dp.PropertyType != typeof(String))));
            return null;
        }
    }
}
