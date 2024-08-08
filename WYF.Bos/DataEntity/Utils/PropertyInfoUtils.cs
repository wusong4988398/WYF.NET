
using WYF.Bos.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.Form.DataEntity.Utils
{
    public sealed class PropertyInfoUtils
    {
        public static bool TryGetDefaultValue(MethodInfo pInfo, ref object defaultValue)
        {
            DefaultValueAttribute defAtt = pInfo.GetCustomAttribute<DefaultValueAttribute>();
            Type returnType = pInfo.ReturnType;
            if (defAtt != null)
            {
                defaultValue = Convert.ChangeType(defAtt.value, returnType);
                return true;
            }
            if (returnType.IsPrimitive)
            {
                if (returnType == typeof(bool))
                {
                    defaultValue = false;
                }
                else if (returnType == typeof(char))
                {
                    defaultValue = ' ';
                }
                else
                {
                    defaultValue = Convert.ChangeType(0, returnType);
                }
                return true;
            }
            if (returnType == typeof(string)  || typeof(object).IsAssignableFrom(returnType))
            {
                defaultValue = null;
                return true;
            }
            defaultValue = null;
            return false;
        }

        internal static MethodInfo GetShouldSerializeMethod(PropertyInfo pInfo)
        {
            return GetMethod(pInfo, "ShouldSerialize", typeof(bool));
        }

        private static MethodInfo GetMethod(PropertyInfo pInfo, string methodPrex, Type returnType)
        {
            Type[] emptyTypes = Type.EmptyTypes;
            return FindMethod(pInfo.DeclaringType, methodPrex + pInfo.Name, emptyTypes, returnType);
        }

        private static MethodInfo FindMethod(Type componentClass, string name, Type[] args, Type returnType)
        {
            MethodInfo info = componentClass.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, args, null);
            if ((info != null) && !info.ReturnType.IsEquivalentTo(returnType))
            {
                info = null;
            }
            return info;
        }
    }
}
