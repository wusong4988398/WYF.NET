using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{
    /// <summary>
    /// 定义了实体类型的特征，可选值包括：Class(缺省)、Abstract、Sealed、Interface
    /// </summary>
    public enum DataEntityTypeFlag
    {
        /// <summary>
        /// 实体类型是抽象类
        /// </summary>
        Class = 0,
        /// <summary>
        /// 实体类型是个普通的类，默认
        /// </summary>
        Abstract = 1,
        /// <summary>
        /// 实体类型已经封装
        /// </summary>
        Final = 2,
        /// <summary>
        /// 实体类型是一个接口类型
        /// </summary>
        Interface = 3,
        /// <summary>
        /// 实体类型是个基元类型，例如int32，例外string也被规划为基元类型。
        /// </summary>
        Primitive = 4,
    }
}
