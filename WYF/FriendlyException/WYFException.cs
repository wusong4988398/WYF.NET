using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public static class WYFException
    {
        /// <summary>
        /// 抛出字符串异常
        /// </summary>
        /// <param name="errorMessage">异常消息</param>
        /// <param name="args">String.Format 参数</param>
        /// <returns>异常实例</returns>
        public static AppFriendlyException Oh(string errorMessage, string errorCode= "500")
        {
            return new AppFriendlyException(errorMessage, errorCode);
        }
     
    }
}
