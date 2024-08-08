using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WYF.Bos.DataEntity.Entity
{
    /// <summary>
    /// 动态实体的本地值存储策略
    /// </summary>
    public interface IDataStorage
    {
        Object getLocalValue(IDataEntityProperty property);

        void setLocalValue(IDataEntityProperty property, Object value);

        IDataStorage memberClone();
    }
}
