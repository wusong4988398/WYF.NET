using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WYF.Bos.algo.sql.g.GParser;
using static WYF.Bos.algo.sql.g.GParser.LogicalNotContext;

namespace WYF.Bos.algo.sql.g
{
    public class GParser : Parser
    {
        protected static readonly DFA[] _decisionToDFA;
        protected static readonly PredictionContextCache _sharedContextCache = new PredictionContextCache();

        public static readonly string[] ruleNames = new string[] {
      "singleExpression", "query", "queryNoWith", "queryOrganization", "queryTerm", "queryPrimary", "sortSet", "sortItem", "querySpecification", "fromClause",
      "aggregation", "setQuantifier", "relation", "joinType", "joinCriteria", "relationPrimary", "tableIdentifier", "namedExpression", "namedExpressionSeq", "expression",
      "booleanExpression", "predicated", "predicate", "valueExpression", "primaryExpression", "constant", "comparisonOperator", "booleanValue", "dataType", "whenClause",
      "qualifiedName", "identifier", "strictIdentifier", "quotedIdentifier", "number", "nonReserved" };

        private static readonly string[] _LITERAL_NAMES = new string[] {
      null, "','", "'('", "')'", "'.'", "'SELECT'", "'FROM'", "'AS'", "'DISTINCT'", "'WHERE'",
      "'GROUP'", "'BY'", "'ORDER'", "'HAVING'", "'LIMIT'", "'OR'", "'AND'", "'IN'", null, "'BETWEEN'",
      "'LIKE'", "'IS'", "'NULL'", "'TRUE'", "'FALSE'", "'ASC'", "'DESC'", "'CASE'", "'WHEN'", "'THEN'",
      "'ELSE'", "'END'", "'JOIN'", "'CROSS'", "'OUTER'", "'INNER'", "'LEFT'", "'RIGHT'", "'FULL'", "'ON'",
      "'CAST'", "'UNION'", null, "'<=>'", "'<>'", "'!='", "'<'", null, "'>'", null,
      "'+'", "'-'", "'*'", "'/'", "'%'", "'?'", "'||'" };

        private static readonly string[] _SYMBOLIC_NAMES = new string[] {
      null, null, null, null, null, "SELECT", "FROM", "AS", "DISTINCT", "WHERE",
      "GROUP", "BY", "ORDER", "HAVING", "LIMIT", "OR", "AND", "IN", "NOT", "BETWEEN",
      "LIKE", "IS", "NULL", "TRUE", "FALSE", "ASC", "DESC", "CASE", "WHEN", "THEN",
      "ELSE", "END", "JOIN", "CROSS", "OUTER", "INNER", "LEFT", "RIGHT", "FULL", "ON",
      "CAST", "UNION", "EQ", "NSEQ", "NEQ", "NEQJ", "LT", "LTE", "GT", "GTE",
      "PLUS", "MINUS", "ASTERISK", "SLASH", "PERCENT", "QUESTION", "CONCAT", "STRING", "BIGINT_LITERAL", "SMALLINT_LITERAL",
      "TINYINT_LITERAL", "BYTELENGTH_LITERAL", "INTEGER_VALUE", "DECIMAL_VALUE", "SCIENTIFIC_DECIMAL_VALUE", "DOUBLE_LITERAL", "IDENTIFIER", "BACKQUOTED_IDENTIFIER", "SIMPLE_COMMENT", "BRACKETED_COMMENT",
      "WS", "UNRECOGNIZED", "DELIMITER" };

        public static readonly IVocabulary VOCABULARY = new Vocabulary(_LITERAL_NAMES, _SYMBOLIC_NAMES);
        public static readonly string[] tokenNames = new string[_SYMBOLIC_NAMES.Length];

        public override string[] RuleNames => throw new NotImplementedException();

        public override IVocabulary Vocabulary => VOCABULARY;

        public override string GrammarFileName => throw new NotImplementedException();

        static GParser()
        {
            _decisionToDFA = new DFA[_ATN.NumberOfDecisions];

            int i;
            for (i = 0; i < tokenNames.Length; i++)
            {
                tokenNames[i] = VOCABULARY.GetLiteralName(i);
                if (tokenNames[i] == null)
                    tokenNames[i] = VOCABULARY.GetSymbolicName(i);
                if (tokenNames[i] == null)
                    tokenNames[i] = "<INVALID>";
            }
        }
        public GParser(ITokenStream input) : base(input)
        {

            Interpreter = new ParserATNSimulator(this, _ATN, _decisionToDFA, _sharedContextCache);
        }

