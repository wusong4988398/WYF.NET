using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class ObjectCreateExpr : SqlExpr
    {
        // Fields
        public string objType;
        private string orgObjType;
        public ArrayList paramList;

        // Methods
        public ObjectCreateExpr() : base(0x17)
        {
            this.paramList = new ArrayList();
        }

        public ObjectCreateExpr(string objType) : base(0x17)
        {
            this.paramList = new ArrayList();
            this.objType = objType;
            this.setOrgObjType(objType);
        }

        public ObjectCreateExpr(string objType, string orgObjType) : base(0x17)
        {
            this.paramList = new ArrayList();
            this.objType = objType;
            this.setOrgObjType(orgObjType);
        }

        public string getOrgObjType()
        {
            return this.orgObjType;
        }

        public void setOrgObjType(string orgObjType)
        {
            this.orgObjType = orgObjType;
        }
    }






}
