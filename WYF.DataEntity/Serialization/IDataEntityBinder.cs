using WYF.DataEntity.Metadata;

namespace WYF.DataEntity.Serialization
{
    public interface IDataEntityBinder
    {
        // 默认接口方法
        bool IsSerializProperty(IDataEntityProperty prop, DataEntitySerializerOption option);
            
          
    }
}