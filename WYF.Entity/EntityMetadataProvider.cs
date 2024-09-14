using WYF.Entity.Service;
using WYF.Form.Service.Metadata;

namespace WYF.Entity
{
    public class EntityMetadataProvider : IEntityMetaDataProvider
    {
        private IMetadataService _metadataService;

        public EntityMetadataProvider()
        {
            _metadataService = ServiceFactory.GetService<IMetadataService>();
        }

        public EntityMetadataProvider(IMetadataService metadataService)
        {
            this._metadataService = metadataService;
        }

        public MainEntityType GetDataEntityType(string entityName)
        {
            string str = this._metadataService.LoadEntityMeta(entityName);
            if (string.IsNullOrEmpty(str))
            {
                throw new Exception($"实体{entityName}元数据不存在。");
            }


            MainEntityType dt = (MainEntityType)EntityItemTypes.FromJsonString(str);
            return dt;



        }
        /// <summary>
        /// 获取引用实体类型
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public RefEntityType GetRefEntityType(string number)
        {
            string str = this._metadataService.LoadEntityMeta(number);
            if (str == null || str.Length == 0)
                throw new Exception($"实体{number}元数据不存在。");

            //JSONObject jsonMap = SerializationUtils.FromJsonString<JSONObject>(str);

            JSONObject jsonMap = JSONObject.Parse(str);

            jsonMap["_Type_"] = "RefEntityType";

            //List<JSONObject> refPropTypes = jsonMap.GetValue<List<JSONObject>>("RefPropTypes", new List<JSONObject>());
            var refPropTypes = jsonMap.Get("RefPropTypes") as JSONArray;

            if (refPropTypes != null && refPropTypes.Count > 0)
            {
                foreach (Dictionary<string, object> refPropType in refPropTypes)
                {
                    if (refPropType.ContainsKey("Master") && refPropType.GetValueOrDefault("Master") != null && (refPropType.GetValueOrDefault("Master")).ToBool())
                        continue;
                    refPropType.Remove("Props");
                }
            }
            return (RefEntityType)EntityItemTypes.FromDictionary(jsonMap);

        }
    }
}