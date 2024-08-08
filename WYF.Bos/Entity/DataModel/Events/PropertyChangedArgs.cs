using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel.Events
{
    public class PropertyChangedArgs
    {
        private ChangeData[] _changeSet;

        private IDataEntityProperty _property;

        public PropertyChangedArgs(IDataEntityProperty property, ChangeData[] changeSet)
        {
            this._property = property;
            this._changeSet = changeSet;
        }

    
        public ChangeData[] ChangeSet
        {
            get { return _changeSet; }
        }




    public IDataEntityProperty Property
        {
            get { return _property; }
        }
    }
}
