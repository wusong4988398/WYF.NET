using WYF.Bos.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata.dynamicobject
{
    /// <summary>
    /// 从一个动态实体定义中提取字段的选项 需要注意的是，未注册提取的实体类型将按源类型原封不动。 DynamicObjectType#Extract方法参数
    /// </summary>
    public class ExtractOption
    {
        private Dictionary<string, HashSet<string>> _dict = new Dictionary<string, HashSet<string>>();
        /// <summary>
        /// 是否提取引用属性Id
        /// </summary>
        public bool IncludeRefId {  get; set; }


        /// <summary>
        /// 注册提取的属性信息。包括实体类型名称及你需要提取的属性列表
        /// </summary>
        /// <param name="dtName"></param>
        /// <param name="properties"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Register(string dtName, params string[] properties)
        {
            if (string.IsNullOrEmpty(dtName))
                throw new ArgumentException("dtName");
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    if (string.IsNullOrEmpty(property))
                        throw new ArgumentException("properties参数中包含空字符串");
                }
            }
            HashSet<string> props;
            if (!_dict.TryGetValue(dtName, out props))
            {
                props = new HashSet<string>();
                _dict[dtName] = props;
            }
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    props.Add(property);
                }
            }
        }

        public HashSet<string> GetExtractedProperties(string dtName)
        {
            if (StringUtils.IsEmpty(dtName))
                throw new Exception("dtName");
            HashSet<String> result;
            if ((result = this._dict.GetValueOrDefault(dtName,null)) != null)
                return result;
            return null;
        }
    }
}
