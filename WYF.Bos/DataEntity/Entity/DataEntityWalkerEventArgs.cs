using WYF.DataEntity.Metadata;
using WYF.Bos.Form.control.events;
using WYF.Bos.Orm.Exceptions;

namespace WYF.DataEntity.Entity
{
    public class DataEntityWalkerEventArgs : EventObject
    {
        public int Count { get; set; }

        public List<Object> DataEntities { get; set; }

        public IDataEntityType DataEntityType { get; set; }

        public PropertyStockNode PropertyStock { get; set; }

        public DataEntityWalkerEventArgs(object source) : base(source)
        {
        }

        public static void DataEntityWalker(List<Object> dataEntities, IDataEntityType dt, Action<DataEntityWalkerEventArgs> callback, bool onlyDbProperty)
        {
            if (dataEntities == null)
                throw new ORMArgInvalidException("100001", "构造搜索器DataEntityWalker失败,构造参数:实体dataEntities不能为空！");
            if (dt == null)
                throw new ORMArgInvalidException("100001", "构造搜索器DataEntityWalker失败,构造参数:实体类型dt不能为空！");
            if (callback == null)
                throw new ORMArgInvalidException("100001", "构造搜索器DataEntityWalker失败,构造参数:回调函数callback不能为空！");
            List<Object> list = new List<Object>();
            list.AddRange(dataEntities);
            PropertyStockNode propertyStack = new PropertyStockNode(dt);
            DataEntityWalker(list, dt, callback, propertyStack, onlyDbProperty);
        }

        private static void DataEntityWalker(List<object> dataEntities, IDataEntityType dt, Action<DataEntityWalkerEventArgs> callback, PropertyStockNode propertyStack, bool onlyDbProperty)
        {
            if (dataEntities.Count == 0)
                return;
            if (onlyDbProperty && dt.IsDbIgnore)
                return;

            DataEntityWalkerEventArgs e = new DataEntityWalkerEventArgs(dataEntities)
            {
                Count = dataEntities.Count,
                DataEntities = dataEntities,
                DataEntityType = dt,
                PropertyStock = propertyStack
            };
            callback.Invoke(e);

            foreach (IComplexProperty cpx in dt.Properties.GetComplexProperties(onlyDbProperty))
            {
                Dictionary<Tuple<IComplexProperty, IDataEntityType>, List<object>> mapTemps = cpx.GetDataEntityWalkerItems(dataEntities);
                foreach (KeyValuePair<Tuple<IComplexProperty, IDataEntityType>, List<object>> tempEntry in mapTemps)
                {
                    DataEntityWalker(tempEntry.Value, tempEntry.Key.Item2, callback, propertyStack.CreateNextNode(tempEntry.Key.Item1, tempEntry.Key.Item2), onlyDbProperty);
                }
            }

            foreach (ICollectionProperty colp in dt.Properties.GetCollectionProperties(onlyDbProperty))
            {
                List<object> temp = new List<object>();
                foreach (object item in dataEntities)
                {
                    temp.AddRange((IEnumerable<object>)colp.GetValue(item));
                }
                DataEntityWalker(temp, colp.ItemType, callback, propertyStack.CreateNextNode(colp), onlyDbProperty);
            }
        }
    }
}
