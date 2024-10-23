
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using WYF.DataEntity.Metadata.Clr;
using WYF.DataEntity.Metadata;
using WYF.DataEntity.Metadata.Dynamicobject;
using Newtonsoft.Json.Linq;

namespace WYF.DataEntity.Entity
{
    public class BoolDataEntityState: DataEntityState
    {
        
        private BitArray _dirtyArray;
        private DataEntityPropertyCollection _properties;
        private static readonly IEnumerable<IDataEntityProperty> EmptyDataEntityPropertyArray = new IDataEntityProperty[0];
        /// <summary>
        /// 返回整个实体是否已发生变更
        /// </summary>
        public override bool DataEntityDirty
        {
            get
            {
                for (int i = 0; i < this._dirtyArray.Length; i++)
                {
                    if (this._dirtyArray[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="dirtyArray"></param>
        /// <param name="pkSnapshots"></param>
        /// <param name="fromDatabase"></param>
        public BoolDataEntityState(DataEntityPropertyCollection properties, BitArray dirtyArray, PkSnapshot[] pkSnapshots, bool fromDatabase) : base(pkSnapshots, fromDatabase)
        {
            this._properties = properties;
            this._dirtyArray = dirtyArray;
        }
        /// <summary>
        /// 获取实体的脏标志位值
        /// </summary>
        /// <returns></returns>
        public override  int[] GetDirtyFlags()
        {
            int length = this._dirtyArray.Length;
            if (length <= 0)
            {
                return null;
            }
            int num2 = ((length - 1) / 0x20) + 1;
            int[] array = new int[num2];
            this._dirtyArray.CopyTo(array, 0);
            return array;
        }


        //public int[] GetDirtyFlags()
        //{
        //    int n = this._dirtyArray.Length;
        //    if (n <= 0)
        //        return new int[0];
        //    return BitArrayToIntArrayConverter.ConvertBitArrayToIntArray(this._dirtyArray);

        //}

        /// <summary>
        /// 返回指定实体中所有变更的属性列表
        /// 这里判断的是实体自创建或从数据库读取后，发生更改的属性列表。
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IDataEntityProperty> GetDirtyProperties()
        {
            return this.GetDirtyProperties(false);
        }
        /// <summary>
        /// 返回指定实体中所有变更的属性列表
        /// </summary>
        /// <param name="includehasDefualt">是否包含默认值</param>
        /// <returns></returns>
        public override IEnumerable<IDataEntityProperty> GetDirtyProperties(bool includehasDefualt)
        {
            List<IDataEntityProperty> list = new List<IDataEntityProperty>();
            IDataEntityProperty item = null;
            for (int i = 0; i < this._dirtyArray.Length; i++)
            {
                item = this._properties[i];
                if (this._dirtyArray[i] || (includehasDefualt && item.HasDefaultValue))
                {
                    list.Add(item);
                }
            }
            return list;
        }
        /// <summary>
        /// 设置实体的所有脏标志位
        /// </summary>
        /// <param name="newValue">要设置脏标志位</param>
        public override void SetDirty(bool newValue)
        {
            this._dirtyArray.SetAll(newValue);
        }
        /// <summary>
        /// 设置实体属性的脏标志
        /// </summary>
        /// <param name="prop">实体属性</param>
        /// <param name="newValue">要设置脏标志位</param>
        public void SetDirty(ISimpleProperty prop, bool newValue)
        {
            this._dirtyArray.Set(prop.Ordinal, newValue);
        }
        /// <summary>
        /// 设置某个属性已经发生改变
        /// </summary>
        /// <param name="e">属性改变事件参数</param>
        public override void SetPropertyChanged(PropertyChangedEventArgs e)
        {
            DataEntityPropertyChangedEventArgs args = e as DataEntityPropertyChangedEventArgs;
            if (args == null)
            {
                IDataEntityProperty property;
               
                if (((e != null) && !string.IsNullOrEmpty(e.PropertyName)) && this._properties.TryGetValue(e.PropertyName, out property))
                {
                    this._dirtyArray[property.Ordinal] = true;
                }
            }
            else if (!args.IsErrorRaise)
            {
                this._dirtyArray[args.Property.Ordinal] = true;
            }
        }

        public override void SetDirtyFlags(int[] values)
        {
            this._dirtyArray = new BitArray(values);
        }

      
    }
}
