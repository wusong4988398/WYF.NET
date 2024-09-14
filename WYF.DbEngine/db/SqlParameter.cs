using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using WYF.Common;

namespace WYF.DbEngine.db
{


    public class SqlParameter : DbParameter
    {

        public SqlParameter(string name, object value)
        {
            this.Value = value;
            this.ParameterName = name;
            if (value != null)
            {
                SettingDataType(value.GetType());
            }
        }

        public SqlParameter(DbType dbType,object value) : this(null, value, dbType)
        {

        }

        public SqlParameter(string name, object value, Type type)
        {
            this.Value = value;
            this.ParameterName = name;
            SettingDataType(type);
        }
        public SqlParameter(string name, object value, Type type, ParameterDirection direction)
        {
            this.Value = value;
            this.ParameterName = name;
            this.Direction = direction;
            SettingDataType(type);
        }
        public SqlParameter(string name, object value, Type type, ParameterDirection direction, int size)
        {
            this.Value = value;
            this.ParameterName = name;
            this.Direction = direction;
            this.Size = size;
            SettingDataType(type);
        }


        public SqlParameter(string name, object value, System.Data.DbType type)
        {
            this.Value = value;
            this.ParameterName = name;
            this.DbType = type;
        }
        public SqlParameter(string name, DataTable value, string SqlServerTypeName)
        {
            this.Value = value;
            this.ParameterName = name;
            this.TypeName = SqlServerTypeName;
        }
        public SqlParameter(string name, object value, System.Data.DbType type, ParameterDirection direction)
        {
            this.Value = value;
            this.ParameterName = name;
            this.Direction = direction;
            this.DbType = type;
        }
        public SqlParameter(string name, object value, System.Data.DbType type, ParameterDirection direction, int size)
        {
            this.Value = value;
            this.ParameterName = name;
            this.Direction = direction;
            this.Size = size;
            this.DbType = type;
        }

        private void SettingDataType(Type type)
        {
            if (type == UtilConstants.ByteArrayType)
            {
                this.DbType = System.Data.DbType.Binary;
            }
            else if (type == UtilConstants.GuidType)
            {
                this.DbType = System.Data.DbType.Guid;
            }
            else if (type == UtilConstants.IntType)
            {
                this.DbType = System.Data.DbType.Int32;
            }
            else if (type == UtilConstants.ShortType)
            {
                this.DbType = System.Data.DbType.Int16;
            }
            else if (type == UtilConstants.LongType)
            {
                this.DbType = System.Data.DbType.Int64;
            }
            else if (type == UtilConstants.DateType)
            {
                this.DbType = System.Data.DbType.DateTime;
            }
            else if (type == UtilConstants.DobType)
            {
                this.DbType = System.Data.DbType.Double;
            }
            else if (type == UtilConstants.DecType)
            {
                this.DbType = System.Data.DbType.Decimal;
            }
            else if (type == UtilConstants.ByteType)
            {
                this.DbType = System.Data.DbType.Byte;
            }
            else if (type == UtilConstants.SByteType)
            {
                this.DbType = System.Data.DbType.SByte;
            }
            else if (type == UtilConstants.FloatType)
            {
                this.DbType = System.Data.DbType.Single;
            }
            else if (type == UtilConstants.BoolType)
            {
                this.DbType = System.Data.DbType.Boolean;
            }
            else if (type == UtilConstants.StringType)
            {
                this.DbType = System.Data.DbType.String;
            }
            else if (type == UtilConstants.DateTimeOffsetType)
            {
                this.DbType = System.Data.DbType.DateTimeOffset;
            }
            else if (type == UtilConstants.TimeSpanType)
            {
                this.DbType = System.Data.DbType.Time;
            }
            else if (type?.Name == "Geometry")
            {
                this.DbType = System.Data.DbType.Object;
            }
            else if (type != null && type.IsEnum())
            {
                this.DbType = System.Data.DbType.Int64;
                if (Value != null)
                {
                    this.Value = Convert.ToInt64(Value);
                }
            }
            else if (type == UtilConstants.UIntType)
            {
                this.DbType = System.Data.DbType.UInt32;
            }
            else if (type == UtilConstants.ULongType)
            {
                this.DbType = System.Data.DbType.UInt64;
            }
            else if (type == UtilConstants.UShortType)
            {
                this.DbType = System.Data.DbType.UInt16;
            }
            else if (type == UtilConstants.ShortType)
            {
                this.DbType = System.Data.DbType.UInt16;
            }
     


        }
        public SqlParameter(string name, object value, bool isOutput)
        {
            this.Value = value;
            this.ParameterName = name;
            if (isOutput)
            {
                this.Direction = ParameterDirection.Output;
            }
        }
        public override System.Data.DbType DbType
        {
            get; set;
        }

        public override ParameterDirection Direction
        {
            get; set;
        }

        public override bool IsNullable
        {
            get; set;
        }

        public override string ParameterName
        {
            get; set;
        }

        public override byte Scale
        {
            get; set;
        }

        public int _Size;

