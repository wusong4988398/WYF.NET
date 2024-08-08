using WYF.Bos.Form.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form
{
    /// <summary>
    /// 子界面关闭后的回调处理接口
    /// 父界面的插件可以实现此接口，在打开的子界面关闭时，处理子界面关闭回调逻辑
    /// </summary>
    public interface ICloseCallBack
    {
        void ClosedCallBack(ClosedCallBackEvent closedCallBackEvent);
    }
}
