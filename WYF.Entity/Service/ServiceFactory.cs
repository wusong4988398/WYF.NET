using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity;

namespace WYF.Entity.Service
{
    public class ServiceFactory
    {
        private static Dictionary<string, string> serviceMap = new Dictionary<string, string>();


        public ServiceFactory()
        {
        }


        public static T GetService<T>()
        {
            return GetService<T>(typeof(T).Name);
        }

        public static T GetService<T>(string serviceName)
        {

            string className = serviceMap.GetOrDefault(serviceName);
            if (string.IsNullOrEmpty(className))
            {
                throw new Exception($"{serviceName}对应的服务实现未找到");
            }
            else
            {
                return TypesContainer.GetOrRegisterSingletonInstance<T>(className);
            }
        }


        static ServiceFactory()
        {
            serviceMap.Add("IMetadataService", "WYF.Form.Service.Metadata.MetadataService");
        }
    }
}
