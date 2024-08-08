using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.parser
{
    public class ParseErrorListener : BaseErrorListener
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);
            throw new ParseError(line, charPositionInLine, msg, e);
        }
    }
}
