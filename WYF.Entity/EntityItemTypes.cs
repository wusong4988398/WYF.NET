

using WYF.DataEntity;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.dynamicobject;
using WYF.DataEntity.Serialization;
using WYF.DataEntity.Utils;
using WYF.Entity.Property;

namespace WYF.Entity
{
    public class EntityItemTypes
    {
        private static Dictionary<string, IDataEntityType> types = new Dictionary<string, IDataEntityType>();
        public EntityItemTypes()
        {
        }
        private static DcBinder CreateDcBinder()
        {
            return new EntityDcBinder();
        }
        public static IDataEntityType FromJsonString(string str)
        {
            DcJsonSerializer ser = new DcJsonSerializer(CreateDcBinder());
            ser.IsLocaleValueFull = true;
            return (IDataEntityType)ser.DeserializeFromString(str, null);
        }

        public static IDataEntityType FromDictionary(JSONObject dict)
        {

            DcJsonSerializer ser = new DcJsonSerializer(CreateDcBinder());
            ser.IsLocaleValueFull = true;
            return (IDataEntityType)ser.DeserializeFromDictionary(dict, null);
        }

        public static IDataEntityType GetDataEntityType(string name)
        {
            return (IDataEntityType)types.GetValueOrDefault(name);
        }


        public static void Register(Type type)
        {
            types[type.Name] = OrmUtils.GetDataEntityType(type);
        }

        static EntityItemTypes()
        {

            Register(typeof(MainEntityType));
            Register(typeof(BooleanProp));
            Register(typeof(LongProp));
            Register(typeof(BillStatusProp));
            Register(typeof(BasedataEntityType));
            Register(typeof(CreaterProp));
            Register(typeof(ModifierProp));
            Register(typeof(CreateDateProp));
            Register(typeof(ModifyDateProp));
            Register(typeof(BigIntProp));
            Register(typeof(UserProp));
            Register(typeof(DateTimeProp));
            Register(typeof(GroupProp));
            Register(typeof(MulComboProp));
            Register(typeof(ComboProp));
            Register(typeof(TextProp));
            Register(typeof(DynamicLocaleProperty));
            Register(typeof(BasedataProp));
            Register(typeof(RefEntityType));
            Register(typeof(EntryProp));



            Register(typeof(OrgProp));
            Register(typeof(MuliLangTextProp));
        }
    }
}