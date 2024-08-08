using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WYF.Bos.algo.datatype;

namespace WYF.Bos.algo
{
    public abstract class DataType
    {
        public readonly int _ordinal;
        protected readonly string _name;

        public static readonly UnknownType UnknownType = new UnknownType();
        protected DataType(int ordinal):this(ordinal, null)
        {
            
        }

        protected DataType(int ordinal, string name)
        {
            this._ordinal = ordinal;
            if (string.IsNullOrEmpty(name))
                name = this.GetType().Name;
            this._name = name;
        }
        public string Name
        {
            get { return this._name; }
        }

        public string Sql
        {
            get { return this._name; }
        }

        public override string ToString()
        {
            return this._name;
        }

        public abstract int GetFixedSize();

        public abstract int GetSqlType();
        public abstract Type GetCsharpType();

        public abstract bool AcceptsType(DataType other);

        public abstract void Write(object paramObject, BinaryWriter stream);

        public abstract object Read(BinaryReader stream);


    }
}
