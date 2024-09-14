using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Util;

namespace WYF.KSQL.Dom.Expr
{
    public class SqlIdentityExpr : SqlExpr
    {
        // Fields
        public string dataType;
        public int increment;
        public string name;
        public int seed;

        // Methods
        public SqlIdentityExpr() : base(0x1c)
        {
            this.seed = 1;
            this.setExprWord("IDENTITY");
        }

        public SqlIdentityExpr(bool isFullName) : base(0x1c)
        {
            this.seed = 1;
            this.name = new UUTN(isFullName, "zs").toString();
            this.setExprWord("IDENTITY");
        }

        public SqlIdentityExpr(bool isFullName, string dataType, int seed, int increment) : base(0x1c)
        {
            this.seed = 1;
            this.dataType = dataType;
            this.seed = seed;
            this.increment = increment;
            this.name = new UUTN(isFullName, "zs").toString();
            this.setExprWord("IDENTITY");
        }

        public SqlIdentityExpr(string identityWord, bool isFullName, string dataType, int seed, int increment) : base(0x1c)
        {
            this.seed = 1;
            this.setExprWord(identityWord);
            this.dataType = dataType;
            this.seed = seed;
            this.increment = increment;
            this.name = new UUTN(isFullName, "zs").toString();
        }

        public override object Clone()
        {
            SqlIdentityExpr expr = new SqlIdentityExpr
            {
                dataType = this.dataType,
                seed = this.seed,
                increment = this.increment,
                name = this.name
            };
            expr.setExprWord(this.getExprWord());
            return expr;
        }
    }






}