        public override int Size
        {
            get
            {
                if (_Size == 0 && Value != null)
                {
                    var isByteArray = Value.GetType() == UtilConstants.ByteArrayType;
                    if (isByteArray)
                        _Size = -1;
                    else
                    {
                        var length = Value.ToString().Length;
                        _Size = length < 4000 ? 4000 : -1;

                    }
                }
                if (_Size == 0)
                    _Size = 4000;
                return _Size;
            }
            set
            {
                _Size = value;
            }
        }

        public override string SourceColumn
        {
            get; set;
        }

        public override bool SourceColumnNullMapping
        {
            get; set;
        }
        public string UdtTypeName
        {
            get;
            set;
        }

        public override object Value
        {
            get; set;
        }

        public Dictionary<string, object> TempDate
        {
            get; set;
        }


        public override void ResetDbType()
        {
            this.DbType = System.Data.DbType.String;
        }


        public string TypeName { get; set; }
        public bool IsJson { get; set; }
        public bool IsArray { get; set; }
        public object CustomDbType { get; set; }
    }
    //public class SqlParameter
    //{


    //    public const int TypeString = 12;
    //    public const int TypeBoolean = 16;
    //    public const int TypeByte = -6;
    //    public const int TypeShort = 5;
    //    public const int TypeInt = 4;
    //    public const int TypeLong = -5;
    //    public const int TypeDecimal = 3;
    //    public const int TypeTimestamp = 93;
    //    public const int TypeOther = 1111;

    //    public string Name { get; set; }

    //    public int DbType { get; set; } = TypeOther;

    //    public object Value { get; set; }

    //    public SqlParameter()
    //    {
    //    }
    //    public SqlParameter(object value) : this(GetType((value == null) ? null : value.GetType()), value)
    //    {

    //    }

    //    public SqlParameter(Type clsType, object value)
    //        : this(GetType(clsType), value)
    //    {
    //    }

    //    public SqlParameter(int columnType, object value)
    //        : this(null, columnType, value)
    //    {
    //    }

    //    public SqlParameter(string name, int columnType, object value)
    //    {
    //        Name = name;
    //        DbType = columnType;
    //        SetValue(name,columnType, value);
    //        Value = value;
    //    }

    //    private static void SetValue(string name,int columnType, object value)
    //    {
    //        switch (columnType)
    //        {
    //            case -7:
    //            case TypeBoolean:
    //                if (value is string stringValue)
    //                {
    //                    value = ("1" == value || value.ToBool());

    //                }
    //                break;
    //            case TypeByte:
    //                if (value is string byteValue)
    //                {
    //                    value = byte.Parse(byteValue);
    //                }
    //                break;
    //            case TypeShort:
    //                if (value is string shortValue)
    //                {
    //                    value = short.Parse(shortValue);
    //                }
    //                break;
    //            case TypeInt:
    //                if (value is string intValue)
    //                {
    //                    value = int.Parse(intValue);
    //                }
    //                break;
    //            case TypeLong:
    //                if (value is string longValue)
    //                {
    //                    value = long.Parse(longValue);
    //                }
    //                break;
    //            case 6:
    //            case 7:
    //                if (value is string floatValue)
    //                {
    //                    value = float.Parse(floatValue);
    //                }
    //                break;
    //            case 2:
    //            case TypeDecimal:
    //            case 8:
    //                if (value is string decimalValue)
    //                {
    //                    value = double.Parse(decimalValue);
    //                }
    //                break;
    //        }
    //        MatchValueType(name, columnType, value);
    //    }

    //    private static void MatchValueType(string name, int columnType, object value)
    //    {
    //        var validTypes = GetValidTypesForColumnType(columnType);
    //        if (validTypes.Contains(value.GetType()))
    //        {
    //            return;
    //        }

    //        string tipName = name ?? "";
    //        // string msg = $"{tipName}{Resources.Get("bos-dbengine", "SqlParameter_0", "Required parameter type")} {GetTypeNameForColumnType(columnType)} {Resources.Get("bos-dbengine", "SqlParameter_1", ", Actual value passed:")} {value} type={value.GetType()}";

    //        string msg = $"要求参数类型:{columnType},实际传入值：{value}";
    //        var e = new ArgumentException(msg);

    //        throw e;
    //    }

    //    private static Type[] GetValidTypesForColumnType(int columnType)
    //    {
    //        // This method should contain logic to return the valid types based on the columnType.
    //        // For simplicity, we're returning an empty array here.
    //        return Array.Empty<Type>();
    //    }

    //    private static string GetTypeNameForColumnType(int columnType)
    //    {
    //        // This method should return the name of the type based on the columnType.
    //        return "Unknown";
    //    }

    //    private static int GetType(Type clsType)
    //    {
    //        return TypeOther;
    //    }

    //    public override string ToString()
    //    {
    //        return Value?.ToString() ?? base.ToString();
    //    }

    //    public void SetValue(DbCommand command, int paramIndex)
    //    {
    //        command.Parameters[paramIndex].Value = Value;
    //        command.CreateParameter();

    //        ParameterSetter.Set(command, GetValue(), paramIndex, DbType);
    //    }

    //    public  object GetValue()
    //    {
    //        if (Value is bool booleanValue)
    //            return booleanValue ? "1" : "0";
    //        return Value;
    //    }
    //}
}
