using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.impl
{
    public class ORMUtil
    {
        public static bool IsDbIgnoreRefBaseData(IDataEntityProperty dp)
        {
            string alias = dp.Alias;
            if (string.IsNullOrEmpty(alias))
                return true;
            return false;
        }
        public static bool IsDbIgnore(IDataEntityProperty dp)
        {
           
            string alias = dp.Alias;
            if (string.IsNullOrEmpty(alias))
                return true;
            return dp.IsDbIgnore;
        }
        public static string GetFullObjNameWithoutRoot(string fullObjName)
        {
            int dot = fullObjName.IndexOf('.');
            if (dot != -1)
                return fullObjName.Substring(dot + 1);
            return "";
        }

        public static string GetParentObjectName(string objName)
        {
            int dot = objName.LastIndexOf('.');
            return (dot == -1) ? "" : objName.Substring(0, dot);
        }

    }
}
