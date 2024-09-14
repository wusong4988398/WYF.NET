using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree;

namespace WYF.Algo.Sql.parser
{
    public class ASTBuilder : GBaseVisitor<object>
    {
        private string originText;
        private int parameterPosition;

        public ASTBuilder(string originText)
        {
            this.originText = originText;
        }

        private Optional<NodeLocation> GetLocation(ITerminalNode terminalNode)
        {
            if (terminalNode == null)
                return Optional<NodeLocation>.Empty();

            string text = terminalNode.GetText();
            NodeLocation location = new NodeLocation(text);
            return Optional<NodeLocation>.Of(location);
        }

        private Optional<NodeLocation> GetLocation(ParserRuleContext parserRuleContext)
        {
            if (parserRuleContext == null)
                return Optional<NodeLocation>.Empty();

            NodeLocation location = new NodeLocation(this.originText, parserRuleContext.Start.StartIndex, (parserRuleContext.Stop.StopIndex + 1) - parserRuleContext.Start.StartIndex);
            return Optional<NodeLocation>.Of(location);
        }


        public override object VisitNamedExpressionSeq(GParser.NamedExpressionSeqContext ctx)
        {
            var result = ctx.namedExpression().Select(e => Expression(e)).ToList();
            return new ExprList(GetLocation(ctx), result.ToArray());
        }

        public Expr Expression(ParserRuleContext ctx)
        {
            return (Expr)ctx.Accept(this);
        }

        public override object VisitNamedExpression(GParser.NamedExpressionContext ctx)
        {
            Expr e = (Expr)VisitExpression(ctx.expression());

            if (ctx.identifier() != null)
            {
                var c = new Alias(GetLocation(ctx), e, ctx.identifier().GetText());
                return c;
            }

            if (ctx.qualifiedName() != null)
            {
                return new Alias(GetLocation(ctx), e, ctx.qualifiedName().GetText());
            }
            return e;
        }

        public override object VisitExpression(GParser.ExpressionContext ctx)
        {
            return base.VisitExpression(ctx);
        }

        public override object VisitQuerySpecification(GParser.QuerySpecificationContext context)
        {
            return base.VisitQuerySpecification(context);
        }

        //public override object VisitPredicated(GParser.PredicatedContext ctx)
        //{
        //    Expr expr = Expression(ctx.valueExpression());
        //    if (ctx.predicate() != null)
        //    {
        //        return WithPredicate(GetLocation(ctx), expr, ctx.predicate());
        //    }
        //    return expr;
        //}
        public override object VisitValueExpressionDefault(GParser.ValueExpressionDefaultContext ctx)
        {
            return base.VisitValueExpressionDefault(ctx);
        }


        private object WithPredicate(Optional<NodeLocation> location, Expr expr, GParser.PredicateContext ctx)
        {
            Expr result;
            bool not = ctx.NOT() != null;
            bool NULL = ctx.NULL() != null;

            return null;
            //if (NULL)
            //{
            //    return not ? new IsNotNull(location, expr) : new IsNull(location, expr);
            //}
            //switch (ctx.kind.Type)
            //{
            //    case 17:
            //        result = new In(location, expr, Expressions((ParserRuleContext[])ctx.expression().ToArray(new ParserRuleContext[ctx.expression().Count])));
            //        break;
            //    case 20:
            //        result = new Like(location, expr, Expression(ctx.pattern));
            //        break;
            //    default:
            //        throw new AlgoException("Not support type: " + ctx.GetText());
            //}
            //return not ? new Not(location, result) : result;
        }

        public override object VisitQuery(GParser.QueryContext ctx)
        {
            QueryBody body = (QueryBody)Visit(ctx.queryNoWith());
            //return new Query(GetLocation(ctx), body);
            return null;
        }

        private static Optional<string> GetTextIfPresent(IToken token)
        {
            return token != null ? Optional<string>.Of(token.Text) : Optional<string>.Empty();
        }

        private Optional<T> VisitIfPresent<T>(ParserRuleContext ctx, Type clazz)
        {
            return ctx != null ? Optional<T>.Of((T)Visit(ctx)) : Optional<T>.Empty();
        }

        private List<T> Visit<T>(List<ParserRuleContext> contexts, Type clazz)
        {
            return contexts.Select(c => (T)Visit(c)).ToList();
        }

