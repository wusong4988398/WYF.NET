using System;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Dom.Stmt;

namespace WYF.KSQL.Formater
{
   
    public class DB2SQLFormater : SQLFormater
    {
     
        public DB2SQLFormater() : base(null)
        {
            this.max_length_of_index_name = 128;
            this.max_length_of_table_name = 128;
            this.max_length_of_constraint_name = 18;
            this.max_length_of_column_name = 30;
            this.max_length_of_column_count = 255;
            this.max_length_of_row_size = -1;
        }

       
        public DB2SQLFormater(StringBuilder sb) : base(sb)
        {
            this.max_length_of_index_name = 128;
            this.max_length_of_table_name = 128;
            this.max_length_of_constraint_name = 18;
            this.max_length_of_column_name = 30;
            this.max_length_of_column_count = 255;
            this.max_length_of_row_size = -1;
        }

       
        protected override void FormatBinaryOpExpr(SqlBinaryOpExpr expr, bool appendBrace)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatTableSource(SqlTableSourceBase tableSource)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatPriorIdentifierExpr(SqlPriorIdentifierExpr expr)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatMethodInvokeExpr(SqlMethodInvokeExpr expr)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatBlockStmt(SqlBlockStmt stmt)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatIfStmt(SqlIfStmt stmt)
        {
            throw new NotImplementedException();
        }

      
        protected override void FormatWhileStmt(SqlWhileStmt stmt)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatDeallocateStmt(SqlDeallocateStmt stmt)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatCloseStmt(SqlCloseStmt stmt)
        {
            throw new NotImplementedException();
        }

        
        protected override void FormatSetLocalVariantStmt(SqlSetLocalVariantStmt stmt)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatOpenStmt(SqlOpenStmt stmt)
        {
            throw new NotImplementedException();
        }

        
        protected override void FormatCursorLoopStmt(SqlCursorLoopStmt stmt)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatFetchStmt(SqlFetchStmt stmt)
        {
            throw new NotImplementedException();
        }

      
        protected override void FormatBreakStmt(SqlBreakStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatContinueStmt(SqlContinueStmt stmt)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatGotoStmt(SqlGotoStmt stmt)
        {
            throw new NotImplementedException();
        }

      
        protected override void FormatLabelStmt(SqlLabelStmt stmt)
        {
            throw new NotImplementedException();
        }

     
        protected override void FormatCreateTableStmt(SqlCreateTableStmt stmt)
        {
            throw new NotImplementedException();
        }

      
        protected override void FormatAlterTableStmt(SqlAlterTableStmt stmt)
        {
            throw new NotImplementedException();
        }

       
        protected override void FormatDateTimeExpr(SqlDateTimeExpr expr)
        {
            throw new NotImplementedException();
        }


        protected override void FormatColumnDef(SqlColumnDef column)
        {
            throw new NotImplementedException();
        }

     
        protected override void FormatExecStmt(SqlExecStmt stmt)
        {
            throw new NotImplementedException();
        }


        protected override void FormatShowTablesStmt(SqlShowTablesStmt stmt)
        {
            throw new NotImplementedException();
        }

     
        protected override void FormatShowColumnsStmt(SqlShowColumnsStmt stmt)
        {
            throw new NotImplementedException();
        }

        protected override void FormatSelect(SqlSelect select)
        {
            throw new NotImplementedException();
        }


        protected override void FormatIdentity(SqlExpr stmt)
        {
            throw new NotImplementedException();
        }
    }
}
