using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.DataEntity.Metadata.Dynamicobject
{
    public class DataEntityPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        private bool _isErrorRaise;
        private IDataEntityProperty _property;
        public DataEntityPropertyChangedEventArgs(IDataEntityProperty property) : base(property.Name)
        {
            this._property = property;
        }

        public DataEntityPropertyChangedEventArgs(IDataEntityProperty property, bool isErrorRaise) : base(property.Name)
        {
            this._property = property;
            this._isErrorRaise = isErrorRaise;
        }

    

        public bool IsErrorRaise
        {
            get
            {
                return this._isErrorRaise;
            }
        }

        public IDataEntityProperty Property
        {
            get
            {
                return this._property;
            }
        }
    }
}