        public static readonly string _serializedATN = "\u0003悋Ꜫ脳맭䅼㯧瞆奤\u0003Gǧ\u0004\u0002\t\u0002\u0004\u0003\t\u0003\u0004\u0004\t\u0004\u0004\u0005\t\u0005\u0004\u0006\t\u0006\u0004\u0007\t\u0007\u0004\b\t\b\u0004\t\t\t\u0004\n\t\n\u0004\u000b\t\u000b\u0004\f\t\f\u0004\r\t\r\u0004\u000e\t\u000e\u0004\u000f\t\u000f\u0004\u0010\t\u0010\u0004\u0011\t\u0011\u0004\u0012\t\u0012\u0004\u0013\t\u0013\u0004\u0014\t\u0014\u0004\u0015\t\u0015\u0004\u0016\t\u0016\u0004\u0017\t\u0017\u0004\u0018\t\u0018\u0004\u0019\t\u0019\u0004\u001a\t\u001a\u0004\u001b\t\u001b\u0004\u001c\t\u001c\u0004\u001d\t\u001d\u0004\u001e\t\u001e\u0004\u001f\t\u001f\u0004 \t \u0004!\t!\u0004\"\t\"\u0004#\t#\u0004$\t$\u0004%\t%\u0003\u0002\u0003\u0002\u0003\u0002\u0003\u0002\u0003\u0002\u0003\u0002\u0005\u0002Q\n\u0002\u0003\u0003\u0003\u0003\u0003\u0004\u0003\u0004\u0003\u0004\u0003\u0005\u0003\u0005\u0003\u0005\u0003\u0005\u0003\u0005\u0007\u0005]\n\u0005\f\u0005\u000e\u0005`\u000b\u0005\u0005\u0005b\n\u0005\u0003\u0005\u0003\u0005\u0003\u0005\u0003\u0005\u0007\u0005h\n\u0005\f\u0005\u000e\u0005k\u000b\u0005\u0005\u0005m\n\u0005\u0003\u0006\u0003\u0006\u0003\u0006\u0003\u0006\u0003\u0006\u0003\u0006\u0005\u0006u\n\u0006\u0003\u0006\u0007\u0006x\n\u0006\f\u0006\u000e\u0006{\u000b\u0006\u0003\u0007\u0003\u0007\u0003\b\u0003\b\u0003\b\u0007\b\u0082\n\b\f\b\u000e\b\u0085\u000b\b\u0003\t\u0003\t\u0005\t\u0089\n\t\u0003\n\u0003\n\u0005\n\u008d\n\n\u0003\n\u0003\n\u0005\n\u0091\n\n\u0003\n\u0003\n\u0005\n\u0095\n\n\u0003\n\u0005\n\u0098\n\n\u0003\n\u0003\n\u0005\n\u009c\n\n\u0003\u000b\u0003\u000b\u0003\u000b\u0003\u000b\u0007\u000b¢\n\u000b\f\u000b\u000e\u000b¥\u000b\u000b\u0003\f\u0003\f\u0003\f\u0003\f\u0003\f\u0007\f¬\n\f\f\f\u000e\f¯\u000b\f\u0003\r\u0003\r\u0003\u000e\u0003\u000e\u0003\u000e\u0003\u000e\u0003\u000e\u0003\u000e\u0005\u000e¹\n\u000e\u0003\u000e\u0003\u000e\u0003\u000e\u0005\u000e¾\n\u000e\u0007\u000eÀ\n\u000e\f\u000e\u000e\u000eÃ\u000b\u000e\u0003\u000f\u0005\u000fÆ\n\u000f\u0003\u000f\u0003\u000f\u0005\u000fÊ\n\u000f\u0003\u000f\u0003\u000f\u0005\u000fÎ\n\u000f\u0003\u000f\u0003\u000f\u0005\u000fÒ\n\u000f\u0005\u000fÔ\n\u000f\u0003\u0010\u0003\u0010\u0003\u0010\u0003\u0011\u0003\u0011\u0005\u0011Û\n\u0011\u0003\u0011\u0005\u0011Þ\n\u0011\u0003\u0012\u0003\u0012\u0003\u0012\u0005\u0012ã\n\u0012\u0003\u0012\u0003\u0012\u0003\u0013\u0003\u0013\u0005\u0013é\n\u0013\u0003\u0013\u0003\u0013\u0005\u0013í\n\u0013\u0005\u0013ï\n\u0013\u0003\u0014\u0003\u0014\u0003\u0014\u0007\u0014ô\n\u0014\f\u0014\u000e\u0014÷\u000b\u0014\u0003\u0015\u0003\u0015\u0003\u0016\u0003\u0016\u0003\u0016\u0003\u0016\u0005\u0016ÿ\n\u0016\u0003\u0016\u0003\u0016\u0003\u0016\u0003\u0016\u0003\u0016\u0003\u0016\u0007\u0016ć\n\u0016\f\u0016\u000e\u0016Ċ\u000b\u0016\u0003\u0017\u0003\u0017\u0005\u0017Ď\n\u0017\u0003\u0017\u0003\u0017\u0003\u0017\u0003\u0017\u0005\u0017Ĕ\n\u0017\u0003\u0018\u0005\u0018ė\n\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0007\u0018Ğ\n\u0018\f\u0018\u000e\u0018ġ\u000b\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0005\u0018Ħ\n\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0005\u0018ī\n\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0003\u0018\u0005\u0018ı\n\u0018\u0003\u0018\u0005\u0018Ĵ\n\u0018\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0005\u0019ĺ\n\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0003\u0019\u0007\u0019ŉ\n\u0019\f\u0019\u000e\u0019Ō\u000b\u0019\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0006\u001aŚ\n\u001a\r\u001a\u000e\u001aś\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0005\u001aţ\n\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0007\u001aŨ\n\u001a\f\u001a\u000e\u001aū\u000b\u001a\u0005\u001aŭ\n\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0006\u001aŴ\n\u001a\r\u001a\u000e\u001aŵ\u0003\u001a\u0003\u001a\u0005\u001aź\n\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0006\u001aƀ\n\u001a\r\u001a\u000e\u001aƁ\u0003\u001a\u0003\u001a\u0005\u001aƆ\n\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0005\u001aƖ\n\u001a\u0003\u001a\u0003\u001a\u0003\u001a\u0007\u001aƛ\n\u001a\f\u001a\u000e\u001aƞ\u000b\u001a\u0003\u001b\u0003\u001b\u0003\u001b\u0003\u001b\u0003\u001b\u0003\u001b\u0003\u001b\u0006\u001bƧ\n\u001b\r\u001b\u000e\u001bƨ\u0005\u001bƫ\n\u001b\u0003\u001c\u0003\u001c\u0003\u001d\u0003\u001d\u0003\u001e\u0003\u001e\u0003\u001e\u0003\u001e\u0003\u001e\u0007\u001eƶ\n\u001e\f\u001e\u000e\u001eƹ\u000b\u001e\u0003\u001e\u0005\u001eƼ\n\u001e\u0003\u001f\u0003\u001f\u0003\u001f\u0003\u001f\u0003\u001f\u0003 \u0003 \u0003 \u0007 ǆ\n \f \u000e ǉ\u000b \u0003!\u0003!\u0003!\u0003!\u0003!\u0003!\u0003!\u0003!\u0003!\u0005!ǔ\n!\u0003\"\u0003\"\u0003\"\u0005\"Ǚ\n\"\u0003#\u0003#\u0003$\u0003$\u0003$\u0003$\u0003$\u0003$\u0005$ǣ\n$\u0003%\u0003%\u0003%\u0002\u0007\n\u001a*02&\u0002\u0004\u0006\b\n\f\u000e\u0010\u0012\u0014\u0016\u0018\u001a\u001c\u001e \"$&(*,.02468:<>@BDFH\u0002\b\u0003\u0002\u001b\u001c\u0003\u000234\u0003\u000257\u0003\u0002,2\u0003\u0002\u0019\u001a\u0005\u0002\u0007!$$**\u0002ȟ\u0002P\u0003\u0002\u0002\u0002\u0004R\u0003\u0002\u0002\u0002\u0006T\u0003\u0002\u0002\u0002\ba\u0003\u0002\u0002\u0002\nn\u0003\u0002\u0002\u0002\f|\u0003\u0002\u0002\u0002\u000e~\u0003\u0002\u0002\u0002\u0010\u0086\u0003\u0002\u0002\u0002\u0012\u008a\u0003\u0002\u0002\u0002\u0014\u009d\u0003\u0002\u0002\u0002\u0016¦\u0003\u0002\u0002\u0002\u0018°\u0003\u0002\u0002\u0002\u001a²\u0003\u0002\u0002\u0002\u001cÓ\u0003\u0002\u0002\u0002\u001eÕ\u0003\u0002\u0002\u0002 Ø\u0003\u0002\u0002\u0002\"â\u0003\u0002\u0002\u0002$æ\u0003\u0002\u0002\u0002&ð\u0003\u0002\u0002\u0002(ø\u0003\u0002\u0002\u0002*þ\u0003\u0002\u0002\u0002,ē\u0003\u0002\u0002\u0002.ĳ\u0003\u0002\u0002\u00020Ĺ\u0003\u0002\u0002\u00022ƕ\u0003\u0002\u0002\u00024ƪ\u0003\u0002\u0002\u00026Ƭ\u0003\u0002\u0002\u00028Ʈ\u0003\u0002\u0002\u0002:ư\u0003\u0002\u0002\u0002<ƽ\u0003\u0002\u0002\u0002>ǂ\u0003\u0002\u0002\u0002@Ǔ\u0003\u0002\u0002\u0002Bǘ\u0003\u0002\u0002\u0002Dǚ\u0003\u0002\u0002\u0002FǢ\u0003\u0002\u0002\u0002HǤ\u0003\u0002\u0002\u0002JK\u0005$\u0013\u0002KL\u0007\u0002\u0002\u0003LQ\u0003\u0002\u0002\u0002MN\u0005&\u0014\u0002NO\u0007\u0002\u0002\u0003OQ\u0003\u0002\u0002\u0002PJ\u0003\u0002\u0002\u0002PM\u0003\u0002\u0002\u0002Q\u0003\u0003\u0002\u0002\u0002RS\u0005\u0006\u0004\u0002S\u0005\u0003\u0002\u0002\u0002TU\u0005\n\u0006\u0002UV\u0005\b\u0005\u0002V\u0007\u0003\u0002\u0002\u0002WX\u0007\u000e\u0002\u0002XY\u0007\r\u0002\u0002Y^\u0005\u0010\t\u0002Z[\u0007\u0003\u0002\u0002[]\u0005\u0010\t\u0002\\Z\u0003\u0002\u0002\u0002]`\u0003\u0002\u0002\u0002^\\\u0003\u0002\u0002\u0002^_\u0003\u0002\u0002\u0002_b\u0003\u0002\u0002\u0002`^\u0003\u0002\u0002\u0002aW\u0003\u0002\u0002\u0002ab\u0003\u0002\u0002\u0002bl\u0003\u0002\u0002\u0002cd\u0007\u0010\u0002\u0002di\u0007>\u0002\u0002ef\u0007\u0003\u0002\u0002fh\u0007>\u0002\u0002ge\u0003\u0002\u0002\u0002hk\u0003\u0002\u0002\u0002ig\u0003\u0002\u0002\u0002ij\u0003\u0002\u0002\u0002jm\u0003\u0002\u0002\u0002ki\u0003\u0002\u0002\u0002lc\u0003\u0002\u0002\u0002lm\u0003\u0002\u0002\u0002m\t\u0003\u0002\u0002\u0002no\b\u0006\u0001\u0002op\u0005\f\u0007\u0002py\u0003\u0002\u0002\u0002qr\f\u0003\u0002\u0002rt\u0007+\u0002\u0002su\u0005\u0018\r\u0002ts\u0003\u0002\u0002\u0002tu\u0003\u0002\u0002\u0002uv\u0003\u0002\u0002\u0002vx\u0005\n\u0006\u0004wq\u0003\u0002\u0002\u0002x{\u0003\u0002\u0002\u0002yw\u0003\u0002\u0002\u0002yz\u0003\u0002\u0002\u0002z\u000b\u0003\u0002\u0002\u0002{y\u0003\u0002\u0002\u0002|}\u0005\u0012\n\u0002}\r\u0003\u0002\u0002\u0002~\u0083\u0005\u0010\t\u0002\u007f\u0080\u0007\u0003\u0002\u0002\u0080\u0082\u0005\u0010\t\u0002\u0081\u007f\u0003\u0002\u0002\u0002\u0082\u0085\u0003\u0002\u0002\u0002\u0083\u0081\u0003\u0002\u0002\u0002\u0083\u0084\u0003\u0002\u0002\u0002\u0084\u000f\u0003\u0002\u0002\u0002\u0085\u0083\u0003\u0002\u0002\u0002\u0086\u0088\u0005(\u0015\u0002\u0087\u0089\t\u0002\u0002\u0002\u0088\u0087\u0003\u0002\u0002\u0002\u0088\u0089\u0003\u0002\u0002\u0002\u0089\u0011\u0003\u0002\u0002\u0002\u008a\u008c\u0007\u0007\u0002\u0002\u008b\u008d\u0005\u0018\r\u0002\u008c\u008b\u0003\u0002\u0002\u0002\u008c\u008d\u0003\u0002\u0002\u0002\u008d\u008e\u0003\u0002\u0002\u0002\u008e\u0090\u0005&\u0014\u0002\u008f\u0091\u0005\u0014\u000b\u0002\u0090\u008f\u0003\u0002\u0002\u0002\u0090\u0091\u0003\u0002\u0002\u0002\u0091\u0094\u0003\u0002\u0002\u0002\u0092\u0093\u0007\u000b\u0002\u0002\u0093\u0095\u0005*\u0016\u0002\u0094\u0092\u0003\u0002\u0002\u0002\u0094\u0095\u0003\u0002\u0002\u0002\u0095\u0097\u0003\u0002\u0002\u0002\u0096\u0098\u0005\u0016\f\u0002\u0097\u0096\u0003\u0002\u0002\u0002\u0097\u0098\u0003\u0002\u0002\u0002\u0098\u009b\u0003\u0002\u0002\u0002\u0099\u009a\u0007\u000f\u0002\u0002\u009a\u009c\u0005*\u0016\u0002\u009b\u0099\u0003\u0002\u0002\u0002\u009b\u009c\u0003\u0002\u0002\u0002\u009c\u0013\u0003\u0002\u0002\u0002\u009d\u009e\u0007\b\u0002\u0002\u009e£\u0005\u001a\u000e\u0002\u009f \u0007\u0003\u0002\u0002 ¢\u0005\u001a\u000e\u0002¡\u009f\u0003\u0002\u0002\u0002¢¥\u0003\u0002\u0002\u0002£¡\u0003\u0002\u0002\u0002£¤\u0003\u0002\u0002\u0002¤\u0015\u0003\u0002\u0002\u0002¥£\u0003\u0002\u0002\u0002¦§\u0007\f\u0002\u0002§¨\u0007\r\u0002\u0002¨\u00ad\u0005(\u0015\u0002©ª\u0007\u0003\u0002\u0002ª¬\u0005(\u0015\u0002«©\u0003\u0002\u0002\u0002¬¯\u0003\u0002\u0002\u0002\u00ad«\u0003\u0002\u0002\u0002\u00ad®\u0003\u0002\u0002\u0002®\u0017\u0003\u0002\u0002\u0002¯\u00ad\u0003\u0002\u0002\u0002°±\u0007\n\u0002\u0002±\u0019\u0003\u0002\u0002\u0002²³\b\u000e\u0001\u0002³´\u0005 \u0011\u0002´Á\u0003\u0002\u0002\u0002µ¸\f\u0004\u0002\u0002¶¹\u0007#\u0002\u0002·¹\u0005\u001c\u000f\u0002¸¶\u0003\u0002\u0002\u0002¸·\u0003\u0002\u0002\u0002¹º\u0003\u0002\u0002\u0002º»\u0007\"\u0002\u0002»½\u0005\u001a\u000e\u0002¼¾\u0005\u001e\u0010\u0002½¼\u0003\u0002\u0002\u0002½¾\u0003\u0002\u0002\u0002¾À\u0003\u0002\u0002\u0002¿µ\u0003\u0002\u0002\u0002ÀÃ\u0003\u0002\u0002\u0002Á¿\u0003\u0002\u0002\u0002ÁÂ\u0003\u0002\u0002\u0002Â\u001b\u0003\u0002\u0002\u0002ÃÁ\u0003\u0002\u0002\u0002ÄÆ\u0007%\u0002\u0002ÅÄ\u0003\u0002\u0002\u0002ÅÆ\u0003\u0002\u0002\u0002ÆÔ\u0003\u0002\u0002\u0002ÇÉ\u0007&\u0002\u0002ÈÊ\u0007$\u0002\u0002ÉÈ\u0003\u0002\u0002\u0002ÉÊ\u0003\u0002\u0002\u0002ÊÔ\u0003\u0002\u0002\u0002ËÍ\u0007'\u0002\u0002ÌÎ\u0007$\u0002\u0002ÍÌ\u0003\u0002\u0002\u0002ÍÎ\u0003\u0002\u0002\u0002ÎÔ\u0003\u0002\u0002\u0002ÏÑ\u0007(\u0002\u0002ÐÒ\u0007$\u0002\u0002ÑÐ\u0003\u0002\u0002\u0002ÑÒ\u0003\u0002\u0002\u0002ÒÔ\u0003\u0002\u0002\u0002ÓÅ\u0003\u0002\u0002\u0002ÓÇ\u0003\u0002\u0002\u0002ÓË\u0003\u0002\u0002\u0002ÓÏ\u0003\u0002\u0002\u0002Ô\u001d\u0003\u0002\u0002\u0002ÕÖ\u0007)\u0002\u0002Ö×\u0005*\u0016\u0002×\u001f\u0003\u0002\u0002\u0002ØÝ\u0005\"\u0012\u0002ÙÛ\u0007\t\u0002\u0002ÚÙ\u0003\u0002\u0002\u0002ÚÛ\u0003\u0002\u0002\u0002ÛÜ\u0003\u0002\u0002\u0002ÜÞ\u0005B\"\u0002ÝÚ\u0003\u0002\u0002\u0002ÝÞ\u0003\u0002\u0002\u0002Þ!\u0003\u0002\u0002\u0002ßà\u0005@!\u0002àá\u0007\u0004\u0002\u0002áã\u0003\u0002\u0002\u0002âß\u0003\u0002\u0002\u0002âã\u0003\u0002\u0002\u0002ãä\u0003\u0002\u0002\u0002äå\u0005@!\u0002å#\u0003\u0002\u0002\u0002æî\u0005(\u0015\u0002çé\u0007\t\u0002\u0002èç\u0003\u0002\u0002\u0002èé\u0003\u0002\u0002\u0002éì\u0003\u0002\u0002\u0002êí\u0005> \u0002ëí\u0005@!\u0002ìê\u0003\u0002\u0002\u0002ìë\u0003\u0002\u0002\u0002íï\u0003\u0002\u0002\u0002îè\u0003\u0002\u0002\u0002îï\u0003\u0002\u0002\u0002ï%\u0003\u0002\u0002\u0002ðõ\u0005$\u0013\u0002ñò\u0007\u0003\u0002\u0002òô\u0005$\u0013\u0002óñ\u0003\u0002\u0002\u0002ô÷\u0003\u0002\u0002\u0002õó\u0003\u0002\u0002\u0002õö\u0003\u0002\u0002\u0002ö'\u0003\u0002\u0002\u0002÷õ\u0003\u0002\u0002\u0002øù\u0005*\u0016\u0002ù)\u0003\u0002\u0002\u0002úû\b\u0016\u0001\u0002ûü\u0007\u0014\u0002\u0002üÿ\u0005*\u0016\u0006ýÿ\u0005,\u0017\u0002þú\u0003\u0002\u0002\u0002þý\u0003\u0002\u0002\u0002ÿĈ\u0003\u0002\u0002\u0002Āā\f\u0005\u0002\u0002āĂ\u0007\u0012\u0002\u0002Ăć\u0005*\u0016\u0006ăĄ\f\u0004\u0002\u0002Ąą\u0007\u0011\u0002\u0002ąć\u0005*\u0016\u0005ĆĀ\u0003\u0002\u0002\u0002Ćă\u0003\u0002\u0002\u0002ćĊ\u0003\u0002\u0002\u0002ĈĆ\u0003\u0002\u0002\u0002Ĉĉ\u0003\u0002\u0002\u0002ĉ+\u0003\u0002\u0002\u0002ĊĈ\u0003\u0002\u0002\u0002ċč\u00050\u0019\u0002ČĎ\u0005.\u0018\u0002čČ\u0003\u0002\u0002\u0002čĎ\u0003\u0002\u0002\u0002ĎĔ\u0003\u0002\u0002\u0002ďĐ\u0007\u0005\u0002\u0002Đđ\u0005,\u0017\u0002đĒ\u0007\u0006\u0002\u0002ĒĔ\u0003\u0002\u0002\u0002ēċ\u0003\u0002\u0002\u0002ēď\u0003\u0002\u0002\u0002Ĕ-\u0003\u0002\u0002\u0002ĕė\u0007\u0014\u0002\u0002Ėĕ\u0003\u0002\u0002\u0002Ėė\u0003\u0002\u0002\u0002ėĘ\u0003\u0002\u0002\u0002Ęę\u0007\u0013\u0002\u0002ęĚ\u0007\u0005\u0002\u0002Ěğ\u0005(\u0015\u0002ěĜ\u0007\u0003\u0002\u0002ĜĞ\u0005(\u0015\u0002ĝě\u0003\u0002\u0002\u0002Ğġ\u0003\u0002\u0002\u0002ğĝ\u0003\u0002\u0002\u0002ğĠ\u0003\u0002\u0002\u0002ĠĢ\u0003\u0002\u0002\u0002ġğ\u0003\u0002\u0002\u0002Ģģ\u0007\u0006\u0002\u0002ģĴ\u0003\u0002\u0002\u0002ĤĦ\u0007\u0014\u0002\u0002ĥĤ\u0003\u0002\u0002\u0002ĥĦ\u0003\u0002\u0002\u0002Ħħ\u0003\u0002\u0002\u0002ħĨ\u0007\u0013\u0002\u0002ĨĴ\u0005(\u0015\u0002ĩī\u0007\u0014\u0002\u0002Īĩ\u0003\u0002\u0002\u0002Īī\u0003\u0002\u0002\u0002īĬ\u0003\u0002\u0002\u0002Ĭĭ\u0007\u0016\u0002\u0002ĭĴ\u00050\u0019\u0002Įİ\u0007\u0017\u0002\u0002įı\u0007\u0014\u0002\u0002İį\u0003\u0002\u0002\u0002İı\u0003\u0002\u0002\u0002ıĲ\u0003\u0002\u0002\u0002ĲĴ\u0007\u0018\u0002\u0002ĳĖ\u0003\u0002\u0002\u0002ĳĥ\u0003\u0002\u0002\u0002ĳĪ\u0003\u0002\u0002\u0002ĳĮ\u0003\u0002\u0002\u0002Ĵ/\u0003\u0002\u0002\u0002ĵĶ\b\u0019\u0001\u0002Ķĺ\u00052\u001a\u0002ķĸ\t\u0003\u0002\u0002ĸĺ\u00050\u0019\u0007Ĺĵ\u0003\u0002\u0002\u0002Ĺķ\u0003\u0002\u0002\u0002ĺŊ\u0003\u0002\u0002\u0002Ļļ\f\u0006\u0002\u0002ļĽ\t\u0004\u0002\u0002Ľŉ\u00050\u0019\u0007ľĿ\f\u0005\u0002\u0002Ŀŀ\t\u0003\u0002\u0002ŀŉ\u00050\u0019\u0006Łł\f\u0004\u0002\u0002łŃ\u00056\u001c\u0002Ńń\u00050\u0019\u0005ńŉ\u0003\u0002\u0002\u0002Ņņ\f\u0003\u0002\u0002ņŇ\u00079\u0002\u0002Ňŉ\u00050\u0019\u0004ňĻ\u0003\u0002\u0002\u0002ňľ\u0003\u0002\u0002\u0002ňŁ\u0003\u0002\u0002\u0002ňŅ\u0003\u0002\u0002\u0002ŉŌ\u0003\u0002\u0002\u0002Ŋň\u0003\u0002\u0002\u0002Ŋŋ\u0003\u0002\u0002\u0002ŋ1\u0003\u0002\u0002\u0002ŌŊ\u0003\u0002\u0002\u0002ōŎ\b\u001a\u0001\u0002ŎƖ\u00054\u001b\u0002ŏƖ\u00075\u0002\u0002Őő\u0005> \u0002őŒ\u0007\u0004\u0002\u0002Œœ\u00075\u0002\u0002œƖ\u0003\u0002\u0002\u0002ŔƖ\u00078\u0002\u0002ŕŖ\u0007\u0005\u0002\u0002Ŗř\u0005(\u0015\u0002ŗŘ\u0007\u0003\u0002\u0002ŘŚ\u0005(\u0015\u0002řŗ\u0003\u0002\u0002\u0002Śś\u0003\u0002\u0002\u0002śř\u0003\u0002\u0002\u0002śŜ\u0003\u0002\u0002\u0002Ŝŝ\u0003\u0002\u0002\u0002ŝŞ\u0007\u0006\u0002\u0002ŞƖ\u0003\u0002\u0002\u0002şŠ\u0005> \u0002ŠŬ\u0007\u0005\u0002\u0002šţ\u0005\u0018\r\u0002Ţš\u0003\u0002\u0002\u0002Ţţ\u0003\u0002\u0002\u0002ţŤ\u0003\u0002\u0002\u0002Ťũ\u0005(\u0015\u0002ťŦ\u0007\u0003\u0002\u0002ŦŨ\u0005(\u0015\u0002ŧť\u0003\u0002\u0002\u0002Ũū\u0003\u0002\u0002\u0002ũŧ\u0003\u0002\u0002\u0002ũŪ\u0003\u0002\u0002\u0002Ūŭ\u0003\u0002\u0002\u0002ūũ\u0003\u0002\u0002\u0002ŬŢ\u0003\u0002\u0002\u0002Ŭŭ\u0003\u0002\u0002\u0002ŭŮ\u0003\u0002\u0002\u0002Ůů\u0007\u0006\u0002\u0002ůƖ\u0003\u0002\u0002\u0002Űű\u0007\u001d\u0002\u0002űų\u00050\u0019\u0002ŲŴ\u0005<\u001f\u0002ųŲ\u0003\u0002\u0002\u0002Ŵŵ\u0003\u0002\u0002\u0002ŵų\u0003\u0002\u0002\u0002ŵŶ\u0003\u0002\u0002\u0002ŶŹ\u0003\u0002\u0002\u0002ŷŸ\u0007 \u0002\u0002Ÿź\u0005(\u0015\u0002Źŷ\u0003\u0002\u0002\u0002Źź\u0003\u0002\u0002\u0002źŻ\u0003\u0002\u0002\u0002Żż\u0007!\u0002\u0002żƖ\u0003\u0002\u0002\u0002Žſ\u0007\u001d\u0002\u0002žƀ\u0005<\u001f\u0002ſž\u0003\u0002\u0002\u0002ƀƁ\u0003\u0002\u0002\u0002Ɓſ\u0003\u0002\u0002\u0002ƁƂ\u0003\u0002\u0002\u0002Ƃƅ\u0003\u0002\u0002\u0002ƃƄ\u0007 \u0002\u0002ƄƆ\u0005(\u0015\u0002ƅƃ\u0003\u0002\u0002\u0002ƅƆ\u0003\u0002\u0002\u0002ƆƇ\u0003\u0002\u0002\u0002Ƈƈ\u0007!\u0002\u0002ƈƖ\u0003\u0002\u0002\u0002ƉƊ\u0007*\u0002\u0002ƊƋ\u0007\u0005\u0002\u0002Ƌƌ\u0005(\u0015\u0002ƌƍ\u0007\t\u0002\u0002ƍƎ\u0005:\u001e\u0002ƎƏ\u0007\u0006\u0002\u0002ƏƖ\u0003\u0002\u0002\u0002ƐƖ\u0005@!\u0002Ƒƒ\u0007\u0005\u0002\u0002ƒƓ\u0005(\u0015\u0002ƓƔ\u0007\u0006\u0002\u0002ƔƖ\u0003\u0002\u0002\u0002ƕō\u0003\u0002\u0002\u0002ƕŏ\u0003\u0002\u0002\u0002ƕŐ\u0003\u0002\u0002\u0002ƕŔ\u0003\u0002\u0002\u0002ƕŕ\u0003\u0002\u0002\u0002ƕş\u0003\u0002\u0002\u0002ƕŰ\u0003\u0002\u0002\u0002ƕŽ\u0003\u0002\u0002\u0002ƕƉ\u0003\u0002\u0002\u0002ƕƐ\u0003\u0002\u0002\u0002ƕƑ\u0003\u0002\u0002\u0002ƖƜ\u0003\u0002\u0002\u0002ƗƘ\f\u0004\u0002\u0002Ƙƙ\u0007\u0004\u0002\u0002ƙƛ\u0005@!\u0002ƚƗ\u0003\u0002\u0002\u0002ƛƞ\u0003\u0002\u0002\u0002Ɯƚ\u0003\u0002\u0002\u0002ƜƝ\u0003\u0002\u0002\u0002Ɲ3\u0003\u0002\u0002\u0002ƞƜ\u0003\u0002\u0002\u0002Ɵƫ\u0007\u0018\u0002\u0002Ơơ\u0005@!\u0002ơƢ\u0007:\u0002\u0002Ƣƫ\u0003\u0002\u0002\u0002ƣƫ\u0005F$\u0002Ƥƫ\u00058\u001d\u0002ƥƧ\u0007:\u0002\u0002Ʀƥ\u0003\u0002\u0002\u0002Ƨƨ\u0003\u0002\u0002\u0002ƨƦ\u0003\u0002\u0002\u0002ƨƩ\u0003\u0002\u0002\u0002Ʃƫ\u0003\u0002\u0002\u0002ƪƟ\u0003\u0002\u0002\u0002ƪƠ\u0003\u0002\u0002\u0002ƪƣ\u0003\u0002\u0002\u0002ƪƤ\u0003\u0002\u0002\u0002ƪƦ\u0003\u0002\u0002\u0002ƫ5\u0003\u0002\u0002\u0002Ƭƭ\t\u0005\u0002\u0002ƭ7\u0003\u0002\u0002\u0002ƮƯ\t\u0006\u0002\u0002Ư9\u0003\u0002\u0002\u0002ưƻ\u0005@!\u0002ƱƲ\u0007\u0005\u0002\u0002ƲƷ\u0007>\u0002\u0002Ƴƴ\u0007\u0003\u0002\u0002ƴƶ\u0007>\u0002\u0002ƵƳ\u0003\u0002\u0002\u0002ƶƹ\u0003\u0002\u0002\u0002ƷƵ\u0003\u0002\u0002\u0002ƷƸ\u0003\u0002\u0002\u0002Ƹƺ\u0003\u0002\u0002\u0002ƹƷ\u0003\u0002\u0002\u0002ƺƼ\u0007\u0006\u0002\u0002ƻƱ\u0003\u0002\u0002\u0002ƻƼ\u0003\u0002\u0002\u0002Ƽ;\u0003\u0002\u0002\u0002ƽƾ\u0007\u001e\u0002\u0002ƾƿ\u0005(\u0015\u0002ƿǀ\u0007\u001f\u0002\u0002ǀǁ\u0005(\u0015\u0002ǁ=\u0003\u0002\u0002\u0002ǂǇ\u0005@!\u0002ǃǄ\u0007\u0004\u0002\u0002Ǆǆ\u0005@!\u0002ǅǃ\u0003\u0002\u0002\u0002ǆǉ\u0003\u0002\u0002\u0002Ǉǅ\u0003\u0002\u0002\u0002Ǉǈ\u0003\u0002\u0002\u0002ǈ?\u0003\u0002\u0002\u0002ǉǇ\u0003\u0002\u0002\u0002Ǌǔ\u0005B\"\u0002ǋǔ\u0007(\u0002\u0002ǌǔ\u0007%\u0002\u0002Ǎǔ\u0007&\u0002\u0002ǎǔ\u0007'\u0002\u0002Ǐǔ\u0007\"\u0002\u0002ǐǔ\u0007#\u0002\u0002Ǒǔ\u0007)\u0002\u0002ǒǔ\u0007+\u0002\u0002ǓǊ\u0003\u0002\u0002\u0002Ǔǋ\u0003\u0002\u0002\u0002Ǔǌ\u0003\u0002\u0002\u0002ǓǍ\u0003\u0002\u0002\u0002Ǔǎ\u0003\u0002\u0002\u0002ǓǏ\u0003\u0002\u0002\u0002Ǔǐ\u0003\u0002\u0002\u0002ǓǑ\u0003\u0002\u0002\u0002Ǔǒ\u0003\u0002\u0002\u0002ǔA\u0003\u0002\u0002\u0002ǕǙ\u0007A\u0002\u0002ǖǙ\u0005D#\u0002ǗǙ\u0005H%\u0002ǘǕ\u0003\u0002\u0002\u0002ǘǖ\u0003\u0002\u0002\u0002ǘǗ\u0003\u0002\u0002\u0002ǙC\u0003\u0002\u0002\u0002ǚǛ\u0007B\u0002\u0002ǛE\u0003\u0002\u0002\u0002ǜǣ\u0007?\u0002\u0002ǝǣ\u0007>\u0002\u0002Ǟǣ\u0007;\u0002\u0002ǟǣ\u0007<\u0002\u0002Ǡǣ\u0007=\u0002\u0002ǡǣ\u0007@\u0002\u0002Ǣǜ\u0003\u0002\u0002\u0002Ǣǝ\u0003\u0002\u0002\u0002ǢǞ\u0003\u0002\u0002\u0002Ǣǟ\u0003\u0002\u0002\u0002ǢǠ\u0003\u0002\u0002\u0002Ǣǡ\u0003\u0002\u0002\u0002ǣG\u0003\u0002\u0002\u0002Ǥǥ\t\u0007\u0002\u0002ǥI\u0003\u0002\u0002\u0002AP^ailty\u0083\u0088\u008c\u0090\u0094\u0097\u009b£\u00ad¸½ÁÅÉÍÑÓÚÝâèìîõþĆĈčēĖğĥĪİĳĹňŊśŢũŬŵŹƁƅƕƜƨƪƷƻǇǓǘǢ";