        //public override object VisitSingleInsertQuery(GParser.SingleInsertQueryContext ctx)
        //{
        //    List<ITerminalNode> integerValues;
        //    QueryBody queryBody = (QueryBody)Visit(ctx.queryTerm());
        //    Optional<OrderBy> orderBy = Optional.Empty();
        //    if (!ctx.queryOrganization().order.IsEmpty)
        //    {
        //        List<SortItem> sortList = ctx.queryOrganization().order.Select(visitSortItem2).ToList();
        //        orderBy = Optional.Of(new OrderBy(GetLocation(ctx.queryOrganization()), sortList));
        //    }
        //    Optional<Limit> limit = Optional.Empty();
        //}



        public override object VisitWhenClause(GParser.WhenClauseContext ctx)
        {

            return base.VisitWhenClause(ctx);
        }

        public override object VisitQualifiedName(GParser.QualifiedNameContext ctx)
        {
            return base.VisitQualifiedName(ctx);
        }

        public override object VisitIdentifier(GParser.IdentifierContext ctx)
        {
            return base.VisitIdentifier(ctx);
        }



        public override object VisitQuotedIdentifier(GParser.QuotedIdentifierContext ctx)
        {
            return base.VisitQuotedIdentifier(ctx);
        }

        //public override object VisitDecimalLiteral(GParser.DecimalLiteralContext ctx)
        //{
        //    string text = ctx.GetText();
        //    decimal bd = decimal.Parse(text);
        //    return new Literal(GetLocation(ctx), bd, DataType.BigDecimalType);
        //}

        //public override object VisitIntegerLiteral(GParser.IntegerLiteralContext ctx)
        //{
        //    string text = ctx.GetText();
        //    long v = long.Parse(text);
        //    if (v > 2147483647L || v < -2147483648L)
        //    {
        //        return new Literal(GetLocation(ctx), v);
        //    }
        //    return new Literal(GetLocation(ctx), (int)v);
        //}

        //public override object VisitBigIntLiteral(GParser.BigIntLiteralContext ctx)
        //{
        //    string text = ctx.GetText();
        //    long v = long.Parse(text.Substring(0, text.Length - 1));
        //    return new Literal(GetLocation(ctx), v);
        //}



        //public override object VisitDoubleLiteral(GParser.DoubleLiteralContext ctx)
        //{
        //    string text = ctx.GetText();
        //    double v = double.Parse(text.Substring(0, text.Length - 1));
        //    return new Literal(GetLocation(ctx), v);
        //}

        public override object VisitNonReserved(GParser.NonReservedContext ctx)
        {
            return base.VisitNonReserved(ctx);
        }

        public override object Visit(IParseTree tree)
        {
            return base.Visit(tree);
        }

        public override object VisitChildren(IRuleNode node)
        {
            if (node.ChildCount >= 1)
            {
                return node.GetChild(0).Accept(this);
            }
            return null;
        }

        public override object VisitTerminal(ITerminalNode node)
        {
            return base.VisitTerminal(node);
        }

        public new object VisitErrorNode(IErrorNode node)
        {
            return base.VisitErrorNode(node);
        }
        protected override object DefaultResult => base.DefaultResult;


        protected new object AggregateResult(object aggregate, object nextResult)
        {
            return base.AggregateResult(aggregate, nextResult);
        }

        protected override bool ShouldVisitNextChild(IRuleNode node, object currentResult)
        {
            return base.ShouldVisitNextChild(node, currentResult);
        }

        //public override object VisitSortSet(GParser.SortSetContext ctx)
        //{
        //    List<Expr> result = ctx.Order.Select(c => expression(c)).ToList();
        //    return new ExprList(GetLocation(ctx), result.ToArray());
        //}

        //public override object VisitTuple(GParser.TupleContext ctx)
        //{
        //    List<Expr> result = ctx.expression.Select(e => expression(e)).ToList();
        //    return new ExprList(GetLocation(ctx), result.ToArray());
        //}

        public override object VisitColumnReference(GParser.ColumnReferenceContext context)
        {

            return new UnresolvedAttribute(GetLocation(context), context.GetText());

            //return base.VisitColumnReference(context);
        }

        public override object VisitSingleExpression(GParser.SingleExpressionContext ctx)
        {
            var ccc = base.VisitSingleExpression(ctx);
            return ccc;
        }




        public override object VisitPredicate(GParser.PredicateContext ctx)
        {
            return base.VisitPredicate(ctx);
        }
    }
}
