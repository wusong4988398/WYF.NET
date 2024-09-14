using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata.database;

namespace WYF.OrmEngine.Drivers
{
    internal static class DriverHelper
    {
  
        public static string GetColumnNameSql(DbMetadataTable currentTable, DbMetadataColumn column)
        {
            if (string.IsNullOrEmpty(column.TableGroup))
            {
                return ("[" + currentTable.Name + "].[" + column.Name + "]");
            }
            return ("[" + currentTable.Name + "_" + column.TableGroup + "].[" + column.Name + "]");
        }

        public static string GetColumnNameSql(string tableName, string columnName)
        {
            return (GetTableNameSql(tableName) + ".[" + columnName + "]");
        }

        public static bool GetIsArchive(this OperateOption option)
        {
            return option.GetVariableValue<bool>("IsArchive", false);
        }

        public static string GetSelectSQL(this DbMetadataTable currentTable, bool isAccess)
        {
            string tableNameSql = GetTableNameSql(currentTable.Name);
            DbMetadataRelation parentRelation = currentTable.ParentRelation;
            DbMetadataTable table = currentTable;
            while (parentRelation != null)
            {
                DbMetadataTable parentTable = parentRelation.ParentTable;
                tableNameSql = (isAccess ? "(" : null) + tableNameSql + " INNER JOIN " + GetTableNameSql(parentTable.Name) + " ON " + GetColumnNameSql(table.Name, parentRelation.ChildColumn.Name) + " = " + GetColumnNameSql(parentTable.Name, parentTable.PrimaryKey.Name) + (isAccess ? ")" : null);
                table = parentRelation.ParentTable;
                parentRelation = table.ParentRelation;
            }
            List<string> tableGroups = GetTableGroups(currentTable);
            if (tableGroups.Count > 0)
            {
                string str2 = " ON " + GetColumnNameSql(currentTable.Name, currentTable.PrimaryKey.Name) + " = ";
                foreach (string str4 in tableGroups)
                {
                    string name = currentTable.Name + "_" + str4;
                    tableNameSql = (isAccess ? "(" : null) + tableNameSql + " LEFT JOIN " + GetTableNameSql(name) + str2 + GetColumnNameSql(name, currentTable.PrimaryKey.Name) + (isAccess ? ")" : null);
                }
            }
            StringBuilder builder = new StringBuilder(currentTable.Columns.Count * 40);
            builder.Append("SELECT ");
            bool flag = true;
            foreach (DbMetadataColumn column in currentTable.Columns)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Append(",");
                }
                builder.Append(GetColumnNameSql(currentTable, column));
            }
            builder.Append(" FROM ");
            builder.Append(tableNameSql);
            return builder.ToString();
        }

        private static List<string> GetTableGroups(DbMetadataTable currentTable)
        {
            List<string> list = new List<string>();
            foreach (DbMetadataColumn column in currentTable.Columns)
            {
                if (!string.IsNullOrEmpty(column.TableGroup) && !list.Contains(column.TableGroup))
                {
                    list.Add(column.TableGroup);
                }
            }
            return list;
        }

        public static string GetTableNameSql(string name)
        {
            return ("[" + name + "]");
        }
    }
}
