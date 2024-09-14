using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Entity.DataModel.Events
{
    /// <summary>
    /// 表单-运行时模型层-插件事件接口-数据改变事件
    /// </summary>
    public interface IDataModelChangeListener
    {

        /// <summary>
        /// 字段值改变前事件
        /// </summary>
        /// <param name="e"></param>
        void BeforePropertyChanged(PropertyChangedArgs e) { }

        /// <summary>
        ///字段值改变后事件
        /// </summary>
        /// <param name="e"></param>
        void PropertyChanged(PropertyChangedArgs e) { }


    }
}
