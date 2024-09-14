using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Schema
{
    [Serializable]
    public class ParameterDef : SqlSchemaObject
    {
        // Fields
        public string dataType;
        public string name;
        public bool nullable;

        // Methods
        public ParameterDef()
        {
            this.nullable = false;
        }

        public ParameterDef(string name, string dataType)
        {
            this.name = name;
            this.dataType = dataType;
            this.nullable = false;
        }

        public ParameterDef(string name, string dataType, bool nullable)
        {
            this.name = name;
            this.dataType = dataType;
            this.nullable = nullable;
        }
    }





}
