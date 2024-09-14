using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.dataType
{
    public class AnyType : DataType
    {
        public static readonly AnyType Instance = new AnyType();

        public AnyType() : base(100, "Any")
        {

        }

        public override bool AcceptsType(DataType other)
        {
            return true;
        }

        public override int GetFixedSize()
        {
            return -1;
        }

        public override Type GetJavaType()
        {
            return typeof(object);
        }

        public override int GetSqlType()
        {
            return 1111;
        }

        public override object Read(BinaryReader dataInputStream)
        {


            return dataInputStream.ReadString();

        }

        public override void Write(object obj, BinaryWriter dataOutputStream)
        {
            dataOutputStream.Write(obj.ToString());

        }
    }
}
