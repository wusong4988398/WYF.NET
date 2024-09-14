using System.IO;
using System;

namespace WYF.SqlParser
{
    public class LongType: IntegralType
    {
        public LongType() : base(3, "Long")
        {
        }
        public override int GetFixedSize()
        {
            return 8;
        }
        public override Type GetJavaType()
        {
      
            return typeof(long);
        }
        public override int GetCompatibleLevel()
        {
            return 2;
        }
        public override void Write(object value, BinaryWriter output)
        {
            output.Write((long)value);
        }
        public override object Read(BinaryReader input)
        {
            return input.ReadInt64();
        }
        public override int GetSqlType()
        {
            return -5;
        }
    }
}