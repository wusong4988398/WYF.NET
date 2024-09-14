using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Entity
{

 

    public interface INotifyPropertyChanged1
    {
        
        void RemovePropertyChangeListener(PropertyChangedEventHandler listener);

        void AddPropertyChangeListener(PropertyChangedEventHandler listener);

        PropertyChangedEventHandler[] GetPropertyChangeListeners=> new PropertyChangedEventHandler[0];
       
    }
}
