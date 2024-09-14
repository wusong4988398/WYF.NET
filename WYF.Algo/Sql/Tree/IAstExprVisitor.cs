using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree.bind;
using WYF.Algo.Sql.Tree.Star;

namespace WYF.Algo.Sql.Tree
{
    public interface IAstExprVisitor<R, C>
    {
        R VisitExpr(Expr node, C context) => default;

        //R VisitBindRef<T>(BindRef<T> node, C context) => VisitLeafExpr(node, context);

        //R VisitColumnRef(ColumnRef node, C context) => VisitBindRef(node, context);

        //R VisitRelationRef(RelationRef node, C context) => VisitBindRef(node, context);

        //R VisitAdd(Add node, C context) => VisitBinaryArithmetic(node, context);
        R VisitUnresolvedAttribute(UnresolvedAttribute node, C context);
        R VisitAlias(Alias node, C context);
        R VisitAttribute(Attribute node, C context);
        R VisitLeafExpr(LeafExpr node, C context);
        R VisitColumnRef(ColumnRef node, C context);
        R VisitBindRef<T>(BindRef<T> node, C context);
        R VisitRelationRef(RelationRef node, C context);
        R VisitBinaryOperator(BinaryOperator node, C context);
        R VisitBinaryExpr(BinaryExpr node, C context);
        R VisitCast(Cast node, C context);
        //R VisitAnd(And node, C context) => VisitExpr(node, context);

        //R VisitAttribute(Attribute node, C context) => VisitLeafExpr(node, context);

        //R VisitBinaryArithmetic(BinaryArithmetic node, C context) => VisitBinaryOperator(node, context);

        //R VisitBinaryComparison(BinaryComparison node, C context) => VisitBinaryOperator(node, context);

        //R VisitBinaryExpr(BinaryExpr node, C context) => VisitExpr(node, context);

        //R VisitBinaryOperator(BinaryOperator node, C context) => VisitBinaryExpr(node, context);

        //R VisitCast(Cast node, C context) => VisitUnaryExpr(node, context);

        //R VisitDivide(Divide node, C context) => VisitBinaryArithmetic(node, context);

        //R VisitEqual(Equal node, C context) => VisitBinaryComparison(node, context);

        R VisitExprList(ExprList node, C context);

        //R VisitGT(GT node, C context) => VisitBinaryComparison(node, context);

        //R VisitGTE(GTE node, C context) => VisitBinaryComparison(node, context);

        //R VisitIn(In node, C context) => VisitExpr(node, context);

        //R VisitIsNotNull(IsNotNull node, C context) => VisitUnaryExpr(node, context);

        //R VisitIsNull(IsNull node, C context) => VisitUnaryExpr(node, context);

        //R VisitLeafExpr(LeafExpr node, C context) => VisitExpr(node, context);

        //R VisitLike(Like node, C context) => VisitStringComparison(node, context);

        //R VisitLiteral(Literal node, C context) => VisitLeafExpr(node, context);

        //R VisitLT(LT node, C context) => VisitBinaryComparison(node, context);

        //R VisitLTE(LTE node, C context) => VisitBinaryComparison(node, context);

        //R VisitMultiply(Multiply node, C context) => VisitBinaryArithmetic(node, context);

        //R VisitNot(Not node, C context) => VisitUnaryExpr(node, context);

        //R VisitNotEqual(NotEqual node, C context) => VisitBinaryComparison(node, context);

        //R VisitOr(Or node, C context) => VisitExpr(node, context);

        //R VisitRemainder(Remainder node, C context) => VisitBinaryArithmetic(node, context);

        //R VisitSortOrder(SortOrder node, C context) => VisitUnaryExpr(node, context);

        //R VisitStringAdd(StringAdd node, C context) => VisitBinaryOperator(node, context);

        //R VisitStringComparison(StringComparison node, C context) => VisitBinaryComparison(node, context);

        //R VisitSubstract(Substract node, C context) => VisitBinaryArithmetic(node, context);

        R VisitUnaryExpr(UnaryExpr node, C context);
        R VisitUnresolvedStar(UnresolvedStar node, C context);

        R VisitRelationAllColumn(RelationAllColumn relationAllColumn, C context);
        //R VisitUnaryMinus(UnaryMinus node, C context) => VisitUnaryExpr(node, context);

        //R VisitUnresolvedAttribute(UnresolvedAttribute node, C context) => VisitAttribute(node, context);

        //R VisitUnresolvedStar(UnresolvedStar node, C context) => VisitAttribute(node, context);

        //R VisitUnresolvedFuncall(UnresolvedFuncall node, C context) => VisitExpr(node, context);

        //R VisitQuestion(Question node, C context) => VisitLeafExpr(node, context);

        //R VisitParameter(Parameter node, C context) => VisitLeafExpr(node, context);

        //R VisitAggExpr(AggExpr node, C context) => VisitUnaryExpr(node, context);

        //R VisitSumExpr(SumExpr node, C context) => VisitAggExpr(node, context);

        //R VisitMaxExpr(MaxExpr node, C context) => VisitAggExpr(node, context);

        //R VisitMinExpr(MinExpr node, C context) => VisitAggExpr(node, context);

        //R VisitAvgExpr(AvgExpr node, C context) => VisitAggExpr(node, context);

        //R VisitCountExpr(CountExpr node, C context) => VisitAggExpr(node, context);

        //R VisitCountDistinctExpr(CountDistinctExpr node, C context) => VisitAggExpr(node, context);

        //R VisitCaseWhenClause(CaseWhenClause caseWhenClause, C context) => VisitBinaryExpr(caseWhenClause, context);

        //R VisitCaseWhenSearch(CaseWhenSearch caseWhenSearch, C context) => VisitExpr(caseWhenSearch, context);

        //R VisitCaseWhenSimple(CaseWhenSimple caseWhenSimple, C context) => VisitExpr(caseWhenSimple, context);

        //R VisitRelationAllColumn(RelationAllColumn relationAllColumn, C context) => VisitExpr(relationAllColumn, context);

        //R VisitAggWithPropertyExpr(AggWithPropertyExpr aggWithPropertyExpr, C context) => VisitBinaryExpr(aggWithPropertyExpr, context);

        //R VisitMaxPExpr(MaxPExpr maxPExpr, C context) => VisitAggWithPropertyExpr(maxPExpr, context);

        //R VisitMinPExpr(MinPExpr minPExpr, C context) => VisitAggWithPropertyExpr(minPExpr, context);
    }
}
