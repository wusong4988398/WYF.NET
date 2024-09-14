
using System;
using System.Text;

namespace WYF.SqlParser
{
    public abstract class Calc
    {
        protected Expr expr;

        public abstract object ExecuteImpl(IRowFeature rowFeature, IRowFeature rowFeature2);

        public Calc(Expr expr)
        {
            this.expr = expr;
        }

        public object Execute(IRowFeature row1, IRowFeature row2)
        {
            try
            {
                return ExecuteImpl(row1, row2);
            }
            catch (Exception e)
            {
                if (e is AlgoException)
                {
                    throw e;
                }
                throw WrapException(e, row1, row2);
            }
        }

        protected void ThrowException(string message)
        {
            throw WrapException(message);
        }

        public AlgoException WrapException(string message)
        {
            if (expr == null || !expr.Location.IsPresent())
            {
                return new AlgoException(message);
            }
            StringBuilder sb = new StringBuilder(message);
            sb.Append(",Expression:").Append(expr.Location.Get().Text);
            return new AlgoException(sb.ToString());
        }

        protected Exception WrapException(Exception e)
        {
            if (expr == null || !expr.Location.IsPresent())
            {
                return e;
            }
            StringBuilder message = new StringBuilder(e.Message);
            message.Append(",Expression:").Append(expr.Location.Get().Text);
            return new AlgoException(message.ToString(), e);
        }

        protected Exception WrapException(Exception e, IRowFeature row1, IRowFeature row2)
        {
            if (expr == null || !expr.Location.IsPresent())
            {
                return e;
            }
            StringBuilder message = new StringBuilder(e.Message);
            message.Append(",Expression:").Append(expr.Location.Get().Text).Append(",");
            if (row2 == null)
            {
                message.Append("Row=").Append(row1.ToString());
            }
            else
            {
                message.Append("Row1=").Append(row1.ToString()).Append("Row2=").Append(row2.ToString());
            }
            return new AlgoException(message.ToString(), e);
        }
    }
}