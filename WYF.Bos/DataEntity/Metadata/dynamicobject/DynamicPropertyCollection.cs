
using WYF.Bos.DataEntity.Metadata.Clr;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.Dynamicobject
{
    /// <summary>
    /// 实体的属性集合
    /// </summary>
    [Serializable]
    public class DynamicPropertyCollection : DataEntityPropertyCollection
    {
       
        /// <summary>
        ///构造函数
        /// </summary>
        /// <param name="list">需要注册的属性集合</param>
        /// <param name="owner"></param>
        public DynamicPropertyCollection(List<IDataEntityProperty> list, IDataEntityType owner) : base(list, owner)
        {
        }

        internal new void Add(IDataEntityProperty property)
        {
            DynamicProperty prop = (DynamicProperty)property;
            if (getParent() != null)
            {
                getParent().CheckUnmodifiable();
             
                prop.Parent= getParent();
            }
            else
            {
                this.owner = property.Parent;
            }
           prop.Ordinal = Count;
            base.Add(prop);
            //base.Dictionary.Add(property.Name, property);
           //base.add(property);
     
        }


        public List<ICollectionProperty> GetCollectionProperties(bool onlyDbProperty)
        {
            List<ICollectionProperty> retList =new List<ICollectionProperty>();
            foreach (IDataEntityProperty p in this)
            {
                if (p is ICollectionProperty&&(!onlyDbProperty || !p.IsDbIgnore))
                {
                    retList.Add((ICollectionProperty)p);
                }
            }
            return retList;
        }
    }
}
