using WYF.Bos.Context;
using WYF.Bos.Entity;
using WYF.Bos.Form;
using WYF.Bos.Form.control;
using WYF.Bos.Mvc;
using WYF.Bos.Mvc.Form;
using WYF.Bos.Service.Metadata;
using WYF.Bos.Service;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WYF.Bos.Orm.query;
using WYF.Bos.ServiceHelper;

using WYF.DataEntity.Entity;
using WYF;
using WYF.Extension;

namespace JNPF.Form
{
    //[ApiDescriptionSettings(Name = "Form", Order = 307)]
    //[Route("api/[controller]")]
    public class FormBillService 
    {
        //private static FormMetadataCache formMetadata = App.GetService<FormMetadataCache>();
        private static FormMetadataCache formMetadata = new FormMetadataCache();
        //[AllowAnonymous]
        //[HttpPost("Test")]
        public async Task<dynamic> FormTest()
        {
            string str = "";
            int pageIndex = 0;
            int pagesize = 15;
            QFilter[] qFilters = null;
            QFilter qFilter4 = new QFilter("user.eid", "=", 0);
            QFilter qFilter7 = qFilter4.Or(qFilter4);

            qFilters = new QFilter[]{ qFilter7 };
            DynamicObject[] dobjs = BusinessDataServiceHelper.Load("bos_bizpartneruser", "bizpartner.number ,bizpartner.name,user.usertype,isadmin,org.id ,user.eid,user.phone,user.name", qFilters, "id", pageIndex, pagesize);
            return str;
        }


        //[AllowAnonymous]
        //[HttpPost("GetConfig")]
        public Task<dynamic> GetConfig(Dictionary<string, object> param)
        {
            string formId = param.Get("formId").ToNullString();
            string rootPageId = param.GetString("rootPageId");
            string pageId = string.Empty;
            if (string.IsNullOrEmpty(rootPageId))
            {
                pageId = "root" + Guid.NewGuid().ToNullString().Replace("-", "");
                param.Add("pageId", pageId);
                param.Add("rootPageId", pageId);
            }
            param.Remove("hasRight");
            param.Remove("permissionItemId");
            //var keys= CacheUtil.Instance().GetAllKeys();


            Dictionary<string, object> config = FormConfigFactory.CreateConfig(param);
            //string config= formMetadata.GetFormConfig("pc_main_console");

            return Task.FromResult<dynamic>(config);
        }

