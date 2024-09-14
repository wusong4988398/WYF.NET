using WYF.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel.Events
{
    public class PropChangedTask
    {
        private List<ChangeData> _changeSet = new List<ChangeData>();

        private IDataEntityProperty _property;

        public PropChangedTask(PropertyChangedArgs eventArgs)
        {
            this._property = eventArgs.Property;
            foreach (var item in eventArgs.ChangeSet)
            {
                this._changeSet.Add(item);
            }
        }

        public IDataEntityProperty Property { get { return _property; } }

        public List<ChangeData> ChangeSet { get { return _changeSet; } }

    }
}
