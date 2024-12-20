﻿using WYF.DataEntity.Metadata;
using WYF.DataEntity.Serialization;
using WYF.DataEntity.Utils;
using JNPF.Form.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Form.control;
using WYF.DataEntity;


namespace WYF.Form
{
    public class FormDcBinder : DcBinder
    {
        public override void ReadCustomJsonProperty(KeyValuePair<string, object> pair, object entity)
        {
            if (entity is Control) {
                ((Control)entity).CustomProperties[pair.Key] = pair.Value;
            }
        }

        public override IDataEntityType TryBindToType(string elementName, Dictionary<string, string> attributes)
        {
            IDataEntityType ret = ControlTypes.GetDataEntityType(elementName);
            if (ret == null && elementName.Contains("."))
            {
                Type type = TypesContainer.GetOrRegister(elementName);
                if (type != null)
                {
                    ret = OrmUtils.GetDataEntityType(type);
                }
            }

            return ret;
        }
    }
}
