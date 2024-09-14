using WYF.DataEntity.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Serialization
{
    /// <summary>
    /// 对象序列化基类
    /// </summary>
    public abstract class DcSerializer
    {
        private DcBinder _binder;
        private bool privateIsLocaleValueFull;
        private bool _serializeComplexProperty;
        /// <summary>
        /// 节点名称和实体类型的映射工具
        /// </summary>
        public DcBinder DcBinder 
        { 
            get { return _binder; } 
            set { _binder = value; }
        }
        /// <summary>
        /// 设置是否序列化复杂属性
        /// </summary>
        public bool IsSerializeComplexProperty
        {
            get { return _serializeComplexProperty; }
            set { _serializeComplexProperty = value; }
        }

        public DcSerializer(DcBinder binder)
        {
            if (binder == null)
            {
                throw new Exception("设置DcSerializer的binder失败，binder不能为空！");
            }
            else
            {
                this._binder = binder;
                this.IsSerializeComplexProperty = true;
            }
        }

        public DcSerializer(IEnumerable<IDataEntityType> dts)
        {
            this.IsSerializeComplexProperty = true;
            throw new NotImplementedException();
            //this._binder = new ListDcxmlBinder(false, dts);
            //this.IsSerializeComplexProperty = true;
        }


    }
}
