using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.dataType
{
    public class NullType : DataType
    {
        public NullType() : base(8, "Null")
        {

        }

        public override bool AcceptsType(DataType other)
        {
            return true;

        }

        public override int GetFixedSize()
        {
            return 4;
        }

        public override Type GetCSharpType()
        {
            return typeof(object);
        }

        public override int GetSqlType()
        {
            return 0;
        }

        public override object Read(BinaryReader dataInputStream)
        {

            return null;

        }

        public override void Write(object obj, BinaryWriter dataOutputStream)
        {

        }
    }
}
