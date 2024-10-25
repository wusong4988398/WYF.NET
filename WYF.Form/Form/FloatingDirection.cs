using System.ComponentModel;

namespace WYF.Form
{
    /// <summary>
    /// 界面显示风格，浮动窗口浮动位置
    /// </summary>
    public enum FloatingDirection
    {
        /// <summary>
        /// 上中
        /// </summary>
        [Description("TopCenter")]
        TopCenter = 0,
        /// <summary>
        /// 左上
        /// </summary>
        TopLeft = 1,
        /// <summary>
        /// 右上
        /// </summary>
        TopRight = 2,
        /// <summary>
        /// 右上
        /// </summary>
        RightTop = 3,
        RightCenter = 4,
        /// <summary>
        /// 右中
        /// </summary>
        RightBottom = 5,
        /// <summary>
        /// 左底
        /// </summary>
        BottomLeft = 6,
        /// <summary>
        /// 底中
        /// </summary>
        BottomCenter = 7,
        /// <summary>
        /// 右底
        /// </summary>
        BottomRight = 8,
        /// <summary>
        /// 左上
        /// </summary>
        LeftTop = 9,
        /// <summary>
        /// 左中
        /// </summary>
        LeftCenter = 10,
        /// <summary>
        /// 左底
        /// </summary>
        LeftBottom = 11
    }
}