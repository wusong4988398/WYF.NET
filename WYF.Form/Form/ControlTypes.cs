using WYF.DataEntity.Metadata;



using WYF.DataEntity.Serialization;
using WYF.DataEntity.Utils;
using WYF.Form.container;
using WYF.Form.control;

namespace WYF.Form
{
    public class ControlTypes
    {

        private static Dictionary<String, IDataEntityType> types = new Dictionary<String, IDataEntityType>();
        private static FormDcBinder formDcBinder = new FormDcBinder();

        static ControlTypes()
        {
            Register(typeof(FormShowParameter));
            Register(typeof(FormConfig));
            //Register(typeof(DynamicScriptPlugin));
            #region Container
            Register(typeof(FormRoot));
            Register(typeof(Container));
            Register(typeof(Tab));
            Register(typeof(TabPage));
            Register(typeof(GridContainer));
            //Register(typeof(BizCustomList));    
            #endregion

            #region Control
            Register(typeof(Image));
            Register(typeof(Vector));
            Register(typeof(FloatMenu));
            Register(typeof(Search));
            Register(typeof(CustomControl));
            
            #endregion


            #region Field
           // Register(typeof(FieldEdit));
            
            #endregion
        }

        public static object FromJsonStringToObj(String str)
        {
            DcJsonSerializer ser = new DcJsonSerializer(formDcBinder);
            ser.IsLocaleValueFull = true;
            return ser.DeserializeFromString(str, null);
        }

        private ControlTypes()
        {
        }
        public static IDataEntityType GetDataEntityType(String name)
        {
           return  types.GetValueOrDefault(name, null);
           // return types[name];
        }
        public static void Register(Type type)
        {
            types.Add(type.Name, OrmUtils.GetDataEntityType(type));
        }

        public static string ToJsonString(FormShowParameter type)
        {
            DcJsonSerializer ser = new DcJsonSerializer(formDcBinder);
            //ser.IsLocaleValueFull = true;
           // return ser.DeserializeFromString(str, null);
            ser.IsLocaleValueFull = true;
            return ser.SerializeToString(type, null);
        }

        public static T FromJsonString<T>(string str) where T : Control
        {
            DcJsonSerializer ser = new DcJsonSerializer(formDcBinder);
            ser.IsLocaleValueFull = true;
            return (T)ser.DeserializeFromString(str, null);
        }
    }
}
