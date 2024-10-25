using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Form.events
{
    /// <summary>
    /// 关闭回调事件参数
    /// </summary>
    public class ClosedCallBackEvent : EventObject
    {
        /// <summary>
        /// 回调标识：由发起者自定义，以便和其他回调来源进行区分
        /// </summary>
        public string ActionId { get; set; }
        /// <summary>
        /// 回调返回数据
        /// </summary>
        public object ReturnData { get; set; }
        /// <summary>
        /// 表单视图
        /// </summary>
        public IFormView View { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">事件源</param>
        /// <param name="actionId">回调标识：由发起者自定义，以便和其他回调来源进行区分</param>
        /// <param name="ret">回调返回数据</param>
        public ClosedCallBackEvent(object obj, string actionId, object ret) : base(obj)
        {
            this.ActionId = actionId;
            this.ReturnData = ret;
        }
    }
}
