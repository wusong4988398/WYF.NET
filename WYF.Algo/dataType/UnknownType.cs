using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.dataType
{
    public class UnknownType : DataType
    {
        public UnknownType() : base(9, "Unknow")
        {

        }

        public override bool AcceptsType(DataType other)
        {
            return other is UnknownType;


        }

        public override int GetFixedSize()
        {
            return 0;
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
