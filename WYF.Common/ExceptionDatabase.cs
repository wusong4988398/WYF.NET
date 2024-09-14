using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Common
{
    [Serializable]
    public class ExceptionDatabase : WYFException
    {
        // Fields
        private string mSqlSource;

        // Methods
        public ExceptionDatabase(string sCode, string sSqlSource, string sMessage) : base(sCode, sMessage)
        {
            this.mSqlSource = string.Empty;
            this.mSqlSource = base.Source = sSqlSource;
            base.Data.Add("Sql语句：", sSqlSource);
        }

        public ExceptionDatabase(string sCode, string sSqlSource, string sMessage, Exception innerException) : base(sCode, sMessage, innerException)
        {
            this.mSqlSource = string.Empty;
            if (innerException != null)
            {
                string str;
                base.Source = str = sSqlSource;
                this.mSqlSource = innerException.Source = str;
            }
            else
            {
                this.mSqlSource = base.Source = sSqlSource;
            }
            base.Data.Add("Sql语句：", sSqlSource);
        }

        public override string GetErrMsg()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("发生时间：" + "\t" + base.Date.ToString("yyyy-MM-dd hh:mm:ss") + "\r\n");
            builder.Append("错误编号：" + "\t" + base.Code + "\r\n");
            builder.Append("错误信息：" + "\t" + this.Message + "\r\n");
            builder.Append("Sql语句：" + "\t" + this.SqlSource + "\r\n");
            builder.Append("===================================================\r\n");
            builder.Append("调用堆栈：\r\n");
            builder.Append(this.StackTrace);
            return builder.ToString();
        }

        // Properties
        public string SqlSource
        {
            get
            {
                return this.mSqlSource;
            }
            set
            {
                this.mSqlSource = value;
            }
        }
    }
}
