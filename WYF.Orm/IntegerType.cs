using System;
using System.IO;

namespace WYF.SqlParser
{
    public class IntegerType : IntegralType
    {
        public IntegerType() : base(2, "Integer")
        {
        }
        public override int GetFixedSize()
        {
            return 4;
        }
        public override Type GetJavaType()
        {
            return typeof(int);
        }
        public override int GetCompatibleLevel()
        {
            return 1;
        }
        public override void Write(object value, BinaryWriter output)
        {
            output.Write((int)value);
        }
        public override object Read(BinaryReader input)
        {
            return input.ReadInt32();
        }
        public override int GetSqlType()
        {
            return 4;
        }
    }
}