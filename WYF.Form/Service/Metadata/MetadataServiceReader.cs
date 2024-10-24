using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Cache;
using WYF.DbEngine;
using WYF.Metadata.Dao;


namespace WYF.Form.Service.Metadata
{
    public class MetadataServiceReader
    {

        public string LoadFormConfig(string number)
        {

            string configStr = QueryFormMeta(number, number, RuntimeMetaType.Config);
            if (string.IsNullOrEmpty(configStr))
            {
                //RebuildRuntimeMeta(number);
                configStr = QueryFormMeta(number, number, RuntimeMetaType.Config);
            }
            return configStr;
        }

        public string LoadFormRuntimeMeta(string number, RuntimeMetaType formMetaType, string key)
        {
            String meta = QueryFormMeta(number, key, formMetaType);
            return meta;
        }

        private string QueryFormMeta(string number, string key, RuntimeMetaType type)
        {
            string val = MetaCacheUtils.GetDistributeCache(number, key, (int)type);
            if (string.IsNullOrEmpty(val))
            {
                val = DoQueryFormMeta(number, key, type);
                // MetaCacheUtils.putDistributeCache(number, key, type, val);
            }
            return val;
        }
        private String DoQueryFormMeta(String number, String ctlKey, RuntimeMetaType type)
        {
            // meta_Form meta = DB.Instance.Queryable<meta_Form>().Where(t => t.FNumber == number && t.FType == (int)type).WhereIF(!string.IsNullOrEmpty(ctlKey), t => t.FKey == ctlKey).First();
            string sql = $"select FDATA from T_META_FORM where FNUMBER = '{number}' and FTYPE = {(int)type}";
            if (!string.IsNullOrEmpty(ctlKey))
            {
                sql = sql + $" and FKey='{ctlKey}'";
            }
            // string sRet= DB.Instance.QuerySqlScalarSync(sql).ToNullString();
            return "";
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
            
            string sRet = DBUtils.ExecuteScalar<string>(new Context(), sql, "");
            return sRet;
            
        }

        public string GetRuntimeFormMetaVersion(string entityName)
        {
          string verison=  ThreadCache.Get<string>("FV." + entityName, () =>
            {
                string redisMetaVer = MetaCacheUtils.GetFormMetaVersion(entityName);
                if (redisMetaVer.IsNullOrEmpty())
                {
                    redisMetaVer = DoQueryFormMeta(entityName, entityName, RuntimeMetaType.Version);
                    if (redisMetaVer.IsNullOrEmpty())
                    {
                        RebuildRuntimeMeta(entityName);
                        redisMetaVer = DoQueryFormMeta(entityName, entityName, RuntimeMetaType.Version);
                    }
                    MetaCacheUtils.SetFormMetaVersion(entityName, redisMetaVer);
                }
                return redisMetaVer;
            });
            return verison;
        }

        private void RebuildRuntimeMeta(string number)
        {
            MetadataDao.RebuildRuntimeMetaByNumber(number);
        }
    }
}
