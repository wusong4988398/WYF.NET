using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WYF.Algo.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WYF.Algo
{
    [Serializable]
    public sealed class RowMeta
    {

        private readonly Field[] fields;
        private readonly StringIntMap nameIndexes;
        private readonly StringIntMap aliasIndexes;
        private readonly StringIntMap ignoreAliasIndexes;
        private int[] dataTypeOrdinals;
        private bool[] nullables;

        public RowMeta(string[] fieldNames, DataType[] dataTypes)
        {
            fields = CreateFields(fieldNames, dataTypes);
            nameIndexes = new StringIntMap(fields.Length);
            aliasIndexes = new StringIntMap(fields.Length);
            ignoreAliasIndexes = new StringIntMap(fields.Length);
            nullables = new bool[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                Field field = fields[i];
                string name = field.Name;
                string alias = field.Alias;
                nameIndexes.Put(name, i);
                if (alias != null)
                {
                    aliasIndexes.Put(alias, i);
                    ignoreAliasIndexes.Put(alias.ToUpper(), i);
                }
                nullables[i] = fields[i].IsNullable;
          
            }
        }

        public RowMeta(params Field[] fields)
        {
            this.fields = fields;
            this.nameIndexes = new StringIntMap(fields.Length);
            this.aliasIndexes = new StringIntMap(fields.Length);
            this.ignoreAliasIndexes = new StringIntMap(fields.Length);
            this.nullables = new bool[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                Field field = fields[i];
                string name = field.Name;
                string alias = field.Alias;

                this.nameIndexes.Put(name, i);
                if (alias != null)
                {
                    this.aliasIndexes.Put(alias, i);
                    this.ignoreAliasIndexes.Put(alias.ToUpper(), i);
                }
                this.nullables[i] = field.IsNullable;
            }
        }

        private static Field[] CreateFields(string[] fieldNames, DataType[] dataTypes)
        {
            //Preconditions.CheckArgument(fieldNames.Length == dataTypes.Length, "fieldNames length must equals to dataTypes, but %d <> %d.", fieldNames.Length, dataTypes.Length);
            Field[] fields = new Field[fieldNames.Length];
            for (int i = 0; i < fieldNames.Length; i++)
                fields[i] = new Field(fieldNames[i], dataTypes[i]);
            return fields;
        }

        public int FieldCount => fields.Length;

        public Field[] Fields => fields;

        public Field GetField(int index)
        {
            return fields[index];
        }

        public Field GetField(string nameOrAlias)
        {
            return GetField(nameOrAlias, true);
        }

        public Field GetField(string nameOrAlias, bool throwException)
        {
            int index = GetFieldIndex(nameOrAlias, throwException);
            if (index > -1)
                return fields[index];
            return null;
        }

        public int GetFieldIndex(string nameOrAlias)
        {
            return GetFieldIndex(nameOrAlias, true);
        }

        public int GetFieldIndex(string nameOrAlias, bool throwException)
        {
            int index = aliasIndexes.Get(nameOrAlias);
            if (index != -1)
                return index;
            index = nameIndexes.Get(nameOrAlias);
            if (index != -1)
            {
                aliasIndexes.Put(nameOrAlias, index);
                return index;
            }
            index = ignoreAliasIndexes.Get(nameOrAlias.ToUpper());
            if (index != -1)
            {
                aliasIndexes.Put(nameOrAlias, index);
                return index;
            }
            if (throwException)
                throw new AlgoException($"field {nameOrAlias} not found.");
            return -1;
        }

        public string GetFieldName(int index)
        {
            return fields[index].Name;
        }

        public string GetFieldAlias(int index)
        {
            return fields[index].Alias;
        }

        public DataType GetFieldDataType(int index)
        {
            return fields[index].DataType;
        }

        public bool IsNullable(int index)
        {
            return nullables[index];
        }

        public Dictionary<string, object> ToMap(IRow row)
        {
            var map = new Dictionary<string, object>();
            ToMap(row, map);
            return map;
        }

        public void ToMap(IRow row, Dictionary<string, object> map)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                object value =row.Get(i);
                value = DataType.ConvertValue(fields[i].DataType, value);
                map[fields[i].Alias] = value;
            }
        }

        public Field[] GetFields(string[] fieldNames)
        {
            Field[] newFields = new Field[fieldNames.Length];
            for (int i = 0; i < newFields.Length; i++)
            {
                Field field = GetField(fieldNames[i]);
                if (field != null)
                    newFields[i] = field.Copy();
            }
            return newFields;
        }

        public static RowMeta FromResultSet(IDataReader rs)
        {
            try
            {
                var fields = new Field[rs.FieldCount];
                for (int i = 0; i < fields.Length; i++)
                {
                    //var sqlType = rs.GetFieldType(i);
                    
                    var name = rs.GetName(i);
                    int sqlType = GetSqlTypeFromClrType(rs.GetFieldType(i)); // 辅助方法
                    fields[i] = new Field(name, DataType.FromSqlType(sqlType), true);
                }
                return new RowMeta(fields);
            }
            catch (Exception e)
            {
                throw new AlgoException(e.Message);
            }
        }


        private static int GetSqlTypeFromClrType(Type clrType)
        {
            switch (Type.GetTypeCode(clrType))
            {
                case TypeCode.Boolean:
                    return 16; // Boolean
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.Int16:
                case TypeCode.UInt32:
                case TypeCode.Int32:
                    return 4; // Integer
                case TypeCode.UInt64:
                case TypeCode.Int64:
                    return -5; // Long
                case TypeCode.Single:
                case TypeCode.Double:
                    return 8; // Double
                case TypeCode.Decimal:
                    return 3; // Decimal
                case TypeCode.DateTime:
                    return 93; // Timestamp
                case TypeCode.String:
                    return 12; // String
                case TypeCode.Object:
                    if (clrType == typeof(DateTime))
                        return 91; // Date
                    else if (clrType == typeof(byte[]))
                        return -2; // Binary
                    break;
                default:
                    throw new NotSupportedException($"Unsupported CLR type: {clrType}");
            }
            throw new NotSupportedException($"Unsupported CLR type: {clrType}");
        }

        public override string ToString()
        {
            return string.Format("RowMeta{0}", string.Join(", ", fields.Select(f => f.ToString()))); 
        }

        public List<DataType> GetTypes()
        {
            return fields.Select(f => f.DataType).ToList();
        }

        public string[] GetFieldNames()
        {
            var names = new string[fields.Length];
            for (int i = 0; i < names.Length; i++)
                names[i] = fields[i].Alias;
            return names;
        }

        public DataType[] GetDataTypes()
        {
            var types = new DataType[fields.Length];
            for (int i = 0; i < types.Length; i++)
                types[i] = fields[i].DataType;
            return types;
        }

        public int[] GetDataTypeOrdinals()
        {
            if (dataTypeOrdinals == null)
            {
                var ret = new int[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                    ret[i] = fields[i].DataType.ordinal;
                dataTypeOrdinals = ret;
            }
            return dataTypeOrdinals;
        }

        public DataType GetDataType(int index)
        {
            return fields[index].DataType;
        }
    }
}
