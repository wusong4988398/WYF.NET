using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Extension
{
    public static class SerializationExtensions
    {
 
        public static T FromJsonString<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception("转换对象失败", ex);
            }
        }



        public static string ToJsonString(this object o)
        {

            try
            {
                return JsonConvert.SerializeObject(o);
            }
            catch (Exception ex)
            {

                throw new Exception("转换对象失败", ex);
            }
        }
    }
}
