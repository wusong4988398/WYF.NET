using WYF.Bos.Form.control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form
{
    /// <summary>
    /// 控件支持帮助提示的接口
    /// 在控件上，显示一个问号标志，鼠标移动上去时会显示详细帮助提示，也支持在提示中链接到表单页面。
    /// 字段、容器实现此接口，支持显示帮助提示
    /// </summary>
    public class TipsSupport: Control,ITipsSupport
    {

    }
}
