using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity
{
    public class RefEntityTypeCache
    {
        private static IEntityMetaDataProvider provider = null;
        public static Dictionary<string, DynamicObjectType> GetRefTypes(List<RefPropType> refPropTypes)
        {
            return GetRefTypes(refPropTypes, true);
        }

        public static Dictionary<string, DynamicObjectType> GetRefTypes(List<RefPropType> refPropTypes, bool fillRef)
        {
            Dictionary<String, DynamicObjectType> types = new Dictionary<String, DynamicObjectType>();

            foreach (RefPropType refPropType in refPropTypes)
            {
                RefEntityType refDT = GetRefEntityType(refPropType.Id, fillRef);
                types[refPropType.Id] = refDT.GetSubEntityType(refPropType.GetPropSet());
     
            }
            return types;
        }

        private static RefEntityType GetRefEntityType(string number, bool fillRef)
        {
            RefEntityType dt = GetProvider().GetRefEntityType(number);

            if (fillRef)
            {
                FillRefTypes(dt);
            }
            return dt;

        }

        private static void FillRefTypes(RefEntityType dt)
        {

            List<RefPropType> refPropTypes = dt.RefPropTypes;
            if (refPropTypes != null && refPropTypes.Any())
            {
                Dictionary<string, DynamicObjectType> types = new Dictionary<string, DynamicObjectType>();
                foreach (RefPropType refPropType in refPropTypes)
                {
                    RefEntityType refDT = null;
                    if (dt.Name==refPropType.Id)
                    {
                        refDT = dt;
                    }
                    else
                    {
                        refDT = GetRefEntityType(refPropType.Id, refPropType.IsMaster);
                    }
                    types[refPropType.Id] = refDT.GetSubEntityType(refPropType.GetPropSet());
                }

                dt.FillRefEntityTypes(types);
            }
        }

        private static IEntityMetaDataProvider GetProvider()
        {
                if (provider == null)
                    provider = new EntityMetadataProvider();
          
            return provider;
        }
    }
}
