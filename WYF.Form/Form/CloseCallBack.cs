
using WYF.Form.plugin;

namespace WYF.Form
{ 
    /// <summary>
    /// 界面关闭回调参数
    /// 父界面打开一个子界面，用户在子界面上完成了操作后关闭，回调通知父界面接收子界面返回的数据  示例 打开子界面时设置回调参数：
    ///  CloseCallBack callBack = new CloseCallBack(this, KEY_LARGETEXT1);   // 由本插件处理子界面的回调
//  showParameter.setCloseCallBack(callBack);
/// </summary>
    [Serializable]
    public class CloseCallBack
    {
        

        public string ActionId { get; set; }
        public string ClassName { get; set; }
        public string ControlKey { get; set; }

        public CloseCallBack()
        {

        }
        public CloseCallBack(IFormPlugin plugin, String actionId): this(plugin.PluginName, actionId)
        {
            
        }
        public CloseCallBack(String callbackClassName, String actionId)
        {
            this.ClassName = callbackClassName;
            this.ActionId = actionId;
        }

    }
}