        public static readonly ATN _ATN = new ATNDeserializer().Deserialize(_serializedATN.ToCharArray());





        public  class SingleExpressionContext : ParserRuleContext
        {
    public NamedExpressionContext NamedExpression()
        {
            return (NamedExpressionContext)GetRuleContext<NamedExpressionContext>(0);
    }

    public ITerminalNode EOF()
    {
        return GetToken(-1, 0);
    }

    public NamedExpressionSeqContext NamedExpressionSeq()
    {
        return (NamedExpressionSeqContext)GetRuleContext<NamedExpressionSeqContext>(0);
    }

public SingleExpressionContext(ParserRuleContext parent, int invokingState):base(parent, invokingState)
{
   
}
            public override int RuleIndex => 0;


public void EnterRule(IParseTreeListener listener)
{
    if (listener is GListener)
        ((GListener)listener).EnterSingleExpression(this);
    }

    public void ExitRule(IParseTreeListener listener)
{
    if (listener is GListener)
        ((GListener)listener).ExitSingleExpression(this);
    }

    public  override T Accept<T>(IParseTreeVisitor <T> visitor) {
    if (visitor is GVisitor<T>)
        return (T)((GVisitor<T>)visitor).VisitSingleExpression(this);
    return (T)visitor.VisitChildren((IRuleNode)this);
}
  }

