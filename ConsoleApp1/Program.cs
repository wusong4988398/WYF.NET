// See https://aka.ms/new-console-template for more information
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Tree;
using SQLParser.Parsers.TSql;
using WYF.Orm;
 ParseErrorListener errorListener = new ParseErrorListener();
Console.WriteLine("Hello, World!");
TSqlLexer lexer = new TSqlLexer(new AntlrInputStream("select*from usert"));
lexer.RemoveErrorListeners();





CommonTokenStream tokenStream = new CommonTokenStream(lexer);
TSqlParser parser = new TSqlParser(tokenStream);

parser.Interpreter.PredictionMode = PredictionMode.SLL;

//AstBuilder builder = new AstBuilder();
TSqlParserBaseVisitor<object> builder=new TSqlParserBaseVisitor<object>();
object result = builder.VisitExpression(parser.expression());

Console.WriteLine(result);