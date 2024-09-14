using WYF.DataEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel.Events
{
    public class ChangeData : RowDataEntity
    {
        private object _oldValue;

        private object _newValue;

        public ChangeData(int rowIndex, DynamicObject dataEntity, object oldValue, object newValue) : base(rowIndex, dataEntity)
        {

            this._oldValue = oldValue;
            this._newValue = newValue;
        }




        public object OldValue
        {
            get { return _oldValue; }
        }

        public object NewValue
        {
            get { return _newValue; }
        }

    }
}
