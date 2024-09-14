using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Parser
{
    public sealed class TokenType
    {
        // Fields
        public const int BIGINT = 0x11;
        public const int Char = 6;
        public const int Decimal = 11;
        public const int Double = 10;
        public const int EOF = 12;
        public const int Float = 9;
        public const int HINT = 0x10;
        public const int Identifier = 1;
        public const int Int = 8;
        public const int Keyword = 3;
        public const int LineComment = 0;
        public const int Long = 15;
        public const int MultiLineComment = 14;
        public const int NChar = 7;
        public const int Operator = 4;
        public const int Punctuation = 5;
        public const int TableFunction = 0x12;
        public const int Unknown = 13;
        public const int Variable = 2;

        // Methods
        private TokenType()
        {
        }

        public static string Typename(int tokType)
        {
            switch (tokType)
            {
                case 0:
                    return "Comment";

                case 1:
                    return "Identifier";

                case 2:
                    return "Variable";

                case 3:
                    return "Keyword";

                case 4:
                    return "Operator";

                case 5:
                    return "Punctuation";

                case 6:
                    return "Char";

                case 7:
                    return "NChar";

                case 8:
                    return "Int";

                case 9:
                    return "Float";

                case 10:
                    return "Double";

                case 11:
                    return "Decimal";

                case 12:
                    return "EOF";

                case 0x10:
                    return "Hints";

                case 0x11:
                    return "BIGINT";
            }
            return "Unknown";
        }
    }





}
