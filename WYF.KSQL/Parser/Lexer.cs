using System;
using System.Collections.Generic;

using System.Text;
using WYF.KSQL.Exception;
using WYF.KSQL.Util;

namespace WYF.KSQL.Parser
{
    public class Lexer
    {
        // Fields
        private int _col;
        private KeyWord _keywords;
        private int _line;
        private int _ptr;
        private StringReader _reader;
        private bool _skipComment;
        private ReservedWord reservedWords;

        // Methods
        public Lexer(StringReader reader)
        {
            this._col = 1;
            this._line = 1;
            this._skipComment = true;
            this._ptr = reader.ptr();
            this._keywords = KeyWord.instance;
            this._reader = reader;
            this.reservedWords = new KSQLReservedWord();
        }

        public Lexer(string text) : this(text, true)
        {
        }

        public Lexer(StringBuilder buffer) : this(buffer.ToString(), true)
        {
        }

        public Lexer(KeyWord keyword, StringReader _reader)
        {
            this._col = 1;
            this._line = 1;
            this._skipComment = true;
            this._keywords = keyword;
            this._reader = _reader;
            this.reservedWords = new KSQLReservedWord();
        }

        public Lexer(string text, bool skipComment)
        {
            this._col = 1;
            this._line = 1;
            this._skipComment = true;
            this._skipComment = skipComment;
            if (text == null)
            {
                throw new IllegalStateException("text");
            }
            this._keywords = KeyWord.instance;
            this._reader = new StringReader(text);
            this.reservedWords = new KSQLReservedWord();
        }

        public Lexer(KeyWord keyword, ReservedWord reservedWords, StringReader _reader)
        {
            this._col = 1;
            this._line = 1;
            this._skipComment = true;
            this._keywords = keyword;
            this.reservedWords = reservedWords;
            this._reader = _reader;
        }

        private bool isOperator(char c)
        {
            switch (c)
            {
                case '*':
                case '+':
                case '-':
                case '/':
                case '&':
                case '!':
                case '|':
                case '~':
                case '=':
                case '^':
                    return true;
            }
            return false;
        }

