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

        public DatabaseType DatabaseType { get; set; }= DatabaseType.MS_SQL_Server;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string SqlConnectionString { get; set; } = "Server=.;Database=WYF;User ID=sa;Password=Wsl4988398;max pool size=10000;Encrypt=True;TrustServerCertificate=True";

        public string DBId { get; set; } = "";
    }
}
