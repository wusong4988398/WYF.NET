using WYF.DataEntity.Entity;

namespace WYF.Entity.DataModel
{
    public interface IDataModelRepository
    {
        DynamicObject GetRootEntity();
    }
}