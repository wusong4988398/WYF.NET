﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm
{
    public enum KDDbType
    {
        AnsiString = 0,
        AnsiStringFixedLength = 0x16,
        Binary = 1,
        Boolean = 3,
        Byte = 2,
        Currency = 4,
        Date = 5,
        DateTime = 6,
        DateTime2 = 0x1a,
        DateTimeOffset = 0x1b,
        Decimal = 7,
        Double = 8,
        Guid = 9,
        Int16 = 10,
        Int32 = 11,
        Int64 = 12,
        Object = 13,
        RefCursor = 0x79,
        SByte = 14,
        Single = 15,
        String = 0x10,
        StringFixedLength = 0x17,
        Time = 0x11,
        udt_inttable = 0xa1,
        udt_nvarchartable = 0xa3,
        udt_varchartable = 0xa2,
        UInt16 = 0x12,
        UInt32 = 0x13,
        UInt64 = 20,
        VarNumeric = 0x15,
        Xml = 0x19
    }
}
