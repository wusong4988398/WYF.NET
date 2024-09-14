using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Common
{
    public static class UtilConstants
    {
        public const string Dot = ".";
        public const char DotChar = '.';
        public const string Space = " ";
        public const char SpaceChar = ' ';
        public const string AssemblyName = "SqlSugar";
        public static string ReplaceKey = "{" + Guid.NewGuid() + "}";
        public const string ReplaceCommaKey = "{112A689B-17A1-4A06-9D27-A39EAB8BC3D5}";

        public static Type UShortType = typeof(ushort);
        public static Type ULongType = typeof(ulong);
        public static Type UIntType = typeof(uint);
        public static Type IntType = typeof(int);
        public static Type LongType = typeof(long);
        public static Type GuidType = typeof(Guid);
        public static Type BoolType = typeof(bool);
        public static Type BoolTypeNull = typeof(bool?);
        public static Type ByteType = typeof(Byte);
        public static Type SByteType = typeof(sbyte);
        public static Type ObjType = typeof(object);
        public static Type DobType = typeof(double);
        public static Type FloatType = typeof(float);
        public static Type ShortType = typeof(short);
        public static Type DecType = typeof(decimal);
        public static Type StringType = typeof(string);
        public static Type DateType = typeof(DateTime);
        public static Type DateTimeOffsetType = typeof(DateTimeOffset);
        public static Type TimeSpanType = typeof(TimeSpan);
        public static Type ByteArrayType = typeof(byte[]);

        public static Type DynamicType = typeof(ExpandoObject);
        public static Type Dicii = typeof(KeyValuePair<int, int>);
        public static Type DicIS = typeof(KeyValuePair<int, string>);
        public static Type DicSi = typeof(KeyValuePair<string, int>);
        public static Type DicSS = typeof(KeyValuePair<string, string>);
        public static Type DicOO = typeof(KeyValuePair<object, object>);
        public static Type DicSo = typeof(KeyValuePair<string, object>);
        public static Type DicArraySS = typeof(Dictionary<string, string>);
        public static Type DicArraySO = typeof(Dictionary<string, object>);






        public static Type[] NumericalTypes = new Type[]
        {
            typeof(int),
            typeof(uint),
            typeof(byte),
            typeof(sbyte),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
        };


        public static string[] DateTypeStringList = new string[]
        {
                "Year",
                "Month",
                "Day",
                "Hour",
                "Second" ,
                "Minute",
                "Millisecond",
                "Date"
        };
    }
}
