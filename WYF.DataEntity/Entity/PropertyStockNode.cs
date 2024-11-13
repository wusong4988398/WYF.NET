using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Bos.DataEntity;

namespace WYF.DataEntity.Entity
{
    /// <summary>
    /// 用于表示数据实体（IDataEntityType）中的属性路径
    /// 主要用于构建和管理数据实体中的属性路径。通过递归的方式，它可以生成从根节点到任意子节点的完整属性路径，
    /// 这对于处理复杂的对象图和属性关系非常有用。
    /// </summary>
    public class PropertyStockNode
    {
        /// <summary>
        /// 当前节点对应的属性
        /// </summary>
        public IDataEntityProperty Property { get; set; }
        /// <summary>
        /// 当前属性所属的数据实体类型
        /// </summary>
        public IDataEntityType DataEntityType { get; set; }
        /// <summary>
        /// 指向前一个节点的引用，用于构建属性路径
        /// </summary>
        public PropertyStockNode Previous { get; set; }
        /// <summary>
        /// 当前节点的完整属性路径
        /// </summary>
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