        public Token next()
        {
            while (!this._reader.eos())
            {
                char c = this._reader.next();
                if (char.IsWhiteSpace(c))
                {
                    this._col++;
                    if (c == '\n')
                    {
                        this._line++;
                        this._col = 1;
                    }
                    continue;
                }
                this._ptr = this._reader.ptr();
                switch (c)
                {
                    case 'N':
                    case 'n':
                        this._col++;
                        if (this._reader.eos())
                        {
                            return new Token("N", c, 1, this._line, this._col, this._ptr);
                        }
                        if (this._reader.next() == '\'')
                        {
                            c = '\'';
                            string str = this.readChar();
                            return new Token(str, string.Concat(new object[] { c, "'", str, "'" }), 7, this._line, this._col, this._ptr);
                        }
                        this._reader.unget();
                        break;

                    case '\'':
                        if (this._reader.eos())
                        {
                            throw new ParserException("unexpected input end");
                        }
                        return new Token(this.readChar(), 6, this._line, this._col, this._ptr);

                    case '"':
                        return new Token(this.readAlias(), 1, this._line, this._col, this._ptr);

                    case '[':
                        {
                            string str4 = this.readAlias_ex();
                            return new Token(str4, "[" + str4 + "]", 1, this._line, this._col, this._ptr);
                        }
                }
                if (((char.IsLetter(c) || (c == '_')) || (c == '#')) || ((c == '!') && (char.IsLetter(this._reader.lookup(0)) || ('_' == this._reader.lookup(0)))))
                {
                    int col = this._col;
                    int line = this._line;
                    string word = this.readIdent(c);
                    if (this._keywords.IsKeyWord(word))
                    {
                        if ("table".EqualsIgnoreCase(word) && (this._reader.lookup(0) == '('))
                        {
                            return new Token(word + this.readTableFunction(), 0x12, this._line, this._col, this._ptr);
                        }
                        return new Token(word, 3, line, col, this._ptr);
                    }
                    if ((this.reservedWords != null) && (((KSQLReservedWord)this.reservedWords).isReservedWord(word) != null))
                    {
                        return new Token("\"" + word + "\"", word, 1, line, col, this._ptr);
                    }
                    return new Token(word, 1, line, col, this._ptr);
                }
                if ((c == '@') || (c == ':'))
                {
                    int num3 = this._col;
                    int num4 = this._line;
                    if (c == ':')
                    {
                        char ch2 = this._reader.lookup(0);
                        if ((ch2 >= '0') && (ch2 < '9'))
                        {
                            return new Token(":", 5, num4, num3, this._ptr);
                        }
                    }
                    string str6 = this.readIdent(c);
                    if ((c == ':') && (str6.Length == 1))
                    {
                        return new Token(":", 5, num4, num3, this._ptr);
                    }
                    return new Token(str6, 2, num4, num3, this._ptr);
                }
                if (c == '?')
                {
                    this._col++;
                    return new Token("?", 2, this._line, this._col, this._ptr);
                }
                if (char.IsDigit(c))
                {
                    return this.readDigit(c, this._col);
                }
                if ((c == '/') && (this._reader.peek() == '/'))
                {
                    this._col++;
                    this._reader.skip();
                    string str7 = this.readlineComment();
                    if (!this._skipComment)
                    {
                        return new Token(str7, 0, this._line, this._col, this._ptr);
                    }
                    continue;
                }
                if ((c == '-') && (this._reader.peek() == '-'))
                {
                    this._col++;
                    this._reader.skip();
                    string str8 = this.readlineComment();
                    if (!this._skipComment)
                    {
                        return new Token(str8, 0, this._line, this._col, this._ptr);
                    }
                    continue;
                }
                if ((c == '/') && (this._reader.peek() == '*'))
                {
                    this._reader.skip();
                    if (this._reader.peek() == '+')
                    {
                        this._reader.skip();
                        StringBuilder builder = new StringBuilder();
                        while (true)
                        {
                            c = this._reader.next();
                            if ((c == '*') && (this._reader.peek() == '/'))
                            {
                                this._reader.skip();
                                return new Token(builder.ToString(), 0x10, this._line, this._col, this._ptr);
                            }
                            builder.Append(c);
                        }
                    }
                    StringBuilder builder2 = new StringBuilder();
                    while (true)
                    {
                        c = this._reader.next();
                        if ((c == '*') && (this._reader.peek() == '/'))
                        {
                            this._reader.skip();
                            break;
                        }
                        builder2.Append(c);
                    }
                    if (this._skipComment)
                    {
                        continue;
                    }
                    return new Token(builder2.ToString(), 14, this._line, this._col, this._ptr);
                }
                Token token = this.readOperator(c);
                if (token != null)
                {
                    return token;
                }
                token = this.readPunctuation(c);
                if (token != null)
                {
                    return token;
                }
                if (c == '$')
                {
                    c = this._reader.peek();
                    this._col++;
                    if (char.IsDigit(c))
                    {
                        return this.readDigit(c, this._col);
                    }
                }
                throw new ParserException(string.Concat(new object[] { "Error: Unknowen char not read at (", this._col, "/", this._line, ") in Lexer.Next()\n It was: ", c }), this._line, this._col);
            }
            return new Token("", 12, this._line, this._col, this._ptr);
        }

        private string readAlias()
        {
            char ch = '\0';
            StringBuilder builder = new StringBuilder();
            builder.Append('"');
            while (!this._reader.eos())
            {
                ch = this._reader.next();
                if (ch == '"')
                {
                    if (this._reader.eos())
                    {
                        builder.Append('"');
                        return builder.ToString();
                    }
                    if (this._reader.next() == '"')
                    {
                        builder.Append('"');
                        continue;
                    }
                    this._reader.unget();
                    ch = '"';
                    builder.Append('"');
                    break;
                }
                this._col++;
                if (ch == '\n')
                {
                    this._line++;
                    this._col = 1;
                }
                builder.Append(ch);
            }
            if (ch != '"')
            {
                string message = "End of File before String terminated at (";
                throw new ParserException(message, this._line, this._col);
            }
            return builder.ToString();
        }

        private string readAlias_ex()
        {
            char ch = '\0';
            StringBuilder builder = new StringBuilder();
            while (!this._reader.eos())
            {
                ch = this._reader.next();
                if (ch == ']')
                {
                    break;
                }
                this._col++;
                if (ch == '\n')
                {
                    this._line++;
                    this._col = 1;
                }
                builder.Append(ch);
            }
            if (ch != ']')
            {
                string message = "End of File before String terminated at (";
                throw new ParserException(message, this._line, this._col);
            }
            return builder.ToString();
        }

        private string readChar()
        {
            char ch = '\0';
            StringBuilder builder = new StringBuilder();
            while (!this._reader.eos())
            {
                ch = this._reader.next();
                if (ch == '\'')
                {
                    if (this._reader.eos())
                    {
                        return builder.ToString();
                    }
                    if (this._reader.next() == '\'')
                    {
                        builder.Append('\'');
                        builder.Append('\'');
                        continue;
                    }
                    this._reader.unget();
                    ch = '\'';
                    break;
                }
                this._col++;
                if (ch == '\n')
                {
                    this._line++;
                    this._col = 1;
                }
                builder.Append(ch);
            }
            if (ch != '\'')
            {
                string message = "End of File before String terminated at (";
                throw new ParserException(message, this._line, this._col);
            }
            return builder.ToString();
        }

