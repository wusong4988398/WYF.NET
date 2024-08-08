using WYF.Bos.algo.sql.tree;
using WYF.Bos.ksql.exception;
using WYF.Bos.ksql.parser;
using WYF.Bos.ksql.util;
using System;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using StringReader = WYF.Bos.ksql.parser.StringReader;

namespace WYF.Bos.ksql
{
    public class Lexer
    {
        KeyWord _keywords;
        ReservedWord reservedWords;
        private StringReader _reader;
        private int _col;
        private int _line;
        private int _ptr;
        private bool _skipComment;


        public Lexer(KeyWord keyword, StringReader _reader)
        {
            this.reservedWords = null;
            this._col = 1;
            this._line = 1;
            this._skipComment = true;
            this._keywords = keyword;
            this._reader = _reader;
            this.reservedWords = new KSQLReservedWord();
        }
        public Lexer(KeyWord keyword, ReservedWord reservedWords, StringReader _reader)
        {
            this.reservedWords = null;
            this._col = 1;
            this._line = 1;
            this._skipComment = true;
            this._keywords = keyword;
            this.reservedWords = reservedWords;
            this._reader = _reader;
        }
        public Lexer(StringReader reader)
        {
            this.reservedWords = null;
            this._col = 1;
            this._line = 1;
            this._skipComment = true;
            this._ptr = reader.Ptr;
            this._keywords = KeyWord.instance;
            this._reader = reader;
            this.reservedWords = new KSQLReservedWord();
        }
        public Lexer(string text) : this(text, true)
        {

        }

        public Lexer(string text, bool skipComment)
        {
            this.reservedWords = null;
            this._col = 1;
            this._line = 1;
            this._skipComment = true;
            this._skipComment = skipComment;
            if (text == null)
            {
                throw new ArgumentException("text");
            }
            this._keywords = KeyWord.instance;
            this._reader = new StringReader(text);
            this.reservedWords = new KSQLReservedWord();
        }

        private string ReadChar(char qoute)
        {
            char ch = '\0';
            StringBuilder text = new StringBuilder(16);
            while (!this._reader.Eos())
            {
                if ((ch = this._reader.Next()) == qoute)
                {
                    if (this._reader.Eos())
                    {
                        return text.ToString();
                    }
                    if (this._reader.Next() != qoute)
                    {
                        this._reader.Unget();
                        ch = qoute;
                        break;
                    }
                    text.Append(qoute);
                    text.Append(qoute);
                }
                else
                {
                    ++this._col;
                    if (ch == '\n')
                    {
                        ++this._line;
                        this._col = 1;
                    }
                    text.Append(ch);
                }
            }
            if (ch != qoute)
            {
                string message = "End of File before String terminated at (";
                throw new ParserException(message, this._line, this._col);
            }
            return text.ToString();
        }


        private string ReadAlias()
        {
            char ch = '\0';
            StringBuilder text = new StringBuilder(16);
            text.Append('\"');
            while (!this._reader.Eos())
            {
                if ((ch = this._reader.Next()) == '\"')
                {
                    if (this._reader.Eos())
                    {
                        text.Append('\"');
                        return text.ToString();
                    }
                    if (this._reader.Next() != '\"')
                    {
                        this._reader.Unget();
                        ch = '\"';
                        text.Append('\"');
                        break;
                    }
                    text.Append('\"');
                }
                else
                {
                    ++this._col;
                    if (ch == '\n')
                    {
                        ++this._line;
                        this._col = 1;
                    }
                    text.Append(ch);
                }
            }
            if (ch != '\"')
            {
                String message = "End of File before String terminated at (";
                throw new ParserException(message, this._line, this._col);
            }
            return text.ToString();
        }

        private string ReadAlias_ex()
        {
            char ch = '\0';
            StringBuilder text = new StringBuilder(16);
            while (!this._reader.Eos() && (ch = this._reader.Next()) != ']')
            {
                ++this._col;
                if (ch == '\n')
                {
                    ++this._line;
                    this._col = 1;
                }
                text.Append(ch);
            }
            if (ch != ']')
            {
                String message = "End of File before String terminated at (";
                throw new ParserException(message, this._line, this._col);
            }
            return text.ToString();
        }

        private string ReadIdent(char c)
        {
            StringBuilder s = new StringBuilder(16);
            s.Append(c);
            ++this._col;
            while (!this._reader.Eos())
            {
                if (!Char.IsLetterOrDigit(c = this._reader.Next()) && c != '_' && c != '#' && c != '$')
                {
                    this._reader.Unget();
                    return s.ToString();
                }
                s.Append(c);
                ++this._col;
            }
            return s.ToString();
        }


