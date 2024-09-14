using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    internal class SqlTableFunctionTableSource : SqlTableSourceBase
    {
        // Fields
        public string PipeFunction;

        // Methods
        public SqlTableFunctionTableSource()
        {
        }

        public SqlTableFunctionTableSource(string pipeFunction)
        {
            this.PipeFunction = pipeFunction;
        }

        public override object Clone()
        {
            SqlTableFunctionTableSource source = new SqlTableFunctionTableSource();
            if (this.PipeFunction != null)
            {
                source.PipeFunction = this.PipeFunction;
            }
            return source;
        }
    }


  



}
