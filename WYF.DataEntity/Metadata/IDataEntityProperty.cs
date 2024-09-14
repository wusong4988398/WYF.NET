using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{
    /// <summary>
    /// 描述一个实体的属性对象
    /// 在一个实体中包含很多的属性，这些属性都是派生自此接口
    /// </summary>
    public interface IDataEntityProperty : IMetadata
    {
        /// <summary>
        /// 父实体类型
        /// </summary>
        /// <returns></returns>
        IDataEntityType Parent { get; }
        /// <summary>
        ///给定一个实体 获取指定属性的值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        object GetValueFast(object dataEntity);
        /// <summary>
        /// 给定一个实体，为给定实体的属性填充指定的值，在确定dataEntity实体类型和此属性对应实体类型一致时采用此方法
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="value"></param>
        void SetValueFast(object dataEntity, object value);
        /// <summary>
        /// 给定一个实体，读取此属性描述符在此实体的值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        object GetValue(object dataEntity);
        /// <summary>
        /// 为给定实体的属性填充指定的值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="value"></param>
        void SetValue(object dataEntity, object value);
        /// <summary>
        /// 返回此属性的返回类型
        /// </summary>
        /// <returns></returns>
        Type PropertyType { get;  }
        /// <summary>
        /// 返回此属性是否是只读属性
        /// </summary>
        /// <returns></returns>
        bool IsReadOnly { get;  }
        /// <summary>
        /// 返回此属性在引用的实体类型中所在的位置
        /// </summary>
        /// <returns></returns>
        int Ordinal { get;  }
        /// <summary>
        /// 此属性是否有缺省值
        /// </summary>
        /// <returns></returns>
        bool HasDefaultValue { get; }
        /// <summary>
        /// 实体属性是否为空值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        bool IsEmpty(object dataEntity);
        /// <summary>
        /// 属性是否支持Json序列化
        /// </summary>
        /// <returns></returns>
        bool IsJsonSerialize => true;

        /// <summary>
        /// 获取拆分表名
        /// </summary>
        /// <returns></returns>
        string TableGroup => "";
        
        /// <summary>
        /// 是否允许存储空值
        /// </summary>
        /// <returns></returns>
        bool IsEnableNull => false;
        
    }

}
