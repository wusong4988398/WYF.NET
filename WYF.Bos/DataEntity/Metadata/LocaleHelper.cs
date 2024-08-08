using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata
{
    public static class LocaleHelper
    {
        public static string GetColumnName(this DynamicProperty property)
        {
            
            object[] customAttributes = property.GetCustomAttributes(typeof(SimplePropertyAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                string alias = ((SimplePropertyAttribute)customAttributes[0]).Alias;
                if (!string.IsNullOrEmpty(alias))
                {
                    return alias;
                }
            }
            return property.Name;
        }
    }
}
