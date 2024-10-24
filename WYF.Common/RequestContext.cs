using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    [Serializable]
    public class RequestContext
    {
        private static ThreadLocal<RequestContext> current = new ThreadLocal<RequestContext>();
        public static RequestContext? Current { get { return current.Value; } }
        public string UserId { get; set; } = "";
        public string AccountId { get; set; } = "";

        public static RequestContext Get()
        {
            return RequestContext.current.Value;
        }
        public static RequestContext Create()
        {
            return Create(true);
        }
        public static RequestContext Create(bool setCurrent)
        {
             RequestContext rc = new RequestContext();
            if (setCurrent)
            {
                Set(rc);
            }
            return rc;
        }

        public static void Set(RequestContext rc)
        {
            current.Value = rc;
        }


    }
}
