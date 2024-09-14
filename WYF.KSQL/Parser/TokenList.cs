using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public class TokenList
    {
        // Fields
        private Lexer _lexer;
        private int count;
        private Element first;
        private bool isFinished;
        private Element last;
        private Element me;
        private SqlParser sqlParser;

        // Methods
        public TokenList(Lexer l)
        {
            this._lexer = l;
            this.first = new Element(this._lexer.next());
            this.last = this.first;
            this.me = this.first;
            if (this.me.Equals(Token.EOFToken))
            {
                this.isFinished = true;
            }
            this.count = 1;
        }

        public bool add()
        {
            if (this.isFinished)
            {
                return false;
            }
            this.last.Next = new Element(this._lexer.next());
            this.last = this.last.Next;
            this.count++;
            if (this.last.Equals(Token.EOFToken))
            {
                this.isFinished = true;
            }
            return true;
        }

        public SqlParser getSqlParser()
        {
            return this.sqlParser;
        }

        public Token lookup(int witch)
        {
            if ((this.count - 1) < witch)
            {
                for (int j = this.count - 1; j < (witch - 1); j++)
                {
                    this.add();
                }
                if (this.add())
                {
                    return this.last.Value;
                }
                return null;
            }
            Element first = this.first;
            for (int i = 0; i < witch; i++)
            {
                first = first.Next;
            }
            return first.Value;
        }

        public void match()
        {
            if (this.next() == null)
            {
                throw new ParserException("\n CodingError in Parser.Match: next Token == null", 0, 0);
            }
        }

        public void match(Token tok)
        {
            Token token = this.lookup(0);
            if (!tok.Equals(token))
            {
                throw new ParserException(string.Concat(new object[] { "\n Error: expect token <", tok.Typename(), ",", tok.value, ">, but current token is <", token.Typename(), ",", token.value, ">, at line ", token.beginLine, ", column ", token.beginColumn, ")" }), token.beginLine, token.beginColumn);
            }
            if (this.next() == null)
            {
                throw new ParserException("\n CodingError in Parser.Match: next Token == null", 0, 0);
            }
        }

        public void match(int tokType)
        {
            Token token = this.lookup(0);
            if (token.type != tokType)
            {
                throw new ParserException(string.Concat(new object[] { "\n Error: expect token type '", TokenType.Typename(tokType), "', but current token type is '", TokenType.Typename(token.type), "', token value is ", token.value, ", at line ", token.beginLine, ", column ", token.beginColumn, ". " }), token.beginLine, token.beginColumn);
            }
            if (this.next() == null)
            {
                throw new ParserException("\n CodingError in Parser.Match: next Token == null", 0, 0);
            }
        }

        public void match(int tokType, string tokValue)
        {
            this.match(tokType, tokValue);
        }

        public void match(int tokType, string tokValue, bool ignoreCase)
        {
            Token token = this.lookup(0);
            bool flag = token.type == tokType;
            if (ignoreCase)
            {
                flag = token.value.EqualsIgnoreCase(tokValue);
            }
            else
            {
                flag = token.value.Equals(tokValue);
            }
            if (!flag)
            {
                throw new ParserException(string.Concat(new object[] { "\n Error: expect token type '", TokenType.Typename(tokType), "', but current token type is '", TokenType.Typename(token.type), "', token value is ", token.value, ", at line ", token.beginLine, ", column ", token.beginColumn, ". " }), token.beginLine, token.beginColumn);
            }
            if (this.next() == null)
            {
                throw new ParserException("\n CodingError in Parser.Match: next Token == null", 0, 0);
            }
        }

        public Token next()
        {
            if (this.count < 1)
            {
                return null;
            }
            if (this.count < 2)
            {
                this.add();
            }
            Element first = this.first;
            this.first = this.first.Next;
            this.count--;
            return first.Value;
        }

        public void setSqlParser(SqlParser sqlParser)
        {
            this.sqlParser = sqlParser;
        }

        // Properties
        public Lexer lexer
        {
            get
            {
                return this._lexer;
            }
            set
            {
                this._lexer = value;
            }
        }

        // Nested Types
        private sealed class Element
        {
            // Fields
            public TokenList.Element Next;
            public Token Value;

            // Methods
            public Element(Token t)
            {
                this.Value = t;
            }
        }
    }






}
