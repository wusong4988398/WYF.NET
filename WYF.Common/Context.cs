using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public class Context
    {
        public IsolationLevel TransIsolationLevel { get; set; }

        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string SqlConnectionString { get; set; }

        public string DBId { get; set; } = "";
    }
}
