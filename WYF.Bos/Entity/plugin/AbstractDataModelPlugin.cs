using WYF.Bos.Entity.DataModel.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Entity.plugin
{
    /// <summary>
    /// 表单运行时模型层插件抽象基类。
    ///实现了运行时模型层插件接口 IDataModelListener、IDataModelChangeListener ，但未提供任何默认实现代码。
    ///通常开发表单界面插件时，不会直接派生本类，而是派生本类的子类 AbstractFormPlugin，该类提供了表单插件的默认内部逻辑。
    /// </summary>
    //public abstract class AbstractDataModelPlugin : IDataModelListener, IDataModelChangeListener
    //{
    //    public AbstractDataModelPlugin()
    //    {

    //    }
    //}
}
