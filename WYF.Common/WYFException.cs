using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Common
{
    [Serializable]
    public class WYFException : Exception
    {

        private IDictionary data;
        private string mCode;
        private DateTime mDate;


        public WYFException(string Code, string Message) : this(Code, Message, (Exception)null)
        {
        }

        public WYFException(string Code, string Message, bool isWriteLog) : this(Code, Message, (Exception)null)
        {
            this.IsWriteLog = isWriteLog;
        }

        public WYFException(string Code, string Message, Exception innerException) : base(Message, innerException)
        {
            this.mDate = DateTime.Now;
            this.mCode = Code;
            if ((innerException != null) && !(innerException is WYFException))
            {
                this.IsNormalException = true;
            }
            this.data = new Dictionary<string, string>();
            this.data.Add("发生时间：", this.mDate.ToLongTimeString());
            this.data.Add("错误编号：", this.mCode);
            this.data.Add("错误来源：", this.Source);
            this.data.Add("错误信息：", Message);
            this.data.Add("调用堆栈：", this.StackTrace);
        }

        public WYFException(string Code, string Message, Exception innerException, bool isWriteLog) : this(Code, Message, innerException)
        {
            this.IsWriteLog = isWriteLog;
            if ((innerException != null) && !(innerException is WYFException))
            {
                this.IsNormalException = true;
            }
        }

        public virtual string GetErrMsg()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("发生时间：" + "\t" + this.mDate.ToLongTimeString() + "\r\n");
            builder.Append("错误编号：" + "\t" + this.mCode + "\r\n");
            builder.Append("错误信息：" + "\t" + this.Message + "\r\n");
            builder.Append("===================================================\r\n");
            builder.Append("调用堆栈：" + "\r\n");
            builder.AppendLine(this.StackTrace);
            if (base.InnerException != null)
            {
                builder.AppendLine(base.InnerException.StackTrace);
            }
            return builder.ToString();
        }

        public virtual string GetErrorInfo()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("发生时间：" + "\t" + this.mDate.ToLongTimeString() + "\r\n");
            builder.Append("错误编号：" + "\t" + this.mCode + "\r\n");
            builder.Append("错误信息：" + "\t" + this.Message + "\r\n");
            int stackTraceLevel = 100;
            switch (stackTraceLevel)
            {
                case 0:
                    break;

                case 100:
                    builder.Append("===================================================\r\n");
                    builder.Append("调用堆栈：" + "\r\n");
                    builder.Append(this.StackTrace);
                    if (base.InnerException != null)
                    {
                        builder.Append(base.InnerException.StackTrace);
                    }
                    break;

                default:
                    builder.Append("===================================================\r\n");
                    builder.AppendLine(string.Format("StackTraceLevel:{0}", stackTraceLevel));
                    break;
            }
            return builder.ToString();
        }

        public static WYFException GetException(Exception innerException)
        {
            string code = Guid.NewGuid().ToString().Replace("-", "");
            Exception exception = innerException;
            if (!(exception is WYFException))
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    if (exception is WYFException)
                    {
                        break;
                    }
                }
            }
            if (exception is WYFException)
            {
                WYFException exception2 = exception as WYFException;
                exception2.Code = code;
                return exception2;
            }
            return new WYFException(code, exception.Message, exception, true);
        }

        public string Code
        {
            get
            {
                return this.mCode;
            }
            set
            {
                this.mCode = value;
            }
        }

        public override IDictionary Data
        {
            get
            {
                return this.data;
            }
        }

        public DateTime Date
        {
            get
            {
                return this.mDate;
            }
        }

        public bool IsNormalException { get; private set; }

        public bool IsWriteLog { get; set; }
    }
}
