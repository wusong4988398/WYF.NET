using WYF.DataEntity.Metadata;
using WYF.Bos.Utils;
using JNPF.Form.DataEntity;
using System.Collections;
using WYF.Bos.DataEntity;


namespace WYF.DataEntity.Entity
{
    public class CloneUtils
    {
        private bool isOnlyDbProperty;

        private bool _clearPrimaryKeyValue;

        public CloneUtils(bool onlyDbProperty, bool clearPrimaryKeyValue)
        {
            this.isOnlyDbProperty = onlyDbProperty;
            this._clearPrimaryKeyValue = clearPrimaryKeyValue;
        }
        public Object Clone(IDataEntityBase dataEntity)
        {
            IDataEntityType dt = dataEntity.DataEntityType;
            return Clone(dt, dataEntity);
        }
        public Object Clone(IDataEntityType dt, Object dataEntity)
        {
            return Clone(dt, dataEntity, this._clearPrimaryKeyValue);
        }

        private Object Clone(IDataEntityType dt, Object dataEntity, bool clearPrimaryKeyValue)
        {
            if (dataEntity == null)
                return null;
            Object newEntity = dt.CreateInstance();
            CopyData(dt, dataEntity, newEntity, clearPrimaryKeyValue);
            return newEntity;
        }

        private void CopyData(IDataEntityType dt, Object dataEntity, Object newEntity, bool clearPrimaryKeyValue)
        {
            IDataEntityType dtOld = dt;
            IDataEntityType dtNew = dt;
            if (dataEntity is IDataEntityBase)
            dtOld = ((IDataEntityBase)dataEntity).DataEntityType;
            if (newEntity is IDataEntityBase)
      dtNew = ((IDataEntityBase)newEntity).DataEntityType;
            if (dataEntity is DynamicObject && dtOld == dtNew) {
                ((DynamicObject)newEntity).DataStorage= ((DynamicObject)dataEntity).DataStorage.memberClone();
            }
            else
            {
                foreach (ISimpleProperty sp in dt.Properties.GetSimpleProperties(this.isOnlyDbProperty))
                {
                    IDataEntityProperty dpOldProp = null;
                    TryGetOldProperty((IDataEntityProperty)sp, dtOld, out dpOldProp);
                    if (!sp.IsReadOnly && dpOldProp != null)
                        sp.SetValue(newEntity, dpOldProp.GetValue(dataEntity));
                }
            }
            if (clearPrimaryKeyValue)
            {
                ISimpleProperty pk = dt.PrimaryKey;
                if (pk != null)
                    pk.ResetValue(newEntity);
                dtNew.SetDirty(newEntity, true);
            }

            foreach (IComplexProperty cpx in dt.Properties.GetComplexProperties(this.isOnlyDbProperty))
            {
                if (!string.IsNullOrEmpty(cpx.RefIdPropName))
                    continue;
                IDataEntityProperty dpOldProp = null;
        
                TryGetOldProperty((IDataEntityProperty)cpx, dtOld, out dpOldProp);
                Object cpxValue = dpOldProp.GetValue(dataEntity);
                if (cpxValue != null)
                {
                    Object cpxNewValue;
                    IDataEntityType cpxType;
                    IDataEntityBase cpxValue2 = (cpxValue is IDataEntityBase) ? (IDataEntityBase)cpxValue : null;
            if (cpxValue2 != null)
            {
                cpxType = cpxValue2.DataEntityType;
            }
            else
            {
                cpxType = cpx.ComplexType;
            }
            if (cpx.IsReadOnly)
            {
                cpxNewValue = cpx.GetValue(newEntity);
                if (cpxNewValue == null)
                    throw new ORMDesignException("100001", "哦，真不幸，只读的属性却返回了NULL值。");
                CopyData(cpxType, cpxValue, cpxNewValue, false);
                continue;
            }
            if (cpx.IsDbIgnore)
            {
                cpxNewValue = Clone(cpxType, cpxValue, false);
            }
            else
            {
                cpxNewValue = Clone(cpxType, cpxValue, this._clearPrimaryKeyValue);
            }
            cpx.SetValue(newEntity, cpxNewValue);
        }
    }

            foreach (ICollectionProperty colp in dt.Properties.GetCollectionProperties(this.isOnlyDbProperty))
            {
                IDataEntityProperty dpOldProp = null;
                TryGetOldProperty((IDataEntityProperty)colp, dtOld, out dpOldProp);
            
                Object colpValue = dpOldProp.GetValue(dataEntity);
                if (colp != null && colpValue != null)
                {
                    IEnumerable? colpListValue = colpValue is IEnumerable ? (IEnumerable)colpValue : null;
                    if (colpListValue == null) throw new ORMDesignException("100001", "哦，真不幸，集合的属性返回值不支持枚举。");
                    if (newEntity is DynamicObject)
          ((DynamicObject)newEntity).DataStorage.setLocalValue((IDataEntityProperty)colp, null);
            Object colpNewValue = colp.GetValue(newEntity);
            if (colpNewValue == null)
                if (!colp.IsReadOnly)
                {
                    colpNewValue = TypesContainer.CreateInstance<Object>(colp.PropertyType);
                    colp.SetValue(newEntity, colpNewValue);
                }
                else
                {
                    throw new ORMDesignException("100001", "哦，真不幸，集合的属性返回值为null。");
                }
                    List<object> colpNewListValue = colpNewValue is List<object> ? (List<object>)colpNewValue : null;
                    if (colpNewListValue == null)
                        throw new ORMDesignException("100001", "哦，真不幸，集合的属性返回值不支持IList。");
            colpNewListValue.Clear();
            foreach (Object item in colpListValue)
            {
                IDataEntityType itemType;
                IDataEntityBase itemAsEntity = (item is IDataEntityBase) ? (IDataEntityBase)item : null;
            if (itemAsEntity == null)
            {
                itemType = colp.ItemType;
            }
            else
            {
                itemType = itemAsEntity.DataEntityType;
            }
            colpNewListValue.Add(Clone(itemType, item));
        }
    }
} 

}


        private bool TryGetOldProperty(IDataEntityProperty dp, IDataEntityType dtOldData, out IDataEntityProperty dpOldProperty)
        {
            dpOldProperty = null;
            if (dtOldData == null || dp == null)
                return false;
            return dtOldData.Properties.TryGetValue(dp.Name, out dpOldProperty);
        }


    }
}
