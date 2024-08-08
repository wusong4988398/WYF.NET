using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.impl
{
    public class ORMEntityTypeCacheMap : Dictionary<string, IDataEntityType>
    {
        private Dictionary<string, IDataEntityType> globalCache;
        private Dictionary<string, IDataEntityType> instanceCache = new Dictionary<string, IDataEntityType>();
        private readonly long globalCacheCreateTime;

        private readonly long instanceCacheCreateTime = (long)(Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000000000.0));

        private readonly string globalCacheCreateThread;
  
        private readonly string instanceCacheCreateThread;
        public ORMEntityTypeCacheMap(Dictionary<String, IDataEntityType> globalCache, long globalCacheCreateTime, string globalCacheCreateThread)
        {
            this.globalCache = globalCache;
            this.globalCacheCreateTime = globalCacheCreateTime;
            this.globalCacheCreateThread = globalCacheCreateThread; 
            this.instanceCacheCreateThread = Thread.CurrentThread.Name;
        }
        public new  IDataEntityType? this[string key] 
        {
            get 
            {
                if (key == null) return null;
                String lk = key.ToLower();
                IDataEntityType dt = this.instanceCache.GetValueOrDefault(lk,null);
                if (dt != null) return dt;
                return this.globalCache.GetValueOrDefault(lk,null);
            }

            set 
            {
                key = key.ToLower();
                this.instanceCache.Remove(key,out IDataEntityType? dt2);
                IDataEntityType dt1 = (this.globalCache[key]= value);
                
            }
        }

        public new  ICollection<string> Keys 
        { 
            get {
                List<string> set=new List<string>(this.globalCache.Count + this.instanceCache.Count);
                set.AddRange(this.globalCache.Keys);
                set.AddRange(this.instanceCache.Keys);
                return set;

            }
        }

        public new ICollection<IDataEntityType> Values
        {
            get
            {
                List<IDataEntityType> set = new List<IDataEntityType>(this.globalCache.Count + this.instanceCache.Count);
                set.AddRange(this.globalCache.Values);
                set.AddRange(this.instanceCache.Values);
                return set;

            }
        }

        public new  int Count
        {
            get
            {

                return Keys.Count;

            }
        }



        //public void Add(string key, IDataEntityType value)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(KeyValuePair<string, IDataEntityType> item)
        //{
        //    throw new NotImplementedException();
        //}

        public new void Clear()
        {
            this.globalCache.Clear();
            this.instanceCache.Clear();
        }

        public bool Contains(KeyValuePair<string, IDataEntityType> item)
        {
            return (this.globalCache.Contains(item) || this.instanceCache.Contains(item));
        }

        public new  bool ContainsKey(string key)
        {
            if (key == null)
                return false;
            String lk = key.ToLower();
            return (this.globalCache.ContainsKey(lk) || this.instanceCache.ContainsKey(lk));
        }

        

 

        //public bool Remove(string key)
        //{
        //    if (key == null) return false;

        //    string lk = key.ToLower();
        //    bool dt1 = this.globalCache.Remove(lk);
        //    bool dt2 = this.instanceCache.Remove(lk);
        //    return (dt2 == null) ? dt1 : dt2;
        //}

        public new IDataEntityType? Remove(string key)
        {
            if (key == null)
                return null;
            string lk = key.ToLower();
            this.globalCache.Remove(lk, out IDataEntityType? dt1);
            this.instanceCache.Remove(lk, out IDataEntityType? dt2);
            return (dt2 == null) ? dt1 : dt2;
        }


        //public bool Remove(KeyValuePair<string, IDataEntityType> item)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool TryGetValue(string key, [MaybeNullWhen(false)] out IDataEntityType value)
        //{
        //    throw new NotImplementedException();
        //}

     
    }
}