        private Token ReadDigit(char ch, int x)
        {
            int y = this._line;
            ++this._col;
            bool isdouble = false;
            StringBuilder text = new StringBuilder(8);
            text.Append(ch);
            while (Char.IsDigit(this._reader.Peek()))
            {
                text.Append(this._reader.Next());
                ++this._col;
            }
            if (this._reader.Peek() == '.')
            {
                isdouble = true;
                text.Append(this._reader.Next());
                ++this._col;
                while (Char.IsDigit(this._reader.Peek()))
                {
                    text.Append(this._reader.Next());
                    ++this._col;
                }
            }
            if (isdouble)
            {
                return new Token(text.ToString(), 10, y, x, this._ptr);
            }

            long longValue = long.Parse(text.ToString());
            if (longValue < -2147483648L || longValue > 2147483647L)
            {
                return new Token(text.ToString(), 15, y, x, this._ptr);
            }
            return new Token(text.ToString(), 8, y, x, this._ptr);
        }
        private bool IsOperator(char c)
        {
            switch (c)
            {
                case '!':
                case '&':
                case '*':
                case '+':
                case '-':
                case '/':
                case '=':
                case '^':
                case '|':
                case '~':
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        private string ReadlineComment()
        {
            if (this._reader.Eos())
            {
                return "";
            }
            StringBuilder comment = new StringBuilder(16);
            for (char ch = this._reader.Next(); ch != '\n' && !this._reader.Eos(); ch = this._reader.Next())
            {
                comment.Append(ch);
            }
            return comment.ToString();
        }
        private Token ReadOperator(char ch)
        {
            int x = this._col;
            int y = this._line;
            ++this._col;
            switch (ch)
            {
                case '+':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("+", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '+':
                                {
                                    ++this._col;
                                    throw new ParserException("ERROR");
                                }
                            case '=':
                                {
                                    ++this._col;
                                    throw new ParserException("ERROR");
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("+", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '-':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("-", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '-':
                                {
                                    ++this._col;
                                    throw new ParserException("ERROR");
                                }
                            case '=':
                                {
                                    ++this._col;
                                    throw new ParserException("ERROR");
                                }
                            case '>':
                                {
                                    ++this._col;
                                    throw new ParserException("ERROR");
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    char peek = this._reader.Peek();
                                    if (this._col <= 0 || peek < '0' || peek > '9')
                                    {
                                        return new Token("-", 4, y, x, this._ptr);
                                    }
                                    int i = -2;
                                    char p;
                                    while (Char.IsWhiteSpace(p = this._reader.Lookup(i)))
                                    {
                                        --i;
                                        if (this._col + i <= 1)
                                        {
                                            break;
                                        }
                                    }
                                    if (p == ',' || p == '(' || this.IsOperator(p))
                                    {
                                        return this.ReadDigit('-', this._col - 1);
                                    }
                                    return new Token("-", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '*':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("*", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '=':
                                {
                                    ++this._col;
                                    return new Token("*=", 4, y, x, this._ptr);
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("*", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '/':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("/", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '=':
                                {
                                    ++this._col;
                                    return new Token("/=", 4, y, x, this._ptr);
                                }
                            case '*':
                                {
                                    throw new ParserException("not support operator: *");
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("/", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '%':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("%", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '=':
                                {
                                    ++this._col;
                                    return new Token("%=", 4, y, x, this._ptr);
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("%", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '&':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("&", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '&':
                                {
                                    ++this._col;
                                    return new Token("&&", 4, y, x, this._ptr);
                                }
                            case '=':
                                {
                                    ++this._col;
                                    return new Token("&=", 4, y, x, this._ptr);
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("&", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '|':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("|", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '|':
                                {
                                    ++this._col;
                                    return new Token("||", 4, y, x, this._ptr);
                                }
                            case '=':
                                {
                                    ++this._col;
                                    return new Token("|=", 4, y, x, this._ptr);
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("|", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '^':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("^", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '=':
                                {
                                    ++this._col;
                                    return new Token("^=", 4, y, x, this._ptr);
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("^", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '!':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("!", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '=':
                                {
                                    ++this._col;
                                    return new Token("!=", 4, y, x, this._ptr);
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("!", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '~':
                    {
                        return new Token("~", 4, y, x, this._ptr);
                    }
                case '=':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("=", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '=':
                                {
                                    ++this._col;
                                    return new Token("==", 4, y, x, this._ptr);
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("=", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '<':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token("<", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '<':
                                {
                                    switch (this._reader.Next())
                                    {
                                        case '=':
                                            {
                                                this._col += 2;
                                                return new Token("<<=", 4, y, x, this._ptr);
                                            }
                                        default:
                                            {
                                                ++this._col;
                                                this._reader.Unget();
                                                return new Token("<<", 4, y, x, this._ptr);
                                            }
                                    }

                                }
                            case '=':
                                {
                                    ++this._col;
                                    return new Token("<=", 4, y, x, this._ptr);
                                }
                            case '>':
                                {
                                    ++this._col;
                                    return new Token("<>", 4, y, x, this._ptr);
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token("<", 4, y, x, this._ptr);
                                }
                        }

                    }
                case '>':
                    {
                        if (this._reader.Eos())
                        {
                            return new Token(">", 4, y, x, this._ptr);
                        }
                        switch (this._reader.Next())
                        {
                            case '>':
                                {
                                    switch (this._reader.Next())
                                    {
                                        case '=':
                                            {
                                                this._col += 2;
                                                return new Token(">>=", 4, y, x, this._ptr);
                                            }
                                        default:
                                            {
                                                ++this._col;
                                                this._reader.Unget();
                                                return new Token(">>", 4, y, x, this._ptr);
                                            }
                                    }

                                }
                            case '=':
                                {
                                    ++this._col;
                                    return new Token(">=", 4, y, x, this._ptr);
                                }
                            default:
                                {
                                    this._reader.Unget();
                                    return new Token(">", 4, y, x, this._ptr);
                                }
                        }

                    }
                default:
                    {
                        --this._col;
                        return null;
                    }
            }
        }


        private Token ReadPunctuation(char ch)
        {
            int x = this._col;
            int y = this._line;
            ++this._col;
            switch (ch)
            {
                case ';':
                    {
                        return new Token(";", 5, y, x, this._ptr);
                    }
                case ':':
                    {
                        return new Token(":", 5, y, x, this._ptr);
                    }
                case ',':
                    {
                        return new Token(",", 5, y, x, this._ptr);
                    }
                case '.':
                    {
                        if (Char.IsDigit(this._reader.Peek()))
                        {
                            this._reader.Unget();
                            this._col -= 2;
                            return this.ReadDigit('0', this._col + 1);
                        }
                        return new Token(".", 5, y, x, this._ptr);
                    }
                case ')':
                    {
                        return new Token(")", 5, y, x, this._ptr);
                    }
                case '(':
                    {
                        return new Token("(", 5, y, x, this._ptr);
                    }
                case ']':
                    {
                        return new Token("]", 5, y, x, this._ptr);
                    }
                case '[':
                    {
                        return new Token("[", 5, y, x, this._ptr);
                    }
                case '}':
                    {
                        return new Token("}", 5, y, x, this._ptr);
                    }
                case '{':
                    {
                        return new Token("{", 5, y, x, this._ptr);
                    }
                default:
                    {
                        --this._col;
                        return null;
                    }
            }
        }
        public Token Next()
        {
            while (!this._reader.Eos())
            {
                char ch = this._reader.Next();
                if (Char.IsWhiteSpace(ch))
                {
                    ++this._col;
                    if (ch != '\n')
                    {
                        continue;
                    }
                    ++this._line;
                    this._col = 1;
                }
                else
                {
                    this._ptr = this._reader.Ptr;
                    if (ch == 'N' || ch == 'n')
                    {
                        ++this._col;
                        if (this._reader.Eos())
                        {
                            return new Token("N", "" + ch, 1, this._line, this._col, this._ptr);
                        }
                        if (this._reader.Next() == '\'')
                        {
                            String tmp = this.ReadChar('\'');
                            return new Token(tmp, ch + "'" + tmp + "'", 7, this._line, this._col, this._ptr);
                        }
                        this._reader.Unget();
                    }
                    if (ch == '\'')
                    {
                        if (this._reader.Eos())
                        {
                            throw new Exception("unexpected input end");
                        }
                        String text = this.ReadChar('\'');
                        return new Token(text, 6, this._line, this._col, this._ptr);
                    }
                    else if (ch == '`')
                    {
                        if (this._reader.Eos())
                        {
                            throw new Exception("unexpected input end");
                        }
                        string text = this.ReadChar('`');
                        return new Token(text, 1, this._line, this._col, this._ptr);
                    }
                    else
                    {
                        if (ch == '\"')
                        {
                            string text = this.ReadAlias();
                            return new Token(text, 1, this._line, this._col, this._ptr);
                        }
                        if (ch == '[')
                        {
                            String text = this.ReadAlias_ex();
                            return new Token(text, "[" + text + "]", 1, this._line, this._col, this._ptr);
                        }

                        if (Char.IsLetter(ch) || ch == '_' || ch == '#' || (ch == '!' && (Char.IsLetter(this._reader.Lookup(0)) || '_' == this._reader.Lookup(0))))
                        {
                            int x = this._col;
                            int y = this._line;
                            String text2 = this.ReadIdent(ch);
                            if (this._keywords.IsKeyWord(text2))
                            {
                                return new Token(text2, 3, y, x, this._ptr);
                            }
                            if (this.reservedWords != null && this.reservedWords.IsReservedWord(text2) != null)
                            {
                                return new Token("\"" + text2 + "\"", text2, 1, y, x, this._ptr);
                            }
                            return new Token(text2, 1, y, x, this._ptr);
                        }
                        else if (ch == '@' || ch == ':')
                        {
                            int x = this._col;
                            int y = this._line;
                            if (ch == ':')
                            {
                                char nextChar = this._reader.Lookup(0);
                                if (nextChar >= '0' && nextChar < '9')
                                {
                                    return new Token(":", 5, y, x, this._ptr);
                                }
                            }
                            String text2 = this.ReadIdent(ch);
                            if (ch == ':' && text2.Length == 1)
                            {
                                return new Token(":", 5, y, x, this._ptr);
                            }
                            return new Token(text2, 2, y, x, this._ptr);
                        }
                        else
                        {
                            if (ch == '?')
                            {
                                ++this._col;
                                return new Token("?", 2, this._line, this._col, this._ptr);
                            }
                            if (Char.IsDigit(ch))
                            {
                                return this.ReadDigit(ch, this._col);
                            }
                            if (ch == '/' && this._reader.Peek() == '/')
                            {
                                ++this._col;
                                this._reader.Skip();
                                string text = this.ReadlineComment();
                                if (this._skipComment)
                                {
                                    continue;
                                }
                                return new Token(text, 0, this._line, this._col, this._ptr);
                            }
                            else if (ch == '-' && this._reader.Peek() == '-')
                            {
                                ++this._col;
                                this._reader.Skip();
                                string text = this.ReadlineComment();
                                if (this._skipComment)
                                {
                                    continue;
                                }
                                return new Token(text, 0, this._line, this._col, this._ptr);
                            }
                            else if (ch == '/' && this._reader.Peek() == '*')
                            {
                                this._reader.Skip();
                                if (this._reader.Peek() == '+')
                                {
                                    this._reader.Skip();
                                    StringBuilder hint = new StringBuilder(16);
                                    while (true)
                                    {
                                        ch = this._reader.Next();
                                        if (ch == '*' && this._reader.Peek() == '/')
                                        {
                                            break;
                                        }
                                        hint.Append(ch);
                                    }
                                    this._reader.Skip();
                                    return new Token(hint.ToString(), 16, this._line, this._col, this._ptr);
                                }
                                StringBuilder comment = new StringBuilder(16);
                                while (true)
                                {
                                    ch = this._reader.Next();
                                    if (ch == '*' && this._reader.Peek() == '/')
                                    {
                                        break;
                                    }
                                    comment.Append(ch);
                                }
                                this._reader.Skip();
                                if (this._skipComment)
                                {
                                    continue;
                                }
                                return new Token(comment.ToString(), 14, this._line, this._col, this._ptr);
                            }
                            else
                            {
                                Token token = this.ReadOperator(ch);
                                if (token != null)
                                {
                                    return token;
                                }
                                token = this.ReadPunctuation(ch);
                                if (token != null)
                                {
                                    return token;
                                }
                                if (ch == '$')
                                {
                                    ch = this._reader.Peek();
                                    ++this._col;
                                    if (Char.IsDigit(ch))
                                    {
                                        return this.ReadDigit(ch, this._col);
                                    }
                                }
                                String message = "Error: Unknowen char not read at (" + this._col + "/" + this._line + ") in Lexer.Next()\n It was: " + ch;
                                throw new ParserException(message, this._line, this._col);
                            }
                        }
                    }
                }
            }
            return new Token("", 12, this._line, this._col, this._ptr);
        }


    }
}
