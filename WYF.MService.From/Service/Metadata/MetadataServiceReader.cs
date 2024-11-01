using WYF.DbEngine;
using WYF.DbEngine.db;
using WYF.Metadata.Dao;
using static IronPython.Modules.PythonCsvModule;

namespace WYF.Service.Metadata
{
    public class MetadataServiceReader
    {

        public string LoadFormConfig(string number)
        {

            string configStr = QueryFormMeta(number, number, (int)RuntimeMetaType.Config);
            if (string.IsNullOrEmpty(configStr))
            {
                //RebuildRuntimeMeta(number);
                configStr = QueryFormMeta(number, number, (int)RuntimeMetaType.Config);
            }
            return configStr;
        }

        public string LoadFormRuntimeMeta(string number, int formMetaType, string key)
        {
            String meta = QueryFormMeta(number, key, formMetaType);
            return meta;
        }

        private string QueryFormMeta(string number, string key, int type)
        {
            string val = MetaCacheUtils.GetDistributeCache(number, key, type);
            if (string.IsNullOrEmpty(val))
            {
                val = DoQueryFormMeta(number, key, type);
                // MetaCacheUtils.putDistributeCache(number, key, type, val);
            }
            return val;
        }
        private String DoQueryFormMeta(String number, String ctlKey, int type)
        {
            // meta_Form meta = DB.Instance.Queryable<meta_Form>().Where(t => t.FNumber == number && t.FType == (int)type).WhereIF(!string.IsNullOrEmpty(ctlKey), t => t.FKey == ctlKey).First();
            string sql = $"select FDATA from T_META_FORM where FNUMBER = '{number}' and FTYPE = {(int)type}";
            if (!string.IsNullOrEmpty(ctlKey))
            {
                sql = sql + $" and FKey='{ctlKey}'";
            }
            string sRet = DB.Query(DBRoute.meta, sql, [], reader =>
            {
                string  str = String.Empty;
                if (reader.Read())
                {
                    str = reader.GetString(0);
                }
                return str;

            });

            return sRet;
        }

        public string LoadEntityMeta(string entityName)
        {
            string metaStr = this.QueryEntityMetaCache(entityName, entityName, RuntimeMetaType.Entity);

            return metaStr;
        }
        private string QueryEntityMetaCache(string number, string fieldKey, RuntimeMetaType type)
        {
            string val = this.DoQueryMetaData(number, fieldKey, type, "T_META_ENTITY");
            return val;
        }
        private String QueryMeta(string number, string key, RuntimeMetaType type, string tableName)
        {
            string val = this.DoQueryMetaData(number, key, type, tableName);
            return val;
        }
        private string DoQueryMetaData(string number, string ctlKey, RuntimeMetaType type, string table)
        {
            string sql;
            if (!string.IsNullOrEmpty(ctlKey))
            {
                sql = $"select FDATA from {table} where FNUMBER='{number}' and FKEY='{ctlKey}' and FTYPE={(int)type}";
            }
            else
            {
                sql = $"select FDATA from {table} where FNUMBER='{number}' and FTYPE={(int)type}";

            }
            string sRet = DB.Query(DBRoute.meta, sql, [], reader =>
            {
                string str = string.Empty;
                if (reader.Read())
                {
                    str = reader.GetString(0);
                }
                return str;

            });

            return sRet;

        }
    }
}