        private Token readDigit(char ch, int x)
        {
            int line = this._line;
            this._col++;
            bool flag = false;
            StringBuilder builder = new StringBuilder();
            builder.Append(ch);
            while (char.IsDigit(this._reader.peek()))
            {
                builder.Append(this._reader.next());
                this._col++;
            }
            if (this._reader.peek() == '.')
            {
                flag = true;
                builder.Append(this._reader.next());
                this._col++;
                while (char.IsDigit(this._reader.peek()))
                {
                    builder.Append(this._reader.next());
                    this._col++;
                }
            }
            if (flag)
            {
                return new Token(builder.ToString(), 10, line, x, this._ptr);
            }
            long num2 = long.Parse(builder.ToString());
            if ((num2 >= -2147483648L) && (num2 <= 0x7fffffffL))
            {
                return new Token(builder.ToString(), 8, line, x, this._ptr);
            }
            return new Token(builder.ToString(), 15, line, x, this._ptr);
        }

        private string readIdent(char c)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(c);
            this._col++;
            while (true)
            {
                if (this._reader.eos())
                {
                    break;
                }
                if ((!char.IsLetterOrDigit(c = this._reader.next()) && (c != '_')) && (c != '#'))
                {
                    this._reader.unget();
                    break;
                }
                builder.Append(c);
                this._col++;
            }
            return builder.ToString();
        }

        private string readlineComment()
        {
            if (this._reader.eos())
            {
                return "";
            }
            StringBuilder builder = new StringBuilder();
            for (char ch = this._reader.next(); (ch != '\n') && !this._reader.eos(); ch = this._reader.next())
            {
                builder.Append(ch);
            }
            return builder.ToString();
        }

