//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from G.g4 by ANTLR 4.13.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace SQLParser.Parsers.TSql {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="GParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.2")]
[System.CLSCompliant(false)]
public interface IGVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.singleExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSingleExpression([NotNull] GParser.SingleExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.query"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQuery([NotNull] GParser.QueryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.queryNoWith"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQueryNoWith([NotNull] GParser.QueryNoWithContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.queryOrganization"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQueryOrganization([NotNull] GParser.QueryOrganizationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.queryTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQueryTerm([NotNull] GParser.QueryTermContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.queryPrimary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQueryPrimary([NotNull] GParser.QueryPrimaryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.sortSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSortSet([NotNull] GParser.SortSetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.sortItem"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSortItem([NotNull] GParser.SortItemContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.querySpecification"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQuerySpecification([NotNull] GParser.QuerySpecificationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.fromClause"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFromClause([NotNull] GParser.FromClauseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.whereClause"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWhereClause([NotNull] GParser.WhereClauseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.groupByClause"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitGroupByClause([NotNull] GParser.GroupByClauseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.havingClause"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitHavingClause([NotNull] GParser.HavingClauseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.aggregation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAggregation([NotNull] GParser.AggregationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.setQuantifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSetQuantifier([NotNull] GParser.SetQuantifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.relation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRelation([NotNull] GParser.RelationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.joinType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitJoinType([NotNull] GParser.JoinTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.joinCriteria"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitJoinCriteria([NotNull] GParser.JoinCriteriaContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.relationPrimary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRelationPrimary([NotNull] GParser.RelationPrimaryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.tableIdentifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTableIdentifier([NotNull] GParser.TableIdentifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.namedExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNamedExpression([NotNull] GParser.NamedExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.namedExpressionSeq"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNamedExpressionSeq([NotNull] GParser.NamedExpressionSeqContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpression([NotNull] GParser.ExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.booleanExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBooleanExpression([NotNull] GParser.BooleanExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.predicated"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPredicated([NotNull] GParser.PredicatedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.predicate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPredicate([NotNull] GParser.PredicateContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.valueExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitValueExpression([NotNull] GParser.ValueExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.primaryExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPrimaryExpression([NotNull] GParser.PrimaryExpressionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.constant"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitConstant([NotNull] GParser.ConstantContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.comparisonOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComparisonOperator([NotNull] GParser.ComparisonOperatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.booleanValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBooleanValue([NotNull] GParser.BooleanValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.dataType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDataType([NotNull] GParser.DataTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.whenClause"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWhenClause([NotNull] GParser.WhenClauseContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.qualifiedName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQualifiedName([NotNull] GParser.QualifiedNameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier([NotNull] GParser.IdentifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.strictIdentifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStrictIdentifier([NotNull] GParser.StrictIdentifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.quotedIdentifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitQuotedIdentifier([NotNull] GParser.QuotedIdentifierContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.number"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumber([NotNull] GParser.NumberContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="GParser.nonReserved"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNonReserved([NotNull] GParser.NonReservedContext context);
}
} // namespace SQLParser.Parsers.TSql
