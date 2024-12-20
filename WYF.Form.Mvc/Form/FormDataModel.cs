﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Dynamicobject;
using WYF.Entity.DataModel;
namespace WYF.Mvc.Form
{
    public class FormDataModel : AbstractFormDataModel
    {
        public bool IsCacheExpireAfter { get; set; }
        public FormDataModel(string entityName, string pageId, Dictionary<Type, object> services, string appId, string permissionItem) : base(entityName, pageId, services, appId, permissionItem)
        {

        }

        public FormDataModel(string entityName, string pageId, Dictionary<Type, object> services) : base(entityName, pageId, services)
        {

        }

        public override DynamicObject LoadReferenceData(DynamicObjectType dt, object pkValue)
        {
            return base.LoadReferenceData(dt, pkValue);
        }

        public override Dictionary<object, DynamicObject> LoadReferenceDataBatch(DynamicObjectType dt, object[] pkValues)
        {
            return base.LoadReferenceDataBatch(dt, pkValues);
        }
    }
}
