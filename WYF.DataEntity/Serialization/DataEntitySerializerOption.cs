using System.Drawing;
using WYF.DataEntity.Metadata;

namespace WYF.DataEntity.Serialization
{
    public class DataEntitySerializerOption
    {
        public DataEntitySerializerOption()
        {
            this.DataEntityBinder = new DefaultDataEntityBinder();
        }
        public IDataEntityBinder DataEntityBinder {  get; set; }
        public bool IsIncludeType {  get; set; }

        public bool IsIncludeComplexProperty { get; set; }

        public bool IsIncludeCollectionProperty { get; set; }

        public bool IsIncludeDataEntityState { get; set; }




        private class DefaultDataEntityBinder : IDataEntityBinder
        {
           public bool IsSerializProperty(IDataEntityProperty prop, DataEntitySerializerOption option)
            {
               return  (prop is ILocaleProperty && prop.IsDbIgnore) ? false :
          (prop is ISimpleProperty) ? true :
          (!option.IsIncludeComplexProperty && prop is IComplexProperty) ? false :
          (!option.IsIncludeCollectionProperty && prop is ICollectionProperty) ? false :
          true;
            }
        }
    }
}