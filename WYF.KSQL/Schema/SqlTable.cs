using System;
using System.Collections;

namespace WYF.KSQL.Schema
{
  
    [Serializable]
    public class SqlTable : SqlSchemaObject
    {
      
        public string name;

       
        public IList columns = new ArrayList();

      
        public IList constraints = new ArrayList();

      
        public Hashtable extendedAttributes = new Hashtable();
    }
}
