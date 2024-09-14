using System;
using System.Collections;

namespace WYF.KSQL.Schema
{
    // Token: 0x02000084 RID: 132
    public class SqlColumn : SqlSchemaObject
    {
        
        public string name;

      
        public string dataType;

     
        public int length;

       
        public int precision;

      
        public int scale;

       
        public bool isNullable;

       
        public Hashtable extendedAttributes = new Hashtable();

       
        public string defaultExpr;
    }
}
