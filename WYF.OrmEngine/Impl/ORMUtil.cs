using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;

namespace WYF.OrmEngine.Impl
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
       

        public static DynamicObjectCollection ToDynamicObjectCollection(DataSet dataSet, DynamicObjectType dynamicObjectType)
        {
            var dynamicObjectCollection = new DynamicObjectCollection(dynamicObjectType, null);
            int columnCount = -1;

            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                var dynamicObject = new DynamicObject(dynamicObjectType, true);
                dynamicObject.BeginInit();

                if (columnCount == -1)
                    columnCount = dataRow.ItemArray.Length;

                for (int i = 0; i < columnCount; i++)
                {
                    dynamicObject[i] = dataRow[i];
                }

                dynamicObject.EndInit();
                dynamicObjectCollection.Add(dynamicObject);
            }

            return dynamicObjectCollection;
        }

        public static DynamicObjectCollection ToDynamicObjectCollection(DataSet dataSet, string entityName)
        {
            DynamicObjectType dt = new DynamicObjectType(entityName);
            int fieldCount = dataSet.Tables[0].Rows.Count;
            for (int i = 0; i < fieldCount; i++)
            {

            }


                throw new NotImplementedException();
        }

        
    }
}
