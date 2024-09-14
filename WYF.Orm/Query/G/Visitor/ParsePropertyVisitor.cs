
using System;
using WYF.SqlParser;

namespace WYF.Orm.Query.G.Visitor
{
    public class ParsePropertyVisitor : ExprVisitor<object>
    {
        private readonly PropertySegExpress _pse = new PropertySegExpress();

        public ParsePropertyVisitor()
        {
        }

        public PropertySegExpress PropertySegExpress => _pse;

        private void AppendProperty(string fullPropertyName)
        {
            _pse.AppendProperty(fullPropertyName);
        }

        public override object DefaultVisit(Expr exp, object context)
        {
            if (exp.ChildrenCount == 0)
            {
                _pse.AppendString(exp.Sql());
                return exp.Sql();
            }
            else
            {
                throw new NotSupportedException($"未处理表达式对象: {exp.GetType().Name}");
            }
        }

        public override object VisitBinaryOperator(BinaryOperator exp, object context)
        {
            bool b1 = exp.GetChildren(0).ChildrenCount > 1;
            if (b1)
            {
                _pse.AppendString("(");
            }

            exp.GetChildren(0).Accept(this, context);
            if (b1)
            {
                _pse.AppendString(")");
            }

            _pse.AppendString(" ").AppendString(exp.GetOperator()).AppendString(" ");
            bool b2 = exp.GetChildren(1).ChildrenCount > 1;
            if (b2)
            {
                _pse.AppendString("(");
            }

            exp.GetChildren(1).Accept(this, context);
            if (b2)
            {
                _pse.AppendString(")");
            }

            return null;
        }

        public  object VisitConcat(Concat exp, object context)
        {
            Expr[] children = exp.GetChildren().ToArray();
            _pse.AppendString("CONCAT(");
            for (int i = 0; i < children.Length; i++)
            {
                if (i > 0)
                {
                    _pse.AppendString(",");
                }

                children[i].Accept(this, context);
            }
            _pse.AppendString(")");
            return null;
        }

        public override object VisitExprList(ExprList exp, object context)
        {
            Expr[] children = exp.GetChildren().ToArray();

            for (int i = 0; i < children.Length; i++)
            {
                if (i > 0)
                {
                    _pse.AppendString(",");
                }

                children[i].Accept(this, context);
            }

            return null;
        }

        //public override object VisitIn(In exp, object context)
        //{
        //    Expr[] children = exp.Children.ToArray();
        //    children[0].Accept(this, context);
        //    _pse.AppendString(" IN (");

        //    for (int i = 1; i < children.Length; i++)
        //    {
        //        if (i > 1)
        //        {
        //            _pse.AppendString(",");
        //        }

        //        children[i].Accept(this, context);
        //    }

        //    _pse.AppendString(")");
        //    return null;
        //}

        public override object VisitUnresolvedAttribute(UnresolvedAttribute exp, object context)
        {
            AppendProperty(exp.Sql());
            return null;
        }

        public override object VisitAlias(Alias exp, object context)
        {
            exp.GetChild().Accept(this, context);
            _pse.AppendString(" AS ").AppendString(exp.GetAlias());
            return null;
        }

        public override object VisitCast(Cast exp, object context)
        {
            _pse.AppendString("CAST(");
            exp.GetChild().Accept(this, context);
            _pse.AppendString(",").AppendString(exp.GetDataType().GetName()).AppendString(")");
            return null;
        }

        //public override object VisitIsNotNull(IsNotNull exp, object context)
        //{
        //    exp.Child().Accept(this, context);
        //    _pse.AppendString(" IS NOT NULL");
        //    return null;
        //}

        //public override object VisitIsNull(IsNull exp, object context)
        //{
        //    exp.Child().Accept(this, context);
        //    _pse.AppendString(" IS NULL");
        //    return null;
        //}

        //public override object VisitNot(Not exp, object context)
        //{
        //    _pse.AppendString("NOT ");
        //    exp.Child().Accept(this, context);
        //    return null;
        //}

        //public override object VisitAnd(And expr, object context)
        //{
        //    List<Expr> children = expr.Children.ToList();
        //    if (children.Count > 1)
        //    {
        //        _pse.AppendString("(");
        //    }

        //    for (int i = 0; i < children.Count; i++)
        //    {
        //        children[i].Accept(this, context);
        //        if (i != children.Count - 1)
        //        {
        //            _pse.AppendString(" and ");
        //        }
        //    }

        //    if (children.Count > 1)
        //    {
        //        _pse.AppendString(")");
        //    }

        //    return null;
        //}

        //public override object VisitOr(Or expr, object context)
        //{
        //    List<Expr> children = expr.Children.ToList();
        //    if (children.Count > 1)
        //    {
        //        _pse.AppendString("(");
        //    }

        //    for (int i = 0; i < children.Count; i++)
        //    {
        //        children[i].Accept(this, context);
        //        if (i != children.Count - 1)
        //        {
        //            _pse.AppendString(" or ");
        //        }
        //    }

        //    if (children.Count > 1)
        //    {
        //        _pse.AppendString(")");
        //    }

        //    return null;
        //}

        //public override object VisitStringAdd(StringAdd exp, object context)
        //{
        //    bool b1 = exp.Child(0).ChildrenCount > 1;
        //    if (b1)
        //    {
        //        _pse.AppendString("(");
        //    }

        //    exp.Child(0).Accept(this, context);
        //    if (b1)
        //    {
        //        _pse.AppendString(")");
        //    }

        //    _pse.AppendString(" ").AppendString("||").AppendString(" ");
        //    bool b2 = exp.Child(1).ChildrenCount > 1;
        //    if (b2)
        //    {
        //        _pse.AppendString("(");
        //    }

        //    exp.Child(1).Accept(this, context);
        //    if (b2)
        //    {
        //        _pse.AppendString(")");
        //    }

        //    return null;
        //}

        //public override object VisitSortOrder(SortOrder exp, object context)
        //{
        //    exp.Child().Accept(this, context);
        //    _pse.AppendString(" ").AppendString(exp.SortDirect.Name);
        //    return null;
        //}

        //public override object VisitUnaryMinus(UnaryMinus exp, object context)
        //{
        //    _pse.AppendString("-");
        //    exp.Child().Accept(this, context);
        //    return null;
        //}

        //public override object VisitUnresolvedFuncall(UnresolvedFuncall exp, object context)
        //{
        //    _pse.AppendString(exp.Name);
        //    _pse.AppendString("(");
        //    if (exp.IsDistinct)
        //    {
        //        _pse.AppendString("DISTINCT ");
        //    }

        //    Expr[] children = exp.Children.ToArray();
        //    if (children != null)
        //    {
        //        for (int i = 0; i < children.Length; i++)
        //        {
        //            if (i > 0)
        //            {
        //                _pse.AppendString(",");
        //            }

        //            children[i].Accept(this, context);
        //        }
        //    }

        //    _pse.AppendString(")");
        //    return null;
        //}

        //public override object VisitAggExpr(AggExpr exp, object context)
        //{
        //    _pse.AppendString($"{exp.Func}(");
        //    exp.Child().Accept(this, context);
        //    _pse.AppendString(")");
        //    return null;
        //}

    }
  }
