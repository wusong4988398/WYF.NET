using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.DataEntity.Metadata.Clr;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel
{
    public class DirtyManager
    {
        private AbstractFormDataModel _model;
        public DirtyManager(AbstractFormDataModel model)
        {
            this._model = model;
        }
        public void ClearDirty()
        {
            this.ClearDataEntityDirty(this._model.GetDataEntity(true));
        }

        private void ClearDataEntityDirty(DynamicObject dataEntity)
        {
            if (dataEntity != null)
            {
                IDataEntityType dt = dataEntity.GetDataEntityType();
                if (!dataEntity.DataEntityState.IsFromDatabase)
                {
                    dataEntity.DataEntityState.SetBizChanged(false);
                }
                List<IComplexProperty> cpList= dt.Properties.GetComplexProperties(false);
                foreach (var property in cpList)
                {
                   object tempVar2= property.GetValueFast(dataEntity);
                   //DynamicObject value = (DynamicObject)((tempVar2 is DynamicObject) ? tempVar2 : null);
                   DynamicObject value = property.GetValueFast(dataEntity) as DynamicObject;
                   this.ClearDataEntityDirty(value);

                }
                List<ICollectionProperty> cpList2 = dt.Properties.GetCollectionProperties(false);
                foreach (ICollectionProperty property in cpList2)
                {
                   object propValue = dataEntity.DataStorage.getLocalValue(property);
                    if (propValue != null)
                    {
                        //if (property is MulBasedataProp && !(property.Parent is MainEntityType)) {
                        //    this.ClearMulBasedataPropDirty((MulBasedataProp)property, (DynamicObjectCollection)propValue, false);
                        //}
                        if (propValue is IEnumerable)
                        {
                            IEnumerable enumerable =propValue as IEnumerable;
                            if (enumerable!=null)
                            {
                                foreach (var item in enumerable)
                                {
                                    if (item is DynamicObject)
                                    {
                                        ClearDataEntityDirty((DynamicObject)item);
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }


        public void SetBizChanged(bool value)
        {
            DynamicObject dataEntity = this._model.GetDataEntity();
            if (value)
            {
                dataEntity.DataEntityState.SetBizChanged(value);
            }
            else
            {
                dataEntity = this._model.GetDataEntity(true);
                SetBizChanged(dataEntity, value);
            }
        }

        private void SetBizChanged(DynamicObject dataEntity, bool value)
        {
            IDataEntityType dt = dataEntity.GetDataEntityType();
            dataEntity.DataEntityState.SetBizChanged(value);
            foreach (var property in dt.Properties.GetCollectionProperties(false))
            {
                DynamicObjectCollection entrys = (DynamicObjectCollection)property.GetValueFast(dataEntity);
                foreach (DynamicObject item in entrys)
                {
                    SetBizChanged(item, value);
                }
            }
           
        }

    }
}
