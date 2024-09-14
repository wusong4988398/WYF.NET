using System;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;

namespace WYF.KSQL.Formater
{
    // Token: 0x020000A6 RID: 166
    public class DerbySQLFormater : SQLFormater
    {
        // Token: 0x06000397 RID: 919 RVA: 0x000108E1 File Offset: 0x0000EAE1
        public DerbySQLFormater() : base(null)
        {
        }

        // Token: 0x06000398 RID: 920 RVA: 0x000108EA File Offset: 0x0000EAEA
        public DerbySQLFormater(StringBuilder sb) : base(sb)
        {
        }

        // Token: 0x06000399 RID: 921 RVA: 0x000108F3 File Offset: 0x0000EAF3
        protected override void FormatBinaryOpExpr(SqlBinaryOpExpr expr, bool appendBrace)
        {
            throw new NotImplementedException();
        }

        // Token: 0x0600039A RID: 922 RVA: 0x000108FA File Offset: 0x0000EAFA
        protected override void FormatTableSource(SqlTableSourceBase tableSource)
        {
            throw new NotImplementedException();
        }

        // Token: 0x0600039B RID: 923 RVA: 0x00010901 File Offset: 0x0000EB01
        protected override void FormatPriorIdentifierExpr(SqlPriorIdentifierExpr expr)
        {
            throw new NotImplementedException();
        }

        // Token: 0x0600039C RID: 924 RVA: 0x00010908 File Offset: 0x0000EB08
        protected override void FormatMethodInvokeExpr(SqlMethodInvokeExpr expr)
        {
            throw new NotImplementedException();
        }

        // Token: 0x0600039D RID: 925 RVA: 0x0001090F File Offset: 0x0000EB0F
        protected override void FormatBlockStmt(SqlBlockStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x0600039E RID: 926 RVA: 0x00010916 File Offset: 0x0000EB16
        protected override void FormatIfStmt(SqlIfStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x0600039F RID: 927 RVA: 0x0001091D File Offset: 0x0000EB1D
        protected override void FormatWhileStmt(SqlWhileStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A0 RID: 928 RVA: 0x00010924 File Offset: 0x0000EB24
        protected override void FormatDeallocateStmt(SqlDeallocateStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A1 RID: 929 RVA: 0x0001092B File Offset: 0x0000EB2B
        protected override void FormatCloseStmt(SqlCloseStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A2 RID: 930 RVA: 0x00010932 File Offset: 0x0000EB32
        protected override void FormatSetLocalVariantStmt(SqlSetLocalVariantStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A3 RID: 931 RVA: 0x00010939 File Offset: 0x0000EB39
        protected override void FormatOpenStmt(SqlOpenStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A4 RID: 932 RVA: 0x00010940 File Offset: 0x0000EB40
        protected override void FormatCursorLoopStmt(SqlCursorLoopStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A5 RID: 933 RVA: 0x00010947 File Offset: 0x0000EB47
        protected override void FormatFetchStmt(SqlFetchStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A6 RID: 934 RVA: 0x0001094E File Offset: 0x0000EB4E
        protected override void FormatBreakStmt(SqlBreakStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A7 RID: 935 RVA: 0x00010955 File Offset: 0x0000EB55
        protected override void FormatContinueStmt(SqlContinueStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A8 RID: 936 RVA: 0x0001095C File Offset: 0x0000EB5C
        protected override void FormatGotoStmt(SqlGotoStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003A9 RID: 937 RVA: 0x00010963 File Offset: 0x0000EB63
        protected override void FormatLabelStmt(SqlLabelStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003AA RID: 938 RVA: 0x0001096A File Offset: 0x0000EB6A
        protected override void FormatCreateTableStmt(SqlCreateTableStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003AB RID: 939 RVA: 0x00010971 File Offset: 0x0000EB71
        protected override void FormatAlterTableStmt(SqlAlterTableStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003AC RID: 940 RVA: 0x00010978 File Offset: 0x0000EB78
        protected override void FormatDateTimeExpr(SqlDateTimeExpr expr)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003AD RID: 941 RVA: 0x0001097F File Offset: 0x0000EB7F
        protected override void FormatColumnDef(SqlColumnDef column)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003AE RID: 942 RVA: 0x00010986 File Offset: 0x0000EB86
        protected override void FormatExecStmt(SqlExecStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003AF RID: 943 RVA: 0x0001098D File Offset: 0x0000EB8D
        protected override void FormatShowTablesStmt(SqlShowTablesStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003B0 RID: 944 RVA: 0x00010994 File Offset: 0x0000EB94
        protected override void FormatShowColumnsStmt(SqlShowColumnsStmt stmt)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003B1 RID: 945 RVA: 0x0001099B File Offset: 0x0000EB9B
        protected override void FormatSelect(SqlSelect select)
        {
            throw new NotImplementedException();
        }

        // Token: 0x060003B2 RID: 946 RVA: 0x000109A2 File Offset: 0x0000EBA2
        protected override void FormatIdentity(SqlExpr stmt)
        {
            throw new NotImplementedException();
        }
    }
}
