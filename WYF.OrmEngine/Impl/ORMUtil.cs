using IronPython.Runtime;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

        public static DynamicObjectCollection ToDynamicObjectCollection(IDataSet ds, string entityName)
        {
            
            return ToDynamicObjectCollection(ds.GetEnumerator(), ds.GetRowMeta(), entityName);
        }

        public static DynamicObjectCollection ToDynamicObjectCollection(IEnumerator<IRow> enumerator, RowMeta rowMeta, string entityName)
        {
            DynamicObjectType dt = new DynamicObjectType(entityName);
            int fieldCount = rowMeta.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                Field field = rowMeta.GetField(i);
                String fieldName = field.Alias;
                if (fieldName.IsEmpty() || dt.Properties.ContainsKey(fieldName))
                    fieldName = "Property" + i;
                Type propertyType = field.DataType.GetCSharpType();
                DynamicSimpleProperty property = new DynamicSimpleProperty(fieldName, propertyType, null);
                dt.RegisterSimpleProperty(property);
            }

            DynamicSimpleProperty[] properties = dt.Properties.ToArray<DynamicSimpleProperty>();
            return ToDynamicObjectCollection(enumerator, properties, dt);

        }

        private static DynamicObjectCollection ToDynamicObjectCollection(IEnumerator<IRow> enumerator, DynamicSimpleProperty[] properties, DynamicObjectType dt)
        {
            DynamicObjectCollection ret = new DynamicObjectCollection(dt, null);
            while (enumerator.MoveNext()) {
                DynamicObject obj = new DynamicObject(dt, true);
                obj.BeginInit();
                IRow row = enumerator.Current;
                for (int i = 0; i < properties.Length; i++)
                    properties[i].SetValueFast(obj, row.Get(i));
                obj.EndInit();
                ret.Add(obj);

            }


            return ret;
        }


        //public static DynamicObjectCollection ToDynamicObjectCollection(DataRowCollection rows, RowMeta rowMeta, String entityName)
        //{
        //    DynamicObjectType dt = new DynamicObjectType(entityName);
        //    int fieldCount = rowMeta.FieldCount;
        //    for (int i = 0; i < fieldCount; i++)
        //    {
        //       //DataRow row= rows[i];

        //        Field field = rowMeta.GetField(i);
        //        string fieldName = field.Alias;
        //        if (fieldName.IsEmpty() || dt.Properties.ContainsKey(fieldName))
        //            fieldName = "Property" + i;

        //        Type propertyType =field.DataType.GetJavaType();
        //        DynamicSimpleProperty property = new DynamicSimpleProperty(fieldName, propertyType, null);
        //        dt.RegisterSimpleProperty(property);
        //    }
        //    DynamicSimpleProperty[] properties = (DynamicSimpleProperty[])dt.Properties.ToArray();
        //    return ToDynamicObjectCollection(rows, properties, dt);
        //}

        public static DynamicObjectCollection ToDynamicObjectCollection(DataRowCollection rows, DynamicSimpleProperty[] properties, DynamicObjectType dt)
        {
            DynamicObjectCollection ret = new DynamicObjectCollection(dt, null);
            
            for (int i = 0; i < rows.Count; i++)
            {
                DynamicObject obj = new DynamicObject(dt, true);
                obj.BeginInit();
                DataRow row = rows[i];
                for (int j = 0; j < properties.Length; i++)
                    properties[j].SetValueFast(obj, row[j]);
                obj.EndInit();
                ret.Add(obj);
            }

         
            return ret;
        }

   

        private static int GetFieldCount(IDataReader dataReader)
        {
            throw new NotImplementedException();
        }

     

       

 

    }
}
