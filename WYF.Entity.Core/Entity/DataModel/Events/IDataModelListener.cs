using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Entity.DataModel.Events
{
    /// <summary>
    /// 表单-运行时模型层-插件事件接口-数据包初始化事件
    /// 表单运行时模型层插件事件接口，提供数据包初始化事件
    /// 插件抽象基类 AbstractFormPlugin 实现了本接口，可基于此抽象基类定义插件
    /// 示例：定义一个表单插件，从插件抽象基类派生：public class DemoFormPlugin extends AbstractFormPlugin {}
    /// </summary>
    public interface IDataModelListener
    {
        /// <summary>
        /// 创建数据包事件，一般在页面初始化时调用
        /// 插件可在此事件，自行构建模型数据包，后续平台将把插件构建的数据包显示在界面上
        /// 如果插件不处理此事件，平台会自行构建空白的模型数据包，并给每个字段填好默认值
        /// </summary>
        /// <param name="e"></param>
        void CreateNewData(BizDataEventArgs e) { }
        /// <summary>
        /// 获取主实体对象时触发此事件
        /// 插件可在此事件，向主实体动态注册新属性，以实现给表单动态添加字段的效果
        /// 特别注意：主实体对象是共享的，不允许直接修改主实体对象本身。必须先复制，然后在拷贝的主实体对象上动态注册新属性
        /// </summary>
        /// <param name="e">事件参数，含默认的主实体对象</param>
        void GetEntityType(GetEntityTypeEventArgs e) { }

        /// <summary>
        /// 数据包创建之后的事件
        /// 插件在此，对已创建好的模型数据包进一步加工，比如调整字段默认值，增加单据体默认行等
        /// public class DemoFormPlugin extends AbstractFormPlugin {}
        /// </summary>
        /// <param name="e"></param>
        void AfterCreateNewData(EventObject e);
    }
}