        private Token readOperator(char ch)
        {
            char ch3;
            int col = this._col;
            int line = this._line;
            this._col++;
            switch (ch)
            {
                case '%':
                    if (!this._reader.eos())
                    {
                        if (this._reader.next() == '=')
                        {
                            this._col++;
                            return new Token("%=", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                        return new Token("%", 4, line, col, this._ptr);
                    }
                    return new Token("%", 4, line, col, this._ptr);

                case '&':
                    if (!this._reader.eos())
                    {
                        switch (this._reader.next())
                        {
                            case '&':
                                this._col++;
                                return new Token("&&", 4, line, col, this._ptr);

                            case '=':
                                this._col++;
                                return new Token("&=", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                        return new Token("&", 4, line, col, this._ptr);
                    }
                    return new Token("&", 4, line, col, this._ptr);

                case '*':
                    if (!this._reader.eos())
                    {
                        if (this._reader.next() == '=')
                        {
                            this._col++;
                            return new Token("*=", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                        return new Token("*", 4, line, col, this._ptr);
                    }
                    return new Token("*", 4, line, col, this._ptr);

                case '+':
                    if (!this._reader.eos())
                    {
                        switch (this._reader.next())
                        {
                            case '+':
                                this._col++;
                                throw new ParserException("ERROR");

                            case '=':
                                this._col++;
                                throw new ParserException("ERROR");
                        }
                        this._reader.unget();
                        return new Token("+", 4, line, col, this._ptr);
                    }
                    return new Token("+", 4, line, col, this._ptr);

                case '-':
                    if (!this._reader.eos())
                    {
                        switch (this._reader.next())
                        {
                            case '=':
                                this._col++;
                                throw new ParserException("ERROR");

                            case '>':
                                this._col++;
                                throw new ParserException("ERROR");

                            case '-':
                                this._col++;
                                throw new ParserException("ERROR");
                        }
                        this._reader.unget();
                        char ch2 = this._reader.peek();
                        if (((this._col <= 0) || (ch2 < '0')) || (ch2 > '9'))
                        {
                            return new Token("-", 4, line, col, this._ptr);
                        }
                        int i = -2;
                        while (char.IsWhiteSpace(ch3 = this._reader.lookup(i)))
                        {
                            i--;
                            if ((this._col + i) <= 1)
                            {
                                break;
                            }
                        }
                        break;
                    }
                    return new Token("-", 4, line, col, this._ptr);

                case '/':
                    if (!this._reader.eos())
                    {
                        switch (this._reader.next())
                        {
                            case '*':
                                throw new ParserException("not support operator: *");

                            case '=':
                                this._col++;
                                return new Token("/=", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                        return new Token("/", 4, line, col, this._ptr);
                    }
                    return new Token("/", 4, line, col, this._ptr);

                case '!':
                    if (!this._reader.eos())
                    {
                        if (this._reader.next() == '=')
                        {
                            this._col++;
                            return new Token("!=", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                    }
                    return new Token("!", 4, line, col, this._ptr);

                case '<':
                    if (!this._reader.eos())
                    {
                        switch (this._reader.next())
                        {
                            case '<':
                                if (this._reader.next() != '=')
                                {
                                    this._col++;
                                    this._reader.unget();
                                    return new Token("<<", 4, line, col, this._ptr);
                                }
                                this._col += 2;
                                return new Token("<<=", 4, line, col, this._ptr);

                            case '=':
                                this._col++;
                                return new Token("<=", 4, line, col, this._ptr);

                            case '>':
                                this._col++;
                                return new Token("<>", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                        return new Token("<", 4, line, col, this._ptr);
                    }
                    return new Token("<", 4, line, col, this._ptr);

                case '=':
                    if (!this._reader.eos())
                    {
                        if (this._reader.next() == '=')
                        {
                            this._col++;
                            return new Token("==", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                        return new Token("=", 4, line, col, this._ptr);
                    }
                    return new Token("=", 4, line, col, this._ptr);

                case '>':
                    if (!this._reader.eos())
                    {
                        switch (this._reader.next())
                        {
                            case '=':
                                this._col++;
                                return new Token(">=", 4, line, col, this._ptr);

                            case '>':
                                if (this._reader.next() != '=')
                                {
                                    this._col++;
                                    this._reader.unget();
                                    return new Token(">>", 4, line, col, this._ptr);
                                }
                                this._col += 2;
                                return new Token(">>=", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                        return new Token(">", 4, line, col, this._ptr);
                    }
                    return new Token(">", 4, line, col, this._ptr);

                case '^':
                    if (!this._reader.eos())
                    {
                        if (this._reader.next() == '=')
                        {
                            this._col++;
                            return new Token("^=", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                    }
                    return new Token("^", 4, line, col, this._ptr);

                case '|':
                    if (!this._reader.eos())
                    {
                        switch (this._reader.next())
                        {
                            case '=':
                                this._col++;
                                return new Token("|=", 4, line, col, this._ptr);

                            case '|':
                                this._col++;
                                return new Token("||", 4, line, col, this._ptr);
                        }
                        this._reader.unget();
                        return new Token("|", 4, line, col, this._ptr);
                    }
                    return new Token("|", 4, line, col, this._ptr);

                case '~':
                    return new Token("~", 4, line, col, this._ptr);

                default:
                    this._col--;
                    return null;
            }
            if ((!ch3.Equals(',') && !ch3.Equals('(')) && !this.isOperator(ch3))
            {
                return new Token("-", 4, line, col, this._ptr);
            }
            return this.readDigit('-', this._col - 1);
        }

        private Token readPunctuation(char ch)
        {
            int col = this._col;
            int line = this._line;
            this._col++;
            switch (ch)
            {
                case '(':
                    return new Token("(", 5, line, col, this._ptr);

                case ')':
                    return new Token(")", 5, line, col, this._ptr);

                case ',':
                    return new Token(",", 5, line, col, this._ptr);

                case '.':
                    if (!char.IsDigit(this._reader.peek()))
                    {
                        return new Token(".", 5, line, col, this._ptr);
                    }
                    this._reader.unget();
                    this._col -= 2;
                    return this.readDigit('0', this._col + 1);

                case ':':
                    return new Token(":", 5, line, col, this._ptr);

                case ';':
                    return new Token(";", 5, line, col, this._ptr);

                case '[':
                    return new Token("[", 5, line, col, this._ptr);

                case ']':
                    return new Token("]", 5, line, col, this._ptr);

                case '{':
                    return new Token("{", 5, line, col, this._ptr);

                case '}':
                    return new Token("}", 5, line, col, this._ptr);
            }
            this._col--;
            return null;
        }

        private string readTableFunction()
        {
            char ch = '\0';
            StringBuilder builder = new StringBuilder();
            int num = 0;
            while (!this._reader.eos())
            {
                ch = this._reader.next();
                if (ch == '(')
                {
                    num++;
                }
                else if (ch == ')')
                {
                    num--;
                    if (num == 0)
                    {
                        builder.Append(ch);
                        break;
                    }
                }
                this._col++;
                if (ch == '\n')
                {
                    this._line++;
                    this._col = 1;
                }
                builder.Append(ch);
            }
            if (ch != ')')
            {
                string message = "End of File before String terminated at (";
                throw new ParserException(message, this._line, this._col);
            }
            return builder.ToString();
        }

        // Properties
        public KeyWord Keywords
        {
            get
            {
                return this._keywords;
            }
            set
            {
                this._keywords = value;
            }
        }
    }






}
