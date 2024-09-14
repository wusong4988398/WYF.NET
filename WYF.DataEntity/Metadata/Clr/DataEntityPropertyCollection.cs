
using WYF.DataEntity.Collections;


namespace WYF.DataEntity.Metadata.Clr
{
    /// <summary>
    /// 描述了实体的属性集合
    /// </summary>
    [Serializable]
    public class DataEntityPropertyCollection : KeyedCollectionBase<string, IDataEntityProperty>
    {

        protected IDataEntityType owner;

  
        public DataEntityPropertyCollection(List<IDataEntityProperty> list, IDataEntityType owner):base(list)
        {
            this.owner = owner;
        }

        public IDataEntityType getParent()
        {
          
            return this.owner;
            
        }


    


        //public bool TryGetValue(String propertyName, out IDataEntityProperty ref_property)
        //{


        //    return base.TryGetValue(propertyName, out ref_property);

        //}

        /// <summary>
        /// 获取简单属性集合
        /// </summary>
        /// <param name="onlyDbProperty"></param>
        /// <returns></returns>
        public List<ISimpleProperty> GetSimpleProperties(bool onlyDbProperty)
        {
           List <ISimpleProperty> list = new List<ISimpleProperty>();
           foreach (IDataEntityProperty prop in this)
            {
                ISimpleProperty sp = (prop is ISimpleProperty) ? (ISimpleProperty)prop : null;
                if (sp != null && (!onlyDbProperty || !prop.IsDbIgnore))
                {
                    list.Add(sp);
                }
            }
           return list;
        }
        /// <summary>
        /// 获取复杂属性集合
        /// </summary>
        /// <param name="onlyDbProperty"></param>
        /// <returns></returns>
        public List<IComplexProperty> GetComplexProperties(bool onlyDbProperty)
        {
            List<IComplexProperty> list = new List<IComplexProperty>();
            foreach (IDataEntityProperty prop in this)
            {
                IComplexProperty sp = (prop is IComplexProperty) ? (IComplexProperty)prop : null;
                if (sp != null && (!onlyDbProperty || !prop.IsDbIgnore))
                {
                    list.Add(sp);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取集合属性List
        /// </summary>
        /// <param name="onlyDbProperty"></param>
        /// <returns></returns>
        public List<ICollectionProperty> GetCollectionProperties(bool onlyDbProperty)
        {
            List<ICollectionProperty> list = new List<ICollectionProperty>();
            foreach (IDataEntityProperty prop in this)
            {
                ICollectionProperty sp = (prop is ICollectionProperty) ? (ICollectionProperty)prop : null;
                if (sp != null && (!onlyDbProperty || !prop.IsDbIgnore))
                {
                    list.Add(sp);
                }
            }
            return list;
        }

        protected override string GetKeyForItem(IDataEntityProperty item)
        {
            return item.Name;
        }
    }
}
