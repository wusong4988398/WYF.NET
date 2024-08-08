using WYF.Bos.Collections.Generic;
using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.datamanager
{
    internal sealed class SaveRow : ISaveMetaRow
    {

        public SaveRow()
        {
            this.Operate = RowOperateType.Unknow;
            this.DirtyValues = new ForWriteList<IColumnValuePair>();
            this.OutputValues = new List<IColumnValuePair>();
        }

        public void RebuildOutputValues()
        {
            List<IColumnValuePair> outputValues = this.OutputValues;
            if (outputValues != null)
            {
                List<IColumnValuePair> list2 = new List<IColumnValuePair>(outputValues.Count);
                if (this.Operate == RowOperateType.Update)
                {
                    foreach (IColumnValuePair pair in outputValues)
                    {
                        if (((pair.Column.AutoSync & AutoSync.OnUpdate) == AutoSync.OnUpdate) && !pair.Column.Name.Equals(this.Oid.Column.Name))
                        {
                            list2.Add(pair);
                        }
                    }
                }
                else if (this.Operate == RowOperateType.Insert)
                {
                    foreach (IColumnValuePair pair2 in outputValues)
                    {
                        if ((pair2.Column.AutoSync & AutoSync.OnInsert) == AutoSync.OnInsert)
                        {
                            list2.Add(pair2);
                        }
                    }
                }
                this.OutputValues = list2;
            }
        }

        public ForWriteList<IColumnValuePair> DirtyValues { get; internal set; }

        public IColumnValuePair Oid { get; internal set; }

        public RowOperateType Operate { get; internal set; }

        public List<IColumnValuePair> OutputValues { get; set; }

        public IColumnValuePair ParentOid { get; set; }

        public IColumnValuePair Version { get; set; }




    }
}
