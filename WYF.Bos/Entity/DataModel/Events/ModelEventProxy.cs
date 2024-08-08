using WYF.Bos.Form.control.events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel.Events
{
    public class ModelEventProxy
    {
        protected List<IDataModelListener> modelListeners = new List<IDataModelListener>();

        private SortedSet<IDataModelChangeListener> modelChangeListeners =new SortedSet<IDataModelChangeListener>();

        public void FireCreateNewData(BizDataEventArgs e)
        {
            this.InvokeModelListenerMethod((modelListener) => { modelListener.CreateNewData(e); }, "CreateNewData", typeof(BizDataEventArgs));
        }

        private void InvokeModelListenerMethod(Action<IDataModelListener> action,string methodName,   Type parameterType) {

            foreach (IDataModelListener item in modelListeners)
            {
                action(item);
            }
        }

        public void FireGetEntityType(GetEntityTypeEventArgs e)
        {
            this.InvokeModelListenerMethod((modelListener) => { modelListener.GetEntityType(e); }, "GetEntityType", typeof(GetEntityTypeEventArgs));

          
        }


        public void FireAfterCreateNewData(EventObject e)
        {
            //foreach (IDataModelListener l in this.modelListeners)
            //{
            //    l.AfterCreateNewData(e);
            //}

            this.InvokeModelListenerMethod(
                (modelListener) => { modelListener.AfterCreateNewData(e); }, 
                "AfterCreateNewData", 
                typeof(BizDataEventArgs)
                );


        }


        public void FireBeforePopertyChanged(PropertyChangedArgs e)
        {
            foreach (IDataModelChangeListener l in modelChangeListeners)
            {
                l.BeforePropertyChanged(e);
            }
        
        }
        public void FirePopertyChanged(PropertyChangedArgs e)
        {
            foreach (IDataModelChangeListener l in modelChangeListeners)
            {
                foreach (var changeData in e.ChangeSet)
                {
                    PropertyChangedArgs singleArg = new PropertyChangedArgs(e.Property, new ChangeData[] { changeData });
                    l.PropertyChanged(singleArg);
                }
            }
        }
        

            public void AddDataModelListener(IDataModelListener l)
        {
            this.modelListeners.Add(l);
        }

        public void AddDataModelChangeListener(IDataModelChangeListener l)
        {
            this.modelChangeListeners.Add(l);
        }

    }
}
