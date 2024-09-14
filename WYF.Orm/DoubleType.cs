using System.IO;
using System;

namespace WYF.SqlParser
{
    public class DoubleType: FractionalType
    {
        public DoubleType() : base(4, "Double")
        {
        }
        public override int GetFixedSize()
        {
            return 8;
        }
        public override Type GetJavaType()
        {

            return typeof(double);
        }
        public override int GetCompatibleLevel()
        {
            return 3;
        }
        public override void Write(object value, BinaryWriter output)
        {
            output.Write((double)value);
        }
        public override object Read(BinaryReader input)
        {
            return input.ReadDouble();
        }
        public override int GetSqlType()
        {
            return 8;
        }
    }
}