        //[AllowAnonymous]
        //[HttpPost("GetMetadata")]
        public async Task<dynamic> GetMetadata(string number)
        {
            IMetadataService _metadataService = ServiceFactory.GetService<IMetadataService>();

            string str = _metadataService.LoadFormRuntimeMeta(number, RuntimeMetaType.Client, number);
            return str;
        }
        //[AllowAnonymous]
        //[HttpPost("BatchInvokeAction")]
        public async Task<string> BatchInvokeAction(string pageId,string dataParams)
        {
            if (string.IsNullOrEmpty(pageId))
            {
                throw WYFException.Oh("参数pageId不能为空");
            }

            Dictionary<string, object> actionParams = new Dictionary<string, object>();
            actionParams["web"] = true;
            //JSONArray paramList =JSONArray.Parse(dataParams);
  
            List<Dictionary<string, object>> paramList= JsonConvert.DeserializeObject<List<Dictionary<string, object>>> (dataParams);
            SessionManager sm = SessionManager.GetCurrent();
            sm.IsRequestThread = true;
            IFormView formView = sm.GetView(pageId);
            String methodName = "";
            if (formView != null)
            {
                string key = "";
                string formId = "";
                string appId = "";
                IFormController srv = formView.GetService<IFormController>();
                List<object> results = new List<object>();
                foreach (var param in paramList)
                {
                    //JSONObject jsonobject = (JSONObject)param;

                    //key = jsonobject.GetValue<string>("key", "");

                    //methodName = jsonobject.GetValue<string>("methodName", "");

                    key =param.GetValueOrDefault("key", "").ToNullString();
                    methodName = param.GetValueOrDefault("methodName", "").ToNullString();

                    formId = formView.FormShowParameter.FormId;
                    string formName = formView.FormShowParameter.GetFormName();
                    appId = formView.FormShowParameter.GetServiceAppId();
                    SetOperationContext(appId, formId, formView.FormShowParameter.GetFormName(), methodName, key);


                    //JSONArray args = jsonobject.GetValue<JSONArray>("args", null);
                    //JSONArray postData = jsonobject.GetValue<JSONArray>("postData", null);

                    //string postDatastr= jsonobject.GetValue<string>("postData", null);

                    string postDatastr = param.GetValueOrDefault("postData", null).ToNullString();

                    string argsstr = param.GetValueOrDefault("args", null).ToNullString();

                    List<Dictionary<string, object>> postData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>> (postDatastr);


                    //List<object> args = JsonConvert.DeserializeObject<List<object>>(argsstr);

                    JSONArray args= JSONArray.Parse(argsstr);

                    //object post = param.GetValueOrDefault("postData");
                    // List<Dictionary<string, object>> postData = (List<Dictionary<string, object>>)post;
                    results.AddRange(InvokeAction(formView, key, methodName, args, postData));

                }
                
                return results.ToJsonString();
            }





                return "";
        }
        private List<object> InvokeAction(IFormView formView, String key, String methodName, JSONArray args, List<Dictionary<string, object>> postData)
        {
            string pageId = formView.PageId;

            try
            {
                IFormController srv = (IFormController)formView.GetService<IFormController>();
                try
                {
                    srv.PostData(postData);
                    if (string.IsNullOrEmpty(key))
                    {
                        OperationContext.Get().OpKey = formView.GetType().Name;
                        InvokeMethod(formView, srv, methodName, args);
                    }
                    else
                    {
                        srv.RegisterListener();
                        Control ctl = formView.GetControl<Control>(key);
                        if (ctl != null)
                        {
                            OperationContext.Get().OpKey = (ctl.GetOpKey(methodName, args));
                            InvokeMethod(formView, ctl, methodName, args);
                        }

                    }
                    List<Object> actionResult = formView.GetActionResult();
                    SessionManager.GetCurrent().CommitCache();
                    return actionResult;
                }
                finally
                {
                    srv.Destory();
                }
            }
            catch (Exception e)
            {
                List<Object> acts = new List<object>();

                return acts;
                
            }
     
        

         
        }
        private static Object InvokeMethod(IFormView formView, Object target, String methodName, JSONArray args)
        {
            String serviceName = target.GetType().Name + "." + methodName + '@' + formView.FormShowParameter.FormId;

            Type type = target.GetType();
            Type[] array = (args == null) ? new Type[0] : new Type[args.Count<object>()];
            object[] array2 = (args == null) ? new object[0] : new object[args.Count<object>()];
            if (args != null)
            {
                for (int i = 0; i < args.Count<object>(); i++)
                {
                    array2[i] = ((args[i] == null) ? string.Empty : args[i]);
                    array[i] = array2[i].GetType();
                }
            }
            string name = methodName.Substring(0, 1).ToUpperInvariant() + methodName.Substring(1);
            MethodInfo method2 = type.GetMethod(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public, null, array, null);
            if (method2 == null)
            {
                method2 = type.GetMethod(methodName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public, null, array, null);
                if (method2 == null)
                {
                    method2 = type.GetMethod(name);
                }
            }

            if (method2 != null)
            {
               Object result= method2.Invoke(target, array2);
                return result;
            }


            return null;
     

        }
        private void SetOperationContext(string appId, string formId, string formname, string method, string opKey)
        {
            OperationContext oc = new OperationContext();
            oc.AppId= appId;
            oc.FormId=formId;
            oc.FormName = formname;
            oc.OpKey = opKey;
            oc.OpMethod = method;
            OperationContext.Set(oc);
        }
    }
}
