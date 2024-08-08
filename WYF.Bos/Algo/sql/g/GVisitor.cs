using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.g
{
    public interface GVisitor<T>: IParseTreeVisitor<T>
    {
        T VisitSingleExpression(GParser.SingleExpressionContext context);
        T VisitNamedExpression(GParser.NamedExpressionContext context);
        T VisitExpression(GParser.ExpressionContext context);

        T VisitLogicalNot(GParser.LogicalNotContext context);

        T VisitPredicated(GParser.PredicatedContext context);

        T VisitBooleanDefault(GParser.BooleanDefaultContext context);

        T VisitPredicate(GParser.PredicateContext context);


        T VisitLogicalBinary(GParser.LogicalBinaryContext context);

        T VisitArithmeticBinary(GParser.ArithmeticBinaryContext context);
        T VisitComparison(GParser.ComparisonContext context);

        T VisitComparisonOperator(GParser.ComparisonOperatorContext context);


        T VisitStringAdd(GParser.StringAddContext context);

        T VisitValueExpressionDefault(GParser.ValueExpressionDefaultContext context);

        T VisitArithmeticUnary(GParser.ArithmeticUnaryContext context);

        T VisitStar(GParser.StarContext context);

        T VisitConstantDefault(GParser.ConstantDefaultContext context);
        T VisitNullLiteral(GParser.NullLiteralContext context);
        T VisitTypeConstructor(GParser.TypeConstructorContext context);
        T VisitIdentifier(GParser.IdentifierContext context);

        T VisitNonReserved(GParser.NonReservedContext context);

        T VisitQuotedIdentifierAlternative(GParser.QuotedIdentifierAlternativeContext context);

        T VisitQuotedIdentifier(GParser.QuotedIdentifierContext context);
        T VisitNumericLiteral(GParser.NumericLiteralContext context);
        T VisitQualifiedName(GParser.QualifiedNameContext context);

        T VisitSimpleCase(GParser.SimpleCaseContext context);
        T VisitWhenClause(GParser.WhenClauseContext context);

        T VisitSearchedCase(GParser.SearchedCaseContext context);

        T VisitCast(GParser.CastContext context);

        T VisitColumnReference(GParser.ColumnReferenceContext context);
        T VisitParenthesizedExpression(GParser.ParenthesizedExpressionContext context);
        T VisitDereference(GParser.DereferenceContext context);
        T VisitBooleanValue(GParser.BooleanValueContext context);

        T VisitBooleanLiteral(GParser.BooleanLiteralContext context);
        T VisitQuestion(GParser.QuestionContext context);
        T VisitFunctionCall(GParser.FunctionCallContext context);
        T VisitSetQuantifier(GParser.SetQuantifierContext context);
        T VisitPrimitiveDataType(GParser.PrimitiveDataTypeContext context);
        T VisitStringLiteral(GParser.StringLiteralContext context);
        T VisitDecimalLiteral(GParser.DecimalLiteralContext context);
        T VisitScientificDecimalLiteral(GParser.ScientificDecimalLiteralContext context);
        T VisitIntegerLiteral(GParser.IntegerLiteralContext context);

        T VisitBigIntLiteral(GParser.BigIntLiteralContext context);
        T VisitSmallIntLiteral(GParser.SmallIntLiteralContext context);

        T VisitTinyIntLiteral(GParser.TinyIntLiteralContext context);

        T VisitDoubleLiteral(GParser.DoubleLiteralContext context);
        T VisitNamedExpressionSeq(GParser.NamedExpressionSeqContext context);
        T VisitUnquotedIdentifier(GParser.UnquotedIdentifierContext context);


    }
}
