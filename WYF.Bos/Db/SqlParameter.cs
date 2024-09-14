using WYF.DataEntity.Entity;
using WYF.DataEntity.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace WYF.Bos.db
{
    public class SqlParameter
    {
        public const int type_string = 12;
        public  const int type_boolean = 16;
        public const int type_byte = -6;
        public const int type_short = 5;
        public const int type_int = 4;
        public const int type_long = -5;
        public const int type_decimal = 3;
        public const int type_timestamp = 93;
        public const int type_other = 1111;
        [SimpleProperty]
        public string Name { get; set; }
        [SimpleProperty]
        public  int DbType { get; set; }
        [SimpleProperty]
        public object Value { get; set; }
   
        public SqlParameter():this(0,null) { }

    

        public SqlParameter(object value):this(GetType((value == null) ? null : value.GetType()), value)
        {
            
        }

        public SqlParameter(Type clsType, object value): this(GetType(clsType), value)
        {
            
        }

        public SqlParameter(int columnType, object value): this(null, columnType, value)
        {
            
        }
        public SqlParameter(string name, int columnType, object value)
        {
            this.Name = name;
            this.DbType = columnType;
            switch (columnType)
            {
                case type_boolean:
                    if (value is string)
                    {
                        value = ("1" == value || value.ToBool());
                    }
                    MatchValueType(name, "bool", value, new Type[] { typeof(bool), typeof(Boolean) });
                    break;
                case type_short:
                    if (value is string)
                    {
                        value = value.ToInt();
                    }
                    MatchValueType(name, "short", value, new Type[] { typeof(short) });
                    break;
                case type_int:
                    if (value is string)
                    {
                        value = value.ToInt();

                    }
                    MatchValueType(name, "int", value, new Type[] { typeof(int) });
                    break;
            }
            this.Value = value;
        }
        private static void MatchValueType(string name, string tag, object value, params Type[] matchClasses)
        {
            if (value!=null)
            {
                Type  type = value.GetType();
                bool matched = false;
                foreach (var matchType in matchClasses)
                {
                    if (type== matchType)
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                {
                    throw new ArgumentException($"要求参数类型:{tag},实际传入值：{value} type={type}");
                }
            }
        }

        private static int GetType(Type clsType)
        {
            return 1111;
        }


    }
}
