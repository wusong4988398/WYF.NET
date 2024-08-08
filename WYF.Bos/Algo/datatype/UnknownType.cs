using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.datatype
{
    public class UnknownType : DataType
    {
        public UnknownType():base(9, "Unknow")
        {
           
        }


        public override bool AcceptsType(DataType other)
        {
            return other is UnknownType;
        }

        public override Type GetCsharpType()
        {
            return this.GetType();
    }

        public override int GetFixedSize()
        {
            return 0;
        }

        public override int GetSqlType()
        {
            return 1111;
        }

        public override object Read(BinaryReader stream)
        {
            throw new NotImplementedException();
        }

        public override void Write(object paramObject, BinaryWriter stream)
        {
            throw new NotImplementedException();
        }
    }
}
