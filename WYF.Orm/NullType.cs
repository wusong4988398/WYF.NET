using System.IO;
using System;

namespace WYF.SqlParser
{
    public class NullType:DataType
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

        public override Type GetJavaType()
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