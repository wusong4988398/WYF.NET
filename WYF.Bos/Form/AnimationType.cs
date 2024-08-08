using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form
{
    /// <summary>
    /// 界面显示动画类型
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// 不显示动画
        /// </summary>
        none,
        /// <summary>
        /// 淡入
        /// </summary>
        fadein,
        /// <summary>
        /// 视距推近，逐步放大
        /// </summary>
        zoomin,
        /// <summary>
        /// 淡入且逐步放大
        /// </summary>
        fadeinandzoomin
    }
}
