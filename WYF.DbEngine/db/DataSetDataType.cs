
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo;


namespace WYF.DbEngine
{
    public static class DataSetDataType
    {
        private static readonly Dictionary<Type, DataType> baseDataTypeMap = new Dictionary<Type, DataType>();

        static DataSetDataType()
        {
            // 初始化类型映射
            baseDataTypeMap[typeof(object)] = DataType.StringType;
            baseDataTypeMap[typeof(bool)] = DataType.BooleanType;
            baseDataTypeMap[typeof(Boolean)] = DataType.BooleanType;
            baseDataTypeMap[typeof(string)] = DataType.StringType;
            baseDataTypeMap[typeof(int)] = DataType.IntegerType;
            baseDataTypeMap[typeof(Int32)] = DataType.IntegerType;
            baseDataTypeMap[typeof(long)] = DataType.LongType;
            baseDataTypeMap[typeof(Int64)] = DataType.LongType;
            baseDataTypeMap[typeof(double)] = DataType.DoubleType;
            baseDataTypeMap[typeof(Double)] = DataType.DoubleType;
    
            baseDataTypeMap[typeof(DateTime)] = DataType.TimestampType;
            //baseDataTypeMap[typeof(Timestamp)] = DataType.TimestampType;
        }

        public static DataType GetDataType(Type cls)
        {
            while (true)
            {
                if (baseDataTypeMap.TryGetValue(cls, out var dt))
                {
                    return dt;
                }
                cls = cls.BaseType;
                if (cls == null)
                {
                    return DataType.StringType;
                }
            }
        }
    }
}
