using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Entity.DataModel.Events
{
    public class DefaultPropChangedContainer : PropChangedContainer
    {
        private IDataModel model;

        private ModelEventProxy modelEventProxy;
        public DefaultPropChangedContainer(IDataModel model, ModelEventProxy modelEventProxy)
        {
            this.model = model;
            this.modelEventProxy = modelEventProxy;
        }
        public bool isSuspend()
        {
            throw new NotImplementedException();
        }

        public void raise(PropertyChangedArgs raiseSource)
        {
            throw new NotImplementedException();
        }

        public void resume()
        {
            throw new NotImplementedException();
        }

        public void suspend()
        {
            throw new NotImplementedException();
        }
    }
}
