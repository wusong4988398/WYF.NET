using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.Entity.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.field
{
    /// <summary>
    /// 运行时元数据-字段编辑控件的基类，与字段实体ORM属性对象关联起来； 
    /// 封装一批字段编辑控件，以便实现各字段个性化的功能，驱动相应的插件事件。
    /// </summary>
    public class FieldEdit : TipsSupport
    {

        private string _fieldKey;

        private IDataEntityProperty _property;

        /// <summary>
        /// 字段标识
        /// </summary>
        [SimpleProperty]
        public string FieldKey 
        { 
            get 
            {
                if (string.IsNullOrEmpty(this._fieldKey))
                {
                    this._fieldKey = this.Key;
                }
                return this._fieldKey;
            }
            set { this._fieldKey = value; }
        } 
        /// <summary>
        ///字段为分录字段时对于的分录控件的Key,如表格、卡片,为了提高效率，当分录实体的Key和表格空间的Key相等时这里为空
        /// </summary>
        public string EntryKey { get; set; } = string.Empty;

        /// <summary>
        ///字段属性对象
        /// </summary>
        protected IDataEntityProperty? Property
        {
            get
            {
                if (this.view == null) return null;
                if (this._property == null&&!string.IsNullOrEmpty(this.FieldKey))
                {
                    this._property = this.view.GetService<IDataModel>().GetProperty(this.FieldKey);
                }
                return this._property;
            }
            set { this._property = value; }
        }

        public override void BindData(BindingContext bctx)
        {
            Object v = GetBindingValue(bctx);
            this.clientViewProxy.SetFieldProperty(this.Key, "v", v);

        }

        public object GetBindingValue(BindingContext bctx)
        {
            return GetFieldValue(bctx);
        }

        protected object GetFieldValue(BindingContext bctx)
        {
            IDataEntityProperty property = this.Property;
            if (property == null) return null;
            return property.GetValue(bctx.DataEntity);
        }

 

    }
}
