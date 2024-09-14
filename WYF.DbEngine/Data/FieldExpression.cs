using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DbEngine
{
    internal class FieldExpression
    {
     
        private ComparisonOperators _compOper;
        private string _valFormat;

   
        public FieldExpression(string columnName, KDbType dbType, string fieldName, string tableAliases = "") : this(columnName, dbType, fieldName, ComparisonOperators.Equal, tableAliases)
        {
        }

        public FieldExpression(string columnName, KDbType dbType, string fieldName, ComparisonOperators compOper, string tableAliases = "")
        {
            this._valFormat = "{0}";
            this.GetFieldExpress(columnName, dbType, fieldName, compOper, tableAliases);
        }

        public FieldExpression(string columnName, KDbType dbType, string fieldName, DatabaseType databaseType, string tableAliases = "")
        {
            this._valFormat = "{0}";
            this.DataBaseType = databaseType;
            this.GetFieldExpress(columnName, dbType, fieldName, ComparisonOperators.Equal, tableAliases);
        }

        private void GetFieldExpress(string columnName, KDbType dbType, string fieldName, ComparisonOperators compOper, string tableAliases = "")
        {
            this._compOper = compOper;
            this.ColumnName = columnName;
            this.DbType = dbType;
            this.FieldName = fieldName;
            this.TableAliases = tableAliases;
            if (!tableAliases.IsNullOrWhiteSpace())
            {
                this.FieldOfAliases = string.Format("{0}.{1}", tableAliases, fieldName);
            }
            else
            {
                this.FieldOfAliases = fieldName;
            }
            switch (dbType)
            {
                case KDbType.AnsiString:
                case KDbType.AnsiStringFixedLength:
                    this.SchemaTypeExpression = "varchar(2000)";
                    return;

                case KDbType.Binary:
                    if (this.DataBaseType != DatabaseType.MS_SQL_Server)
                    {
                        if (((this.DataBaseType == DatabaseType.Oracle) || (this.DataBaseType == DatabaseType.Oracle9)) || (this.DataBaseType == DatabaseType.Oracle10))
                        {
                            this.SchemaTypeExpression = "blob";
                        }
                        break;
                    }
                    this.SchemaTypeExpression = "image";
                    return;

                case KDbType.Byte:
                case KDbType.Boolean:
                case KDbType.Currency:
                case KDbType.Object:
                case KDbType.SByte:
                case KDbType.Single:
                case KDbType.Time:
                case KDbType.UInt16:
                case KDbType.UInt32:
                case KDbType.UInt64:
                case KDbType.VarNumeric:
                case (KDbType.String | KDbType.Double):
                case KDbType.Xml:
                    break;

                case KDbType.Date:
                case KDbType.DateTime:
                case KDbType.DateTime2:
                case KDbType.DateTimeOffset:
                    this.SchemaTypeExpression = "datetime";
                    return;

                case KDbType.Decimal:
                case KDbType.Double:
                    this.SchemaTypeExpression = "decimal(23, 10)";
                    return;

                case KDbType.Guid:
                    this.SchemaTypeExpression = "varchar(36)";
                    return;

                case KDbType.Int16:
                case KDbType.Int32:
                case KDbType.Int64:
                    this.SchemaTypeExpression = "bigint";
                    return;

                case KDbType.String:
                case KDbType.StringFixedLength:
                    this.SchemaTypeExpression = "nvarchar(2000)";
                    return;

                default:
                    return;
            }
        }

        public string ToExpr(string tableAliases, string paramPrefix, string valExpr)
        {
            string str = "=";
            switch (this._compOper)
            {
                case ComparisonOperators.Equal:
                    str = "=";
                    break;

                case ComparisonOperators.GreaterThan:
                    str = ">";
                    break;

                case ComparisonOperators.LessThan:
                    str = "<";
                    break;

                case ComparisonOperators.GreaterEqual:
                    str = ">=";
                    break;

                case ComparisonOperators.LessEqual:
                    str = "<=";
                    break;

                case ComparisonOperators.IsNotEqual:
                    str = "<>";
                    break;

                default:
                    str = "=";
                    break;
            }
            if (tableAliases.IsNullOrWhiteSpace())
            {
                return string.Format("{0} {1} {2}{3}", new object[] { this.FieldName, str, paramPrefix, valExpr });
            }
            return string.Format("{0}.{1} {2} {3}{4}", new object[] { tableAliases, this.FieldName, str, paramPrefix, valExpr });
        }

 
        public string ColumnName { get; private set; }

        public DatabaseType DataBaseType { get; set; }

        public KDbType DbType { get; private set; }

        public string FieldName { get; private set; }

        public string FieldOfAliases { get; private set; }

        public string SchemaTypeExpression { get; private set; }

        public string TableAliases { get; private set; }

        public string ValFormat
        {
            get
            {
                return this._valFormat;
            }
            set
            {
                this._valFormat = value;
            }
        }
    }
}
