using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql.parser
{
    public class TokenType
    {
        public static readonly int LineComment = 0;
        public static readonly int Identifier = 1;
        public static readonly int Variable = 2;
        public static readonly int Keyword = 3;
        public static readonly int Operator = 4;
        public static readonly int Punctuation = 5;
        public static readonly int Char = 6;
        public static readonly int NChar = 7;
        public static readonly int Int = 8;
        public static readonly int Float = 9;
        public static readonly int Double = 10;
        public static readonly int Decimal = 11;
        public static readonly int EOF = 12;
        public static readonly int Unknown = 13;
        public static readonly int MultiLineComment = 14;
        public static readonly int Long = 15;
        public static readonly int HINT = 16;

        private TokenType()
        {

        }
        public static  string Typename( int tokType)
        {
            switch (tokType)
            {
                case 0:
                    {
                        return "Comment";
                    }
                case 1:
                    {
                        return "Identifier";
                    }
                case 2:
                    {
                        return "Variable";
                    }
                case 3:
                    {
                        return "Keyword";
                    }
                case 4:
                    {
                        return "Operator";
                    }
                case 5:
                    {
                        return "Punctuation";
                    }
                case 6:
                    {
                        return "Char";
                    }
                case 7:
                    {
                        return "NChar";
                    }
                case 8:
                    {
                        return "Int";
                    }
                case 9:
                    {
                        return "Float";
                    }
                case 10:
                    {
                        return "Double";
                    }
                case 11:
                    {
                        return "Decimal";
                    }
                case 12:
                    {
                        return "EOF";
                    }
                case 16:
                    {
                        return "Hints";
                    }
                default:
                    {
                        return "Unknown";
                    }
            }
        }
    }
}
