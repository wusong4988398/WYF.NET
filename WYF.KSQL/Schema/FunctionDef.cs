using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Formater;

namespace WYF.KSQL.Schema
{
   
        [Serializable]
        public class FunctionDef : SqlSchemaObject
        {
            // Fields
            public string dataType;
            public string name;
            private readonly ArrayList paramList;
            private readonly Hashtable paramMap;

            // Methods
            public FunctionDef()
            {
                this.paramMap = new Hashtable();
                this.paramList = new ArrayList();
            }

            public FunctionDef(string name)
            {
                this.paramMap = new Hashtable();
                this.paramList = new ArrayList();
                this.name = name;
            }

            public FunctionDef(string name, string dataType)
            {
                this.paramMap = new Hashtable();
                this.paramList = new ArrayList();
                this.name = name;
                this.dataType = dataType;
            }

            public void addParam(ParameterDef param)
            {
                this.paramList.Add(param);
                this.paramMap.Add(param.name, param);
            }

            public void clearParameter()
            {
                this.paramList.Clear();
                this.paramMap.Clear();
            }

            public ParameterDef getParameter(int paramIndex)
            {
                return (ParameterDef)this.paramList[paramIndex];
            }

            public ParameterDef getParameter(string paramName)
            {
                return (ParameterDef)this.paramMap[paramName];
            }

            public int parameterCount()
            {
                return this.paramList.Count;
            }

            public void removeParameter(ParameterDef param)
            {
                this.paramList.Remove(param);
                this.paramMap.Remove(param.name);
            }

            public void removeParameterAt(int index)
            {
                ParameterDef def = (ParameterDef)this.paramList[index];
                this.paramList.Remove(index);
                this.paramMap.Remove(def.name);
            }
        }


    }








