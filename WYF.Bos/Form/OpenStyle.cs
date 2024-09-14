using WYF.DataEntity.Entity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form
{
    /// <summary>
    /// 界面展示风格
    /// </summary>
    [Serializable]
    public class OpenStyle
    {

        public OpenStyle()
        {
            this.ShowType = ShowType.Default;
        
        }
        /// <summary>
        /// 缓存标识
        /// </summary>
        [SimpleProperty]
        public string CacheId { get; set; }

        [SimpleProperty]
        public StyleCss InlineStyleCss { get; set; }
        /// <summary>
        /// 界面显示位置
        /// </summary>
        [SimpleProperty]
        public FloatingDirection FloatingDirection { get; set; }

        /// <summary>
        /// 界面展示风格
        /// </summary>
        [SimpleProperty]
        public ShowType ShowType { get; set; }
        /// <summary>
        /// 父容器标识
        /// </summary>
        
        [SimpleProperty]
        public string TargetKey { get; set; }
        /// <summary>
        /// 插件中调用showForm时，可以放入自定义参数返回前端
        /// </summary>
        [SimpleProperty]
        public Dictionary<String, String> CustParam { get; set; }


        public Dictionary<string, object> GetConfig()
        {
            Dictionary<string, object> config = new Dictionary<string, object>();
            config.Add("showType", (int)this.ShowType);
            if (!string.IsNullOrEmpty(this.TargetKey))
            {
                config.Add("targetKey", this.TargetKey);
            }
            if (!string.IsNullOrEmpty(this.CacheId))
            {
                config.Add("cacheId", this.CacheId);
            }
            if (this.CustParam!=null&& this.CustParam.Count>0)
            {
                foreach (KeyValuePair<string, string> item in this.CustParam)
                {
                    config.Add(item.Key, item.Value);
                }
            }
            if (this.FloatingDirection != null)
            {
                config.Add("direction", (int)this.FloatingDirection);
            }

            return config;
        }


    }
}
