using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel
{
    public class DefaultValueCalculator
    {
        protected IDataModel model;

        protected DynamicObject dataEntity;

        protected IDataEntityProperty fieldProp;

        public Object GetValue(IDataEntityProperty property, object defVal)
        {
            this.model = null;
            this.fieldProp = property;
            Object ret = defVal;
            if (defVal is String) {
                switch ((String)defVal)
                {
                    case "#CurrentDate#":
                    case "#CurrentDatetime#":
                    case "#CurrentUser#":
                    case "#CurrentOrg#":
                        //ret = GetVariableValue((String)defVal);
                        return ret;
                }
            }
            return ret;
        }
    }
}
