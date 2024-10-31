using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Mvc;
using WYF.Mvc.Cache;

namespace WYF.Form.Mvc
{
    public class FormConfigFactory
    {
        public static Dictionary<string, object> CreateConfig(Dictionary<string, object> openParameter)
        {
            FormShowParameter showParameter = FormShowParameter.CreateFormShowParameter(openParameter);

            return CreateConfig(showParameter);
        }

        public static Dictionary<string, object> CreateConfig(FormShowParameter showParameter)
        {
            string appId = "appId";
            Dictionary<string, object> setting = new Dictionary<string, object>();

            Dictionary<string, object> ret = showParameter.CreateClientConfig(setting);
            if (ret.GetValueOrDefault("cancel", null) != null)
            {
                return ret;
            }
            ret.Add("setting", setting);

            if (showParameter.PageId == showParameter.RootPageId)
            {
                //未实现
            }
            IPageCache cache = SessionManager.GetCurrent().GetPageCache(showParameter.PageId);

            if ("pc_main_console" == showParameter.FormId.Trim())
            {
                SessionManager.GetCurrent().PutMainPageId(showParameter.RootPageId, showParameter.PageId);
            }

            RootPageCache.AddPageId(showParameter.RootPageId, showParameter.PageId);
            showParameter.CacheExpireTime = DateTime.Now;
            cache.Add(typeof(FormShowParameter).Name, showParameter.ToString());
            cache.SaveChanges();

            return ret;

        }
    }
}
