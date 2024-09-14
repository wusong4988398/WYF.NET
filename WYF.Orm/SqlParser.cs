using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.SqlParser
{
    public class TSqlParser
    {

        private SyntaxErrorListener errorListener = new SyntaxErrorListener();
        public   Expr ParseExpr(string expr)
        {
            GParser parser = Parse(expr);
            ASTBuilder visitor = new ASTBuilder(expr);
            Expr result = (Expr)visitor.VisitSingleExpression(parser.singleExpression());
            return result;
        }

     


        private GParser Parse(string sql)
        {
            GLexer lexer = new GLexer(new NoCaseStringStream(sql));
            lexer.RemoveErrorListeners();
            //lexer.AddErrorListener(this.errorListener);
            CommonTokenStream tokenStream = new CommonTokenStream((ITokenSource)lexer);
            GParser parser = new GParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(this.errorListener);
            try
            {
                parser.Interpreter.PredictionMode = PredictionMode.Sll;
                return parser;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }

    public class AntlrParserException : Exception
    {
        public AntlrParserException(string msg) : base(msg) { }
    }

    public class SyntaxErrorListener : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            var stack = ((Parser)recognizer).GetRuleInvocationStack();
            string errormsg = $@"{string.Join("->", stack)} : {line}|{charPositionInLine}|{offendingSymbol}|{msg}";
            throw new AntlrParserException(errormsg);
        }
    }


    public class NoCaseStringStream : AntlrInputStream
    {
        public NoCaseStringStream(String input) : base(input)
        {

        }

        //public override int LA(int i)
        //{
        //    int la = base.LA(i);
        //    if (la == 0 || la == -1)
        //        return la;
        //    "11".ToUpper();
        //    return Character.toUpperCase(la);
        //}

        public override int La(int i)
        {
            int la = base.La(i);
            if (la == 0 || la == -1)
                return la;
            return Char.ToUpper((char)la);
        }
   

    }
}