        public class NamedExpressionContext : ParserRuleContext
        {
            public ExpressionContext Expression()
            {
                return GetRuleContext<ExpressionContext>(0);
            }
            public QualifiedNameContext QualifiedName()
            {
                return GetRuleContext<QualifiedNameContext>(0);
            }
            public IdentifierContext Identifier()
            {
                return GetRuleContext<IdentifierContext>(0);
            }
            public ITerminalNode AS()
            {
                return GetToken(7, 0);
            }
            public NamedExpressionContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }

            public int GetRuleIndex()
            {
                return 17;
            }
            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {

                    ((GListener)listener).EnterNamedExpression(this);
                }

            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {

                    ((GListener)listener).ExitNamedExpression(this);
                }

            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                {

                    ((GVisitor<T>)visitor).VisitNamedExpression(this);
                }

                return visitor.VisitChildren(this);
            }

        }
        public class ExpressionContext : ParserRuleContext
        {
            public BooleanExpressionContext BooleanExpression()
            {
                return GetRuleContext<BooleanExpressionContext>(0);
            }

            public ExpressionContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public int GetRuleIndex()
            {
                return 19;
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {

                    ((GListener)listener).EnterExpression(this);
                }

            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {

                    ((GListener)listener).ExitExpression(this);
                }

            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                {

                    ((GVisitor<T>)visitor).VisitExpression(this);
                }

                return visitor.VisitChildren(this);
            }


        }
        public class QualifiedNameContext : ParserRuleContext
        {
            public List<IdentifierContext> identifier()
            {
                return GetRuleContexts<IdentifierContext>().ToList();
            }

            public IdentifierContext Identifier(int i)
            {
                return GetRuleContext<IdentifierContext>(i);
            }

            public QualifiedNameContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 30;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterQualifiedName(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitQualifiedName(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitQualifiedName(this);
                return visitor.VisitChildren(this);
            }
        }



        public class StrictIdentifierContext : ParserRuleContext
        {
            public StrictIdentifierContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }

            public override int RuleIndex => 32;


            public StrictIdentifierContext() { }

            public void CopyFrom(StrictIdentifierContext ctx)
            {
                base.CopyFrom(ctx);
            }
        }

        public class IdentifierContext : ParserRuleContext
        {
            public StrictIdentifierContext StrictIdentifier()
            {
                return GetRuleContext<StrictIdentifierContext>(0);
            }

            public ITerminalNode FULL()
            {
                return GetToken(38, 0);
            }

            public ITerminalNode INNER()
            {
                return GetToken(35, 0);
            }

            public ITerminalNode LEFT()
            {
                return GetToken(36, 0);
            }

            public ITerminalNode RIGHT()
            {
                return GetToken(37, 0);
            }

            public ITerminalNode JOIN()
            {
                return GetToken(32, 0);
            }

            public ITerminalNode CROSS()
            {
                return GetToken(33, 0);
            }

            public ITerminalNode ON()
            {
                return GetToken(39, 0);
            }

            public ITerminalNode UNION()
            {
                return GetToken(41, 0);
            }

            public IdentifierContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 31;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterIdentifier(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitIdentifier(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitIdentifier(this);
                return visitor.VisitChildren(this);
            }
        }

        public class BooleanExpressionContext : ParserRuleContext
        {
            public BooleanExpressionContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public int GetRuleIndex()
            {
                return 20;
            }
            public BooleanExpressionContext()
            {

            }

            public void CopyFrom(BooleanExpressionContext ctx)
            {
                base.CopyFrom(ctx);
            }
        }

        public class LogicalNotContext : BooleanExpressionContext
        {
            public ITerminalNode NOT()
            {
                return GetToken(18, 0);
            }

            public BooleanExpressionContext BooleanExpression()
            {
                return GetRuleContext<BooleanExpressionContext>(0);
            }

            public LogicalNotContext(BooleanExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).EnterLogicalNot(this);
                }

            }

            public void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).ExitLogicalNot(this);
                }

            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                return visitor is GVisitor<T> ? ((GVisitor<T>)visitor).VisitLogicalNot(this) : visitor.VisitChildren(this);
            }

        }

        public class BooleanDefaultContext : BooleanExpressionContext
        {
            public PredicatedContext Predicated()
            {
                return GetRuleContext<PredicatedContext>(0);
            }

            public BooleanDefaultContext(BooleanExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public void enterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).EnterBooleanDefault(this);
                }

            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).ExitBooleanDefault(this);
                }

            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                return visitor is GVisitor<T> ? ((GVisitor<T>)visitor).VisitBooleanDefault(this) : visitor.VisitChildren(this);
            }
        }

        public class ValueExpressionContext : ParserRuleContext
        {
            public ValueExpressionContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 23;


            public ValueExpressionContext()
            {
            }
            public void CopyFrom(ValueExpressionContext ctx)
            {
                base.CopyFrom(ctx);
            }

        }

        public class PredicateContext : ParserRuleContext
        {
            public IToken kind;
            public ValueExpressionContext pattern;

            public List<ExpressionContext> Expression()
            {
                return GetRuleContexts<ExpressionContext>().ToList();
            }

            public ExpressionContext Expression(int i)
            {
                return GetRuleContext<ExpressionContext>(i);
            }

            public ITerminalNode IN()
            {
                return GetToken(17, 0);
            }


            public ITerminalNode NOT()
            {
                return GetToken(18, 0);
            }

            public ITerminalNode LIKE()
            {
                return GetToken(20, 0);
            }

            public ValueExpressionContext ValueExpression()
            {
                return GetRuleContext<ValueExpressionContext>(0);
            }

            public ITerminalNode IS()
            {
                return GetToken(21, 0);
            }

            public ITerminalNode NULL()
            {
                return GetToken(22, 0);
            }

            public PredicateContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 22;



            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).EnterPredicate(this);
                }

            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).ExitPredicate(this);
                }

            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                return visitor is GVisitor<T> ? ((GVisitor<T>)visitor).VisitPredicate(this) : visitor.VisitChildren(this);
            }
        }

        public class PredicatedContext : ParserRuleContext
        {
            public ValueExpressionContext valueExpression()
            {
                return GetRuleContext<ValueExpressionContext>(0);
            }
            public PredicateContext Predicate()
            {
                return GetRuleContext<PredicateContext>(0);
            }

            public PredicatedContext Predicated()
            {
                return GetRuleContext<PredicatedContext>(0);
            }

            public PredicatedContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 21;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).EnterPredicated(this);
                }

            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).ExitPredicated(this);
                }

            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                return visitor is GVisitor<T> ? ((GVisitor<T>)visitor).VisitPredicated(this) : visitor.VisitChildren(this);
            }
        }
        public class LogicalBinaryContext : BooleanExpressionContext
        {
            public BooleanExpressionContext left;
            public IToken operators;
            public BooleanExpressionContext right;

            public List<BooleanExpressionContext> booleanExpression()
            {
                return GetRuleContexts<BooleanExpressionContext>().ToList();
            }

            public BooleanExpressionContext BooleanExpression(int i)
            {
                return GetRuleContext<BooleanExpressionContext>(i);
            }

            public ITerminalNode AND()
            {
                return GetToken(16, 0);
            }

            public ITerminalNode OR()
            {
                return GetToken(15, 0);
            }

            public LogicalBinaryContext(BooleanExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).EnterLogicalBinary(this);
                }

            }

            public void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                {
                    ((GListener)listener).ExitLogicalBinary(this);
                }

            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                return visitor is GVisitor<T> ? ((GVisitor<T>)visitor).VisitLogicalBinary(this) : visitor.VisitChildren(this);
            }
        }

        public class ArithmeticBinaryContext : ValueExpressionContext
        {
            public ValueExpressionContext left;

            public IToken operators;

            public ValueExpressionContext right;

            public List<ValueExpressionContext> ValueExpression()
            {
                return GetRuleContexts<ValueExpressionContext>().ToList();
            }

            public ValueExpressionContext ValueExpression(int i)
            {
                return GetRuleContext<ValueExpressionContext>(i);
            }

            public ITerminalNode ASTERISK()
            {
                return GetToken(52, 0);
            }

            public ITerminalNode SLASH()
            {
                return GetToken(53, 0);
            }

            public ITerminalNode PERCENT()
            {
                return GetToken(54, 0);
            }

            public ITerminalNode PLUS()
            {
                return GetToken(50, 0);
            }

            public ITerminalNode MINUS()
            {
                return GetToken(51, 0);
            }

            public ArithmeticBinaryContext(ValueExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public void enterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterArithmeticBinary(this);
            }

            public void exitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitArithmeticBinary(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitArithmeticBinary(this);
                return visitor.VisitChildren(this);
            }
        }

        public class ComparisonContext : ValueExpressionContext
        {
            public ValueExpressionContext left;

            public ValueExpressionContext right;

            public ComparisonOperatorContext ComparisonOperator()
            {
                return GetRuleContext<ComparisonOperatorContext>(0);
            }

            public List<ValueExpressionContext> ValueExpression()
            {
                return GetRuleContexts<ValueExpressionContext>().ToList();
            }

            public ValueExpressionContext ValueExpression(int i)
            {
                return GetRuleContext<ValueExpressionContext>(i);
            }

            public ComparisonContext(ValueExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterComparison(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitComparison(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitComparison(this);
                return visitor.VisitChildren(this);
            }
        }



        public class ComparisonOperatorContext : ParserRuleContext
        {
            public ITerminalNode EQ()
            {
                return GetToken(42, 0);
            }

            public ITerminalNode NEQ()
            {
                return GetToken(44, 0);
            }

            public ITerminalNode NEQJ()
            {
                return GetToken(45, 0);
            }

            public ITerminalNode LT()
            {
                return GetToken(46, 0);
            }

            public ITerminalNode LTE()
            {
                return GetToken(47, 0);
            }

            public ITerminalNode GT()
            {
                return GetToken(48, 0);
            }

            public ITerminalNode GTE()
            {
                return GetToken(49, 0);
            }

            public ITerminalNode NSEQ()
            {
                return GetToken(43, 0);
            }

            public ComparisonOperatorContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 26;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterComparisonOperator(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitComparisonOperator(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitComparisonOperator(this);
                return visitor.VisitChildren(this);
            }
        }


        public class StringAddContext : ValueExpressionContext
        {
            public ValueExpressionContext left;

            public ValueExpressionContext right;

            public ITerminalNode CONCAT()
            {
                return GetToken(56, 0);
            }

            public List<ValueExpressionContext> ValueExpression()
            {
                return GetRuleContexts<ValueExpressionContext>().ToList();
            }

            public ValueExpressionContext valueExpression(int i)
            {
                return GetRuleContext<ValueExpressionContext>(i);
            }

            public StringAddContext(ValueExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterStringAdd(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitStringAdd(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitStringAdd(this);
                return visitor.VisitChildren(this);
            }
        }



        public class PrimaryExpressionContext : ParserRuleContext
        {
            public PrimaryExpressionContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 24;


            public PrimaryExpressionContext() { }

            public void CopyFrom(PrimaryExpressionContext ctx)
            {
                base.CopyFrom(ctx);

            }
        }

        public class ValueExpressionDefaultContext : ValueExpressionContext
        {
            public PrimaryExpressionContext PrimaryExpression()
            {
                return GetRuleContext<PrimaryExpressionContext>(0);
            }

            public ValueExpressionDefaultContext(ValueExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterValueExpressionDefault(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitValueExpressionDefault(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitValueExpressionDefault(this);
                return visitor.VisitChildren(this);
            }
        }


        public class ArithmeticUnaryContext : ValueExpressionContext
        {
            public IToken operators;

            public ValueExpressionContext valueExpression()
            {
                return GetRuleContext<ValueExpressionContext>(0);
            }

            public ITerminalNode MINUS()
            {
                return GetToken(51, 0);
            }

            public ITerminalNode PLUS()
            {
                return GetToken(50, 0);
            }

            public ArithmeticUnaryContext(ValueExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterArithmeticUnary(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitArithmeticUnary(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitArithmeticUnary(this);
                return visitor.VisitChildren(this);
            }
        }

        public class StarContext : PrimaryExpressionContext
        {
            public ITerminalNode ASTERISK()
            {
                return GetToken(52, 0);
            }

            public QualifiedNameContext QualifiedName()
            {
                return GetRuleContext<QualifiedNameContext>(0);
            }

            public StarContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterStar(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitStar(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitStar(this);
                return visitor.VisitChildren(this);
            }
        }

        public class ConstantDefaultContext : PrimaryExpressionContext
        {
            public ConstantContext Constant()
            {
                return GetRuleContext<ConstantContext>(0);
            }

            public ConstantDefaultContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterConstantDefault(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitConstantDefault(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitConstantDefault(this);
                return visitor.VisitChildren(this);
            }
        }


        public class ConstantContext : ParserRuleContext
        {
            public ConstantContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 25;


            public ConstantContext() { }

            public void CopyFrom(ConstantContext ctx)
            {
                base.CopyFrom(ctx);
            }
        }

        public class NullLiteralContext : ConstantContext
        {
            public ITerminalNode NULL()
            {
                return GetToken(22, 0);
            }

            public NullLiteralContext(ConstantContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterNullLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitNullLiteral(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitNullLiteral(this);
                return visitor.VisitChildren(this);
            }
        }

        public class TypeConstructorContext : ConstantContext
        {
            public IdentifierContext Identifier()
            {
                return GetRuleContext<IdentifierContext>(0);
            }

            public ITerminalNode STRING()
            {
                return GetToken(57, 0);
            }

            public TypeConstructorContext(ConstantContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterTypeConstructor(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitTypeConstructor(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitTypeConstructor(this);
                return visitor.VisitChildren(this);
            }
        }

        public class UnquotedIdentifierContext : StrictIdentifierContext
        {
            public ITerminalNode IDENTIFIER()
            {
                return GetToken(66, 0);
            }

            public NonReservedContext nonReserved()
            {
                return GetRuleContext<NonReservedContext>(0);
            }

            public UnquotedIdentifierContext(StrictIdentifierContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterUnquotedIdentifier(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitUnquotedIdentifier(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitUnquotedIdentifier(this);
                return visitor.VisitChildren(this);
            }
        }


        public class NonReservedContext : ParserRuleContext
        {
            public List<ITerminalNode> NULL()
            {
                return GetTokens(22).ToList();
            }

            public ITerminalNode NULL(int i)
            {
                return GetToken(22, i);
            }

            public ITerminalNode ASC()
            {
                return GetToken(25, 0);
            }

            public ITerminalNode DESC()
            {
                return GetToken(26, 0);
            }

            public ITerminalNode LIMIT()
            {
                return GetToken(14, 0);
            }

            public ITerminalNode AS()
            {
                return GetToken(7, 0);
            }

            public ITerminalNode BETWEEN()
            {
                return GetToken(19, 0);
            }

            public ITerminalNode BY()
            {
                return GetToken(11, 0);
            }

            public ITerminalNode FALSE()
            {
                return GetToken(24, 0);
            }

            public ITerminalNode GROUP()
            {
                return GetToken(10, 0);
            }

            public ITerminalNode IN()
            {
                return GetToken(17, 0);
            }

            public ITerminalNode IS()
            {
                return GetToken(21, 0);
            }

            public ITerminalNode LIKE()
            {
                return GetToken(20, 0);
            }

            public ITerminalNode ORDER()
            {
                return GetToken(12, 0);
            }

            public ITerminalNode OUTER()
            {
                return GetToken(34, 0);
            }

            public ITerminalNode TRUE()
            {
                return GetToken(23, 0);
            }

            public ITerminalNode AND()
            {
                return GetToken(16, 0);
            }

            public ITerminalNode CASE()
            {
                return GetToken(27, 0);
            }

            public ITerminalNode CAST()
            {
                return GetToken(40, 0);
            }

            public ITerminalNode DISTINCT()
            {
                return GetToken(8, 0);
            }

            public ITerminalNode ELSE()
            {
                return GetToken(30, 0);
            }

            public ITerminalNode END()
            {
                return GetToken(31, 0);
            }

            public ITerminalNode OR()
            {
                return GetToken(15, 0);
            }

            public ITerminalNode THEN()
            {
                return GetToken(29, 0);
            }

            public ITerminalNode WHEN()
            {
                return GetToken(28, 0);
            }

            public ITerminalNode SELECT()
            {
                return GetToken(5, 0);
            }

            public ITerminalNode FROM()
            {
                return GetToken(6, 0);
            }

            public ITerminalNode WHERE()
            {
                return GetToken(9, 0);
            }

            public ITerminalNode HAVING()
            {
                return GetToken(13, 0);
            }

            public ITerminalNode NOT()
            {
                return GetToken(18, 0);
            }

            public NonReservedContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 35;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterNonReserved(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitNonReserved(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitNonReserved(this);
                return visitor.VisitChildren(this);
            }
        }
        public class QuotedIdentifierAlternativeContext : StrictIdentifierContext
        {
            public QuotedIdentifierContext QuotedIdentifier()
            {
                return GetRuleContext<QuotedIdentifierContext>(0);
            }

            public QuotedIdentifierAlternativeContext(StrictIdentifierContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterQuotedIdentifierAlternative(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitQuotedIdentifierAlternative(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitQuotedIdentifierAlternative(this);
                return visitor.VisitChildren(this);
            }
        }


        public class QuotedIdentifierContext : ParserRuleContext
        {
            public ITerminalNode BACKQUOTED_IDENTIFIER()
            {
                return GetToken(67, 0);
            }

            public QuotedIdentifierContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 33;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterQuotedIdentifier(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitQuotedIdentifier(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitQuotedIdentifier(this);
                return visitor.VisitChildren(this);
            }
        }

        public class NumericLiteralContext : ConstantContext
        {
            public NumberContext Number()
            {
                return GetRuleContext<NumberContext>(0);
            }

            public NumericLiteralContext(ConstantContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterNumericLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitNumericLiteral(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitNumericLiteral(this);
                return visitor.VisitChildren(this);
            }
        }

        public class NumberContext : ParserRuleContext
        {
            public NumberContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 34;


            public NumberContext() { }

            public void CopyFrom(NumberContext ctx)
            {
                base.CopyFrom(ctx);
            }
        }

        public class SimpleCaseContext : PrimaryExpressionContext
        {
            public ExpressionContext elseExpression;

            public ITerminalNode CASE()
            {
                return GetToken(27, 0);
            }

            public ValueExpressionContext ValueExpression()
            {
                return GetRuleContext<ValueExpressionContext>(0);
            }

            public ITerminalNode END()
            {
                return GetToken(31, 0);
            }

            public List<WhenClauseContext> WhenClause()
            {
                return GetRuleContexts<WhenClauseContext>().ToList();
            }

            public WhenClauseContext WhenClause(int i)
            {
                return GetRuleContext<WhenClauseContext>(i);
            }

            public ITerminalNode ELSE()
            {
                return GetToken(30, 0);
            }

            public ExpressionContext Expression()
            {
                return GetRuleContext<ExpressionContext>(0);
            }

            public SimpleCaseContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterSimpleCase(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitSimpleCase(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitSimpleCase(this);
                return visitor.VisitChildren(this);
            }
        }


        public class WhenClauseContext : ParserRuleContext
        {
            public ExpressionContext condition;

            public ExpressionContext result;

            public ITerminalNode WHEN()
            {
                return GetToken(28, 0);
            }

            public ITerminalNode THEN()
            {
                return GetToken(29, 0);
            }

            public List<ExpressionContext> Expression()
            {
                return GetRuleContexts<ExpressionContext>().ToList();
            }

            public ExpressionContext Expression(int i)
            {
                return GetRuleContext<ExpressionContext>(i);
            }

            public WhenClauseContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 29;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterWhenClause(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitWhenClause(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitWhenClause(this);
                return visitor.VisitChildren(this);
            }
        }

        public class SearchedCaseContext : PrimaryExpressionContext
        {
            public ExpressionContext elseExpression;

            public ITerminalNode CASE()
            {
                return GetToken(27, 0);
            }

            public ITerminalNode END()
            {
                return GetToken(31, 0);
            }

            public List<WhenClauseContext> WhenClause()
            {
                return GetRuleContexts<WhenClauseContext>().ToList();
            }

            public WhenClauseContext WhenClause(int i)
            {
                return GetRuleContext<WhenClauseContext>(i);
            }

            public ITerminalNode ELSE()
            {
                return GetToken(30, 0);
            }

            public ExpressionContext Expression()
            {
                return GetRuleContext<ExpressionContext>(0);
            }

            public SearchedCaseContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterSearchedCase(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitSearchedCase(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitSearchedCase(this);
                return visitor.VisitChildren(this);
            }
        }


        public class CastContext : PrimaryExpressionContext
        {
            public ITerminalNode CAST()
            {
                return GetToken(40, 0);
            }

            public ExpressionContext expression()
            {
                return GetRuleContext<ExpressionContext>(0);
            }

            public ITerminalNode AS()
            {
                return GetToken(7, 0);
            }

            public DataTypeContext DataType()
            {
                return GetRuleContext<DataTypeContext>(0);
            }

            public CastContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterCast(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitCast(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitCast(this);
                return visitor.VisitChildren(this);
            }
        }


        public class DataTypeContext : ParserRuleContext
        {
            public DataTypeContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 28;


            public DataTypeContext() { }

            public void CopyFrom(DataTypeContext ctx)
            {
                base.CopyFrom(ctx);
            }
        }

        public class ColumnReferenceContext : PrimaryExpressionContext
        {
            public IdentifierContext Identifier()
            {
                return GetRuleContext<IdentifierContext>(0);
            }

            public ColumnReferenceContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterColumnReference(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitColumnReference(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitColumnReference(this);
                return visitor.VisitChildren(this);
            }
        }


        public class ParenthesizedExpressionContext : PrimaryExpressionContext
        {
            public ExpressionContext expression()
            {
                return GetRuleContext<ExpressionContext>(0);
            }

            public ParenthesizedExpressionContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterParenthesizedExpression(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitParenthesizedExpression(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitParenthesizedExpression(this);
                return visitor.VisitChildren(this);
            }
        }


        public class DereferenceContext : PrimaryExpressionContext
        {
            public PrimaryExpressionContext bases;

            public IdentifierContext fieldName;

            public PrimaryExpressionContext PrimaryExpression()
            {
                return GetRuleContext<PrimaryExpressionContext>(0);
            }

            public IdentifierContext Identifier()
            {
                return GetRuleContext<IdentifierContext>(0);
            }

            public DereferenceContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterDereference(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitDereference(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitDereference(this);
                return visitor.VisitChildren(this);
            }
        }


        public class BooleanLiteralContext : ConstantContext
        {
            public BooleanValueContext BooleanValue()
            {
                return GetRuleContext<BooleanValueContext>(0);
            }

            public BooleanLiteralContext(ConstantContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterBooleanLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitBooleanLiteral(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitBooleanLiteral(this);
                return visitor.VisitChildren(this);
            }
        }
        public class BooleanValueContext : ParserRuleContext
        {
            public ITerminalNode TRUE()
            {
                return GetToken(23, 0);
            }

            public ITerminalNode FALSE()
            {
                return GetToken(24, 0);
            }

            public BooleanValueContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 27;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterBooleanValue(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitBooleanValue(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitBooleanValue(this);
                return visitor.VisitChildren(this);
            }
        }


        public class QuestionContext : PrimaryExpressionContext
        {
            public ITerminalNode QUESTION()
            {
                return GetToken(55, 0);
            }

            public QuestionContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterQuestion(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitQuestion(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitQuestion(this);
                return visitor.VisitChildren(this);
            }
        }

        public class FunctionCallContext : PrimaryExpressionContext
        {
            public QualifiedNameContext qualifiedName()
            {
                return GetRuleContext<QualifiedNameContext>(0);
            }

            public List<ExpressionContext> Expression()
            {
                return GetRuleContexts<ExpressionContext>().ToList();
            }

            public ExpressionContext Expression(int i)
            {
                return GetRuleContext<ExpressionContext>(i);
            }

            public SetQuantifierContext setQuantifier()
            {
                return GetRuleContext<SetQuantifierContext>(0);
            }

            public FunctionCallContext(PrimaryExpressionContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterFunctionCall(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitFunctionCall(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitFunctionCall(this);
                return visitor.VisitChildren(this);
            }
        }


        public class SetQuantifierContext : ParserRuleContext
        {
            public ITerminalNode DISTINCT()
            {
                return GetToken(8, 0);
            }

            public SetQuantifierContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 11;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterSetQuantifier(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitSetQuantifier(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitSetQuantifier(this);
                return visitor.VisitChildren(this);
            }
        }

        public class PrimitiveDataTypeContext : DataTypeContext
        {
            public IdentifierContext Identifier()
            {
                return GetRuleContext<IdentifierContext>(0);
            }

            public List<ITerminalNode> INTEGER_VALUE()
            {
                return GetTokens(62).ToList();
            }

            public ITerminalNode INTEGER_VALUE(int i)
            {
                return GetToken(62, i);
            }

            public PrimitiveDataTypeContext(DataTypeContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterPrimitiveDataType(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitPrimitiveDataType(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitPrimitiveDataType(this);
                return visitor.VisitChildren(this);
            }
        }
        public class StringLiteralContext : ConstantContext
        {
            public List<ITerminalNode> STRING()
            {
                return GetTokens(57).ToList();
            }

            public ITerminalNode STRING(int i)
            {
                return GetToken(57, i);
            }

            public StringLiteralContext(ConstantContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterStringLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitStringLiteral(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitStringLiteral(this);
                return visitor.VisitChildren(this);
            }
        }

        public class DecimalLiteralContext : NumberContext
        {
            public ITerminalNode DECIMAL_VALUE()
            {
                return GetToken(63, 0);
            }

            public DecimalLiteralContext(NumberContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterDecimalLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitDecimalLiteral(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitDecimalLiteral(this);
                return visitor.VisitChildren(this);
            }
        }


        public class ScientificDecimalLiteralContext : NumberContext
        {
            public ITerminalNode SCIENTIFIC_DECIMAL_VALUE()
            {
                return GetToken(64, 0);
            }

            public ScientificDecimalLiteralContext(NumberContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterScientificDecimalLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitScientificDecimalLiteral(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitScientificDecimalLiteral(this);
                return visitor.VisitChildren(this);
            }
        }


        public class IntegerLiteralContext : NumberContext
        {
            public ITerminalNode INTEGER_VALUE()
            {
                return GetToken(62, 0);
            }

            public IntegerLiteralContext(NumberContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterIntegerLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitIntegerLiteral(this);
            }

            public T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitIntegerLiteral(this);
                return visitor.VisitChildren(this);
            }
        }

        public class BigIntLiteralContext : NumberContext
        {
            public ITerminalNode BIGINT_LITERAL()
            {
                return GetToken(58, 0);
            }

            public BigIntLiteralContext(NumberContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterBigIntLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitBigIntLiteral(this);
            }

            public T accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitBigIntLiteral(this);
                return visitor.VisitChildren(this);
            }
        }

        public class SmallIntLiteralContext : NumberContext
        {
            public ITerminalNode SMALLINT_LITERAL()
            {
                return GetToken(59, 0);
            }

            public SmallIntLiteralContext(NumberContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterSmallIntLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitSmallIntLiteral(this);
            }

            public T accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitSmallIntLiteral(this);
                return visitor.VisitChildren(this);
            }
        }


        public class TinyIntLiteralContext : NumberContext
        {
            public ITerminalNode TINYINT_LITERAL()
            {
                return GetToken(60, 0);
            }

            public TinyIntLiteralContext(NumberContext ctx)
            {
                CopyFrom(ctx);
            }

            public void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterTinyIntLiteral(this);
            }

            public void exitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitTinyIntLiteral(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitTinyIntLiteral(this);
                return visitor.VisitChildren(this);
            }
        }

        public class DoubleLiteralContext : NumberContext
        {
            public ITerminalNode DOUBLE_LITERAL()
            {
                return GetToken(65, 0);
            }

            public DoubleLiteralContext(NumberContext ctx)
            {
                CopyFrom(ctx);
            }

            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterDoubleLiteral(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitDoubleLiteral(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitDoubleLiteral(this);
                return visitor.VisitChildren(this);
            }
        }


        public class NamedExpressionSeqContext : ParserRuleContext
        {
            public List<NamedExpressionContext> NamedExpression()
            {
                return GetRuleContexts<NamedExpressionContext>().ToList();
            }

            public NamedExpressionContext NamedExpression(int i)
            {
                return GetRuleContext<NamedExpressionContext>(i);
            }

            public NamedExpressionSeqContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
            {

            }
            public override int RuleIndex => 18;


            public override void EnterRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).EnterNamedExpressionSeq(this);
            }

            public override void ExitRule(IParseTreeListener listener)
            {
                if (listener is GListener)
                    ((GListener)listener).ExitNamedExpressionSeq(this);
            }

            public override T Accept<T>(IParseTreeVisitor<T> visitor)
            {
                if (visitor is GVisitor<T>)
                    return ((GVisitor<T>)visitor).VisitNamedExpressionSeq(this);
                return visitor.VisitChildren(this);
            }
        }

        public SingleExpressionContext SingleExpression()
        {

            SingleExpressionContext _localctx = new SingleExpressionContext(Context, State);
            EnterRule(_localctx, 0, 0);
            try
            {
                State = 78;
                ErrorHandler.Sync(this);
                switch (Interpreter.AdaptivePredict(TokenStream, 0, Context))
                {
                    case 1:
                        EnterOuterAlt(_localctx, 1);
                        State = 72;
                        NamedExpression();
                        State = 73;
                        Match(-1);
                        break;
                    case 2:
                        EnterOuterAlt(_localctx, 2);
                        State = 75;

                        NamedExpressionSeq();
                        State = 76;

                        Match(-1);
                        break;
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;


        }

        public NamedExpressionContext NamedExpression()
        {
            NamedExpressionContext _localctx = new NamedExpressionContext(Context, State);
            EnterRule(_localctx, 34, 17);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 253;
                Expression();
                State = 261;

                ErrorHandler.Sync(this);
                switch (Interpreter.AdaptivePredict(TokenStream, 35, Context))
                {
                    case 1:
                        State = 255;
                        ErrorHandler.Sync(this);
                        switch (Interpreter.AdaptivePredict(TokenStream, 33, Context))
                        {
                            case 1:
                                State = 254;
                                Match(7);
                                break;
                        }
                        State = 259;

                        ErrorHandler.Sync(this);
                        switch (Interpreter.AdaptivePredict(TokenStream, 34, Context))
                        {
                            case 1:
                                State = 257;
                                QualifiedName();
                                break;
                            case 2:
                                State = 258;

                                Identifier();
                                break;
                        }
                        break;
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public ExpressionContext Expression()
        {
            ExpressionContext _localctx = new ExpressionContext(Context, State);
            EnterRule(_localctx, 38, 19);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 271;
                BooleanExpression(0);
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        private BooleanExpressionContext BooleanExpression(int _p)
        {
            ParserRuleContext _parentctx = Context;
            int _parentState = State;
            BooleanExpressionContext _localctx = new BooleanExpressionContext(Context, _parentState);
            int _startState = 40;
            EnterRecursionRule(_localctx, 40, 20, _p);
            try
            {
                EnterOuterAlt(_localctx, 1);

                State = 252;

                ErrorHandler.Sync(this);

                switch (Interpreter.AdaptivePredict(TokenStream, 31, Context))
                {
                    case 1:
                        _localctx = new LogicalNotContext(_localctx);
                        Context = _localctx;
                        State = 249;
                        Match(18);
                        State = 250;
                        BooleanExpression(4);
                        break;
                    case 2:
                        _localctx = new BooleanDefaultContext(_localctx);
                        Context = _localctx;
                        State = 251;
                        Predicated();
                        break;
                }

                Context.Stop = TokenStream.LT(-1);
                State = 262;
                ErrorHandler.Sync(this);

                for (int _alt = Interpreter.AdaptivePredict(TokenStream, 33, Context);
                    _alt != 2 && _alt != 0;
                    _alt = Interpreter.AdaptivePredict(TokenStream, 33, Context))
                {
                    if (_alt == 1)
                    {

                        if (ParseListeners != null)
                        {
                            TriggerExitRuleEvent();
                        }

                        BooleanExpressionContext _prevctx = _localctx;
                        State = 260;
                        ErrorHandler.Sync(this);
                        switch (Interpreter.AdaptivePredict(TokenStream, 32, Context))
                        {
                            case 1:
                                _localctx = new LogicalBinaryContext(new BooleanExpressionContext(_parentctx, _parentState));
                                ((LogicalBinaryContext)_localctx).left = _prevctx;
                                PushNewRecursionContext(_localctx, _startState, 20);
                                State = 254;
                                if (!Precpred(Context, 3))
                                {
                                    throw new FailedPredicateException(this, "precpred(_ctx, 3)");
                                }
                                State = 255;
                                ((LogicalBinaryContext)_localctx).operators = Match(16);
                                State = 256;

                                ((LogicalBinaryContext)_localctx).right = BooleanExpression(4);
                                break;
                            case 2:
                                _localctx = new LogicalBinaryContext(new BooleanExpressionContext(_parentctx, _parentState));
                                ((LogicalBinaryContext)_localctx).left = _prevctx;
                                PushNewRecursionContext(_localctx, _startState, 20);
                                State = 257;
                                if (!Precpred(Context, 2))
                                {
                                    throw new FailedPredicateException(this, "precpred(_ctx, 2)");
                                }
                                State = 258;
                                ((LogicalBinaryContext)_localctx).operators = Match(15);
                                State = 259;
                                ((LogicalBinaryContext)_localctx).right = BooleanExpression(3);
                                break;
                        }
                    }

                    State = 264;
                    ErrorHandler.Sync(this);
                }


            }
            catch (RecognitionException ex)
            {

                _localctx.exception = ex;
                ErrorHandler.ReportError(this, ex);
                ErrorHandler.Recover(this, ex);
            }

            return _localctx;
        }

        public PredicatedContext Predicated()
        {
            PredicatedContext _localctx = new PredicatedContext(Context, State);
            EnterRule(_localctx, 42, 21);

            try
            {

                State = 273;
                ErrorHandler.Sync(this);
                switch (Interpreter.AdaptivePredict(TokenStream, 35, Context))
                {
                    case 1:
                        EnterOuterAlt(_localctx, 1);
                        State = 265;
                        ValueExpression(0);
                        State = 267;
                        ErrorHandler.Sync(this);
                        switch (Interpreter.AdaptivePredict(TokenStream, 34, Context))
                        {
                            case 1:
                                State = 266;
                                Predicate();
                                return _localctx;
                            default:
                                return _localctx;
                        }
                    case 2:
                        EnterOuterAlt(_localctx, 2);
                        State = 269;
                        Match(3);
                        State = 270;
                        Predicated();
                        State = 271;
                        Match(4);
                        break;
                }
            }
            catch (RecognitionException ex)
            {
                _localctx.exception = ex;
                ErrorHandler.ReportError(this, ex);
                ErrorHandler.Recover(this, ex);
            }
            finally
            {
                ExitRule();
            }

            return _localctx;
        }

        public PredicateContext Predicate()
        {
            PredicateContext _localctx = new PredicateContext(Context, State);
            EnterRule(_localctx, 44, 22);

            try
            {
                State = 305;
                ErrorHandler.Sync(this);
                int _la;
                switch (Interpreter.AdaptivePredict(TokenStream, 41, Context))
                {
                    case 1:
                        EnterOuterAlt(_localctx, 1);
                        State = 276;
                        ErrorHandler.Sync(this);
                        _la = TokenStream.LA(1);
                        if (_la == 18)
                        {
                            State = 275;
                            Match(18);
                        }
                        State = 278;
                        _localctx.kind = Match(17);
                        State = 279;
                        Match(3);
                        State = 280;
                        Expression();
                        State = 285;
                        ErrorHandler.Sync(this);

                        for (_la = TokenStream.LA(1); _la == 1; _la = TokenStream.LA(1))
                        {
                            State = 281;

                            Match(1);
                            State = 282;
                            Expression();
                            State = 287;
                            ErrorHandler.Sync(this);
                        }
                        State = 288;
                        Match(4);
                        break;
                    case 2:
                        EnterOuterAlt(_localctx, 2);
                        State = 291;
                        ErrorHandler.Sync(this);
                        _la = TokenStream.LA(1);
                        if (_la == 18)
                        {
                            State = 290;
                            Match(18);
                        }
                        State = 293;
                        _localctx.kind = Match(17);
                        State = 294;
                        Expression();
                        break;
                    case 3:
                        EnterOuterAlt(_localctx, 3);
                        State = 296;
                        ErrorHandler.Sync(this);
                        _la = TokenStream.LA(1);
                        if (_la == 18)
                        {
                            State = 295;
                            Match(18);
                        }

                        State = 298;
                        _localctx.kind = Match(20);
                        State = 299;
                        _localctx.pattern = ValueExpression(0);
                        break;
                    case 4:
                        EnterOuterAlt(_localctx, 4);
                        State = 300;
                        Match(21);
                        State = 302;
                        ErrorHandler.Sync(this);
                        _la = TokenStream.LA(1);
                        if (_la == 18)
                        {
                            State = 301;
                            Match(18);
                        }

                        State = 304;
                        _localctx.kind = Match(22);
                        break;
                }
            }
            catch (RecognitionException ex)
            {
                _localctx.exception = ex;
                ErrorHandler.ReportError(this, ex);
                ErrorHandler.Recover(this, ex);
            }
            finally
            {
                ExitRule();
            }

            return _localctx;
        }

        private ValueExpressionContext ValueExpression(int _p)
        {
            ParserRuleContext _parentctx = Context;
            int _parentState = State;
            ValueExpressionContext _localctx = new ValueExpressionContext(Context, _parentState);
            ValueExpressionContext _prevctx = _localctx;
            int _startState = 46;
            EnterRecursionRule(_localctx, 46, 23, _p);
            try
            {
                int _la;
                EnterOuterAlt(_localctx, 1);
                State = 336;
                ErrorHandler.Sync(this);
                switch (TokenStream.LA(1))
                {
                    case 2:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                    case 34:
                    case 35:
                    case 36:
                    case 37:
                    case 38:
                    case 39:
                    case 40:
                    case 41:
                    case 52:
                    case 55:
                    case 57:
                    case 58:
                    case 59:
                    case 60:
                    case 62:
                    case 63:
                    case 64:
                    case 65:
                    case 66:
                    case 67:
                        _localctx = new ValueExpressionDefaultContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 333;
                        PrimaryExpression(0);
                        break;
                    case 50:
                    case 51:
                        _localctx = new ArithmeticUnaryContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 334;
                        ((ArithmeticUnaryContext)_localctx).operators = TokenStream.LT(1);
                        _la = TokenStream.LA(1);
                        if (_la != 50 && _la != 51)
                        {
                            ((ArithmeticUnaryContext)_localctx).operators = ErrorHandler.RecoverInline(this);
                        }
                        else
                        {
                            if (TokenStream.LA(1) == -1)
                                // this.MatchedEOF = true;
                                ErrorHandler.ReportMatch(this);
                            Consume();
                        }
                        State = 335;
                        ValueExpression(5);
                        break;
                    default:
                        throw new NoViableAltException(this);
                }
                Context.Stop = TokenStream.LT(-1);
                State = 353;

                ErrorHandler.Sync(this);
                int _alt = Interpreter.AdaptivePredict(TokenStream, 50, Context);
                while (_alt != 2 && _alt != 0)
                {
                    if (_alt == 1)
                    {
                        if (ParseListeners != null)
                            TriggerExitRuleEvent();
                        _prevctx = _localctx;
                        State = 351;
                        ErrorHandler.Sync(this);
                        switch (Interpreter.AdaptivePredict(TokenStream, 49, Context))
                        {
                            case 1:
                                _localctx = new ArithmeticBinaryContext(new ValueExpressionContext(_parentctx, _parentState));
                                ((ArithmeticBinaryContext)_localctx).left = _prevctx;
                                PushNewRecursionContext(_localctx, _startState, 23);
                                State = 338;

                                if (!Precpred(Context, 4))
                                    throw new FailedPredicateException(this, "precpred(_ctx, 4)");

                                State = 339;

                                ((ArithmeticBinaryContext)_localctx).operators = TokenStream.LT(1);
                                _la = TokenStream.LA(1);
                                if ((_la & 0xFFFFFFC0) != 0 || (1L << _la & 0x70000000000000L) == 0L)
                                {
                                    ((ArithmeticBinaryContext)_localctx).operators = ErrorHandler.RecoverInline(this);
                                }
                                else
                                {
                                    if (TokenStream.LA(1) == -1)
                                        //this.MatchedEOF = true;

                                        ErrorHandler.ReportMatch(this);
                                    Consume();
                                }
                                State = 340;


                                ((ArithmeticBinaryContext)_localctx).right = ValueExpression(5);
                                break;
                            case 2:
                                _localctx = new ArithmeticBinaryContext(new ValueExpressionContext(_parentctx, _parentState));
                                ((ArithmeticBinaryContext)_localctx).left = _prevctx;
                                PushNewRecursionContext(_localctx, _startState, 23);
                                State = 341;

                                if (!Precpred(Context, 3))
                                    throw new FailedPredicateException(this, "precpred(_ctx, 3)");
                                State = 342;

                                ((ArithmeticBinaryContext)_localctx).operators = TokenStream.LT(1);
                                _la = TokenStream.LA(1);
                                if (_la != 50 && _la != 51)
                                {
                                    ((ArithmeticBinaryContext)_localctx).operators = ErrorHandler.RecoverInline(this);
                                }
                                else
                                {
                                    if (TokenStream.LA(1) == -1)
                                        // this.matchedEOF = true;
                                        ErrorHandler.RecoverInline(this);
                                    Consume();
                                }
                                State = 343;

                                ((ArithmeticBinaryContext)_localctx).right = ValueExpression(4);
                                break;
                            case 3:
                                _localctx = new ComparisonContext(new ValueExpressionContext(_parentctx, _parentState));
                                ((ComparisonContext)_localctx).left = _prevctx;
                                PushNewRecursionContext(_localctx, _startState, 23);
                                State = 344;

                                if (!Precpred(Context, 2))
                                    throw new FailedPredicateException(this, "precpred(_ctx, 2)");
                                State = 345;

                                ComparisonOperator();
                                State = 346;
                                ((ComparisonContext)_localctx).right = ValueExpression(3);
                                break;
                            case 4:
                                _localctx = new StringAddContext(new ValueExpressionContext(_parentctx, _parentState));
                                ((StringAddContext)_localctx).left = _prevctx;
                                PushNewRecursionContext(_localctx, _startState, 23);
                                State = 348;
                                if (!Precpred(Context, 1))
                                    throw new FailedPredicateException(this, "precpred(_ctx, 1)");
                                State = 349;
                                Match(56);
                                State = 350;
                                ((StringAddContext)_localctx).right = ValueExpression(2);
                                break;
                        }
                    }
                    State = 355;
                    ErrorHandler.Sync(this);
                    _alt = Interpreter.AdaptivePredict(TokenStream, 50, Context);
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                UnrollRecursionContexts(_parentctx);
            }
            return _localctx;
        }

        public ComparisonOperatorContext ComparisonOperator()
        {
            ComparisonOperatorContext _localctx = new ComparisonOperatorContext(Context, State);
            EnterRule(_localctx, 52, 26);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 441;
                int _la = TokenStream.LA(1);
                if ((_la & 0xFFFFFFC0) != 0 || (1L << _la & 0x3FC0000000000L) == 0L)
                {
                    ErrorHandler.RecoverInline(this);
                }
                else
                {
                    if (TokenStream.LA(1) == -1)
                        //this.matchedEOF = true;
                        ErrorHandler.ReportMatch(this);
                    Consume();
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }


        private PrimaryExpressionContext PrimaryExpression(int _p)
        {
            ParserRuleContext _parentctx = Context;
            int _parentState = State;
            PrimaryExpressionContext _localctx = new PrimaryExpressionContext(Context, _parentState);
            PrimaryExpressionContext _prevctx = _localctx;
            int _startState = 48;
            EnterRecursionRule(_localctx, 48, 24, _p);
            try
            {
                int _la;
                EnterOuterAlt(_localctx, 1);
                State = 418;

                ErrorHandler.Sync(this);
                switch (Interpreter.AdaptivePredict(TokenStream, 58, Context))
                {
                    case 1:
                        _localctx = new ConstantDefaultContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 357;
                        Constant();
                        break;
                    case 2:
                        _localctx = new StarContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 358;
                        Match(52);
                        break;
                    case 3:
                        _localctx = new StarContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 359;
                        QualifiedName();
                        State = 360;

                        Match(4);
                        State = 361;
                        Match(52);
                        break;
                    case 4:
                        _localctx = new QuestionContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 363;
                        Match(55);
                        break;
                    case 5:
                        _localctx = new FunctionCallContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 364;
                        QualifiedName();
                        State = 365;

                        Match(2);
                        State = 377;
                        ErrorHandler.Sync(this);
                        _la = TokenStream.LA(1);
                        if ((_la & 0xFFFFFFC0) == 0 && (1L << _la & -2406043702876176412L) != 0L || (_la - 64 & 0xFFFFFFC0) == 0 && (1L << _la - 64 & 0xFL) != 0L)
                        {
                            State = 367;
                            ErrorHandler.Sync(this);
                            switch (Interpreter.AdaptivePredict(TokenStream, 51, Context))
                            {
                                case 1:
                                    State = 366;
                                    SetQuantifier();
                                    break;
                            }
                            State = 369;

                            Expression();
                            State = 374;

                            ErrorHandler.Sync(this);
                            _la = TokenStream.LA(1);
                            while (_la == 1)
                            {
                                State = 370;
                                Match(1);
                                State = 371;
                                Expression();
                                State = 376;
                                ErrorHandler.Sync(this);
                                _la = TokenStream.LA(1);
                            }
                        }
                        State = 379;
                        Match(3);
                        break;
                    case 6:
                        _localctx = new SimpleCaseContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 381;
                        Match(27);
                        State = 382;
                        ValueExpression(0);
                        State = 384;
                        ErrorHandler.Sync(this);
                        _la = TokenStream.LA(1);
                        while (true)
                        {
                            State = 383;

                            WhenClause();

                            State = 386;

                            ErrorHandler.Sync(this);
                            _la = TokenStream.LA(1);
                            if (_la != 28)
                            {
                                State = 390;

                                ErrorHandler.Sync(this);
                                _la = TokenStream.LA(1);
                                if (_la == 30)
                                {
                                    State = 388;

                                    Match(30);
                                    State = 389;

                                    ((SimpleCaseContext)_localctx).elseExpression = Expression();
                                }
                                State = 392;
                                Match(31);
                                break;
                            }
                        }
                        break;
                    case 7:
                        _localctx = new SearchedCaseContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 394;


                        Match(27);
                        State = 396;
                        ErrorHandler.Sync(this);
                        _la = TokenStream.LA(1);
                        while (true)
                        {
                            State = 395;

                            WhenClause();
                            State = 398;

                            ErrorHandler.Sync(this);
                            _la = TokenStream.LA(1);
                            if (_la != 28)
                            {
                                State = 402;

                                ErrorHandler.Sync(this);
                                _la = TokenStream.LA(1);
                                if (_la == 30)
                                {
                                    State = 400;

                                    Match(30);
                                    State = 401;

                                    ((SearchedCaseContext)_localctx).elseExpression = Expression();
                                }
                                State = 404;
                                Match(31);
                                break;
                            }
                        }
                        break;
                    case 8:
                        _localctx = new CastContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 406;

                        Match(40);
                        State = 407;

                        Match(2);
                        State = 408;

                        Expression();
                        State = 409;

                        Match(7);
                        State = 410;

                        DataType();
                        State = 411;

                        Match(3);
                        break;
                    case 9:
                        _localctx = new ColumnReferenceContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 413;

                        Identifier();
                        break;
                    case 10:
                        _localctx = new ParenthesizedExpressionContext(_localctx);
                        Context = _localctx;
                        _prevctx = _localctx;
                        State = 414;

                        Match(2);
                        State = 415;
                        Expression();
                        State = 416;
                        Match(3);
                        break;
                }
                Context.Stop = TokenStream.LT(-1);
                State = 425;

                ErrorHandler.Sync(this);
                int _alt = Interpreter.AdaptivePredict(TokenStream, 59, Context);
                while (_alt != 2 && _alt != 0)
                {
                    if (_alt == 1)
                    {
                        if (ParseListeners != null)
                            TriggerExitRuleEvent();
                        _prevctx = _localctx;
                        _localctx = new DereferenceContext(new PrimaryExpressionContext(_parentctx, _parentState));
                        ((DereferenceContext)_localctx).bases = _prevctx;
                        PushNewRecursionContext(_localctx, _startState, 24);
                        State = 420;

                        if (!Precpred(Context, 2))
                            throw new FailedPredicateException(this, "precpred(_ctx, 2)");
                        State = 421;

                        Match(4);
                        State = 422;

                        ((DereferenceContext)_localctx).fieldName = Identifier();
                    }
                    State = 427;

                    ErrorHandler.Sync(this);
                    _alt = Interpreter.AdaptivePredict(TokenStream, 59, Context);
                }
            }
            catch (RecognitionException ex)
            {
                _localctx.exception = ex;
                ErrorHandler.ReportError(this, ex);
                ErrorHandler.Recover(this, ex);
            }
            finally
            {
                UnrollRecursionContexts(_parentctx);
            }
            return _localctx;
        }

        public ConstantContext Constant()
        {
            ConstantContext _localctx = new ConstantContext(Context, State);
            EnterRule(_localctx, 50, 25);
            try
            {
                int _alt;
                State = 439;
                ErrorHandler.Sync(this);
                switch (Interpreter.AdaptivePredict(TokenStream, 61, Context))
                {
                    case 1:
                        _localctx = new NullLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 1);
                        State = 428;

                        Match(22);
                        break;
                    case 2:
                        _localctx = new TypeConstructorContext(_localctx);
                        EnterOuterAlt(_localctx, 2);
                        State = 429;

                        Identifier();
                        State = 430;
                        Match(57);
                        break;
                    case 3:
                        _localctx = new NumericLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 3);
                        State = 432;
                        Number();
                        break;
                    case 4:
                        _localctx = new BooleanLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 4);
                        State = 433;

                        BooleanValue();
                        break;
                    case 5:
                        _localctx = new StringLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 5);
                        State = 435;

                        ErrorHandler.Sync(this);
                        _alt = 1;
                        do
                        {
                            switch (_alt)
                            {
                                case 1:
                                    State = 434;

                                    Match(57);
                                    break;
                                default:
                                    throw new NoViableAltException(this);
                            }
                            State = 437;

                            ErrorHandler.Sync(this);
                            _alt = Interpreter.AdaptivePredict(TokenStream, 60, Context);
                        } while (_alt != 2 && _alt != 0);
                        break;
                }
            }
            catch (RecognitionException ex)
            {
                _localctx.exception = ex;
                ErrorHandler.ReportError(this, ex);
                ErrorHandler.Recover(this, ex);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public IdentifierContext Identifier()
        {
            IdentifierContext _localctx = new IdentifierContext(Context, State);
            EnterRule(_localctx, 62, 31);
            try
            {
                State = 480;
                ErrorHandler.Sync(this);
                switch (TokenStream.LA(1))
                {
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 34:
                    case 40:
                    case 66:
                    case 67:
                        EnterOuterAlt(_localctx, 1);
                        State = 471;
                        StrictIdentifier();
                        break;
                    case 38:
                        EnterOuterAlt(_localctx, 2);
                        State = 472;

                        Match(38);
                        break;
                    case 35:
                        EnterOuterAlt(_localctx, 3);
                        State = 473;

                        Match(35);
                        break;
                    case 36:
                        EnterOuterAlt(_localctx, 4);
                        State = 474;

                        Match(36);
                        break;
                    case 37:
                        EnterOuterAlt(_localctx, 5);
                        State = 475;

                        Match(37);
                        break;
                    case 32:
                        EnterOuterAlt(_localctx, 6);
                        State = 476;

                        Match(32);
                        break;
                    case 33:
                        EnterOuterAlt(_localctx, 7);
                        State = 477;

                        Match(33);
                        break;
                    case 39:
                        EnterOuterAlt(_localctx, 8);
                        State = 478;
                        Match(39);
                        break;
                    case 41:
                        EnterOuterAlt(_localctx, 9);
                        State = 479;
                        Match(41);
                        break;
                    default:
                        throw new NoViableAltException(this);
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public StrictIdentifierContext StrictIdentifier()
        {
            StrictIdentifierContext _localctx = new StrictIdentifierContext(Context, State);
            EnterRule(_localctx, 64, 32);
            try
            {
                State = 485;

                ErrorHandler.Sync(this);
                switch (TokenStream.LA(1))
                {
                    case 66:
                        _localctx = new UnquotedIdentifierContext(_localctx);
                        EnterOuterAlt(_localctx, 1);
                        State = 482;

                        Match(66);
                        break;
                    case 67:
                        _localctx = new QuotedIdentifierAlternativeContext(_localctx);
                        EnterOuterAlt(_localctx, 2);
                        State = 483;

                        QuotedIdentifier();
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 34:
                    case 40:
                        _localctx = new UnquotedIdentifierContext(_localctx);
                        EnterOuterAlt(_localctx, 3);
                        State = 484;

                        NonReserved();
                        break;
                    default:
                        throw new NoViableAltException(this);
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public QuotedIdentifierContext QuotedIdentifier()
        {
            QuotedIdentifierContext _localctx = new QuotedIdentifierContext(Context, State);
            EnterRule(_localctx, 66, 33);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 487;
                Match(67);
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public NonReservedContext NonReserved()
        {
            NonReservedContext _localctx = new NonReservedContext(Context, State);
            EnterRule(_localctx, 70, 35);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 498;
                int _la = TokenStream.LA(1);
                if ((_la & 0xFFFFFFC0) != 0 || (1L << _la & 0x104FFFFFFE0L) == 0L)
                {
                    ErrorHandler.RecoverInline(this);
                }
                else
                {
                    if (TokenStream.LA(1) == -1)
                        //this.matchedEOF = true;
                        ErrorHandler.ReportMatch(this);
                    Consume();
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }


        public QualifiedNameContext QualifiedName()
        {
            QualifiedNameContext _localctx = new QualifiedNameContext(Context, State);
            EnterRule(_localctx, 60, 30);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 463;

                Identifier();
                State = 468;

                ErrorHandler.Sync(this);
                int _alt = Interpreter.AdaptivePredict(TokenStream, 64, Context);
                while (_alt != 2 && _alt != 0)
                {
                    if (_alt == 1)
                    {
                        State = 464;

                        Match(4);
                        State = 465;

                        Identifier();
                    }
                    State = 470;

                    ErrorHandler.Sync(this);
                    _alt = Interpreter.AdaptivePredict(TokenStream, 64, Context);
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }
        public DataTypeContext DataType()
        {
            DataTypeContext _localctx = new DataTypeContext(Context, State);
            EnterRule(_localctx, 56, 28);
            try
            {
                _localctx = new PrimitiveDataTypeContext(_localctx);
                EnterOuterAlt(_localctx, 1);
                State = 445;

                Identifier();
                State = 456;

                ErrorHandler.Sync(this);
                int _la = TokenStream.LA(1);
                if (_la == 2)
                {
                    State = 446;

                    Match(2);
                    State = 447;

                    Match(62);
                    State = 452;

                    ErrorHandler.Sync(this);
                    _la = TokenStream.LA(1);
                    while (_la == 1)
                    {
                        State = 448;

                        Match(1);
                        State = 449;

                        Match(62);
                        State = 454;

                        ErrorHandler.Sync(this);
                        _la = TokenStream.LA(1);
                    }
                    State = 455;

                    Match(3);
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public WhenClauseContext WhenClause()
        {
            WhenClauseContext _localctx = new WhenClauseContext(Context, State);
            EnterRule(_localctx, 58, 29);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 458;

                Match(28);
                State = 459;

                _localctx.condition = Expression();
                State = 460;

                Match(29);
                State = 461;

                _localctx.result = Expression();
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }


        public SetQuantifierContext SetQuantifier()
        {
            SetQuantifierContext _localctx = new SetQuantifierContext(Context, State);
            EnterRule(_localctx, 22, 11);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 179;
                Match(8);
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public BooleanValueContext BooleanValue()
        {
            BooleanValueContext _localctx = new BooleanValueContext(Context, State);
            EnterRule(_localctx, 54, 27);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 443;
                int _la = TokenStream.LA(1);
                if (_la != 23 && _la != 24)
                {
                    ErrorHandler.RecoverInline(this);
                }
                else
                {
                    if (TokenStream.LA(1) == -1)
                        //this.matchedEOF = true;
                        ErrorHandler.ReportMatch(this);
                    Consume();
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public NumberContext Number()
        {
            NumberContext _localctx = new NumberContext(Context, State);
            EnterRule(_localctx, 68, 34);
            try
            {
                State = 496;
                ErrorHandler.Sync(this);
                switch (TokenStream.LA(1))
                {
                    case 63:
                        _localctx = new DecimalLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 1);
                        State = 489;

                        Match(63);
                        break;
                    case 64:
                        _localctx = new ScientificDecimalLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 2);
                        State = 490;

                        Match(64);
                        break;
                    case 62:
                        _localctx = new IntegerLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 3);
                        State = 491;
                        Match(62);
                        break;
                    case 58:
                        _localctx = new BigIntLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 4);
                        State = 492;
                        Match(58);
                        break;
                    case 59:
                        _localctx = new SmallIntLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 5);
                        State = 493;
                        Match(59);
                        break;
                    case 60:
                        _localctx = new TinyIntLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 6);
                        State = 494;
                        Match(60);
                        break;
                    case 65:
                        _localctx = new DoubleLiteralContext(_localctx);
                        EnterOuterAlt(_localctx, 7);
                        State = 495;
                        Match(65);
                        break;
                    default:
                        throw new NoViableAltException(this);
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

        public NamedExpressionSeqContext NamedExpressionSeq()
        {
            NamedExpressionSeqContext _localctx = new NamedExpressionSeqContext(Context, State);
            EnterRule(_localctx, 36, 18);
            try
            {
                EnterOuterAlt(_localctx, 1);
                State = 263;
                NamedExpression();
                State = 268;
                ErrorHandler.Sync(this);
                int _alt = Interpreter.AdaptivePredict(TokenStream, 36, Context);
                while (_alt != 2 && _alt != 0)
                {
                    if (_alt == 1)
                    {
                        State = 264;

                        Match(1);
                        State = 265;

                        NamedExpression();
                    }
                    State = 270;

                    ErrorHandler.Sync(this);
                    _alt = Interpreter.AdaptivePredict(TokenStream, 36, Context);
                }
            }
            catch (RecognitionException re)
            {
                _localctx.exception = re;
                ErrorHandler.ReportError(this, re);
                ErrorHandler.Recover(this, re);
            }
            finally
            {
                ExitRule();
            }
            return _localctx;
        }

    }
}
