using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataEntityTypeAttribute : Attribute
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 数据库表名称
        /// </summary>
        public string TableName { get; set; } = "";
        /// <summary>
        /// 数据库路由标识
        /// </summary>
        public string DbRouteKey { get; set; } = "basedata";
        /// <summary>
        /// 是否关联数据库物理表，与数据库字段对应。false:关联，true：不关联
        /// </summary>
        public bool IsDbIgnore { get; set; } = false;
    }
}
