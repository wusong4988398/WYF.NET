using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel
{
    internal class DataModelLocalRepository: IDataModelRepository
    {
        private DynamicObject dataEntity;
        private AbstractFormDataModel model;

        public DataModelLocalRepository(AbstractFormDataModel model, DynamicObject dataEntity)
        {
            this.model = model;
            this.dataEntity = dataEntity;
        }

        public DynamicObject GetRootEntity()
        {
            return this.dataEntity;
        }
    }
}
