using Microsoft.Extensions.Primitives;
using MySqlX.XDevAPI;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Utils;
using WYF.DbEngine;
using WYF.Entity;
using WYF.OrmEngine;
using WYF.OrmEngine.Query;

namespace WYF.Metadata.Dao
{
    public  class MetadataDao
    {
        private static readonly Dictionary<string, object> Locks = new Dictionary<string, object>();

        public static void RebuildRuntimeMetaByNumber(string number)
        {
            RebuildRuntimeMetaByNumber(number, false);
        }

        private static void RebuildRuntimeMetaByNumber(string number, bool diffVer)
        {
            string id = GetIdByNumber(number, MetaCategory.Form);
            if (id != null)
            {
                if (diffVer)
                {
                    RebuildMetaOfDiffVerById(id);
                }
                else
                {
                    RebuildRuntimeMetaById(id);
                }
            }
        }

        public static void RebuildRuntimeMetaById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;
            string tId = id.Replace("/", "aaaaaa");
            // 获取或创建锁对象
            object lockObject;
            if (!Locks.TryGetValue(tId, out lockObject))
            {
                lockObject = new object();
                Locks[tId] = lockObject;
            }
            // 使用锁确保互斥访问
            //lock (lockObject)
            //{
            //    AbstractMetadata formMetadata = ReadRuntimeMeta(id, MetaCategory.Form);
            //    if (formMetadata == null)
            //        throw new Exception($"id为{id}的元数据不存在.");
            //    var metadataWriter = new MetadataWriter(formMetadata.ModelType);
            //    metadataWriter.RebuildRuntimeMeta(new[] { formMetadata });
            //}
        }

  

        private static int RebuildMetaOfDiffVerById(string id)
        {
            return RebuildMetaOfDiffVerById(id, "");
        }

        private static int RebuildMetaOfDiffVerById(string id, string currRuntimeVersion)
        {
            if (id.IsNullOrWhiteSpace()) return 2;

            object lockObj = Locks.Get(id);
            if (lockObj == null)
            {
                lockObj = new Object();
                Locks[id]= lockObj;
            }
            string version = BuildRuntimeVersion(id);
            if (version != null)
            {
                string oldVer = currRuntimeVersion;
                if (currRuntimeVersion.IsNullOrWhiteSpace())
                {
                    string number = GetNumberById(id);
                    List<string> numbers = new List<string>();
                    numbers.Add(number);
                    IDictionary<string, string> map = GetRuntimeMetaVersion(numbers);
                    oldVer = map.GetOrDefault(number);
                }
                if (!oldVer.IsNullOrEmpty()&& CompareRuntimeVersion(version, oldVer))
                    return 3;
            }
            RebuildRuntimeMetaById(id);
            return 1;

        }



        public static bool CompareRuntimeVersion(string ver1, string ver2)
        {
            int index1 = ver1.LastIndexOf('.');
            string ver1Core = ver1.Substring(BOSRuntime.Version.Length + 1, index1 - (BOSRuntime.Version.Length + 1));

            int index2 = ver2.LastIndexOf('.');
            if (index2 < BOSRuntime.Version.Length)
                return false;
            string ver2Core = ver2.Substring(BOSRuntime.Version.Length + 1, index2 - (BOSRuntime.Version.Length + 1));

            if (ver1Core.IsNumericString() && ver2Core.IsNumericString())
                return Convert.ToInt64(ver1Core) <= Convert.ToInt64(ver2Core);

            if (ver1Core.IsNumericString() && !ver2Core.IsNumericString())
                return false;
            return string.Compare(ver1Core, ver2Core, StringComparison.Ordinal) < 0;
        }

        private static IDictionary<string, string> GetRuntimeMetaVersion(List<string> numbers)
        {
            if (numbers == null || numbers.Count == 0) return new Dictionary<string, string>();
            IDictionary<string, string> verMap = new Dictionary<string, string>();
            StringBuilder numberIns = new StringBuilder();
            int listsize = numbers.Count;
            numberIns.Append("'").Append(numbers[0]).Append("'");
            for (int i = 1; i < listsize; i++)
                numberIns.Append(", '").Append(numbers[i]).Append("'");
            //IDataEntityType runtimeFormMetaType = OrmUtils.GetDataEntityType(typeof(RuntimeFormMeta));
            //ORM orm = ORM.Create();
            
            //orm.SetDataEntityType(runtimeFormMetaType.Name, runtimeFormMetaType);
            //QFilter[] qfilters = new QFilter[2];
            //qfilters[0] = new QFilter("number", "in", numbers);
            //qfilters[1] = new QFilter("type", "=", 10);
 
            //using (IDataSet ds = orm.QueryDataSet("MetadataDao.getRuntimeMetaVersion", runtimeFormMetaType.Name, "id,number,data", qfilters))
            //{
            //    foreach (var row in ds)
            //    {
            //        verMap[row.GetString(1)] = row.GetString(2);
            //    }
            //}
            return verMap;
        }

        public static string GetNumberById(string id)
        {
            MetadataReader reader = new MetadataReader();
            return reader.GetNumberById(id);
        }
        private static string BuildRuntimeVersion(string masterId)
        {
            //IDataEntityType dt = OrmUtils.GetDataEntityType(typeof(DesignFormMeta));
            //List<Object[]> list = new List<object[]>();
            //string sql = $"SELECT FID, FISV, FVERSION, FTYPE from {dt.Alias} WHERE FID = {masterId} OR (FMasterId = {masterId} and FType = '2') ";
            //using (IDataReader reader = DBUtils.ExecuteReader(new Context(), sql, new object[] { }))
            //{
            //    while (reader.Read())
            //    {
            //        string id = reader.GetString(reader.GetOrdinal("FID"));
            //        string isv = reader.GetString(reader.GetOrdinal("FISV"));
            //        long version = reader.GetInt64(reader.GetOrdinal("FVERSION"));
            //        string type = reader.GetString(reader.GetOrdinal("FTYPE"));
            //        object[] v = new object[4];
            //        v[0] = id;
            //        v[1] = isv;
            //        v[2] = version;
            //        v[3] = type;
            //        list.Add(v);
            //    }
            //}

            //long totalVer = 0L;
            //foreach (object[] v in list)
            //{
            //    string id = (string)v[0];
            //    if (masterId.Equals(id) &&"2".Equals(v[3])) return null;
            //    long ver = ((long)v[2]);
            //    totalVer += ver;
            //}


            //string dateFormat = "yyyy-MM-dd HH:mm:ss:fff";
            //DateTime now = DateTime.UtcNow;
            //string formattedDate = now.ToString(dateFormat, CultureInfo.InvariantCulture);
            //// 获取版本号和totalVer

            //// 拼接字符串
            //return $"{BOSRuntime.Version}.{totalVer}.{formattedDate}";

            return "";

        }

        public static string GetIdByNumber(string id, MetaCategory category)
        {
            MetadataReader reader = new MetadataReader();
            return reader.LoadIdByNumber(id, category);
        }

    }
}
