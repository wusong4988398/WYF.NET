using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.KSQL.Formater;

namespace WYF.KSQL.Shell.Trace
{
    public class TraceInfo
    {
        // Fields
        public int bindPort;
        public int dbType;
        public string driverName;
        public bool enable;
        public string filter;
        private FormatOptions formatOptions;
        public string logFileUrl;
        public int OptimizeMode;
        public string originalUrl;
        public string password;
        public long tempRecycleTime;
        public string[] tempTableSpaces;
        public bool translate;
        public string url;
        public string userName;

        // Methods
        public TraceInfo(TraceInfo traceInfo)
        {
            this.tempRecycleTime = 0x6ddd00L;
            this.OptimizeMode = 1;
            this.originalUrl = traceInfo.originalUrl;
            this.enable = traceInfo.enable;
            this.logFileUrl = traceInfo.logFileUrl;
            this.filter = traceInfo.filter;
            this.dbType = traceInfo.dbType;
            this.driverName = traceInfo.driverName;
            this.translate = traceInfo.translate;
            this.bindPort = traceInfo.bindPort;
            this.url = traceInfo.url;
            this.tempTableSpaces = traceInfo.tempTableSpaces;
            this.tempRecycleTime = traceInfo.tempRecycleTime;
            this.formatOptions = traceInfo.getFormatOptions().Clone();
        }

        public TraceInfo(int dbType)
        {
            this.tempRecycleTime = 0x6ddd00L;
            this.OptimizeMode = 1;
            this.dbType = dbType;
            this.originalUrl = "";
            this.enable = true;
            this.logFileUrl = "null";
            this.filter = "";
            this.driverName = "";
            this.translate = true;
            this.bindPort = 0;
            this.url = "";
        }

        public TraceInfo(string driverName, int dbType, string originalUrl, bool enable, string logFileUrl, string filter, bool translate, int bindPort, string url)
        {
            this.tempRecycleTime = 0x6ddd00L;
            this.OptimizeMode = 1;
            if (driverName == null)
            {
                throw new ArgumentException("driverName is null");
            }
            this.dbType = dbType;
            this.originalUrl = originalUrl;
            this.enable = enable;
            this.logFileUrl = (logFileUrl == null) ? "null" : logFileUrl;
            this.filter = filter;
            this.driverName = driverName;
            this.translate = translate;
            this.bindPort = bindPort;
            this.url = url;
        }

        public TraceInfo(string driverName, int dbType, string originalUrl, bool enable, string logFileUrl, string filter, bool translate, int bindPort, int optimizeMode, string[] tmpTblSpcs, string url, long tempRecycleTime)
        {
            this.tempRecycleTime = 0x6ddd00L;
            this.OptimizeMode = 1;
            if (driverName == null)
            {
                throw new ArgumentException("driverName is null");
            }
            this.dbType = dbType;
            this.originalUrl = originalUrl;
            this.enable = enable;
            this.logFileUrl = (logFileUrl == null) ? "null" : logFileUrl;
            this.filter = filter;
            this.driverName = driverName;
            this.translate = translate;
            this.bindPort = bindPort;
            this.url = url;
            this.OptimizeMode = optimizeMode;
            this.tempTableSpaces = tmpTblSpcs;
            this.tempRecycleTime = tempRecycleTime;
        }

        public TraceInfo(string driverName, int dbType, string originalUrl, bool enable, string logFileUrl, string filter, bool translate, int bindPort, int optimizeMode, string userName, string password, string url)
        {
            this.tempRecycleTime = 0x6ddd00L;
            this.OptimizeMode = 1;
            if (driverName == null)
            {
                throw new ArgumentException("driverName is null");
            }
            this.dbType = dbType;
            this.originalUrl = originalUrl;
            this.enable = enable;
            this.logFileUrl = (logFileUrl == null) ? "null" : logFileUrl;
            this.filter = filter;
            this.driverName = driverName;
            this.translate = translate;
            this.bindPort = bindPort;
            this.url = url;
            this.OptimizeMode = optimizeMode;
            this.userName = userName;
            this.password = password;
        }

        public object clone()
        {
            return new TraceInfo(this);
        }

        public FormatOptions getFormatOptions()
        {
            return this.formatOptions;
        }

        public string randomTableSpace()
        {
            if ((this.tempTableSpaces == null) || (this.tempTableSpaces.Length <= 0))
            {
                return "";
            }
            int num = this.tempTableSpaces.Length - 1;
            int index = (int)(new Random().NextDouble() * num);
            return this.tempTableSpaces[index];
        }

        public void setFormatOptions(FormatOptions formatOptions)
        {
            this.formatOptions = formatOptions;
        }
    }
}
