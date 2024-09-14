using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Bos.DataEntity;

namespace WYF.DataEntity.Entity
{
    public class PropertyStockNode
    {
        public IDataEntityProperty Property { get; set; }

        public IDataEntityType DataEntityType { get; set; }

        public PropertyStockNode Previous { get; set; }
        public string PropertyPath {
            get {
                if (this.Property == null)
                    return "";
                if (this.Previous != null)
                {
                    String result = this.Previous.PropertyPath;
                    if (result.Length == 0)
                        return this.Property.Name;
                    return result + "." + this.Property.Name;
                }
                return this.Property.Name;
            }
                
        }

        public PropertyStockNode(IDataEntityType dt)
        {
            if (dt == null)
                throw new ORMArgInvalidException("100001", "构造PropertyStockNode失败，构造参数:dt不能为空！");
            this.DataEntityType = dt;
        }

        private PropertyStockNode(PropertyStockNode previousNode, IDataEntityProperty property, IDataEntityType dt)
        {
            this.Previous = previousNode;
            this.Property = property;
            this.DataEntityType = dt;
        }
        public PropertyStockNode CreateNextNode(IComplexProperty property, IDataEntityType complextype)
        {
            if (property == null)
                throw new ORMArgInvalidException("100001", "构造CreateNextNode失败，构造参数:property不能为空！");
            return new PropertyStockNode(this, (IDataEntityProperty)property, complextype);
        }

        public PropertyStockNode CreateNextNode(ICollectionProperty property)
        {
            if (property == null)
                throw new ORMArgInvalidException("100001", "构造CreateNextNode失败，构造参数:property不能为空！");
            return new PropertyStockNode(this, (IDataEntityProperty)property, property.ItemType);
        }
    }
}
