
using WYF.DataEntity.Metadata;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace WYF.DataEntity.Entity
{
    /// <summary>
    /// 数据包的状态，描述和管理单个实体的脏标志
    /// </summary>
    [Serializable]
    public abstract class DataEntityState
    {

        private bool _fromDatabase;
        private PkSnapshotSet _pkSnapshotSet;
      
        private BitArray _bizChanged=new BitArray(0);
        private IDictionary<string, EntryInfo> _entryInfos;
        /// <summary>
        /// 是否移除对象标记
        /// </summary>
        private bool removedItems;

        protected DataEntityState()
        {
        }

        protected DataEntityState(PkSnapshot[] pkSnapshots, bool fromDatabase)
        {
            if ((pkSnapshots != null) && (pkSnapshots.Length > 0))
            {
                PkSnapshotSet set = new PkSnapshotSet(pkSnapshots.Length);
                set.Snapshots.AddRange(pkSnapshots);
                this._pkSnapshotSet = set;
            }
            this._fromDatabase = fromDatabase;
        }
        /// <summary>
        /// 返回指定实体中所有变更的属性列表
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IDataEntityProperty> GetDirtyProperties();
        /// <summary>
        /// 返回指定实体中所有变更的属性列表
        /// </summary>
        /// <param name="includehasDefualt">是否包含默认值</param>
        /// <returns></returns>
        public abstract IEnumerable<IDataEntityProperty> GetDirtyProperties(bool includehasDefualt);
        public abstract void SetDirty(bool newValue);

       

        /// <summary>
        /// 设置此实体是否是从数据库中加载过来
        /// </summary>
        /// <param name="value">是否是从数据库中加载过来</param>
        /// <param name="clearDirty">清除脏标识</param>
        public void SetFromDatabase(bool value, bool clearDirty)
        {
            this._fromDatabase = value;
            if (clearDirty)
            {
                this.IsFromDatabase = value;
            }
            
        }

        public  void SetFromDatabase(bool value)
        {
            this._fromDatabase = value;
            if (this._fromDatabase)
            {
                SetDirty(false);
                removedItems = false;
                for (int i = 0; i < this._bizChanged.Length; i++)
                {
                    this._bizChanged.Set(i, false);
                }
                    
            }
        }
       
    
    /// <summary>
    /// 是否移除对象标记
    /// </summary>
    public bool RemovedItems
        {
            get { return this.removedItems; }
            set { this.removedItems = value; }
        }

        public IDictionary<string, EntryInfo> EntryInfos
        {
            get { return this._entryInfos; }
            set { this._entryInfos = value; }
        }

        public  void SetBizChangeFlags(int[] values)
        {
           
            this._bizChanged = new BitArray(values);
        }


        public  int[] GetBizChangeFlags()
        {
            return BitArrayToIntArrayConverter.ConvertBitArrayToIntArray(this._bizChanged);
        }

        public  void SetBizChanged(bool value)
        {
            if (value)
            {
                this._bizChanged.Set(1, true);
            }
            else
            {
                for (int i = 1; i < this._bizChanged.Count; ++i)
                {
                    this._bizChanged.Set(i, false);
                }
            }

        }

        public void SetEntryRowCount(string entryName, int rowCount)
        {
            if (this.EntryInfos == null)
                this.EntryInfos = new Dictionary<string, EntryInfo>();
            this.EntryInfos.TryGetValue(entryName,out EntryInfo? entryInfo);
            if (entryInfo == null)
            {
                entryInfo = new EntryInfo();
                this.EntryInfos[entryName]=entryInfo;
            }
            entryInfo.RowCount = rowCount;

        }

        public void SetEntryPageSize(string entryName, int pageSize)
        {
            if (this.EntryInfos == null)
                this.EntryInfos = new Dictionary<string, EntryInfo>();
            this.EntryInfos.TryGetValue(entryName,out EntryInfo ? entryInfo);
            if (entryInfo == null)
            {
                entryInfo = new EntryInfo();
                this.EntryInfos[entryName]=entryInfo;
            }
            entryInfo.PageSize = pageSize;
        }
        public abstract int[] GetDirtyFlags();
        public abstract void SetDirtyFlags(int[] values);
        public void SetEntryStartRowIndex(string entryName, int startRowIndex)
        {
            if (this.EntryInfos == null)
                this.EntryInfos = new Dictionary<string, EntryInfo>();
            this.EntryInfos.TryGetValue(entryName, out EntryInfo? entryInfo);
            if (entryInfo == null)
            {
                entryInfo = new EntryInfo();
                this.EntryInfos[entryName] = entryInfo;
            }
            entryInfo.StartRowIndex = startRowIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public abstract void SetPropertyChanged(PropertyChangedEventArgs e);

        /// <summary>
        /// 设置实体属性的脏标志
        /// </summary>
        /// <param name="prop">实体属性</param>
        /// <param name="newValue">要设置脏标志位</param>
        public void setDirty(ISimpleProperty prop, bool newValue) { }
        /// <summary>
        /// 返回整个实体是否已发生变更
        /// </summary>
        public abstract bool DataEntityDirty { get; }
        /// <summary>
        /// 返回此实体是否是从数据库中加载过来
        /// </summary>
        public bool IsFromDatabase
        {
            get
            {
                return this._fromDatabase;
            }
            protected internal set
            {
                this._fromDatabase = value;
                if (this._fromDatabase)
                {
                    this.SetDirty(false);
                    this.RemovedItems = false;
                    for (int i = 0; i < this._bizChanged.Count; ++i)
                    {
                        this._bizChanged.Set(i, false);
                    }
                }
              
            }
        }



        /// <summary>
        /// 返回此实体携带的快照对象，可能返回null
        /// </summary>
        protected internal PkSnapshotSet PkSnapshotSet
        {
            get
            {
                return this._pkSnapshotSet;
            }
            set
            {
                this._pkSnapshotSet = value;
            }
        }
    }
}
