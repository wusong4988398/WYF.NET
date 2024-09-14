using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Entity
{
    public interface IEntityMetaDataProvider
    {
        /// <summary>
        /// 返回实体元数据
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        MainEntityType GetDataEntityType(string entityName);

        RefEntityType GetRefEntityType(string number);
    }
}
