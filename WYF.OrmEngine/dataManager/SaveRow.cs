using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata;

namespace WYF.OrmEngine.dataManager
{
    public class SaveRow : ISaveMetaRow
    {
        public ForWriteList<IColumnValuePair> DirtyValues { get; set; }

        public IColumnValuePair Oid { get; set; }

        public RowOperateType Operate { get; set; }
       

        public List<IColumnValuePair> OutputValues { get; set; }
        public IColumnValuePair ParentOid {  get; set; }    
        public IColumnValuePair Version { get; set; }

        public SaveRow()
        {
            this.Operate = RowOperateType.Unknow;
            this.DirtyValues = new ForWriteList<IColumnValuePair>();
            this.OutputValues = new List<IColumnValuePair>();
        }

        public void RebuildOutputValues()
        {
            List<IColumnValuePair> oldList = this.OutputValues;
            if (oldList!=null)
            {
                List<IColumnValuePair> newList = new List<IColumnValuePair>(oldList.Count);
                if (this.Operate== RowOperateType.Update)
                {
                    foreach (var item in oldList)
                    {
                        if (item.Column.AutoSync==AutoSync.OnUpdate&&item.Column.Name==this.Oid.Column.Name)
                        {
                            newList.Add(item);
                        }
                    }
                }
                else if(this.Operate == RowOperateType.Insert)
                {
                    foreach (var item in oldList)
                    {
                        if (item.Column.AutoSync == AutoSync.OnInsert)
                        {
                            newList.Add(item);
                        }
                    }
                }
                this.OutputValues=newList;
            }
        }

    }
}
