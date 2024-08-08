using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Context
{
    [Serializable]
    public  class RequestContext
    {
        private static ThreadLocal<RequestContext> current= new ThreadLocal<RequestContext>();


        public string UserId { get; set; } = "";

        public static RequestContext Get()
        {
            return RequestContext.current.Value;
        }
    }
}
