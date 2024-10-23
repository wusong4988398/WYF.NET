using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Metadata;

namespace WYF.DataEntity.Serialization
{
    public class DataEntitySerializer
    {
        public static object ConvertMapToDataEntity(IDataEntityType dt, IDictionary<string, object> mapObj)
        {
            return ConvertMapToDataEntity(dt, mapObj, null);
        }

        public static object ConvertMapToDataEntity(IDataEntityType dt, IDictionary<string, object> mapObj, DataEntityDeserializerOption option)
        {
            DataEntitySerializerReader reader = new DataEntitySerializerReader(option);
            return reader.ReadObject(dt, mapObj);
        }

        public static string SerializerToString(object dataEntity)
        {
            DataEntitySerializerWriter write = new DataEntitySerializerWriter();
            return SerializationUtils.ToJsonString(write.SerializerToMap(dataEntity));
        }

    }
}
