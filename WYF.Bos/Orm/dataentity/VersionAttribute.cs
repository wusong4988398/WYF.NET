﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.dataentity
{
    [Serializable, AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class VersionAttribute : Attribute
    {
    }
}