using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.dataType
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
        public override Type GetCSharpType()
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
