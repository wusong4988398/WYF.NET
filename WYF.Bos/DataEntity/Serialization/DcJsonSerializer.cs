
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Serialization;
using WYF.Bos.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace JNPF.Form.DataEntity.Serialization
{
    public class DcJsonSerializer : DcSerializer
    {
        public static readonly string ExtAttributes = "_Type_";
        public static readonly string ElementName = "Name";

        public bool IsLocaleValueFull { get; set; }
        public DcJsonSerializer(DcBinder binder) : base(binder)
        {
        }

        public DcJsonSerializer(IEnumerable<IDataEntityType> dts) : base(dts)
        {
            
        }
     


        public  Object DeserializeFromString(String jsonString, Object entity)
        {
            DcJsonSerializerReaderImplement worker = new DcJsonSerializerReaderImplement(this.DcBinder, this.IsLocaleValueFull);
           
            JSONObject map=JSONObject.Parse(jsonString);
            Object result = worker.ReadElement(map, null, entity);
            worker.EndInitialize();
            return result;
        }

        public object DeserializeFromDictionary(JSONObject dict, object entity)
        {
            DcJsonSerializerReaderImplement worker = new DcJsonSerializerReaderImplement(this.DcBinder, this.IsLocaleValueFull);
            object result = worker.ReadElement(dict, null, entity);
            worker.EndInitialize();
            return result;
        }

        public string SerializeToString(Object currentEntity, object baseEntity)
        {
            DcJsonSerializerWriteImplement worker = new DcJsonSerializerWriteImplement(this.DcBinder, this.IsSerializeComplexProperty, this.IsLocaleValueFull);
            return worker.Serialize(null, currentEntity, baseEntity);
        }

   
    }
}
