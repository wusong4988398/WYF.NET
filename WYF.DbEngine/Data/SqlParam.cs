
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Common;

namespace WYF.DbEngine
{
    [Serializable]
    public class SqlParam : ISqlParam
    {
        public const int TypeOther = 1111;
        private string _name;
        private object _OriginalValue;
        private object _value;


        public SqlParam(object value) : this(GetType(value?.GetType()), value) 
        {
          
        }

        public SqlParam(int columnType, object value):this(null, (KDbType)columnType, value)
        {
            
        }

        public SqlParam(string name, object value)
        {
            this.Value = value;
            this.Name = name;
            if (value != null)
            {
                SettingDataType(value.GetType());
            }
        }


  


        public SqlParam(KDbType dbType, object value):this(null,dbType,value)
        {
           
        }

        public SqlParam(string name, KDbType dbType, object value)
        {
            this.Name = name;
            this.KDbType = dbType;
            this._value = value;
            this.Direction = ParameterDirection.Input;
            this.IsCustemDbType = true;
        }
        public SqlParam(string name, object value, KDbType dbType)
        {
            this.Name = name;
            this.KDbType = dbType;
            this._value = value;
            this.Direction = ParameterDirection.Input;
            this.IsCustemDbType = true;
        }


        public SqlParam(string name, KDbType dbType, object value, ParameterDirection direction)
        {
            this.Name = name;
            this.KDbType = dbType;
            this._value = value;
            this.Direction = direction;
            this.IsCustemDbType = true;
        }
     
        private void SettingDataType(Type type)
        {
            if (type == UtilConstants.ByteArrayType)
            {
                this.KDbType = KDbType.Binary;
            }
            else if (type == UtilConstants.GuidType)
            {
                this.KDbType = KDbType.Guid;
            }
            else if (type == UtilConstants.IntType)
            {
                this.KDbType = KDbType.Int32;
            }
            else if (type == UtilConstants.ShortType)
            {
                this.KDbType = KDbType.Int16;
            }
            else if (type == UtilConstants.LongType)
            {
                this.KDbType = KDbType.Int64;
            }
            else if (type == UtilConstants.DateType)
            {
                this.KDbType = KDbType.DateTime;
            }
            else if (type == UtilConstants.DobType)
            {
                this.KDbType = KDbType.Double;
            }
            else if (type == UtilConstants.DecType)
            {
                this.KDbType = KDbType.Decimal;
            }
            else if (type == UtilConstants.ByteType)
            {
                this.KDbType = KDbType.Byte;
            }
            else if (type == UtilConstants.SByteType)
            {
                this.KDbType = KDbType.SByte;
            }
            else if (type == UtilConstants.FloatType)
            {
                this.KDbType = KDbType.Single;
            }
            else if (type == UtilConstants.BoolType)
            {
                this.KDbType = KDbType.Boolean;
            }
            else if (type == UtilConstants.StringType)
            {
                this.KDbType = KDbType.String;
            }
            else if (type == UtilConstants.DateTimeOffsetType)
            {
                this.KDbType = KDbType.DateTimeOffset;
            }
            else if (type == UtilConstants.TimeSpanType)
            {
                this.KDbType = KDbType.Time;
            }
            else if (type?.Name == "Geometry")
            {
                this.KDbType = KDbType.Object;
            }
            else if (type != null && type.IsEnum())
            {
                this.KDbType = KDbType.Int64;
                if (Value != null)
                {
                    this.Value = Convert.ToInt64(Value);
                }
            }
            else if (type == UtilConstants.UIntType)
            {
                this.KDbType = KDbType.UInt32;
            }
            else if (type == UtilConstants.ULongType)
            {
                this.KDbType = KDbType.UInt64;
            }
            else if (type == UtilConstants.UShortType)
            {
                this.KDbType = KDbType.UInt16;
            }
            else if (type == UtilConstants.ShortType)
            {
                this.KDbType = KDbType.Int16;
            }



        }
        private static int GetType(Type clsType)
        {
            return TypeOther;
        }
        public string ConvertParamName(DatabaseType dbtype)
        {
            string name = this.Name;
            switch (dbtype)
            {
                case DatabaseType.Oracle:
                case DatabaseType.Oracle9:
                case DatabaseType.Oracle10:
                    return name.ReplaceFirst("@", "");

                case DatabaseType.MS_SQL_Server:
                    return name;

                case ((DatabaseType)4):
                case DatabaseType.PostgreSQL:
                case DatabaseType.MySQL:
                    return name;
            }
            return name;
        }

        //public OleDbParameter ConvertToOleDbParameter(DatabaseType dbtype, int paraID)
        //{
        //    return new OleDbParameter { ParameterName = paraID.ToString(), DbType = this.DBType, Value = this.Value };
        //}

        public static SqlParam CreateUdtParamter(string paramterName, KDbType dbType, object value)
        {
            switch (dbType)
            {
                case KDbType.Int16:
                case KDbType.Int32:
                case KDbType.Int64:
                    return new SqlParam(paramterName, KDbType.udt_inttable, value);

                case KDbType.String:
                    return new SqlParam(paramterName, KDbType.udt_nvarchartable, value);

                case KDbType.AnsiString:
                    return new SqlParam(paramterName, KDbType.udt_varchartable, value);
            }
            return new SqlParam(paramterName, (KDbType)dbType, value);
        }

        private void SetValue(object value)
        {
            this._OriginalValue = value;
            if (this.KDbType == KDbType.Binary)
            {
                this._value = value;
            }
            else if (value is Array)
            {
                StringBuilder builder = new StringBuilder();
                foreach (object obj2 in (Array)value)
                {
                    builder.Append(",").Append(obj2.ToString());
                }
                this._value = builder.ToString().Substring(1);
            }
            else
            {
                this._value = value;
            }
        }


        public KDbType KDbType { get; set; }

        public ParameterDirection Direction { get; set; }

        public bool IsCustemDbType { get; private set; }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (value.StartsWith("@") || value.StartsWith(":"))
                {
                    this._name = value;
                }
                else
                {
                    this._name = "@" + value;
                }
            }
        }

        public int Size { get; set; }
        public object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this.SetValue(value);
            }
        }
    }
}
