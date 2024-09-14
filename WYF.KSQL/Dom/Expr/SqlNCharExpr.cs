using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlNCharExpr : SqlExpr
    {
        // Fields
        public string text;

        // Methods
        public SqlNCharExpr() : base(6)
        {
        }

        public SqlNCharExpr(string text) : base(6)
        {
            this.text = text;
        }

        public override object Clone()
        {
            return new SqlNCharExpr(this.text);
        }

        public string getJavaString()
        {
            StringBuilder builder = new StringBuilder();
            int length = this.text.Length;
            for (int i = 0; i < length; i++)
            {
                char ch = char.Parse(this.text.Substring(i, 1));
                if (((ch != '\'') || (i == (length - 1))) || (char.Parse(this.text.Substring(i + 1, 1)) != '\''))
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }
    }




}
