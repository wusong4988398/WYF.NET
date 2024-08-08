
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using WYF.Bos.algo.sql.g;
using WYF.Bos.algo.sql.tree;
using WYF.Bos.algo.sql.tree.expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser = WYF.Bos.algo.sql.tree.expr.Parser;

namespace WYF.Bos.algo.sql.parser
{
    public class SqlParser : Parser
    {
        private ParseErrorListener errorListener = new ParseErrorListener();
        public override Expr ParseExpr(string expr)
        {
            GParser parser = Parse(expr);
            ASTBuilder visitor = new ASTBuilder(expr);
            Expr result = (Expr)visitor.VisitSingleExpression(parser.SingleExpression());

            return result;
        }

        public override Expr ParseSortList(string expr)
        {
            throw new NotImplementedException();
        }

        private GParser Parse(string sql)
        {
            GLexer lexer = new GLexer((ICharStream)new NoCaseStringStream(sql));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener((IAntlrErrorListener<int>)this.errorListener);
            CommonTokenStream tokenStream = new CommonTokenStream((ITokenSource)lexer);
            GParser parser = new GParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener((IAntlrErrorListener<IToken>)this.errorListener);
            try
            {
                parser.Interpreter.PredictionMode = PredictionMode.SLL;
                return parser;
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }
    }
}
