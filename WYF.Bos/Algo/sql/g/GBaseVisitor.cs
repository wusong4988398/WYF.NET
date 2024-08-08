using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.g
{
    public class GBaseVisitor<T> : AbstractParseTreeVisitor<T>, GVisitor<T>
    {
        public T VisitArithmeticBinary(GParser.ArithmeticBinaryContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitArithmeticUnary(GParser.ArithmeticUnaryContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitBigIntLiteral(GParser.BigIntLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitBooleanDefault(GParser.BooleanDefaultContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitBooleanLiteral(GParser.BooleanLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitBooleanValue(GParser.BooleanValueContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitCast(GParser.CastContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitColumnReference(GParser.ColumnReferenceContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitComparison(GParser.ComparisonContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitComparisonOperator(GParser.ComparisonOperatorContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitConstantDefault(GParser.ConstantDefaultContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitDecimalLiteral(GParser.DecimalLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitDereference(GParser.DereferenceContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitDoubleLiteral(GParser.DoubleLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitExpression(GParser.ExpressionContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitFunctionCall(GParser.FunctionCallContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitIdentifier(GParser.IdentifierContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitIntegerLiteral(GParser.IntegerLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitLogicalBinary(GParser.LogicalBinaryContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitLogicalNot(GParser.LogicalNotContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitNamedExpression(GParser.NamedExpressionContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitNamedExpressionSeq(GParser.NamedExpressionSeqContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitNonReserved(GParser.NonReservedContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitNullLiteral(GParser.NullLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitNumericLiteral(GParser.NumericLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitParenthesizedExpression(GParser.ParenthesizedExpressionContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitPredicate(GParser.PredicateContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitPredicated(GParser.PredicatedContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitPrimitiveDataType(GParser.PrimitiveDataTypeContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitQualifiedName(GParser.QualifiedNameContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitQuestion(GParser.QuestionContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitQuotedIdentifier(GParser.QuotedIdentifierContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitQuotedIdentifierAlternative(GParser.QuotedIdentifierAlternativeContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitScientificDecimalLiteral(GParser.ScientificDecimalLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitSearchedCase(GParser.SearchedCaseContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitSetQuantifier(GParser.SetQuantifierContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitSimpleCase(GParser.SimpleCaseContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public virtual T VisitSingleExpression(GParser.SingleExpressionContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitSmallIntLiteral(GParser.SmallIntLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitStar(GParser.StarContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitStringAdd(GParser.StringAddContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitStringLiteral(GParser.StringLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitTinyIntLiteral(GParser.TinyIntLiteralContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitTypeConstructor(GParser.TypeConstructorContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitUnquotedIdentifier(GParser.UnquotedIdentifierContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitValueExpressionDefault(GParser.ValueExpressionDefaultContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }

        public T VisitWhenClause(GParser.WhenClauseContext context)
        {
            return (T)VisitChildren((IRuleNode)context);
        }
    }
}
