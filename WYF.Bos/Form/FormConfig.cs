

using WYF.DataEntity.Entity;
using WYF.Bos.Metadata;
using JNPF.Form.DataEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form
{
    /// <summary>
    /// 界面配置参数类
    /// </summary>
    public class FormConfig
    {
        private String listFormId = "bos_list";
        private String f7ListFormId = "bos_listf7";
        private static readonly String ENV_SCRIPT = "/FormPlugin.js";
        private static readonly String SCRIPTNUMBER = "scriptnumber";
        private static readonly String SCRIPTTYPE = "scripttype";
        [SimpleProperty]
        public string Caption { get; set; }

        [CollectionProperty(CollectionItemPropertyType =typeof(Metadata.Plugin))]
        public List<Plugin> Plugins { get;  set; }=new List<Plugin>();

        [SimpleProperty]
        public string Version { get; set; }
        public string Bosver { get; set; }

        /// <summary>
        /// 实体编码
        /// </summary>
        [SimpleProperty]
        public string EntityTypeId { get; set; }

        [SimpleProperty]
        public string Width { get; set; }
        [SimpleProperty]
        public string Height { get; set; }
        /// <summary>
        /// 模型类型
        /// </summary>
        [SimpleProperty]
        public string ModelType { get; set; }

        [DefaultValue("true")]
        [SimpleProperty(Name = "ShowTitle")]
        public bool IsShowTitle { get; set; } = true;

        [SimpleProperty]
        public ShowType ShowType { get; set; }= ShowType.MainNewTabPage;

        [SimpleProperty]
        public ShowType ViewShowType { get; set; } = ShowType.MainNewTabPage;

        [SimpleProperty(Name = "ShowWidget")]
        public bool IsShowWidget { get; set;}
        public String ListOpenLayoutBill { get; set;}

        [SimpleProperty(Name = "CustomForm")]
        public bool IsCustomForm { get; set; }

        [SimpleProperty]
        public string AppId { get; set; }

        [ComplexProperty]
        public FormRoot FormRoot { get; set; }


        public List<IFormPlugin> CreatePlugin(string formId)
        {
            List<IFormPlugin> formPlugins = new List<IFormPlugin>();
            if (this.Plugins != null&&this.Plugins.Count>0)
            {
                foreach (var ins in this.Plugins)
                {
                    if (ins.Type==0)
                    {
                        formPlugins.Add(TypesContainer.CreateInstance<IFormPlugin>(ins.ClassName));
                        continue;
                    }


                }
            }
            return formPlugins;
        }

    }
}
