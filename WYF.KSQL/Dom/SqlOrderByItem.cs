using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom.Expr;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlOrderByItem : SqlObject
    {
        // Fields
        public int chineseOrderByMode;
        public SqlExpr expr;
        public int mode;
        private string orgChineseOrderByType;
        private string orgOrderByName;

        // Methods
        public SqlOrderByItem()
        {
        }

        public SqlOrderByItem(SqlExpr expr)
        {
            this.expr = expr;
            this.mode = 0;
            this.chineseOrderByMode = -1;
            this.setOrgOrderByName("");
        }

        public SqlOrderByItem(SqlExpr expr, int mode)
        {
            this.expr = expr;
            this.mode = mode;
            this.chineseOrderByMode = -1;
            if (this.mode == 0)
            {
                this.setOrgOrderByName("");
            }
            else
            {
                this.setOrgOrderByName("DESC");
            }
        }

        public SqlOrderByItem(SqlExpr expr, int mode, int chineseOrderByMode)
        {
            this.expr = expr;
            this.mode = mode;
            this.chineseOrderByMode = chineseOrderByMode;
            if (this.mode == 0)
            {
                this.setOrgOrderByName("");
            }
            else
            {
                this.setOrgOrderByName("DESC");
            }
            if (this.chineseOrderByMode == 2)
            {
                this.setOrgChineseOrderByType("PINYIN");
            }
            else if (this.chineseOrderByMode == 4)
            {
                this.setOrgChineseOrderByType("RADICAL");
            }
            else if (this.chineseOrderByMode == 3)
            {
                this.setOrgChineseOrderByType("STROKE");
            }
        }

        public override object Clone()
        {
            return new SqlOrderByItem(this.expr, this.mode, this.chineseOrderByMode);
        }

        public string getOrgChineseOrderByType()
        {
            return this.orgChineseOrderByType;
        }

        public string getOrgOrderByName()
        {
            if ((this.orgOrderByName != null) && (this.orgOrderByName.Trim().Length != 0))
            {
                return this.orgOrderByName;
            }
            if (this.mode == 0)
            {
                return "";
            }
            return "DESC";
        }

        public void setOrgChineseOrderByType(string orgChineseOrderByType)
        {
            this.orgChineseOrderByType = orgChineseOrderByType;
        }

        public void setOrgOrderByName(string orgOrderByName)
        {
            this.orgOrderByName = orgOrderByName;
        }
    }






}
