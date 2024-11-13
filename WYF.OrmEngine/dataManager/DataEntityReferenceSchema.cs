using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Clr;
using WYF.DataEntity.Metadata;
using WYF.Bos.DataEntity;
using WYF.DataEntity;

namespace WYF.OrmEngine.dataManager
{
    /// <summary>
    /// 通过管理数据实体之间的引用关系，提供了一种机制来描述和解析这些关系
    /// 这对于实现ORM框架中的高级功能（如懒加载、级联操作等）非常有用。它确保了引用关系的一致性和完整性，并提供了方便的方法来访问和查询这些关系
    /// </summary>
    public class DataEntityReferenceSchema
    {
        private Dictionary<string, Dictionary<ISimpleProperty, DataEntityReferenceSchemaItem>> _dict = new Dictionary<string, Dictionary<ISimpleProperty, DataEntityReferenceSchemaItem>>();
        private static readonly DataEntityReferenceSchema _empty = new DataEntityReferenceSchema();

        public void Add(DataEntityReferenceSchemaItem item)
        {
            if (item == null)
            {
                throw new ORMArgInvalidException("01", "扫描实体引用属性定义情况时添加元素失败，扫描结果不能为空！");
            }
            if (string.IsNullOrEmpty(item.PropertyPath))
            {
                item.PropertyPath = "";
            }
            if (item.ReferenceOidProperty == null)
            {
                throw new ORMArgInvalidException("02", "扫描实体引用属性定义情况时添加元素失败，引用属性[ReferenceOidProperty]不能为空！");
            }
            else if (item.ReferenceTo.IsNullOrEmpty())
            {
                throw new ORMArgInvalidException("03", "扫描实体引用属性定义情况时添加元素失败，引用属性指向[ReferenceTo]不能为空！");
            }
            else
            {
                if (item.ReferenceObjectProperty != null && item.ReferenceObjectProperty.ComplexType != null)
                {
                    if (item.ReferenceObjectProperty.ComplexType.PrimaryKey == null)
                    {
                        throw new ORMDesignException("04", $"扫描实体引用属性定义情况时添加元素失败，携带值的复杂属性[{item.ReferenceObjectProperty.Name}]必须包含主键，否则在加载时无法找到对应关系！");
                    }
                    else if (item.ReferenceObjectProperty.ComplexType.PrimaryKey.PropertyType != item.ReferenceOidProperty.PropertyType)
                    {
                        throw new ORMDesignException("05", $"扫描实体引用属性定义情况时添加元素失败，携带值的复杂属性[{item.ReferenceObjectProperty.Name}]主键类型和引用的类型不一致！");
                    }
                }

                if (!_dict.ContainsKey(item.PropertyPath))
                {
                    _dict[item.PropertyPath] = new Dictionary<ISimpleProperty, DataEntityReferenceSchemaItem>();
                }

                var dictForPath = _dict[item.PropertyPath];
                if (dictForPath.ContainsKey(item.ReferenceOidProperty))
                {
                    throw new ORMDesignException("06", $"扫描实体引用属性定义情况时添加元素失败，要添加的属性[{item.ReferenceOidProperty.Name}]已经添加过了！");
                }

                dictForPath.Add(item.ReferenceOidProperty, item);
            }
        }

        public IEnumerable<DataEntityReferenceSchemaItem> GetItemsByPropertyPath(string propertyPath)
        {
            return GetItemsByPropertyPath(propertyPath, false);
        }

        public IEnumerable<DataEntityReferenceSchemaItem> GetItemsByPropertyPath(string propertyPath, bool mustHaveReferenceObjectProperty)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                propertyPath = "";
            }

            if (_dict.TryGetValue(propertyPath, out var d1))
            {
                foreach (var item in d1.Values)
                {
                    if (!mustHaveReferenceObjectProperty || item.ReferenceObjectProperty != null)
                    {
                        yield return item;
                    }
                }
            }
        }

        public DataEntityReferenceSchema GetReferenceSchema(IDataEntityType dt, bool onlyDbProperty)
        {
            if (dt == null)
            {
                throw new ORMArgInvalidException("07", "分析一个指定的实体类型获取其引用定义情况失败，要分析的实体类型不能为空！");
            }

            var result = dt.DataEntityReferenceSchema as DataEntityReferenceSchema;
            if (result == null)
            {
                result = Create(dt, onlyDbProperty);
                dt.DataEntityReferenceSchema = result;
            }

            if (result._dict.Count == 0)
            {
                return null;
            }

            return result;
        }

        private DataEntityReferenceSchema Create(IDataEntityType dt, bool onlyDbProperty)
        {
            var schema = new DataEntityReferenceSchema();
            var stack = new PropertyStockNode(dt);
            SearchDataEntityReferenceSchema(dt, schema, stack, onlyDbProperty);
            if (schema._dict.Count == 0)
            {
                return _empty;
            }

            return schema;
        }

        private static void SearchDataEntityReferenceSchema(IDataEntityType dt, DataEntityReferenceSchema schema, PropertyStockNode stock, bool onlyDbProperty)
        {
            foreach (var cp in dt.Properties.GetComplexProperties(false))
            {
                var schemaItem = new DataEntityReferenceSchemaItem();
                var dataEntityProperty = dt.Properties[cp.RefIdPropName];
                if (dataEntityProperty != null)
                {
                    var sp = (ISimpleProperty)dataEntityProperty;
                    schemaItem.ReferenceOidProperty = sp;
                    schemaItem.ReferenceTo = cp.Name;
                    schemaItem.PropertyPath = stock.PropertyPath;
                    schemaItem.ReferenceObjectProperty = cp;
                    schema.Add(schemaItem);
                }
            }

            foreach (var colp in dt.Properties.GetCollectionProperties(onlyDbProperty))
            {
                SearchDataEntityReferenceSchema(colp.ItemType, schema, stock.CreateNextNode(colp), onlyDbProperty);
            }
        }
    }

}