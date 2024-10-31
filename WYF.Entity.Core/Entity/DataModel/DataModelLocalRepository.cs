using WYF.DataEntity.Entity;

namespace WYF.Entity.DataModel
{
    internal class DataModelLocalRepository : IDataModelRepository
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