using WYF.Bos.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.Metadata
{
    [Serializable]
    public class SqlParam : ISqlParam
    {

        private string _name;
        private object _OriginalValue;
        private object _value;


        public SqlParam(string name, KDDbType custemDbType, object value)
        {
            this.Name = name;
            this.CustemDbType = custemDbType;
            this._value = value;
            this.Direction = ParameterDirection.Input;
            this.IsCustemDbType = true;
        }

        [Obsolete("该方法即将废弃，请使用SqlParam(string name, KDDbType dbType, object value)")]
        public SqlParam(string name, DbType dbType, object value)
        {
            this.Name = name;
            this.DBType = dbType;
            this.SetValue(value);
            this.Direction = ParameterDirection.Input;
        }

        public SqlParam(string name, KDDbType custemDbType, object value, ParameterDirection direction)
        {
            this.Name = name;
            this.CustemDbType = custemDbType;
            this._value = value;
            this.Direction = direction;
            this.IsCustemDbType = true;
        }

        [Obsolete("该方法即将废弃，请使用SqlParam(string name, KDDbType dbType, object value, ParameterDirection direction)")]
        public SqlParam(string name, DbType dbType, object value, ParameterDirection direction)
        {
            this.Name = name;
            this.DBType = dbType;
            this.SetValue(value);
            this.Direction = direction;
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

        public static SqlParam CreateUdtParamter(string paramterName, DbType dbType, object value)
        {
            switch (dbType)
            {
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                    return new SqlParam(paramterName, KDDbType.udt_inttable, value);

                case DbType.String:
                    return new SqlParam(paramterName, KDDbType.udt_nvarchartable, value);

                case DbType.AnsiString:
                    return new SqlParam(paramterName, KDDbType.udt_varchartable, value);
            }
            return new SqlParam(paramterName, (KDDbType)dbType, value);
        }

        private void SetValue(object value)
        {
            this._OriginalValue = value;
            if (this.DBType == DbType.Binary)
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

        // Properties
        public KDDbType CustemDbType { get; set; }

        [Obsolete("即将废弃，请使用CustemDbType")]
        public DbType DBType { get; set; }

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

        [Obsolete("即将废弃")]
        public string UdtTypeName { get; set; }

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
