using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel
{
    public class RowDataEntity
    {
        public int ParentRowIndex { get; set; }

        public int RowIndex { get; set; }

        public DynamicObject DataEntity { get; set; }

        public RowDataEntity(int rowIndex, DynamicObject dataEntity)
        {
            this.ParentRowIndex = -1;
            this.RowIndex = rowIndex;
            this.DataEntity = dataEntity;
        }
    }
}
