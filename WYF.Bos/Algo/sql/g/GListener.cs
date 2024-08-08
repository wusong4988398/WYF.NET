using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.g
{
    public interface GListener : IParseTreeListener
    {
        void EnterSingleExpression(GParser.SingleExpressionContext context);

        void ExitSingleExpression(GParser.SingleExpressionContext context);
        void EnterNamedExpression(GParser.NamedExpressionContext namedExpressionContext);

        void ExitNamedExpression(GParser.NamedExpressionContext namedExpressionContext);


        void EnterExpression(GParser.ExpressionContext expressionContext);

        void ExitExpression(GParser.ExpressionContext expressionContext);


        void EnterLogicalNot(GParser.LogicalNotContext context);

        void ExitLogicalNot(GParser.LogicalNotContext context);


        void EnterPredicate(GParser.PredicateContext context);

        void ExitPredicate(GParser.PredicateContext context);

        void EnterBooleanDefault(GParser.BooleanDefaultContext context);

        void ExitBooleanDefault(GParser.BooleanDefaultContext context);


        void EnterPredicated(GParser.PredicatedContext context);

        void ExitPredicated(GParser.PredicatedContext context);

        void EnterLogicalBinary(GParser.LogicalBinaryContext context);

        void ExitLogicalBinary(GParser.LogicalBinaryContext context);

        void EnterArithmeticBinary(GParser.ArithmeticBinaryContext context);

        void ExitArithmeticBinary(GParser.ArithmeticBinaryContext context);

        void EnterComparison(GParser.ComparisonContext context);

        void ExitComparison(GParser.ComparisonContext context);

        void EnterComparisonOperator(GParser.ComparisonOperatorContext context);

        void ExitComparisonOperator(GParser.ComparisonOperatorContext context);

        void EnterStringAdd(GParser.StringAddContext context);

        void ExitStringAdd(GParser.StringAddContext context);

        void EnterValueExpressionDefault(GParser.ValueExpressionDefaultContext context);

        void ExitValueExpressionDefault(GParser.ValueExpressionDefaultContext context);
        void EnterArithmeticUnary(GParser.ArithmeticUnaryContext context);

        void ExitArithmeticUnary(GParser.ArithmeticUnaryContext context);
        void EnterStar(GParser.StarContext context);

        void ExitStar(GParser.StarContext context);
        void EnterConstantDefault(GParser.ConstantDefaultContext context);

        void ExitConstantDefault(GParser.ConstantDefaultContext context);
        void EnterNullLiteral(GParser.NullLiteralContext context);

        void ExitNullLiteral(GParser.NullLiteralContext context);

        void EnterTypeConstructor(GParser.TypeConstructorContext context);

        void ExitTypeConstructor(GParser.TypeConstructorContext context);
        void EnterIdentifier(GParser.IdentifierContext context);

        void ExitIdentifier(GParser.IdentifierContext context);
        void EnterNonReserved(GParser.NonReservedContext context);

        void ExitNonReserved(GParser.NonReservedContext context);
        void EnterQuotedIdentifierAlternative(GParser.QuotedIdentifierAlternativeContext context);

        void ExitQuotedIdentifierAlternative(GParser.QuotedIdentifierAlternativeContext context);
        void EnterQuotedIdentifier(GParser.QuotedIdentifierContext context);

        void ExitQuotedIdentifier(GParser.QuotedIdentifierContext context);
        void EnterNumericLiteral(GParser.NumericLiteralContext context);

        void ExitNumericLiteral(GParser.NumericLiteralContext context);
        void EnterQualifiedName(GParser.QualifiedNameContext context);

        void ExitQualifiedName(GParser.QualifiedNameContext context);
        void EnterSimpleCase(GParser.SimpleCaseContext context);

        void ExitSimpleCase(GParser.SimpleCaseContext context);

        void EnterWhenClause(GParser.WhenClauseContext context);

        void ExitWhenClause(GParser.WhenClauseContext context);
        void EnterSearchedCase(GParser.SearchedCaseContext context);

        void ExitSearchedCase(GParser.SearchedCaseContext context);

        void EnterCast(GParser.CastContext context);

        void ExitCast(GParser.CastContext context);
        void EnterColumnReference(GParser.ColumnReferenceContext context);

        void ExitColumnReference(GParser.ColumnReferenceContext context);
        void EnterParenthesizedExpression(GParser.ParenthesizedExpressionContext context);

        void ExitParenthesizedExpression(GParser.ParenthesizedExpressionContext context);
        void EnterDereference(GParser.DereferenceContext context);

        void ExitDereference(GParser.DereferenceContext context);
        void EnterBooleanValue(GParser.BooleanValueContext context);

        void ExitBooleanValue(GParser.BooleanValueContext context);
        void EnterBooleanLiteral(GParser.BooleanLiteralContext context);

        void ExitBooleanLiteral(GParser.BooleanLiteralContext context);
        void EnterQuestion(GParser.QuestionContext context);

        void ExitQuestion(GParser.QuestionContext context);
        void EnterFunctionCall(GParser.FunctionCallContext context);

        void ExitFunctionCall(GParser.FunctionCallContext context);
        void EnterSetQuantifier(GParser.SetQuantifierContext context);

        void ExitSetQuantifier(GParser.SetQuantifierContext context);
        void EnterPrimitiveDataType(GParser.PrimitiveDataTypeContext context);

        void ExitPrimitiveDataType(GParser.PrimitiveDataTypeContext context);

        void EnterStringLiteral(GParser.StringLiteralContext context);

        void ExitStringLiteral(GParser.StringLiteralContext context);
        void EnterDecimalLiteral(GParser.DecimalLiteralContext context);

        void ExitDecimalLiteral(GParser.DecimalLiteralContext context);
        void EnterScientificDecimalLiteral(GParser.ScientificDecimalLiteralContext context);

        void ExitScientificDecimalLiteral(GParser.ScientificDecimalLiteralContext context);
        void EnterIntegerLiteral(GParser.IntegerLiteralContext context);

        void ExitIntegerLiteral(GParser.IntegerLiteralContext context);
        void EnterBigIntLiteral(GParser.BigIntLiteralContext context);

        void ExitBigIntLiteral(GParser.BigIntLiteralContext context);
        void EnterSmallIntLiteral(GParser.SmallIntLiteralContext context);

        void ExitSmallIntLiteral(GParser.SmallIntLiteralContext context);
        void EnterTinyIntLiteral(GParser.TinyIntLiteralContext context);

        void ExitTinyIntLiteral(GParser.TinyIntLiteralContext context);
        void EnterDoubleLiteral(GParser.DoubleLiteralContext context);

        void ExitDoubleLiteral(GParser.DoubleLiteralContext context);
        void EnterNamedExpressionSeq(GParser.NamedExpressionSeqContext context);

        void ExitNamedExpressionSeq(GParser.NamedExpressionSeqContext context);
        void EnterUnquotedIdentifier(GParser.UnquotedIdentifierContext context);

        void ExitUnquotedIdentifier(GParser.UnquotedIdentifierContext context);

    }
}
