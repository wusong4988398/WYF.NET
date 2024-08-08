using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.parser
{
    public class ParseError : AlgoException
    {


    private int line;

    private int charPositionInLine;

    private String msg;

    private RecognitionException cause;

    public ParseError(int line, int charPositionInLine, String msg, RecognitionException e): base(CreateMsg(line, charPositionInLine, msg), e)
        {
        
        this.line = line;
        this.charPositionInLine = charPositionInLine;
        this.msg = msg;
        this.cause = e;
    }

    private static String CreateMsg(int line, int charPositionInLine, String msg)
    {
       return $"Parse error, line:{line}, position:{charPositionInLine}, msg:{msg}";
    }

        public int Line
        {
            get { return this.line; }
        }

        public int CharPositionInLine
        {
            get { return this.charPositionInLine; }
        }

        public string Msg
        {
            get { return this.msg; }
        }

        public RecognitionException Cause
        {
            get { return this.cause; }
        }
    }
}
