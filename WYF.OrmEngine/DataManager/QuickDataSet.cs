using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Bos.DataEntity;
using WYF.DataEntity;

namespace WYF.OrmEngine.dataManager
{
    public class QuickDataSet
    {
        public QuickDataTableCollection Tables = new QuickDataTableCollection();

        public int FindIndex(QuickDataTable table)
        {
            QuickDataTableCollection tables = this.Tables;
            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i] == table)
                    return i;
            }
            throw new ORMDesignException("100001", "从QuickDataSet表结构中查找表[{0}]失败，表[{0}]不存在！");
        }

    }
}
