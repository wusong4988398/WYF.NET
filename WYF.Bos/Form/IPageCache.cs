using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form
{
    /// <summary>
    /// 页面缓存服务接口
    /// 按页面缓存数据，页面关闭时自动回收
    /// 表单插件可使用 this.getView().getPageCache() 获取本接口
    /// </summary>
    public interface IPageCache
    {
        /// <summary>
        /// PageId
        /// </summary>
        public string PageId { get; }


        /// <summary>
        /// 新增页面缓存
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        void Add(String keyName, String value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        String Get(String keyName);
        /// <summary>
        /// 移除页面缓存
        /// </summary>
        /// <param name="keyName"></param>
        void Remove(String keyName);

        /// <summary>
        /// 批量写入缓存
        /// 默认情况下，页面缓存数据会在页面请求完成时写入缓存服务器。
        /// 代码可调用此方法，要求即时写入缓存服务器
        /// </summary>
        void SaveChanges();
    }
}
