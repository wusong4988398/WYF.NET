using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Form.control.events
{
    public class ItemClickEvent: EventObject
    {
        /// <summary>
        /// 按钮标识(可能是终止任务、查看详情等子按钮)
        /// </summary>
        public String ItemKey { get; private set; }
        /// <summary>
        /// 当前操作的标识
        /// </summary>
        public String OperationKey { get; private set; }

        public Object Source { get { return this.source; } }

        public  Dictionary<String, Object> ParamsDic { get; set; }

       

        public ItemClickEvent(Object source, String itemKey, String operationKey):base(source)
        {
        
            this.ItemKey = itemKey;
            this.OperationKey = operationKey;
        }

        public ItemClickEvent(Object source, Dictionary<String, Object> paramsMap) : base(source)
        {
         
            this.ParamsDic = paramsMap;
        }


    }
}
