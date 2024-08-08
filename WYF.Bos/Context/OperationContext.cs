using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WYF.Bos.Context
{

    public class OperationContext
    {
        public const string Key_AppId = "appId";
        public const string Key_OpKey = "opKey";
        public const string Key_OpMethod = "opMethod";
        public const string Key_FormId = "formId";
        public const string Key_FormName = "formName";
        public const string Key_TenantId = "tenantId";

        private static readonly ThreadLocal<OperationContext> _threadLocal = new ThreadLocal<OperationContext>(() => new OperationContext());
        /// <summary>
        /// APPID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// OpKey
        /// </summary>
        public string OpKey { get; set; }
        public string OpMethod { get; set; }
        public string FormId { get; set; }
        public string FormName { get; set; }
        public string TenantId { get; set; }

        public static void Set(OperationContext context)
        {
            _threadLocal.Value = context ?? throw new ArgumentNullException(nameof(context));
        }

        public static OperationContext Get()
        {
            return _threadLocal.Value;
        }

        public static void Remove()
        {
            _threadLocal.Value = null;
        }

        public OperationContext Copy()
        {
            return new OperationContext
            {
                AppId = this.AppId,
                FormId = this.FormId,
                FormName = this.FormName,
                OpKey = this.OpKey,
                OpMethod = this.OpMethod,
                TenantId = this.TenantId
            };
        }
    }
}
