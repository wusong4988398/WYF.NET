using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SimplePropertyAttribute : Attribute
    {
        //属性的名称
        public string Name { get; set; } = "";
        //设置元数据的显示名称，例如列的中文名称
        public string DisplayName { get; set; } = "";
        //字段名
        public string Alias { get; set; } = "";
        //是否是主键
        public bool IsPrimaryKey { get; set; } = false;
        /// <summary>
        /// 字段是否需要加密
        /// </summary>
        public bool IsEncrypt { get; set; } = false;
        //设置字段的大小
        public int Size { get; set; } = 0;
        //设置字段小数部分的长度
        public byte Scale { get; set; } = 0;
        ///对应的数据库类型
        public int DbType { get; set; } = 0;
        //设置此字段存储的扩展表后缀
        public string TableGroup { get; set; } = "";
        /// <summary>
        /// 设置AutoSync的枚举值
        /// </summary>
        public AutoSync AutoSync { get; set; } = AutoSync.Never;
        /// <summary>
        /// 是否关联数据库物理表，与数据库字段对应。false:关联，true：不关联
        /// </summary>
        public bool IsDbIgnore { get; set; } = false;
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool IsEnableNull { get; set; } = false;
    }

}
