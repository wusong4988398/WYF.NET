﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.dataType
{
    public class TimestampType : DataType
    {
        public TimestampType() : base(7, "Timestamp")
        {

        }

        public override bool AcceptsType(DataType other)
        {
            return (other is NullType) || (other is AnyType) || (other is UnknownType) || (other is TimestampType) || (other is DateType);

        }

        public override int GetFixedSize()
        {
            return 8;
        }

        public override Type GetCSharpType()
        {
            return typeof(DateTime);
        }

        public override int GetSqlType()
        {
            return 93;
        }

        public override object Read(BinaryReader dataInputStream)
        {

            long time = dataInputStream.ReadInt64();
            return new DateTime(time * TimeSpan.TicksPerMillisecond, DateTimeKind.Utc).ToLocalTime();

        }

        public override void Write(object obj, BinaryWriter dataOutputStream)
        {
            dataOutputStream.Write(((DateTime)obj).ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond);
        }
    }
}
