using System.IO;
using System;
using static IronPython.Modules.PythonCsvModule;

namespace WYF.SqlParser
{
    public class UnknownType:DataType
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