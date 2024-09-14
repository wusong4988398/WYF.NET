using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DbEngine
{
    public class BatchSqlParam
    {

        private string _createTempTableSQL;
        private Dictionary<string, FieldExpression> dictSetField = new Dictionary<string, FieldExpression>();
        private Dictionary<string, FieldExpression> dictWhereField = new Dictionary<string, FieldExpression>();
        private List<string> listJoin = new List<string>();

        // Methods
        public BatchSqlParam(string tableName, DataTable dt, string createTempTableSQL = "")
        {
            this.TableName = tableName;
            this.data = dt;
            this._createTempTableSQL = createTempTableSQL;
        }

        public void AddJoinExpression(string joinExpr)
        {
            this.listJoin.Add(joinExpr);
        }

        public void AddSetExpression(string columnName, KDbType dbType, string fieldName)
        {
            FieldExpression expression = new FieldExpression(columnName, dbType, fieldName, this.DataBaseType, "");
            this.dictSetField.Add(columnName, expression);
        }

        public void AddSetExpression(string columnName, KDbType dbType, string fieldName, string valFormat)
        {
            FieldExpression expression = new FieldExpression(columnName, dbType, fieldName, "")
            {
                ValFormat = valFormat
            };
            this.dictSetField.Add(columnName, expression);
        }

        public void AddWhereExpression(string columnName, KDbType dbType, string fieldName, string tableAliases = "")
        {
            FieldExpression expression = new FieldExpression(columnName, dbType, fieldName, this.DataBaseType, tableAliases);
            this.dictWhereField.Add(columnName, expression);
        }

        public void AddWhereExpression(string columnName, KDbType dbType, string fieldName, ComparisonOperators compOper, string tableAliases = "")
        {
            FieldExpression expression = new FieldExpression(columnName, dbType, fieldName, compOper, tableAliases);
            this.dictWhereField.Add(columnName, expression);
        }

        public Dictionary<string, string> ColumnTypeExprOfSetField()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (FieldExpression expression in this.dictSetField.Values)
            {
                if (!expression.ColumnName.IsNullOrWhiteSpace())
                {
                    dictionary.Add(expression.ColumnName, expression.SchemaTypeExpression);
                }
            }
            return dictionary;
        }

        public Dictionary<string, string> ColumnTypeExprOfWhereField()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (FieldExpression expression in this.dictWhereField.Values)
            {
                dictionary.Add(expression.ColumnName, expression.SchemaTypeExpression);
            }
            return dictionary;
        }

        public string GetCreateTempTableSQL()
        {
            return this._createTempTableSQL;
        }

        public Dictionary<string, SqlParam> GetEachColumnParam()
        {
            Dictionary<string, SqlParam> dictionary = new Dictionary<string, SqlParam>();
            Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
            foreach (DataColumn column in this.data.Columns)
            {
                object[] objArray = new object[this.data.Rows.Count];
                for (int i = 0; i < this.data.Rows.Count; i++)
                {
                    objArray[i] = this.data.Rows[i][column.ColumnName];
                }
                dictionary2.Add(column.ColumnName, objArray);
            }
            foreach (KeyValuePair<string, FieldExpression> pair in this.dictSetField)
            {
                object valFormat;
                if (!dictionary2.TryGetValue(pair.Key, out valFormat))
                {
                    if ("{0}".Equals(pair.Value.ValFormat))
                    {
                        throw new ArgumentNullException(pair.Key);
                    }
                    valFormat = pair.Value.ValFormat;
                }
                else
                {
                    string name = string.Format("@{0}", pair.Key);
                    SqlParam param = new SqlParam(name, pair.Value.DbType, valFormat);
                    dictionary.Add(pair.Key, param);
                }
            }
            foreach (KeyValuePair<string, FieldExpression> pair2 in this.dictWhereField)
            {
                string introduced15 = string.Format("@{0}", pair2.Key);
                SqlParam param2 = new SqlParam(introduced15, pair2.Value.DbType, dictionary2[pair2.Key]);
                dictionary.Add(pair2.Key, param2);
            }
            return dictionary;
        }

        public List<string> GetJoinExpression()
        {
            return this.listJoin;
        }

        public Dictionary<string, string> GetSetField()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (KeyValuePair<string, FieldExpression> pair in this.dictSetField)
            {
                dictionary.Add(pair.Key, pair.Value.FieldName);
            }
            return dictionary;
        }

        public Dictionary<string, string[]> GetSetFieldExpression()
        {
            Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
            foreach (KeyValuePair<string, FieldExpression> pair in this.dictSetField)
            {
                string[] strArray = new string[2];
                if (string.IsNullOrWhiteSpace(pair.Value.TableAliases))
                {
                    strArray[0] = pair.Value.FieldName;
                }
                else
                {
                    strArray[0] = string.Format("{0}.{1}", pair.Value.TableAliases, pair.Value.FieldName);
                }
                strArray[1] = string.Format(pair.Value.ValFormat, ":" + pair.Value.ColumnName);
                dictionary.Add(pair.Key, strArray);
            }
            return dictionary;
        }

        public Dictionary<string, string> GetSetSqlForSQLServer(string tempTabAliases)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (KeyValuePair<string, FieldExpression> pair in this.dictSetField)
            {
                string str = string.Format("{0}.{1}", tempTabAliases, pair.Key);
                dictionary.Add(pair.Value.FieldName, string.Format(pair.Value.ValFormat, str));
            }
            return dictionary;
        }

        public Dictionary<string, string> GetWhereField(string tempTabAliases = "")
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string tableAliases = this.TableAliases;
            if (string.IsNullOrWhiteSpace(tableAliases))
            {
                tableAliases = this.TableName;
            }
            foreach (KeyValuePair<string, FieldExpression> pair in this.dictWhereField)
            {
                string str3;
                string key = pair.Key;
                if (!string.IsNullOrWhiteSpace(tempTabAliases))
                {
                    key = string.Format("{0}.{1}", tempTabAliases, pair.Key);
                }
                if (!string.IsNullOrWhiteSpace(pair.Value.TableAliases))
                {
                    str3 = pair.Value.ToExpr(pair.Value.TableAliases, "", key);
                }
                else
                {
                    str3 = pair.Value.ToExpr(tableAliases, "", key);
                }
                dictionary.Add(pair.Key, str3);
            }
            return dictionary;
        }

        public Dictionary<string, string> GetWhereFieldExpression()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string tableAliases = this.TableAliases;
            if (string.IsNullOrWhiteSpace(tableAliases))
            {
                tableAliases = this.TableName;
            }
            foreach (KeyValuePair<string, FieldExpression> pair in this.dictWhereField)
            {
                if (string.IsNullOrWhiteSpace(pair.Value.TableAliases))
                {
                    dictionary.Add(pair.Key, pair.Value.ToExpr(tableAliases, ":", pair.Value.ColumnName));
                }
                else
                {
                    dictionary.Add(pair.Key, pair.Value.ToExpr(pair.Value.TableAliases, ":", pair.Value.ColumnName));
                }
            }
            return dictionary;
        }

  
        public DataTable data { get; private set; }

        public DatabaseType DataBaseType { get; set; }

        public string TableAliases { get; set; }

        public string TableName { get; private set; }
    }
}
