using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.Orm.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Entity
{
    public class DataEntityReferenceList : List<DataEntityReference>
    {
        private ConcurrentDictionary<object, DataEntityReference> _dict;
        private string _dataEntityTypeName;
        private IDataEntityType _dataEntityType;

        public DataEntityReferenceList(IDataEntityType dataEntityType)
        {
            _dataEntityType = dataEntityType ?? throw new ORMArgInvalidException("100001", "构造实体引用列表DataEntityReferenceList失败，构造参数不能为空！");
            _dict = new ConcurrentDictionary<object, DataEntityReference>();
        }

        public DataEntityReferenceList(string dataEntityTypeName)
        {
            _dataEntityTypeName = string.IsNullOrEmpty(dataEntityTypeName) ? throw new ORMArgInvalidException("100001", "构造实体引用列表DataEntityReferenceList失败，构造参数不能为空！") : dataEntityTypeName;
            _dict = new ConcurrentDictionary<object, DataEntityReference>();
        }

        public string DataEntityTypeName => _dataEntityTypeName ?? _dataEntityType?.Name;

        public IDataEntityType DataEntityType => _dataEntityType;

        public new void Add(DataEntityReference item)
        {
            AddToDictionary(item);
            base.Add(item);
        }

        public new void Insert(int index, DataEntityReference item)
        {
            AddToDictionary(item);
            base.Insert(index, item);
        }

        private void AddToDictionary(DataEntityReference item)
        {
            if (item == null || item.Oid == null)
                throw new ORMArgInvalidException("100001", "添加实体失败,试图往引用列表DataEntityReferenceList中添加空实体或主键值为空的实体！");
            _dict.TryAdd(item.Oid, item);
        }

        public DataEntityReference AddId(object oid)
        {
            if (oid == null)
                throw new ORMArgInvalidException("100001", "添加实体失败,试图往引用对象DataEntityReference中添加空实体！");
            if (!_dict.TryGetValue(oid, out var item))
            {
                item = new DataEntityReference(oid);
                Add(item);
            }
            return item;
        }

        public new void Clear()
        {
            base.Clear();
            _dict.Clear();
        }

        public bool TryGet(object oid, out DataEntityReference item)
        {
            return _dict.TryGetValue(oid, out item);
        }

        public new bool Contains(DataEntityReference item)
        {
            return item != null && item.Oid != null && _dict.ContainsKey(item.Oid);
        }

        public object[] GetOids()
        {
            return this.Select(item => item.Oid).ToArray();
        }

        public object[] GetNotLoadedOids()
        {
            return this.Where(item => !item.IsDataEntityLoaded).Select(item => item.Oid).ToArray();
        }

        public List<DataEntityReference> GetNotLoadedTasks()
        {
            return this.Where(item => !item.IsDataEntityLoaded).ToList();
        }

        public new bool Remove(DataEntityReference item)
        {
            if (item == null || item.Oid == null)
                return false;
            _dict.TryRemove(item.Oid, out _);
            return base.Remove(item);
        }
    }
}
