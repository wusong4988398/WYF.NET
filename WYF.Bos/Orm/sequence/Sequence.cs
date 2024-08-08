using WYF.Bos.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.sequence
{
    public class Sequence
    {
        private static object lockObject = new object();

        protected string sqlSequenceObjectQuery;

        protected string createSql;

        protected string sqlSequenceQuery;

        protected string sqlTableQuery;

        protected string sqlPkQuery;

        protected DBRoute dbRoute;

        public Sequence(DBRoute dbRoute)
        {
            this.dbRoute = dbRoute;
        }



    }
}
