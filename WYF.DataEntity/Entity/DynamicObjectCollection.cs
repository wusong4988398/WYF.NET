

using WYF.DataEntity.Metadata.Dynamicobject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WYF.DataEntity.Entity
{
    [Serializable]
    public class DynamicObjectCollection : DataEntityCollection<DynamicObject>
    {
        private IList<DynamicObject> _deleteRows;
        private DynamicObjectType dt;
        private bool isInitializing;
        //private static IEntryReader entryReader;


        public DynamicObjectCollection()
        {
            this.isInitializing = false;
        }

        public DynamicObjectCollection(DynamicObjectType dt, Object parent) : base(parent)
        {
            this.isInitializing = false;
            if (dt == null)
            {
                throw new ArgumentNullException("DynamicObjectType 不能为空！");
            }
            this.dt = dt;
        }

        public DynamicObjectCollection(DynamicObjectType dynamicItemPropertyType, Object parent, List<DynamicObject> list) : base(parent, list)
        {

            this.isInitializing = false;
            if (dynamicItemPropertyType == null)
            {
                throw new ArgumentNullException("DynamicObjectType 不能为空！");
            }
            this.dt = dynamicItemPropertyType;
        }

        public DynamicObjectType DynamicObjectType { get { return this.dt; } }

        public  DynamicObjectType DynamicCollectionItemPropertyType { get { return this.DynamicObjectType; } }

        public DynamicObject AddNew()
        {
            DynamicObject item = (DynamicObject)this.dt.CreateInstance();
            base.Add(item);
            return item;
        }

        public new void Add( int index,  DynamicObject item)
        {
            if (this.dt != null)
            {
                if (this.dt.Name!= item.DynamicObjectType.Name || this.dt.Name != item.DynamicObjectType.Name)
                {
                    throw new ArgumentNullException("添加行到DynamicObjectCollection失败，数据行类型与集合类型不匹配！");
                }
            }
            else
            {
                this.dt = item.DynamicObjectType;
                this.parent = item.Parent;
            }
            base.Add(index, item);
        }
        public IList<DynamicObject> DeleteRows
        {
            get
            {
                if (this._deleteRows == null)
                {
                    this._deleteRows = new List<DynamicObject>();
                }
                return this._deleteRows;
            }
            private set
            {
                this._deleteRows = value;
            }
        }
 

     
    }
}
