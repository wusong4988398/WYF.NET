using WYF.Bos.Orm.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.Metadata
{
    public sealed class AttributeCacheManager
    {
    
        private List<KeyValuePair<Type, object>> _cachedAttributes;
        private bool _isCached;

     
        public AttributeCacheManager(ICustomAttributeProvider attributeProvider, bool searchInherit)
        {
            if (attributeProvider == null)
            {
                throw new ORMArgInvalidException("??????", "创建缓存属性管理器失败，参数[attributeProvider]不能为空！");
            }
            this.AttributeProvider = attributeProvider;
            this.SearchInherit = searchInherit;
            this._cachedAttributes = new List<KeyValuePair<Type, object>>();
        }

        public T GetAttributeByIndex<T>(int index) where T : Attribute
        {
            this.InitCache();
            KeyValuePair<Type, object> pair = this._cachedAttributes[index];
            return (T)pair.Value;
        }

        private void InitCache()
        {
            if (!this._isCached)
            {
                lock (this)
                {
                    if (!this._isCached)
                    {
                        object[] customAttributes = this.AttributeProvider.GetCustomAttributes(this.SearchInherit);
                        if ((customAttributes != null) && (customAttributes.Length > 0))
                        {
                            foreach (object obj2 in customAttributes)
                            {
                                for (int i = 0; i < this._cachedAttributes.Count; i++)
                                {
                                    KeyValuePair<Type, object> pair = this._cachedAttributes[i];
                                    if ((pair.Value == null) && pair.Key.IsInstanceOfType(obj2))
                                    {
                                        this._cachedAttributes[i] = new KeyValuePair<Type, object>(pair.Key, obj2);
                                    }
                                }
                            }
                        }
                        this._isCached = true;
                    }
                }
            }
        }

        public int RegisterTypedAttributeIndex(Type attributeType)
        {
            KeyValuePair<Type, object> pair;
            if (attributeType == null)
            {
                throw new ORMArgInvalidException("??????", "缓存属性管理器注册属性失败，属性[attributeType]不能为空！");
            }
            for (int i = 0; i < this._cachedAttributes.Count; i++)
            {
                pair = this._cachedAttributes[i];
                if (pair.Key.IsAssignableFrom(attributeType))
                {
                    this._cachedAttributes[i] = new KeyValuePair<Type, object>(attributeType, pair.Value);
                    return i;
                }
                if (attributeType.IsAssignableFrom(pair.Key))
                {
                    return i;
                }
            }
            pair = new KeyValuePair<Type, object>(attributeType, null);
            this._cachedAttributes.Add(pair);
            return (this._cachedAttributes.Count - 1);
        }

    
        public ICustomAttributeProvider AttributeProvider { get; private set; }

        public bool SearchInherit { get; private set; }
    }
}
