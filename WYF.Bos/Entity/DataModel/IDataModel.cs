using WYF.Bos.DataEntity.Entity;
using WYF.Bos.DataEntity.Metadata;
using WYF.Bos.Entity.DataModel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.DataModel
{
    /// <summary>
    /// 动态表单运行时数据模型层访问、控制接口
    /// 本接口提供方法控制动态表单运行时数据模型层，单据、基础资料也继承了此接口。
    /// 插件代码可使用 this.getModel()获取此接口实例
    /// </summary>

    public interface IDataModel: IDataProvider,ISupportInitialize
    {
        void PutContextVariable(String name, Object value);
        /// <summary>
        /// 创建空的数据包
        /// 所谓的空白数据包，实际上并不是完全空白，各个字段已经设置了默认值；单据体也已经创建了默认行
        /// </summary>
        /// <returns></returns>
        object CreateNewData();

        /// <summary>
        /// 新建数据包
        /// 
        /// </summary>
        /// <param name="newObject">动态表单数据包；如果传入了此参数，将把数据包绑定到界面上</param>
        /// <returns></returns>
        object CreateNewData(DynamicObject newObject);

        /// <summary>
        /// 获取根数据包,当在缓存情况下不包含分录
        /// </summary>
        /// <returns></returns>
        DynamicObject GetDataEntity();
        /// <summary>
        /// 获取根数据包，可以指定在缓存情况是否含分录
        /// </summary>
        /// <param name="includeEntry">是否包含分录，设置为true会从缓存中恢复所有分录行放在根数据包中</param>
        /// <returns></returns>
        DynamicObject GetDataEntity(bool includeEntry);

        /// <summary>
        /// 获取字段在主实体中对应的属性对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IDataEntityProperty GetProperty(String name);

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="dataEntity">数据包</param>
        /// <param name="value">值</param>
        void SetValue(IDataEntityProperty prop, DynamicObject dataEntity, Object value);


        /// <summary>
        /// 获取上下文变量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        object GetContextVariable(string name);

        /// <summary>
        /// 新增数据包初始化事件
        /// </summary>
        /// <param name="listener"></param>
        void AddDataModelListener(IDataModelListener listener);


        void AddDataModelChangeListener(IDataModelChangeListener listener);


    }
}
