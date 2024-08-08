using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Entity
{
    /// <summary>
    /// 描述一个对象，支持获取和设置父对象的功能
    /// </summary>
    public interface IObjectWithParent
    {
        object Parent { get; set; }
    }
}
