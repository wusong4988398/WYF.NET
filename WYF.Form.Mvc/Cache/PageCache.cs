using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Cache;
using WYF.Entity.Cache;
using WYF.Form;

namespace WYF.Mvc.Cache
{
    public class PageCache : IPageCache
    {

        public string PageId { get { return _pageId; } }


        private string _regionKey;

        private string _pageId;

        private Dictionary<string, string> cacheMap = new Dictionary<string, string>();

        private HashSet<string> changeSet = new HashSet<string>();

        private HashSet<string> removedKeys = new HashSet<string>();

        private bool _batchSave;
        //private static ICache _cache = CacheUtil.Instance;


        private static IDistributeSessionlessCache _cache = CacheFactory.GetCommonCacheFactory().GetDistributeSessionlessCache("form-pagecache");


        public PageCache(string pageId, bool batchSave) : this(pageId)
        {

            this._batchSave = batchSave;
        }

        public PageCache(String pageId)
        {
            this._pageId = pageId;
            IPageCache lp = SessionManager.GetCurrent().GetPageCacheInMemory(pageId);
            if (lp != null)
                lp.SaveChanges();
            this._regionKey = CacheKeyUtil.GetAcctId() + ".pagecache." + this.PageId;
            Restore();
            SessionManager.GetCurrent().PutPageCacheInMemory(this);
        }

        private void Restore()
        {
            this.cacheMap = _cache.GetAll(this._regionKey);
        }




        /// <summary>
        /// 批量写入缓存
        /// 默认情况下，页面缓存数据会在页面请求完成时写入缓存服务器。
        /// 代码可调用此方法，要求即时写入缓存服务器
        /// </summary>
        public void SaveChanges()
        {
            if (changeSet.Count > 0)
            {
                Dictionary<string, string> changes = new Dictionary<string, string>();
                foreach (var key in this.changeSet)
                {
                    changes.Add(key, this.cacheMap.GetValueOrDefault(key));
                }

                _cache.Put(this._regionKey, changes, CacheKeyUtil.GetPageCacheKeyTimeout());

                this.changeSet.Clear();
            }
        }
        /// <summary>
        /// 新增页面缓存
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        public void Add(string keyName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Remove(keyName);
            }
            else if (value != this.cacheMap.GetValueOrDefault(keyName))
            {
                CommitCache(keyName, value);
                this.cacheMap[keyName] = value;
                this.changeSet.Add(keyName);
                this.removedKeys.Remove(keyName);
            }



        }
        /// <summary>
        /// 移除页面缓存
        /// </summary>
        /// <param name="keyName"></param>
        public void Remove(String keyName)
        {
            CommitRemove(keyName);
            this.cacheMap.Remove(keyName);
            this.changeSet.Remove(keyName);
            this.removedKeys.Add(keyName);
        }

        private void CommitCache(string keyName, string value)
        {
            if (!this._batchSave)
                _cache.Put(this._regionKey, keyName, value, CacheKeyUtil.GetPageCacheKeyTimeout());
        }
        private void CommitRemove(string keyName)
        {
            if (!this._batchSave)

                _cache.Remove(this._regionKey, keyName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string Get(string keyName)
        {
            string v = this.cacheMap.GetValueOrDefault(keyName);

            return v;
        }
    }
}
