using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.DataEntity.Metadata
{
    /// <summary>
    /// 复杂属性，指向一个复杂结构
    /// 例如你可能希望"联系人"实体包含个人联系方式和公司联系方式，而每种联系方式都有很多的子属性，这种情况下你就可以使用复杂属性
    /// </summary>
    public interface IComplexProperty : IDataEntityProperty
    {
        /// <summary>
        /// 返回此属性的实体类型，此属性指向一个实体类型
        /// </summary>
        /// <returns></returns>
        public IDataEntityType ComplexType { get;}
        /// <summary>
        /// 专用于LoadRefence赋值
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <param name="value"></param>
        void LoadValue(object dataEntity, object value)
        {
            SetValueFast(dataEntity, value);
        }
        /// <summary>
        /// 多类别基础资料按数据行获取类型,使子类可以按所在行基础资料类型数据返回相关数据
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        IDataEntityType GetComplexType(object dataEntity)
        {
            return this.ComplexType;
        }
        /// <summary>
        /// 引用属性字段名
        /// </summary>
        /// <returns></returns>
        public  string RefIdPropName { get;}
        /// <summary>
        /// 获取字段类型与值的映射
        /// </summary>
        /// <param name="dataEntities"></param>
        /// <returns></returns>
        Dictionary<Tuple<IComplexProperty, IDataEntityType>, List<object>> GetDataEntityWalkerItems(List<object> dataEntities)
        {
            Dictionary<Tuple<IComplexProperty, IDataEntityType>, List<object>> mapItems = new Dictionary<Tuple<IComplexProperty, IDataEntityType>, List<object>>(1);
            List<object> temp = new List<object>();
            foreach (object item in dataEntities)
            {
                object value = GetValue(item);
                if (value != null)
                    temp.Add(value);
            }
            mapItems.Add(new Tuple<IComplexProperty, IDataEntityType>(this, this.ComplexType), temp);
            return mapItems;
        }
    }

}
