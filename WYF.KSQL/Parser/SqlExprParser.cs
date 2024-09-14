using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Expr;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public class SqlExprParser : SqlParserBase
    {
        private TokenList _tokenList;
        public bool allowStringAdditive;
        private KeyWord inSubQueryKeyword;

        public SqlExprParser(Lexer lexer)
        {
            this._tokenList = new TokenList(lexer);
        }

        public SqlExprParser(TokenList tokList)
        {
            this._tokenList = tokList;
        }

        public SqlExprParser(string text)
        {
            Lexer l = new Lexer(text);
            this._tokenList = new TokenList(l);
        }

        public SqlExpr additive()
        {
            SqlExpr expr = this.multiplicative();
            return this.additiveRest(expr);
        }

        public SqlExpr additiveRest(SqlExpr expr)
        {
            if (this._tokenList.lookup(0).Equals(Token.PlusToken))
            {
                this._tokenList.match();
                SqlExpr right = this.multiplicativeNoIn();
                if (!this.allowStringAdditive && (((expr is SqlCharExpr) || (expr is SqlNCharExpr)) || ((right is SqlCharExpr) || (right is SqlNCharExpr))))
                {
                    throw new ParserException("Error. additive operator can not use add Char Or NChar");
                }
                expr = new SqlBinaryOpExpr(expr, 0, right);
                expr = this.additiveRest(expr);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.ConcatOpToken))
            {
                this._tokenList.match();
                SqlExpr expr3 = this.multiplicativeNoIn();
                expr = new SqlBinaryOpExpr(expr, 0x2a, expr3);
                expr = this.additiveRest(expr);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.MinusToken))
            {
                this._tokenList.match();
                SqlExpr expr4 = this.multiplicativeNoIn();
                if (((expr is SqlCharExpr) || (expr is SqlNCharExpr)) || ((expr4 is SqlCharExpr) || (expr4 is SqlNCharExpr)))
                {
                    throw new ParserException("Error. minus operator can not use add Char Or NChar");
                }
                expr = new SqlBinaryOpExpr(expr, 0x1a, expr4);
                expr = this.additiveRest(expr);
            }
            return expr;
        }

        public SqlExpr and()
        {
            SqlExpr expr = this.equality();
            return this.andRest(expr);
        }

        public SqlExpr andRest(SqlExpr expr)
        {
            while (this._tokenList.lookup(0).Equals(Token.AndToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                SqlExpr right = this.equality();
                expr = new SqlBinaryOpExpr(expr, 7, right);
                expr.setExprWord(orgValue);
            }
            return expr;
        }

        public SqlExpr equality()
        {
            SqlExpr expr = this.relational();
            return this.equalityRest(expr);
        }

        public SqlExpr equalityRest(SqlExpr expr)
        {
            if (this._tokenList.lookup(0).Equals(Token.EqualToken))
            {
                this._tokenList.match();
                SqlExpr expr2 = this.relational();
                expr2 = this.equalityRest(expr2);
                expr = new SqlBinaryOpExpr(expr, 10, expr2);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.NotEqualToken))
            {
                this._tokenList.match();
                SqlExpr expr3 = this.relational();
                expr3 = this.equalityRest(expr3);
                expr = new SqlBinaryOpExpr(expr, 0x17, expr3);
            }
            return expr;
        }

        public SqlExpr expr()
        {
            if (this._tokenList.lookup(0).Equals(Token.MulToken))
            {
                this._tokenList.match();
                return new SqlAllColumnExpr();
            }
            SqlExpr expr = this.unary();
            if (this._tokenList.lookup(0).Equals(Token.InToken))
            {
                expr = this.inRest(expr);
            }
            if (this._tokenList.lookup(0).Equals(Token.CommaToken))
            {
                return expr;
            }
            return this.exprRest(expr);
        }

        public void exprList(IList exprCol)
        {
            if (!this._tokenList.lookup(0).Equals(Token.CloseBraceToken) && !this._tokenList.lookup(0).Equals(Token.EOFToken))
            {
                SqlExpr expr = this.expr();
                exprCol.Add(expr);
                while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                {
                    this._tokenList.match();
                    expr = this.expr();
                    exprCol.Add(expr);
                }
            }
        }

        public SqlExpr exprRest(SqlExpr expr)
        {
            expr = this.multiplicativeRest(expr);
            expr = this.additiveRest(expr);
            expr = this.inRest(expr);
            expr = this.relationalRest(expr);
            expr = this.equalityRest(expr);
            expr = this.andRest(expr);
            expr = this.orRest(expr);
            return expr;
        }

        public KeyWord getInSubQueryKeyword()
        {
            return this.inSubQueryKeyword;
        }

        public SqlExpr inRest(SqlExpr expr)
        {
            if (this._tokenList.lookup(0).Equals(Token.InToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                this._tokenList.match(Token.OpenBraceToken);
                KeyWord keywords = null;
                if (this.inSubQueryKeyword != null)
                {
                    keywords = this._tokenList.lexer.Keywords;
                    this._tokenList.lexer.Keywords = this.inSubQueryKeyword;
                }
                try
                {
                    if (this._tokenList.lookup(0).value.EqualsIgnoreCase("SELECT"))
                    {
                        this._tokenList.lookup(0).type = 3;
                        SqlSelectBase subQuery = new SelectParser(this._tokenList).select();
                        SqlInSubQueryExpr expr2 = new SqlInSubQueryExpr(expr, subQuery);
                        expr2.setExprWord(orgValue);
                        expr = expr2;
                    }
                    else
                    {
                        SqlInListExpr expr3 = new SqlInListExpr(expr);
                        this.exprList(expr3.targetList);
                        expr3.setExprWord(orgValue);
                        expr = expr3;
                    }
                }
                finally
                {
                    if (keywords != null)
                    {
                        this._tokenList.lexer.Keywords = keywords;
                    }
                }
                this._tokenList.match(Token.CloseBraceToken);
            }
            expr = this.andRest(expr);
            return expr;
        }

        public SqlExpr multiplicative()
        {
            SqlExpr expr = this.unary();
            if (this._tokenList.lookup(0).Equals(Token.InToken))
            {
                expr = this.inRest(expr);
            }
            return this.multiplicativeRest(expr);
        }

        public SqlExpr multiplicativeNoIn()
        {
            SqlExpr expr = this.unary();
            return this.multiplicativeRest(expr);
        }

        public SqlExpr multiplicativeRest(SqlExpr expr)
        {
            if (this._tokenList.lookup(0).Equals(Token.MulToken))
            {
                this._tokenList.match();
                SqlExpr right = this.unary();
                expr = new SqlBinaryOpExpr(expr, 0x16, right);
                expr = this.multiplicativeRest(expr);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.DivToken))
            {
                this._tokenList.match();
                SqlExpr expr3 = this.unary();
                expr = new SqlBinaryOpExpr(expr, 9, expr3);
                expr = this.multiplicativeRest(expr);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.ModToken))
            {
                this._tokenList.match();
                SqlExpr expr4 = this.unary();
                expr = new SqlBinaryOpExpr(expr, 0x15, expr4);
                expr = this.multiplicativeRest(expr);
            }
            return expr;
        }

        public SqlExpr or()
        {
            SqlExpr expr = this.and();
            return this.orRest(expr);
        }

        public SqlExpr orRest(SqlExpr expr)
        {
            while (this._tokenList.lookup(0).Equals(Token.OrToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                SqlExpr right = this.and();
                expr = new SqlBinaryOpExpr(expr, 8, right);
                expr.setExprWord(orgValue);
            }
            return expr;
        }

        private SqlExpr OverExpr()
        {
            this._tokenList.match();
            if (!this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
            {
                throw new ParserException("invaild Over Expr.");
            }
            SqlOverExpr expr = new SqlOverExpr();
            this._tokenList.match();
            if (this._tokenList.lookup(0).Equals(Token.PartitionToken))
            {
                this._tokenList.match();
                if (!this._tokenList.lookup(0).Equals(Token.ByToken))
                {
                    throw new ParserException("invaild Over Expr.");
                }
                do
                {
                    this._tokenList.match();
                    SqlExpr expr2 = new SqlIdentifierExpr(this._tokenList.lookup(0).value);
                    expr.partition.Add(expr2);
                }
                while (this._tokenList.lookup(0).Equals(Token.CommaToken));
                this._tokenList.match();
            }
            if (this._tokenList.lookup(0).Equals(Token.OrderToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                do
                {
                    SqlOrderByItem item;
                    this._tokenList.match();
                    SqlExpr expr3 = this.expr();
                    string orgChineseOrderByType = null;
                    int chineseOrderByMode = -1;
                    if (this._tokenList.lookup(0).Equals(Token.PinYinToken))
                    {
                        orgChineseOrderByType = this._tokenList.lookup(0).GetOrgValue();
                        chineseOrderByMode = 2;
                        this._tokenList.match();
                    }
                    else if (this._tokenList.lookup(0).Equals(Token.StrokeToken))
                    {
                        orgChineseOrderByType = this._tokenList.lookup(0).GetOrgValue();
                        chineseOrderByMode = 3;
                        this._tokenList.match();
                    }
                    else if (this._tokenList.lookup(0).Equals(Token.RadicalToken))
                    {
                        orgChineseOrderByType = this._tokenList.lookup(0).GetOrgValue();
                        chineseOrderByMode = 4;
                        this._tokenList.match();
                    }
                    else
                    {
                        chineseOrderByMode = -1;
                    }
                    if (this._tokenList.lookup(0).Equals(Token.AscToken))
                    {
                        item = new SqlOrderByItem(expr3, 0, chineseOrderByMode);
                        item.setOrgOrderByName(this._tokenList.lookup(0).GetOrgValue());
                        item.setOrgChineseOrderByType(orgChineseOrderByType);
                        this._tokenList.match();
                    }
                    else if (this._tokenList.lookup(0).Equals(Token.DescToken))
                    {
                        item = new SqlOrderByItem(expr3, 1, chineseOrderByMode);
                        item.setOrgOrderByName(this._tokenList.lookup(0).GetOrgValue());
                        item.setOrgChineseOrderByType(orgChineseOrderByType);
                        this._tokenList.match();
                    }
                    else
                    {
                        item = new SqlOrderByItem(expr3, 0, chineseOrderByMode);
                        item.setOrgOrderByName(this._tokenList.lookup(0).GetOrgValue());
                        item.setOrgChineseOrderByType(orgChineseOrderByType);
                    }
                    expr.orderBy.Add(item);
                }
                while (this._tokenList.lookup(0).Equals(Token.CommaToken));
            }
            this._tokenList.match(Token.CloseBraceToken);
            return expr;
        }

        private SqlDateTimeExpr parseDatetimeExpr()
        {
            if (this._tokenList.lookup(0).Equals(Token.DateToken))
            {
                return this.parseSql99DateExpr();
            }
            if (this._tokenList.lookup(0).Equals(Token.TimeStampToken))
            {
                return this.parseSql99TimeStampExpr();
            }
            this._tokenList.match(Token.OpenCurlyBraceToken);
            SqlDateTimeExpr expr = new SqlDateTimeExpr();
            if (this._tokenList.lookup(0).type == 8)
            {
                expr.setDataTimeWord("");
                string s = this._tokenList.lookup(0).value;
                this._tokenList.match(8);
                int num = int.Parse(s);
                expr.setYear(num);
                this._tokenList.match(Token.MinusToken);
                string str2 = this._tokenList.lookup(0).value;
                this._tokenList.match(8);
                int num2 = int.Parse(str2);
                expr.setMonth(num2);
                this._tokenList.match(Token.MinusToken);
                string str3 = this._tokenList.lookup(0).value;
                this._tokenList.match(8);
                int num3 = int.Parse(str3);
                expr.setDate(num3);
                if (this._tokenList.lookup(0).Equals(Token.CloseCurlyBraceToken))
                {
                    this._tokenList.match(Token.CloseCurlyBraceToken);
                    return expr;
                }
                string str4 = this._tokenList.lookup(0).value;
                this._tokenList.match(8);
                int num4 = int.Parse(str4);
                expr.setHour(num4);
                this._tokenList.match(Token.ColonToken);
                string str5 = this._tokenList.lookup(0).value;
                this._tokenList.match(8);
                int num5 = int.Parse(str5);
                expr.setMinute(num5);
                this._tokenList.match(Token.ColonToken);
                if (this._tokenList.lookup(0).type == 8)
                {
                    string str6 = this._tokenList.lookup(0).value;
                    this._tokenList.match();
                    int num6 = int.Parse(str6);
                    expr.setSecond(num6);
                }
                else
                {
                    if (this._tokenList.lookup(0).type != 10)
                    {
                        throw new ParserException("Error DateTime Formate.");
                    }
                    string source = this._tokenList.lookup(0).value;
                    string str8 = source.substring(0, source.IndexOf('.'));
                    this._tokenList.match();
                    int num7 = int.Parse(str8);
                    expr.setSecond(num7);
                    int millisecond = int.Parse(source.Substring(source.IndexOf('.') + 1));
                    expr.setMillisecond(millisecond);
                }
                this._tokenList.match(Token.CloseCurlyBraceToken);
                return expr;
            }
            if (this._tokenList.lookup(0).type != 1)
            {
                throw new ParserException("Error DateTime Formate.");
            }
            string str10 = this._tokenList.lookup(0).value;
            if (str10.EqualsIgnoreCase("ts"))
            {
                expr.setTimeType(-19000);
                expr.setDataTimeWord(str10);
            }
            else if (str10.EqualsIgnoreCase("d"))
            {
                expr.setTimeType(-19001);
                expr.setDataTimeWord(str10);
            }
            else
            {
                if (!str10.EqualsIgnoreCase("t"))
                {
                    throw new ParserException("Error DateTime Formate.");
                }
                expr.setTimeType(-19002);
                expr.setDataTimeWord(str10);
            }
            this._tokenList.match();
            string text = this._tokenList.lookup(0).value;
            this._tokenList.match(6);
            Lexer l = new Lexer(text);
            TokenList list = new TokenList(l);
            if ((expr.timeType() == -19000) || (expr.timeType() == -19001))
            {
                string str12 = list.lookup(0).value;
                list.match(8);
                int num9 = int.Parse(str12);
                expr.setYear(num9);
                list.match(Token.MinusToken);
                string str13 = list.lookup(0).value;
                list.match(8);
                int num10 = int.Parse(str13);
                expr.setMonth(num10);
                list.match(Token.MinusToken);
                string str14 = list.lookup(0).value;
                list.match(8);
                int num11 = int.Parse(str14);
                expr.setDate(num11);
            }
            if (list.lookup(0).Equals(Token.EOFToken))
            {
                list.match();
                if (expr.timeType() == -19000)
                {
                    expr.setHour(0);
                    expr.setMinute(0);
                    expr.setSecond(0);
                }
            }
            else if ((expr.timeType() == -19000) || (expr.timeType() == -19002))
            {
                string str15 = list.lookup(0).value;
                list.match(8);
                int num12 = int.Parse(str15);
                expr.setHour(num12);
                list.match(Token.ColonToken);
                string str16 = list.lookup(0).value;
                list.match(8);
                int num13 = int.Parse(str16);
                expr.setMinute(num13);
                list.match(Token.ColonToken);
                string str17 = list.lookup(0).value;
                list.match(8);
                int num14 = int.Parse(str17);
                expr.setSecond(num14);
            }
            this._tokenList.match(Token.CloseCurlyBraceToken);
            return expr;
        }

        private SqlDateTimeExpr parseSql99DateExpr()
        {
            this._tokenList.match(Token.DateToken);
            SqlDateTimeExpr expr = new SqlDateTimeExpr();
            string text = this._tokenList.lookup(0).value;
            this._tokenList.match(6);
            Lexer l = new Lexer(text);
            TokenList list = new TokenList(l);
            string s = list.lookup(0).value;
            list.match(8);
            int num = int.Parse(s);
            expr.setYear(num);
            list.match(Token.MinusToken);
            string str3 = list.lookup(0).value;
            list.match(8);
            int num2 = int.Parse(str3);
            expr.setMonth(num2);
            list.match(Token.MinusToken);
            string str4 = list.lookup(0).value;
            list.match(8);
            int num3 = int.Parse(str4);
            expr.setDate(num3);
            return expr;
        }

        private SqlDateTimeExpr parseSql99TimeStampExpr()
        {
            this._tokenList.match(Token.TimeStampToken);
            SqlDateTimeExpr expr = new SqlDateTimeExpr();
            string text = this._tokenList.lookup(0).value;
            this._tokenList.match(6);
            Lexer l = new Lexer(text);
            TokenList list = new TokenList(l);
            string s = list.lookup(0).value;
            list.match(8);
            int num = int.Parse(s);
            expr.setYear(num);
            list.match(Token.MinusToken);
            string str3 = list.lookup(0).value;
            list.match(8);
            int num2 = int.Parse(str3);
            expr.setMonth(num2);
            list.match(Token.MinusToken);
            string str4 = list.lookup(0).value;
            list.match(8);
            int num3 = int.Parse(str4);
            expr.setDate(num3);
            if (list.lookup(0).Equals(Token.EOFToken))
            {
                list.match();
                return expr;
            }
            string str5 = list.lookup(0).value;
            list.match(8);
            int num4 = int.Parse(str5);
            expr.setHour(num4);
            list.match(Token.ColonToken);
            string str6 = list.lookup(0).value;
            list.match(8);
            int num5 = int.Parse(str6);
            expr.setMinute(num5);
            list.match(Token.ColonToken);
            if (list.lookup(0).type == 8)
            {
                string str7 = list.lookup(0).value;
                list.match();
                int num6 = int.Parse(str7);
                expr.setSecond(num6);
                return expr;
            }
            if (list.lookup(0).type != 10)
            {
                throw new ParserException("Error DateTime Formate.");
            }
            string source = list.lookup(0).value;
            string str9 = source.substring(0, source.IndexOf('.'));
            list.match();
            int num7 = int.Parse(str9);
            expr.setSecond(num7);
            int millisecond = int.Parse(source.Substring(source.IndexOf('.') + 1));
            expr.setMillisecond(millisecond);
            return expr;
        }

        public SqlExpr primary()
        {
            SqlExpr instance = null;
            if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
            {
                this._tokenList.match();
                instance = this.expr();
                this._tokenList.match(Token.CloseBraceToken);
            }
            else if (this._tokenList.lookup(0).type == 1)
            {
                instance = new SqlIdentifierExpr(this._tokenList.lookup(0).value, this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
            }
            else if (((this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.KSQL_COL_NAME.value) || this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.TABNAME.value)) || (this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.INDNAME.value) || this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.KSQL_CONS_NAME.value))) || (this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.KSQL_COL_DEFAULT.value) || this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.KSQL_COL_NULLABLE.value)))
            {
                instance = new SqlIdentifierExpr(this._tokenList.lookup(0).value, this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).value.EqualsIgnoreCase("IDENTITY"))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.IdentityToken);
                this._tokenList.match(Token.OpenBraceToken);
                string dataType = this._tokenList.lookup(0).value;
                this._tokenList.match();
                this._tokenList.match(Token.CommaToken);
                int seed = int.Parse(this._tokenList.lookup(0).value);
                this._tokenList.match();
                this._tokenList.match(Token.CommaToken);
                int increment = int.Parse(this._tokenList.lookup(0).value);
                this._tokenList.match();
                this._tokenList.match(Token.CloseBraceToken);
                instance = new SqlIdentityExpr(orgValue, false, dataType, seed, increment);
            }
            else if (this._tokenList.lookup(0).Equals(Token.New))
            {
                this._tokenList.match();
                StringBuilder builder = new StringBuilder();
                StringBuilder builder2 = new StringBuilder();
                builder.Append(this._tokenList.lookup(0).value);
                builder2.Append(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                while (this._tokenList.lookup(0).Equals(Token.PeriodToken))
                {
                    this._tokenList.match();
                    builder.Append('.');
                    builder2.Append('.');
                    builder.Append(this._tokenList.lookup(0).value);
                    builder2.Append(this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match();
                }
                this._tokenList.match(Token.OpenBraceToken);
                ObjectCreateExpr expr2 = new ObjectCreateExpr(builder.ToString(), builder2.ToString());
                this.exprList(expr2.paramList);
                this._tokenList.match(Token.CloseBraceToken);
                instance = expr2;
            }
            else if (this._tokenList.lookup(0).type == 8)
            {
                instance = new SqlIntExpr(this._tokenList.lookup(0).value);
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).type == 15)
            {
                instance = new SqlLongExpr(this._tokenList.lookup(0).value);
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).type == 10)
            {
                instance = new SqlDoubleExpr(this._tokenList.lookup(0).value);
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).type == 6)
            {
                instance = new SqlCharExpr(this._tokenList.lookup(0).value);
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).type == 7)
            {
                instance = new SqlNCharExpr(this._tokenList.lookup(0).value);
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).type == 2)
            {
                instance = new SqlVarRefExpr(this._tokenList.lookup(0).value);
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).Equals(Token.CaseToken))
            {
                SqlCaseExpr expr3 = new SqlCaseExpr();
                expr3.setExprWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                if (!this._tokenList.lookup(0).Equals(Token.WhenToken))
                {
                    expr3.valueExpr = this.expr();
                }
                string whenWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.WhenToken);
                SqlExpr conditionExpr = this.expr();
                string thenWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.ThenToken);
                SqlExpr valueExpr = this.expr();
                SqlCaseItem item = new SqlCaseItem(conditionExpr, valueExpr);
                item.setWhenWord(whenWord);
                item.setThenWord(thenWord);
                expr3.itemList.Add(item);
                while (this._tokenList.lookup(0).Equals(Token.WhenToken))
                {
                    whenWord = this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match();
                    conditionExpr = this.expr();
                    thenWord = this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match(Token.ThenToken);
                    valueExpr = this.expr();
                    item = new SqlCaseItem(conditionExpr, valueExpr);
                    item.setWhenWord(whenWord);
                    item.setThenWord(thenWord);
                    expr3.itemList.Add(item);
                }
                if (this._tokenList.lookup(0).Equals(Token.ElseToken))
                {
                    string elseWord = this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match();
                    expr3.elseExpr = this.expr();
                    expr3.setElseWord(elseWord);
                }
                string endWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.EndToken);
                expr3.setEndWord(endWord);
                instance = expr3;
            }
            else if (this._tokenList.lookup(0).Equals(Token.ExistsToken))
            {
                string exprWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                this._tokenList.match(Token.OpenBraceToken);
                SqlSelectBase subQuery = new SelectParser(this._tokenList).select();
                this._tokenList.match(Token.CloseBraceToken);
                instance = new SqlExistsExpr(subQuery);
                instance.setExprWord(exprWord);
            }
            else if (this._tokenList.lookup(0).Equals(Token.NotToken))
            {
                string str10 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.ExistsToken))
                {
                    str10 = str10 + " " + this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match();
                    this._tokenList.match(Token.OpenBraceToken);
                    SqlSelectBase base3 = new SelectParser(this._tokenList).select();
                    this._tokenList.match(Token.CloseBraceToken);
                    instance = new SqlExistsExpr(base3, true);
                }
                else
                {
                    if (!this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        throw new ParserException("not support token" + this._tokenList.lookup(0), 0, 0);
                    }
                    this._tokenList.match(Token.OpenBraceToken);
                    instance = new SqlNotExpr(this.expr());
                    this._tokenList.match(Token.CloseBraceToken);
                }
                instance.setExprWord(str10);
            }
            else if (this._tokenList.lookup(0).Equals(Token.AllToken))
            {
                string orgWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                this._tokenList.match(Token.OpenBraceToken);
                SqlSelectBase base4 = new SelectParser(this._tokenList).select();
                this._tokenList.match(Token.CloseBraceToken);
                instance = new SqlAllExpr(orgWord, base4);
            }
            else if (this._tokenList.lookup(0).Equals(Token.LeftToken))
            {
                instance = new SqlIdentifierExpr("LEFT", this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).Equals(Token.RightToken))
            {
                instance = new SqlIdentifierExpr("RIGHT", this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).Equals(Token.ConvertToken))
            {
                instance = new SqlIdentifierExpr("CONVERT", this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).Equals(Token.NullIfToken))
            {
                instance = new SqlIdentifierExpr("NULLIF", this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
            }
            else if (this._tokenList.lookup(0).Equals(Token.AnyToken))
            {
                string str12 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                this._tokenList.match(Token.OpenBraceToken);
                SqlSelectBase base5 = new SelectParser(this._tokenList).select();
                this._tokenList.match(Token.CloseBraceToken);
                instance = new SqlAnyExpr(str12, base5);
            }
            else if (this._tokenList.lookup(0).Equals(Token.SomeToken))
            {
                string someWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                this._tokenList.match(Token.OpenBraceToken);
                SqlSelectBase base6 = new SelectParser(this._tokenList).select();
                this._tokenList.match(Token.CloseBraceToken);
                instance = new SqlSomeExpr(someWord, base6);
            }
            else if (this._tokenList.lookup(0).Equals(Token.NullToken))
            {
                string str14 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                instance = SqlNullExpr.instance;
                instance.setExprWord(str14);
            }
            else if (this._tokenList.lookup(0).Equals(Token.EmptyToken))
            {
                string str15 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                instance = SqlEmptyExpr.instance;
                instance.setExprWord(str15);
            }
            else
            {
                if (this._tokenList.lookup(0).Equals(Token.OpenCurlyBraceToken) || (((this._tokenList.lookup(0).Equals(Token.DateToken) || this._tokenList.lookup(0).Equals(Token.TimeToken)) || this._tokenList.lookup(0).Equals(Token.TimeStampToken)) && (this._tokenList.lookup(1).type == 6)))
                {
                    return this.parseDatetimeExpr();
                }
                if (this._tokenList.lookup(0).Equals(Token.SelectToken))
                {
                    SelectParser parser6 = new SelectParser(this._tokenList);
                    QueryExpr expr6 = new QueryExpr(parser6.select());
                    instance = expr6;
                }
                else if (this._tokenList.lookup(0).Equals(Token.PriorToken))
                {
                    this._tokenList.match();
                    instance = new SqlPriorIdentifierExpr(this._tokenList.lookup(0).value);
                    this._tokenList.match(1);
                }
                else if (this._tokenList.lookup(0).Equals(Token.UserToken) || this._tokenList.lookup(0).Equals(Token.IndexToken))
                {
                    instance = new SqlIdentifierExpr("\"" + this._tokenList.lookup(0).value + "\"", this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match();
                }
                else
                {
                    if ((!this._tokenList.lookup(0).Equals(Token.DateToken) && !this._tokenList.lookup(0).Equals(Token.TimeToken)) && (!this._tokenList.lookup(0).Equals(Token.TimeStampToken) && !this._tokenList.lookup(0).Equals(Token.GroupToken)))
                    {
                        throw new ParserException(string.Concat(new object[] { "ERROR. Token's value : ", this._tokenList.lookup(0).value, ", token's type : ", this._tokenList.lookup(0).Typename(), ", line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                    }
                    instance = new SqlIdentifierExpr("\"" + this._tokenList.lookup(0).value + "\"", this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match();
                }
            }
            return this.primaryRest(instance);
        }

        public SqlExpr primaryRest(SqlExpr expr)
        {
            if (expr == null)
            {
                throw new ArgumentException("expr");
            }
            if (this._tokenList.lookup(0).Equals(Token.PeriodToken))
            {
                SqlExpr expr2;
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.MulToken))
                {
                    expr2 = new SqlAllColumnExpr();
                    this._tokenList.match();
                    expr = new SqlBinaryOpExpr(expr, 20, expr2);
                }
                else if (this._tokenList.lookup(1).Equals(Token.OpenBraceToken))
                {
                    string methodName = this._tokenList.lookup(0).value;
                    this._tokenList.match();
                    this._tokenList.match(Token.OpenBraceToken);
                    SqlMethodInvokeExpr expr3 = new SqlMethodInvokeExpr(methodName);
                    if (this._tokenList.lookup(0).Equals(Token.CloseBraceToken))
                    {
                        this._tokenList.match();
                    }
                    else
                    {
                        this.exprList(expr3.parameters);
                        this._tokenList.match(Token.CloseBraceToken);
                    }
                    expr = expr3;
                }
                else
                {
                    expr2 = new SqlIdentifierExpr(this._tokenList.lookup(0).value, this._tokenList.lookup(0).GetOrgValue());
                    this._tokenList.match();
                    expr = new SqlBinaryOpExpr(expr, 20, expr2);
                }
                expr = this.primaryRest(expr);
                if (this._tokenList.lookup(0).Equals(Token.InToken))
                {
                    expr = this.inRest(expr);
                }
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
            {
                if (expr.type != 4)
                {
                    throw new ParserException("not support token:" + this._tokenList.lookup(0), 0, 0);
                }
                SqlIdentifierExpr expr4 = (SqlIdentifierExpr)expr;
                string word = expr4.value;
                this._tokenList.match();
                if (this._tokenList.lexer.Keywords.IsAggreateFunction(word) || this._tokenList.lexer.Keywords.IsRankingFunction(word))
                {
                    SqlAggregateExpr expr5;
                    if (this._tokenList.lookup(0).Equals(Token.AllToken))
                    {
                        expr5 = new SqlAggregateExpr(word, 1, this._tokenList.lookup(0).GetOrgValue());
                        this._tokenList.match();
                    }
                    else if (this._tokenList.lookup(0).Equals(Token.DistinctToken))
                    {
                        expr5 = new SqlAggregateExpr(word, 0, this._tokenList.lookup(0).GetOrgValue());
                        this._tokenList.match();
                    }
                    else
                    {
                        expr5 = new SqlAggregateExpr(word, 1);
                        expr5.setOptionWord("");
                    }
                    if (!this._tokenList.lookup(0).Equals(Token.CloseBraceToken))
                    {
                        this.exprList(expr5.paramList);
                    }
                    this._tokenList.match(Token.CloseBraceToken);
                    if (this._tokenList.lookup(0).Equals(Token.OverToken))
                    {
                        expr5.overExpr = (SqlOverExpr)this.OverExpr();
                    }
                    return expr5;
                }
                SqlMethodInvokeExpr expr6 = new SqlMethodInvokeExpr(word);
                if (!this._tokenList.lookup(0).Equals(Token.CloseBraceToken))
                {
                    bool flag = false;
                    do
                    {
                        if (flag)
                        {
                            this._tokenList.match();
                        }
                        SqlExpr expr7 = this.expr();
                        if (expr7 is SqlMethodInvokeExpr)
                        {
                            SqlMethodInvokeExpr expr8 = (SqlMethodInvokeExpr)expr7;
                            if (word.EqualsIgnoreCase("CONVERT") && ((expr8.methodName.EqualsIgnoreCase("VARCHAR") || expr8.methodName.EqualsIgnoreCase("CHAR")) || (expr8.methodName.EqualsIgnoreCase("NVARCHAR") || expr8.methodName.EqualsIgnoreCase("NCHAR"))))
                            {
                                SqlConvertTypeExpr expr9 = new SqlConvertTypeExpr(expr8.methodName);
                                expr9.setLen(int.Parse(((SqlIntExpr)expr8.parameters[0]).text));
                                expr7 = expr9;
                            }
                        }
                        expr6.parameters.Add(expr7);
                        flag = true;
                    }
                    while (this._tokenList.lookup(0).Equals(Token.CommaToken));
                }
                this._tokenList.match(Token.CloseBraceToken);
                expr = this.primaryRest(expr6);
                if (this._tokenList.lookup(0).Equals(Token.InToken))
                {
                    expr = this.inRest(expr);
                }
            }
            return expr;
        }

        public SqlExpr relational()
        {
            SqlExpr expr = this.additive();
            return this.relationalRest(expr);
        }

        public SqlExpr relationalRest(SqlExpr expr)
        {
            if (this._tokenList.lookup(0).Equals(Token.LessThanToken))
            {
                this._tokenList.match();
                SqlExpr expr2 = this.additive();
                expr2 = this.relationalRest(expr2);
                expr = new SqlBinaryOpExpr(expr, 14, expr2);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.LessThanOrEqualToken))
            {
                this._tokenList.match();
                SqlExpr expr3 = this.additive();
                expr3 = this.relationalRest(expr3);
                expr = new SqlBinaryOpExpr(expr, 15, expr3);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.GreaterThanToken))
            {
                this._tokenList.match();
                SqlExpr expr4 = this.additive();
                expr4 = this.relationalRest(expr4);
                expr = new SqlBinaryOpExpr(expr, 11, expr4);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.GreaterThanOrEqualToken))
            {
                this._tokenList.match();
                SqlExpr expr5 = this.additive();
                expr5 = this.relationalRest(expr5);
                expr = new SqlBinaryOpExpr(expr, 12, expr5);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.NotLessThanToken))
            {
                this._tokenList.match();
                SqlExpr expr6 = this.additive();
                expr6 = this.relationalRest(expr6);
                expr = new SqlBinaryOpExpr(expr, 0x18, expr6);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.NotGreaterThanToken))
            {
                this._tokenList.match();
                SqlExpr expr7 = this.additive();
                expr7 = this.relationalRest(expr7);
                expr = new SqlBinaryOpExpr(expr, 0x19, expr7);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.LessThanOrGreaterThanToken))
            {
                this._tokenList.match();
                SqlExpr expr8 = this.additive();
                expr8 = this.relationalRest(expr8);
                expr = new SqlBinaryOpExpr(expr, 0x10, expr8);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.LikeToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                SqlExpr expr9 = this.additive();
                expr9 = this.relationalRest(expr9);
                if (((expr is SqlCharExpr) || (expr is SqlNCharExpr)) || (expr is SqlVarRefExpr))
                {
                    throw new ParserException("invaild like Expr.");
                }
                if (expr9 is SqlBinaryOpExpr)
                {
                    SqlBinaryOpExpr expr10 = (SqlBinaryOpExpr)expr9;
                    if ((!(expr10.left is SqlCharExpr) && !(expr10.left is SqlNCharExpr)) || (!(expr10.right is SqlCharExpr) && !(expr10.right is SqlNCharExpr)))
                    {
                        throw new ParserException("invaild like Expr.");
                    }
                }
                expr = new SqlBinaryOpExpr(expr, 0x12, expr9);
                expr.setExprWord(orgValue);
                if (this._tokenList.lookup(0).Equals(Token.EscapeToken))
                {
                    string exprWord = this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match();
                    expr9 = this.expr();
                    expr = new SqlBinaryOpExpr(expr, 0x2b, expr9);
                    expr.setExprWord(exprWord);
                }
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.NotToken))
            {
                int num;
                string str3 = this._tokenList.lookup(0).GetOrgValue();
                if (this._tokenList.lookup(1).Equals(Token.LikeToken))
                {
                    str3 = str3 + " " + this._tokenList.lookup(1).GetOrgValue();
                    this._tokenList.match();
                    this._tokenList.match();
                    num = 40;
                }
                else
                {
                    if (this._tokenList.lookup(1).Equals(Token.InToken) || this._tokenList.lookup(1).value.EqualsIgnoreCase(Token.InnerToken.value))
                    {
                        str3 = str3 + " " + this._tokenList.lookup(1).GetOrgValue();
                        this._tokenList.match();
                        this._tokenList.match();
                        this._tokenList.match(Token.OpenBraceToken);
                        if (this._tokenList.lookup(0).value.EqualsIgnoreCase(Token.SelectToken.value))
                        {
                            this._tokenList.lookup(0).type = 3;
                            KeyWord keywords = null;
                            if (this.inSubQueryKeyword != null)
                            {
                                keywords = this._tokenList.lexer.Keywords;
                                this._tokenList.lexer.Keywords = this.inSubQueryKeyword;
                            }
                            SqlSelectBase subQuery = new SelectParser(this._tokenList).select();
                            SqlInSubQueryExpr expr11 = new SqlInSubQueryExpr(expr, subQuery, true);
                            expr = expr11;
                            if (keywords != null)
                            {
                                this._tokenList.lexer.Keywords = keywords;
                            }
                        }
                        else
                        {
                            SqlInListExpr expr12 = new SqlInListExpr(expr, true);
                            this.exprList(expr12.targetList);
                            expr = expr12;
                        }
                        expr.setExprWord(str3);
                        this._tokenList.match(Token.CloseBraceToken);
                        expr = this.relationalRest(expr);
                        return expr;
                    }
                    if (this._tokenList.lookup(1).Equals(Token.BetweenToken))
                    {
                        str3 = str3 + " " + this._tokenList.lookup(1).GetOrgValue();
                        this._tokenList.match();
                        this._tokenList.match();
                        SqlExpr beginExpr = this.primary();
                        string andWord = this._tokenList.lookup(0).GetOrgValue();
                        this._tokenList.match(Token.AndToken);
                        SqlExpr endExpr = this.primary();
                        expr = new SqlBetweenExpr(expr, beginExpr, endExpr, true);
                        expr.setExprWord(str3);
                        ((SqlBetweenExpr)expr).setAndWord(andWord);
                        expr = this.relationalRest(expr);
                        return expr;
                    }
                    if (!this._tokenList.lookup(1).Equals(Token.NullToken))
                    {
                        throw new ParserException("not support token:" + this._tokenList.lookup(1));
                    }
                    str3 = str3 + " " + this._tokenList.lookup(1).GetOrgValue();
                    expr.setExprWord(str3);
                    return expr;
                }
                SqlExpr expr15 = this.additive();
                expr15 = this.relationalRest(expr15);
                expr = new SqlBinaryOpExpr(expr, num, expr15);
                expr.setExprWord(str3);
                if (((num == 0x12) || (num == 40)) && this._tokenList.lookup(0).Equals(Token.EscapeToken))
                {
                    this._tokenList.match();
                    expr15 = this.expr();
                    expr = new SqlBinaryOpExpr(expr, 0x2b, expr15);
                    expr.setExprWord(str3);
                }
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.BetweenToken))
            {
                string str5 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                SqlExpr expr16 = this.primary();
                string str6 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.AndToken);
                SqlExpr expr17 = this.primary();
                expr = new SqlBetweenExpr(expr, expr16, expr17);
                ((SqlBetweenExpr)expr).setExprWord(str5);
                ((SqlBetweenExpr)expr).setAndWord(str6);
                return expr;
            }
            if (this._tokenList.lookup(0).Equals(Token.IsToken))
            {
                int num2;
                string str7 = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.NotToken))
                {
                    str7 = str7 + " " + this._tokenList.lookup(0).GetOrgValue();
                    this._tokenList.match();
                    num2 = 0x29;
                }
                else
                {
                    num2 = 13;
                }
                if (this._tokenList.lookup(0).Equals(Token.EmptyToken))
                {
                    SqlEmptyExpr expr18 = new SqlEmptyExpr();
                    expr18.setExprWord(this._tokenList.lookup(0).GetOrgValue());
                    expr = new SqlBinaryOpExpr(expr, num2, expr18);
                    expr.setExprWord(str7);
                    this._tokenList.match();
                    return expr;
                }
                SqlNullExpr right = new SqlNullExpr();
                right.setExprWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(Token.NullToken);
                expr = new SqlBinaryOpExpr(expr, num2, right);
                expr.setExprWord(str7);
            }
            return expr;
        }

        public void setInSubQueryKeyword(KeyWord inSubQueryKeyword)
        {
            this.inSubQueryKeyword = inSubQueryKeyword;
        }

        public SqlExpr unary()
        {
            if (this._tokenList.lookup(0).Equals(Token.MinusToken))
            {
                if (this._tokenList.lookup(1).type == 8)
                {
                    SqlIntExpr expr = new SqlIntExpr("-" + this._tokenList.lookup(1).value);
                    this._tokenList.match();
                    this._tokenList.match();
                    return expr;
                }
                if ((this._tokenList.lookup(1).type == 11) || (this._tokenList.lookup(1).type == 10))
                {
                    SqlDoubleExpr expr2 = new SqlDoubleExpr("-" + this._tokenList.lookup(1).value);
                    this._tokenList.match();
                    this._tokenList.match();
                    return expr2;
                }
                if (this._tokenList.lookup(1).type == 15)
                {
                    SqlLongExpr expr3 = new SqlLongExpr("-" + this._tokenList.lookup(1).value);
                    this._tokenList.match();
                    this._tokenList.match();
                    return expr3;
                }
            }
            return this.primary();
        }
    }






}
