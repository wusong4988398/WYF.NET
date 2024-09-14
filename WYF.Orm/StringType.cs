using System;
using System.IO;

namespace WYF.SqlParser
{
    public class StringType : DataType
    {
        public StringType():base(1, "String")
        {
            
        }

        public override bool AcceptsType(DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override int GetFixedSize()
        {
            throw new NotImplementedException();
        }

        public override Type GetJavaType()
        {
            throw new NotImplementedException();
        }

        public override int GetSqlType()
        {
            throw new NotImplementedException();
        }

        public override object Read(BinaryReader dataInputStream)
        {
            throw new NotImplementedException();
        }

        public override void Write(object obj, BinaryWriter dataOutputStream)
        {
            throw new NotImplementedException();
        }
    }
}