﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db.tx
{
    public  enum Propagation
    {
        //支持当前事务，如果当前没有事务，就以非事务方式执行。 
        SUPPORTS,
        //支持当前事务，如果当前没有事务，就新建一个事务。这是最常见的选择。
        REQUIRED,
        //新建事务，如果当前存在事务，把当前事务挂起。 
        REQUIRES_NEW,
        // 以非事务方式执行操作，如果当前存在事务，就把当前事务挂起。
        NOT_SUPPORTED
    }
}
