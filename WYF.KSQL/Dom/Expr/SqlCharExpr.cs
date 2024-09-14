using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom.Expr
{
    [Serializable]
    public class SqlCharExpr : SqlExpr
    {
        // Fields
        public string text;

        // Methods
        public SqlCharExpr() : base(5)
        {
        }

        public SqlCharExpr(string text) : base(5)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            this.text = text;
        }

        public override object Clone()
        {
            return new SqlCharExpr(this.text);
        }

        public string getJavaString()
        {
            return getJavaString(this.text);
        }

        public static string getJavaString(string char_expr_value)
        {
            if (char_expr_value == null)
            {
                return "NULL";
            }
            StringBuilder builder = new StringBuilder();
            int length = char_expr_value.Length;
            for (int i = 0; i < length; i++)
            {
                char ch = char.Parse(char_expr_value.Substring(i, 1));
                if (((ch != '\'') || (i == (length - 1))) || (char.Parse(char_expr_value.Substring(i + 1, 1)) != '\''))
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }

        public override void output(StringBuilder buff)
        {
            buff.Append(this.getJavaString());
        }
    }





}
