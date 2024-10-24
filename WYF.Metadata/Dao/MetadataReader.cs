using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WYF.Cache;
using WYF.DbEngine;
using static IronPython.Modules._ast;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WYF.Metadata.Dao
{
    public class MetadataReader
    {
        private static readonly string CACHEKEY_ENTITYID = "entityid";
        private static readonly string CACHEKEY_FORMID = "formid";
        private static readonly string ENTITYDESIGN_TABLE = "T_META_ENTITYDESIGN";
        private static readonly string FORMDESIGN_TABLE = "T_META_FORMDESIGN";
        private static readonly string CACHEKEY_FORMNUMBER = "formnumber";

        private bool isExtend;
        private string localeId;

        public MetadataReader(): this(false)
        {
            
        }
        public MetadataReader(bool isExtend)
        {
            this.localeId = "zh_CN";
            this.isExtend = isExtend;
        }

        public string LoadIdByNumber(string number,  MetaCategory category)
        {
           string str=  ThreadCache.Get<string>(number + "."+ Enum.GetName(typeof(MetaCategory), category) + ".id", () =>
           {
               string cacheId = MetaCacheUtils.GetDistributeCache(number, category == MetaCategory.Form ? MetadataReader.CACHEKEY_FORMID : MetadataReader.CACHEKEY_ENTITYID, 0);
               if (!cacheId.IsNullOrWhiteSpace())
               {
                   return cacheId;
               }
               string sql  = $"select FID from {GetTableName(category)} where FNUMBER='{number}'";
               string cacheId2 = DBUtils.ExecuteScalar<string>(new Context(), sql, "");
               if (!cacheId2.IsNullOrWhiteSpace())
               {
                   MetaCacheUtils.PutDistributeCache(number, category == MetaCategory.Form ? MetadataReader.CACHEKEY_FORMID : MetadataReader.CACHEKEY_ENTITYID, 0, cacheId2);

               }



               return cacheId2;
           }
            );

            return str;
        }

        public string GetNumberById(string id)
        {
           return ThreadCache.Get(id + ".formnumber", () =>
            {
                string number = MetaCacheUtils.GetDistributeCache(id, MetadataReader.CACHEKEY_FORMNUMBER, 0);
                if (number != null)
                {
                    return number;
                }
                string sql = $"select FNumber from {MetadataReader.FORMDESIGN_TABLE} where FId = {id} ";
                number = DBUtils.ExecuteScalar<string>(new Context(), sql, "");
                if (!number.IsNullOrWhiteSpace())
                {
                    MetaCacheUtils.PutDistributeCache(id, MetadataReader.CACHEKEY_FORMNUMBER, 0, number);

                }

                return number;
            });
        }

        private string GetTableName(MetaCategory category)
        {
            switch (category)
            {   
                case MetaCategory.Form:
                    return FORMDESIGN_TABLE;

                case MetaCategory.Entity:
                    return ENTITYDESIGN_TABLE;
                default:
                    return "";
            }
        }

    }
}
