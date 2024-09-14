using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel.Events
{
    public class PropChangedTaskCollection
    {
        private long _maxTaskCount = 100000L;

        private List<PropChangedTask> _tasks;

        private int _head;


        public void Clear()
        {
            if (this._tasks != null)
                this._tasks.Clear();
        }
        public void AddAll(List<PropChangedTask> newTasks)
        {
            if (newTasks == null) return;
            if (this._tasks != null && this._tasks.Count() > this._maxTaskCount)
            {
                StringBuilder sb = new StringBuilder();
                String errMessage = "警告：字段值改变事件调用可能进入了死循环！";
                sb.Append(errMessage);
                sb.Append("可疑字段：");
                int seq = 1;
                foreach (var task in newTasks)
                {
                    if (seq > 5)
                        break;
                    sb.Append(seq).Append(". ").Append(task.Property.Name);
                    seq++;
                }

                throw new Exception(sb.ToString());
            }

            foreach (PropChangedTask task in newTasks)
            {
                if (tryGetTask(task.Property, out PropChangedTask item))
                {
                    if(!object.Equals(task.ChangeSet, item.ChangeSet))
                    {
                        item.ChangeSet.AddRange(task.ChangeSet);
                    }
                }
                if (this._tasks == null)
                    this._tasks = new List<PropChangedTask>();
                this._tasks.Add(task);
            }
        }


        private bool tryGetTask(IDataEntityProperty prop, out PropChangedTask item)
        {
            item = null;
            if (this._tasks != null)
            {
                for (int i = this._head; i < this._tasks.Count; i++)
                {
                    item= this._tasks[i];
                    if (item.Property.Name== prop.Name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


    }
}
