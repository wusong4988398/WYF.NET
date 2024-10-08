using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.dataType
{
    public class BooleanType : DataType
    {
        public BooleanType() : base(0, "Boolean")
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

        public override Type GetCSharpType()
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
