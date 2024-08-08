using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata
{
    public interface IMetadata
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        /// <returns></returns>

        public string Name { get;  }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get { return this.Name; } }
     
        /// <summary>
        /// 获取物理字段名(表名)
        /// </summary>
        /// <returns></returns>
        public  string Alias { get; }
        /// <summary>
        /// 是否关联物理表 false:关联，true：不关联
        /// </summary>
        /// <returns></returns>
        bool IsDbIgnore { get; }

        object Clone() { return null; }
    }
}
