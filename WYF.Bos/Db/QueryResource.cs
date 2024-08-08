using WYF.Bos.db.tx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.db
{
    public  class QueryResource:IDisposable
    {
        private  DelegateConnection con;
  
        private  string name;
  

  
        public string TraceStack { get; set; }

        //public QueryResource(DelegateConnection con, String name, AutoCloseable beforeReleaseListener)
        //{
        //    this.con = con;
        //    this.name = name;
        //    this.beforeReleaseListener = beforeReleaseListener;
        //    con.addReleaseResource(this);
        //}

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
