using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db
{
    public class RequestContextInfo
    {

        private object rc;
        private static ThreadLocal<RequestContextInfo> th = new ThreadLocal<RequestContextInfo>();
        public string TenantId { get; set; }

        public string AccountId { get; set; }
        private static MethodInfo requestContext_get;

        private static MethodInfo requestContext_set;

        private static MethodInfo requestContext_setTenantId;

        private static MethodInfo requestContext_getTenantId;

        private static MethodInfo requestContext_setAccountId;

        private static MethodInfo requestContext_getAccountId;

        private static MethodInfo requestContext_createAndSet;

        private static MethodInfo requestContext_getSetStack;

        static RequestContextInfo()
        {
            Type requestcontextCls = Type.GetType("ws.bos.context.RequestContext");
            requestContext_getTenantId = requestcontextCls.GetMethod("GetTenantId", new Type[0]);
            requestContext_getAccountId = requestcontextCls.GetMethod("GetAccountId", new Type[0]);
            requestContext_setTenantId = requestcontextCls.GetMethod("SetTenantId",new Type[] {typeof(string)});
            requestContext_setAccountId = requestcontextCls.GetMethod("SetAccountId", new Type[] { typeof(string) });

            requestContext_get = requestcontextCls.GetMethod("Get", new Type[0]);
            requestContext_set = requestcontextCls.GetMethod("Set", new Type[] { requestcontextCls });

            requestContext_createAndSet = requestcontextCls.GetMethod("Create", new Type[] { typeof(bool) });
            requestContext_getSetStack = requestcontextCls.GetMethod("GetSetStack", new Type[0]);

        }


    
        public RequestContextInfo(string tenantId, string accountId)
        {
            this.TenantId = tenantId;
            this.AccountId = accountId;
        }

        public static RequestContextInfo Get()
        {
            try
            {
                object rc = requestContext_get.Invoke(null, new Object[0]);
                if (rc == null)
                {
                    ArgumentException e = new ArgumentException("RequestContext为空，请检查线程环境是否正确。");
                    throw e;
                }
                RequestContextInfo ret = th.Value;
                if (ret == null || ret.rc != rc)
                {
                    ret = new RequestContextInfo((string)requestContext_getTenantId.Invoke(rc, new Object[0]), (string)requestContext_getAccountId.Invoke(rc, new Object[0]));
                    ret.rc = rc;
                    th.Value = ret;
              
                }
                return ret;
            }
            catch (Exception e)
            {
                throw e;
                //throw ExceptionUtil.asRuntimeException(e);
            }
        }
    }
}
