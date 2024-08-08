using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel.Events
{
    public interface PropChangedContainer
    {
        void raise(PropertyChangedArgs raiseSource);
        void suspend();

        bool isSuspend();

        void resume();
    }
}
