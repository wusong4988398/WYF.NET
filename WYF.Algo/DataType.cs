using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.dataType;

namespace WYF.Algo
{
    [Serializable]
    public abstract class DataType
    {
        public const int BooleanTypeOrdinal = 0;
        public const int StringTypeOrdinal = 1;
        public const int IntegerTypeOrdinal = 2;
        public const int LongTypeOrdinal = 3;
        public const int DoubleTypeOrdinal = 4;
        public const int BigDecimalTypeOrdinal = 5;
        public const int DateTypeOrdinal = 6;
        public const int TimestampTypeOrdinal = 7;
        public const int NullTypeOrdinal = 8;
        public const int UnknownTypeOrdinal = 9;
        public const int AnyTypeOrdinal = 100;
        public const int NumericTypeOrdinal = 200;

        public static readonly BooleanType BooleanType = new BooleanType();
        public static readonly StringType StringType = new StringType();
        public static readonly IntegerType IntegerType = new IntegerType();
        public static readonly LongType LongType = new LongType();
        public static readonly DoubleType DoubleType = new DoubleType();
        public static readonly BigDecimalType BigDecimalType = new BigDecimalType();
        public static readonly DateType DateType = new DateType();
        public static readonly TimestampType TimestampType = new TimestampType();
        public static readonly NullType NullType = new NullType();
        public static readonly UnknownType UnknownType = new UnknownType();
        public static readonly AnyType AnyType = AnyType.Instance;

        protected readonly string name;
        public readonly int ordinal;

        protected DataType(int ordinal, string name = null)
        {
            this.ordinal = ordinal;
            this.name = name ?? GetType().Name;
        }


        public abstract int GetFixedSize();
        public abstract int GetSqlType();
        public abstract Type GetCSharpType();
        public abstract bool AcceptsType(DataType dataType);
        public abstract void Write(object obj, BinaryWriter dataOutputStream);
        public abstract object Read(BinaryReader dataInputStream);


        public static BigDecimalType CreateBigDecimalType(int precision, int scale = 0)
        {
            return BigDecimalType;
        }

        public string GetName() => name;
        public string GetSql() => name;
        public override string ToString() => name;

        public virtual int GuessHeapSize(object value) => 0;

        public object ConvertValue(object value) => ConvertValue(this, value);

        public static object ConvertValue(DataType dataType, object value)
        {
            if (value == null) return null;

            try
            {
                switch (dataType.ordinal)
                {
                    case BooleanTypeOrdinal:
                        return Convert.ToBoolean(value);
                    case StringTypeOrdinal:
                        return value.ToString();
                    case IntegerTypeOrdinal:
                        return Convert.ToInt32(value);
                    case LongTypeOrdinal:
                        return Convert.ToInt64(value);
                    case DoubleTypeOrdinal:
                        return Convert.ToDouble(value);
                    case BigDecimalTypeOrdinal:
                        return Convert.ToDecimal(value);
                    case DateTypeOrdinal:
                        return Convert.ToDateTime(value);
                    case TimestampTypeOrdinal:
                        return Convert.ToDateTime(value);
                    case NumericTypeOrdinal:
                        return Convert.ToDecimal(value);
                    default:
                        return value;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot convert {value} to {dataType.GetName()}", e);
            }
        }

        public static DateTime? ToDate(string v)
        {
            if (string.IsNullOrEmpty(v)) return null;

            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd" };
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(v, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
            }
            return null;
        }

        public virtual bool IsAbstract() => false;

        public static DataType FromSqlType(int sqlType)
        {
            switch (sqlType)
            {
                case -15:
                case -9:
                case -1:
                case 1:
                case 12:
                    return StringType;
                case -6:
                case 4:
                case 5:
                    return IntegerType;
                case -5:
                    return LongType;
                case 0:
                    return NullType;
                case 2:
                case 3:
                case 7:
                    return BigDecimalType;
                case 6:
                case 8:
                    return DoubleType;
                case 16:
                    return BooleanType;
                case 91:
                    return DateType;
                case 93:
                    return TimestampType;
                default:
                    throw new NotSupportedException($"Unsupported SQL type: {sqlType}");
            }
        }

        public static int ToSqlType(DataType dataType)
        {
            switch (dataType.ordinal)
            {
                case BooleanTypeOrdinal:
                    return (int)SqlDbType.Bit;
                case StringTypeOrdinal:
                    return (int)SqlDbType.VarChar;
                case IntegerTypeOrdinal:
                    return (int)SqlDbType.Int;
                case LongTypeOrdinal:
                    return (int)SqlDbType.BigInt;
                case DoubleTypeOrdinal:
                    return (int)SqlDbType.Float;
                case BigDecimalTypeOrdinal:
                    return (int)SqlDbType.Decimal;
                case DateTypeOrdinal:
                    return (int)SqlDbType.Date;
                case TimestampTypeOrdinal:
                    return (int)SqlDbType.DateTime;
                default:
                    throw new NotSupportedException($"Not supported data type: {dataType.GetName()}");
            }
        }

        public override bool Equals(object obj)
        {
            return obj is DataType other && other.ordinal == this.ordinal;
        }

        public override int GetHashCode() => this.ordinal;
    }
}
