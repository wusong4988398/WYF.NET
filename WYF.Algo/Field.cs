using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo
{
    [Serializable]
    public sealed class Field
    {


        public string Name { get; private set; }
        public string Alias { get; private set; }
        public DataType DataType { get; private set; }
        public bool IsNullable { get; private set; }

        public Field(string name, DataType dataType)
            : this(name, name, dataType, false) { }

        public Field(string name, DataType dataType, bool nullable)
            : this(name, name, dataType, nullable) { }

        public Field(string name, string alias, DataType dataType, bool nullable)
        {
            Name = name;
            Alias = string.IsNullOrEmpty(alias) ? name : alias;
            DataType = dataType;
            IsNullable = nullable;
        }

        public Field Copy()
        {
            return new Field(Name, Alias, DataType, IsNullable);
        }

        public Field DeriveAlias(string alias)
        {
            return new Field(Name, alias, DataType, IsNullable);
        }

        public Field DeriveName(string name)
        {
            return new Field(name, name, DataType, IsNullable);
        }

        public Field Derive(string name, string alias)
        {
            return new Field(name, alias, DataType, IsNullable);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Field))
                return false;

            var other = (Field)obj;
            return (Alias ?? Name) == (other.Alias ?? other.Name) && DataType == other.DataType;
        }

        public override int GetHashCode()
        {
            unchecked // Allow arithmetic overflow, just wrap around
            {
                int hash = 17;
                hash = hash * 23 + (Alias ?? Name).GetHashCode();
                hash = hash * 23 + DataType.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(Name);
            if (!string.IsNullOrEmpty(Alias) && !Alias.Equals(Name))
            {
                sb.Append(" as ").Append(Alias);
            }
            return sb.ToString();
        }
    }
}
