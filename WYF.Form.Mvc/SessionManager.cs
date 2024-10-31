using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Form;
using WYF.Mvc.Cache;
using WYF.Mvc.Form;

namespace WYF.Mvc
{
    public class SessionManager
    {
        private static ThreadLocal<SessionManager> current = new ThreadLocal<SessionManager>();
        private ConcurrentDictionary<string, IFormView> _viewCache = new ConcurrentDictionary<string, IFormView>();

        private Dictionary<string, IPageCache> _ipageCache = new Dictionary<string, IPageCache>();



        //private SessionDAO sessionDAO;

        public bool IsRequestThread { get; set; } = false;

        public SessionManager()
        {
            //this._ipageCache = new Dictionary<string, IPageCache>();
            //this._viewCache = new Dictionary<string, IFormView>();
        }
        public static SessionManager GetCurrent()
        {
            SessionManager currentSession = current.Value;
            if (currentSession == null)
            {
                currentSession = new SessionManager();
                current.Value = currentSession;
            }
            return currentSession;
        }

        public IPageCache GetPageCache(string pageId)
        {
            IPageCache cache = this._ipageCache.GetValueOrDefault(pageId, null);
            bool batchSave = !this.IsRequestThread;

            if (cache == null)
            {
                return new PageCache(pageId, batchSave);
            }
            return cache;
        }
        public void PutPageCacheInMemory(IPageCache pageCache)
        {

            this._ipageCache[pageCache.PageId] = pageCache;
        }
        public IPageCache? GetPageCacheInMemory(String pageId)
        {
            IPageCache? cache = _ipageCache.GetValueOrDefault(pageId, null);
            return cache;
        }

        public IFormView? GetView(string pageId)
        {
            if (string.IsNullOrEmpty(pageId)) return null;
            IFormView view = (IFormView)this._viewCache.GetValueOrDefault(pageId, null);
            if (view == null)
            {
                IPageCache pageCache = this.GetPageCache(pageId);
                string parameter = pageCache.Get(typeof(FormShowParameter).Name);
                if (!string.IsNullOrEmpty(parameter))
                {
                    FormShowParameter formShowParameter = FormShowParameter.FromJsonString(parameter);
                    formShowParameter.EndInit();
                    view = formShowParameter.CreateView();

                    view.AddService(typeof(IPageCache), pageCache);
                    this.InitView(view, formShowParameter);
                    this._viewCache[pageId] = view;
                }

            }

            return view;

        }

        public IFormView GetViewNoPlugin(string pageId)
        {
            if (string.IsNullOrEmpty(pageId)) return null;
            IFormView view = this._viewCache.GetValueOrDefault("np." + pageId, null);
            if (view == null)
            {
                IPageCache pageCache = this.GetPageCache(pageId);
                string parameter = pageCache.Get(typeof(FormShowParameter).Name);
                if (!string.IsNullOrEmpty(parameter))
                {
                    FormShowParameter formShowParameter = FormShowParameter.FromJsonString(parameter);
                    formShowParameter.EndInit();
                    view = formShowParameter.CreateView();
                    ((FormView)view).IgnorePlugin = true;
                    view.AddService(typeof(IPageCache), pageCache);
                    this.InitView(view, formShowParameter);
                    this._viewCache["np." + pageId] = view;
                }
            }
            return view;
        }


        private void InitView(IFormView view, FormShowParameter formShowParameter)
        {
            view.Initialize(formShowParameter);
        }
        public void CommitCache()
        {
            foreach (IPageCache cache in this._ipageCache.Values)
            {
                cache.SaveChanges();
            }

        }
        /// <summary>
        /// 获取首页页面
        /// </summary>
        /// <param name="rootPageId"></param>
        /// <returns></returns>
        public IFormView GetMainView(string rootPageId)
        {
            //string pageId = GetMainPageId(rootPageId);
            //return GetView(pageId);
            return GetView(rootPageId);
        }

        public string GetMainPageId(string rootPageId)
        {
            string key = "mainviewpageid" + rootPageId;
            return Get(key);
        }


        public string Get(string key)
        {
            //return (string)ThreadCache.get(key, ()-> this.sessionDAO.getAttribute(key));
            throw new NotImplementedException();
        }

        public void PutMainPageId(string rootPageId, string pageId)
        {
            String key = "mainviewpageid" + rootPageId;
            this.Put(key, pageId);
        }

        public void Put(string key, string value)
        {
            //this.sessionDAO.setAttribute(key, value);
        }

        //public void PutMainPageId(string rootPageId, string pageId)
        //{
        //    string key = "mainviewpageid" + rootPageId;
        //    Put(key, pageId);
        //}

        //public void Put(string key, string value)
        //{
        //    this.sessionDAO.setAttribute(key, value);
        //}
    }
}
