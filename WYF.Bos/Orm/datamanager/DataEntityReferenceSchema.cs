using Antlr4.Runtime.Misc;
using WYF.Bos.DataEntity;
using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.Orm.Exceptions;
using WYF.Bos.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF;

namespace WYF.Bos.Orm.datamanager
{
    public class DataEntityReferenceSchema
    {
        private Dictionary<String, Dictionary<ISimpleProperty, DataEntityReferenceSchemaItem>> _dict = new Dictionary<string, Dictionary<ISimpleProperty, DataEntityReferenceSchemaItem>>();

        private static  DataEntityReferenceSchema _empty = new DataEntityReferenceSchema();

        public DataEntityReferenceSchema GetReferenceSchema(IDataEntityType dt, bool onlyDbProperty)
        {
            if (dt == null)
                throw new ORMArgInvalidException("100001", "分析一个指定的实体类型获取其引用定义情况失败，要分析的实体类型不能为空！");
            DataEntityReferenceSchema result = (DataEntityReferenceSchema)dt.DataEntityReferenceSchema;
            if (result == null)
            {
                result = Create(dt, onlyDbProperty);
                dt.DataEntityReferenceSchema=result;
            }
            if (result._dict.Count == 0)
                return null;
            return result;
        }

        private DataEntityReferenceSchema Create(IDataEntityType dt, bool onlyDbProperty)
        {
            DataEntityReferenceSchema schema = new DataEntityReferenceSchema();
            PropertyStockNode stack = new PropertyStockNode(dt);
            SearchDataEntityReferenceSchema(dt, schema, stack, onlyDbProperty);
            if (schema._dict.IsEmpty())
                return _empty;
            return schema;
        }
        public List<DataEntityReferenceSchemaItem> GetItemsByPropertyPath(String propertyPath, bool mustHaveReferenceObjectProperty)
        {
            if (propertyPath == null)
                propertyPath = "";
            Dictionary<ISimpleProperty, DataEntityReferenceSchemaItem> d1 = null;
            List<DataEntityReferenceSchemaItem> items = new List<DataEntityReferenceSchemaItem> ();
            if ((d1 = this._dict.GetValueOrDefault(propertyPath)) != null)
                foreach (DataEntityReferenceSchemaItem item in d1.Values)
                {
                    if (!mustHaveReferenceObjectProperty || item.ReferenceObjectProperty != null)
                        items.Add(item);
                }
            return items;
        }
        private static void SearchDataEntityReferenceSchema(IDataEntityType dt, DataEntityReferenceSchema schema, PropertyStockNode stock, bool onlyDbProperty)
        {
            foreach (IComplexProperty cp in dt.Properties.GetComplexProperties(false))
            {
                DataEntityReferenceSchemaItem schemaItem = new DataEntityReferenceSchemaItem();
                IDataEntityProperty dataEntityProperty = (IDataEntityProperty)dt.Properties[cp.RefIdPropName];
                if (dataEntityProperty != null)
                {
                    ISimpleProperty sp = (ISimpleProperty)dataEntityProperty;
                    schemaItem.ReferenceOidProperty = sp;
                    schemaItem.ReferenceTo = cp.Name;
                    schemaItem.PropertyPath = stock.PropertyPath;
                    schemaItem.ReferenceObjectProperty = cp;
             
                    schema.Add(schemaItem);
                }
            }
            List<ICollectionProperty> collectionProperties = dt.Properties.GetCollectionProperties(onlyDbProperty);
            foreach (ICollectionProperty colp in collectionProperties)
                SearchDataEntityReferenceSchema(colp.ItemType, schema, stock.CreateNextNode(colp), onlyDbProperty);
        }

        public void Add(DataEntityReferenceSchemaItem item)
        {
            if (item == null)
                throw new ORMArgInvalidException("100001", "扫描实体引用属性定义情况时添加元素失败，扫描结果不能为空！");
            if (string.IsNullOrEmpty(item.PropertyPath))
                item.PropertyPath = "";
            if (item.ReferenceOidProperty == null)
                throw new ORMArgInvalidException("100001", "扫描实体引用属性定义情况时添加元素失败，引用属性[ReferenceOidProperty]不能为空！");
            if (StringUtils.IsEmpty(item.ReferenceTo))
                throw new ORMArgInvalidException("100001", "扫描实体引用属性定义情况时添加元素失败，引用属性指向[ReferenceTo]不能为空！");
            if (item.ReferenceObjectProperty != null && item.ReferenceObjectProperty.ComplexType != null)
            {
                if (item.ReferenceObjectProperty.ComplexType.PrimaryKey == null)
                    throw new ORMDesignException("100001", string.Format("扫描实体引用属性定义情况时添加元素失败，携带值的复杂属性[{0}]必须包含主键，否则在加载时无法找到对应关系！", item.ReferenceObjectProperty.Name));
                if (item.ReferenceObjectProperty.ComplexType.PrimaryKey.PropertyType != item.ReferenceOidProperty.PropertyType)
                    throw new ORMDesignException("100001", String.Format("扫描实体引用属性定义情况时添加元素失败，携带值的复杂属性[%s]主键类型和引用的类型不一致！", item.ReferenceObjectProperty.Name));
            }
            Dictionary<ISimpleProperty, DataEntityReferenceSchemaItem> d1 = null;
            if ((d1 = this._dict.GetValueOrDefault(item.PropertyPath)) == null)
            {
                d1 = new Dictionary<ISimpleProperty, DataEntityReferenceSchemaItem>();
                this._dict[item.PropertyPath] = d1;
            }
            DataEntityReferenceSchemaItem d2 = null;
            if ((d2 = d1.GetValueOrDefault(item.ReferenceOidProperty)) != null)
                throw new ORMDesignException("100001", String.Format("扫描实体引用属性定义情况时添加元素失败，要添加的属性[{%s]已经添加过了！", item.ReferenceOidProperty.Name));
            d1[item.ReferenceOidProperty] = item;
        }

    }
}
