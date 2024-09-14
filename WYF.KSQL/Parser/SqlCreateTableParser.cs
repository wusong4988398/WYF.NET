
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Dom.Stmt;
using WYF.KSQL.Exception;
using WYF.KSQL.Util;

namespace WYF.KSQL.Parser
{
    public class SqlCreateTableParser : SqlParserBase
    {
      
        private TokenList _tokenList;
        //private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Methods
        public SqlCreateTableParser(TokenList tokenList)
        {
            this._tokenList = tokenList;
        }

        public SqlCreateTableParser(string sql)
        {
            Lexer l = new Lexer(sql);
            this._tokenList = new TokenList(l);
        }

        public SqlCreateTableStmt parse()
        {
            this._tokenList.match(Token.TableToken);
            string tablename = this._tokenList.lookup(0).value;
            this._tokenList.match(1);
            if (this._tokenList.lookup(0).Equals(Token.PeriodToken))
            {
                tablename = tablename + ".";
                this._tokenList.match();
                string str2 = this._tokenList.lookup(0).value;
                tablename = tablename + str2;
                this._tokenList.match(1);
                if (!this._tokenList.lookup(0).Equals(Token.PeriodToken))
                {
                    if (str2.Length > 30)
                    {
                        string message = string.Concat(new object[] { "table name is too long. max length is 30, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn });
                        //logger.Debug(message);
                        if ((!UUTN.isTempTable(str2) && !UUTN.isGlobalTempTable(str2)) && this._tokenList.getSqlParser().isThrowExWhenNameTooLong())
                        {
                            throw new ParserException(message, this._tokenList.lookup(0).beginLine, this._tokenList.lookup(0).beginColumn);
                        }
                    }
                }
                else
                {
                    tablename = tablename + ".";
                    this._tokenList.match();
                    str2 = this._tokenList.lookup(0).value;
                    tablename = tablename + str2;
                    this._tokenList.match(1);
                    if (str2.Length > 30)
                    {
                        string str3 = string.Concat(new object[] { "table name is too long. max length is 30, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn });
                        //logger.Debug(str3);
                        if ((!UUTN.isTempTable(str2) && !UUTN.isGlobalTempTable(str2)) && this._tokenList.getSqlParser().isThrowExWhenNameTooLong())
                        {
                            throw new ParserException(str3, this._tokenList.lookup(0).beginLine, this._tokenList.lookup(0).beginColumn);
                        }
                    }
                }
            }
            else if (tablename.Length > 30)
            {
                string str5 = string.Concat(new object[] { "table name is too long. max length is 30, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn });
                //logger.Debug(str5);
                if ((!UUTN.isTempTable(tablename) && !UUTN.isGlobalTempTable(tablename)) && this._tokenList.getSqlParser().isThrowExWhenNameTooLong())
                {
                    throw new ParserException(str5, this._tokenList.lookup(0).beginLine, this._tokenList.lookup(0).beginColumn);
                }
            }
            this._tokenList.match(Token.OpenBraceToken);
            SqlCreateTableStmt stmt = new SqlCreateTableStmt(tablename);
            this.parseColumnDefList(stmt.columnList);
            this.tableConstraintList(stmt.constraintList);
            this._tokenList.match(Token.CloseBraceToken);
            this.parseTableSpace(stmt);
            return stmt;
        }

        public SqlColumnDef parseColumnDef()
        {
            string str = this._tokenList.lookup(0).value;
            this._tokenList.match(1);
            SqlColumnDef def = new SqlColumnDef
            {
                name = str
            };
            if (this._tokenList.lookup(0).Equals(Token.AsToken))
            {
                def.IsComputedColumn = true;
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    this._tokenList.match(Token.OpenBraceToken);
                }
                def.ComputedColumnExpr = new SqlExprParser(this._tokenList).expr();
                this._tokenList.match(Token.CloseBraceToken);
                if (this._tokenList.lookup(0).Equals(Token.PERSISTEDToken))
                {
                    def.ComputedColumnisPersisted = true;
                    this._tokenList.match(Token.PERSISTEDToken);
                }
                return def;
            }
            string str2 = this._tokenList.lookup(0).value.ToUpper();
            string orgValue = this._tokenList.lookup(0).GetOrgValue();
            this._tokenList.match();
            def.dataType = str2;
            if (this._tokenList.lookup(0).Equals(Token.AsToken))
            {
                def.IsComputedColumn = true;
                this._tokenList.match();
                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    this._tokenList.match(Token.OpenBraceToken);
                }
                def.ComputedColumnExpr = new SqlExprParser(this._tokenList).expr();
                this._tokenList.match(Token.CloseBraceToken);
                return def;
            }
            def.setOrgDataTypeWord(orgValue);
            if (str2.Equals("INT") || str2.Equals("INTEGER"))
            {
                if (this._tokenList.lookup(0).Equals(Token.IdentityToken))
                {
                    this._tokenList.match(3);
                    this._tokenList.match(5);
                    this._tokenList.match(8);
                    this._tokenList.match(5);
                    this._tokenList.match(8);
                    this._tokenList.match(5);
                    def.dataType = "INT";
                    def.autoIncrement = true;
                }
                else
                {
                    def.dataType = "INT";
                }
                def.length = 4;
            }
            else if (str2.EqualsIgnoreCase("BIGINT"))
            {
                def.length = 8;
            }
            else if (str2.Equals("SMALLINT"))
            {
                def.length = 2;
            }
            else if ((str2.Equals("DECIMAL") || str2.Equals("NUMBER")) || str2.Equals("NUMERIC"))
            {
                def.dataType = "DECIMAL";
                def.length = 9;
                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    this._tokenList.match();
                    def.precision = int.Parse(this._tokenList.lookup(0).value);
                    this._tokenList.match(8);
                    if (this._tokenList.lookup(0).Equals(Token.CommaToken))
                    {
                        this._tokenList.match();
                        def.scale = int.Parse(this._tokenList.lookup(0).value);
                        this._tokenList.match(8);
                    }
                    this._tokenList.match(Token.CloseBraceToken);
                }
            }
            else if (str2.Equals("CHAR"))
            {
                this._tokenList.match(Token.OpenBraceToken);
                def.length = int.Parse(this._tokenList.lookup(0).value);
                if (def.length > 0xfe)
                {
                    throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'char' is 254, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                }
                this._tokenList.match(8);
                this._tokenList.match(Token.CloseBraceToken);
            }
            else if (str2.Equals("NCHAR"))
            {
                this._tokenList.match(Token.OpenBraceToken);
                def.length = int.Parse(this._tokenList.lookup(0).value);
                if (def.length > 0xfe)
                {
                    throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'nchar' is 254, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                }
                this._tokenList.match(8);
                this._tokenList.match(Token.CloseBraceToken);
            }
            else if (str2.Equals("VARCHAR"))
            {
                this._tokenList.match(Token.OpenBraceToken);
                def.length = int.Parse(this._tokenList.lookup(0).value);
                if (def.length > 0xfa0)
                {
                    throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'varchar' is 4000, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                }
                this._tokenList.match(8);
                this._tokenList.match(Token.CloseBraceToken);
            }
            else if (str2.Equals("NVARCHAR"))
            {
                this._tokenList.match(Token.OpenBraceToken);
                def.length = int.Parse(this._tokenList.lookup(0).value);
                if (def.length > 0x7d0)
                {
                    throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'nvarchar' is 2000, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                }
                this._tokenList.match(8);
                this._tokenList.match(Token.CloseBraceToken);
            }
            else if (!str2.Equals("DATETIME"))
            {
                if (str2.Equals("TIMESTAMP"))
                {
                    def.dataType = "DATETIME";
                }
                else if (str2.Equals("BINARY"))
                {
                    this._tokenList.match(Token.OpenBraceToken);
                    def.length = int.Parse(this._tokenList.lookup(0).value);
                    if (def.length > 0x7d0)
                    {
                        throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'binary' is 2000, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                    }
                    this._tokenList.match(8);
                    this._tokenList.match(Token.CloseBraceToken);
                }
                else if (str2.Equals("VARBINARY"))
                {
                    this._tokenList.match(Token.OpenBraceToken);
                    def.length = int.Parse(this._tokenList.lookup(0).value);
                    if (def.length > 0x7d0)
                    {
                        throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'varbinary' is 2000, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                    }
                    this._tokenList.match(8);
                    this._tokenList.match(Token.CloseBraceToken);
                }
                else if (str2.Equals("BLOG"))
                {
                    if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        this._tokenList.match(Token.OpenBraceToken);
                        def.length = int.Parse(this._tokenList.lookup(0).value);
                        this._tokenList.match(8);
                        this._tokenList.match(Token.CloseBraceToken);
                    }
                }
                else if (str2.Equals("CLOB"))
                {
                    if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        this._tokenList.match(Token.OpenBraceToken);
                        def.length = int.Parse(this._tokenList.lookup(0).value);
                        if (def.length > 0x40000000)
                        {
                            throw new ParserException(string.Concat(new object[] { "the max lenght of the data type 'CLOB' is 1073741824, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                        }
                        this._tokenList.match(8);
                        this._tokenList.match(Token.CloseBraceToken);
                    }
                }
                else if (str2.Equals("NCLOB"))
                {
                    if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        this._tokenList.match(Token.OpenBraceToken);
                        def.length = int.Parse(this._tokenList.lookup(0).value);
                        this._tokenList.match(8);
                        this._tokenList.match(Token.CloseBraceToken);
                    }
                }
                else if (str2.Equals("SMALLINT"))
                {
                    if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        this._tokenList.match(Token.OpenBraceToken);
                        def.length = 1;
                        this._tokenList.match(8);
                        this._tokenList.match(Token.CloseBraceToken);
                    }
                }
                else if (str2.Equals("BLOB"))
                {
                    if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        this._tokenList.match(Token.OpenBraceToken);
                        def.length = int.Parse(this._tokenList.lookup(0).value);
                        this._tokenList.match(8);
                        this._tokenList.match(Token.CloseBraceToken);
                    }
                }
                else
                {
                    if (!str2.Equals("XMLTYPE"))
                    {
                        throw new ParserException(string.Concat(new object[] { "NOT SUPPORT DATA TYPE '", str2, "', at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                    }
                    if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                    {
                        this._tokenList.match(Token.OpenBraceToken);
                        def.length = int.Parse(this._tokenList.lookup(0).value);
                        this._tokenList.match(8);
                        this._tokenList.match(Token.CloseBraceToken);
                    }
                }
            }
            if (this._tokenList.lookup(0).Equals(Token.CollateToken))
            {
                def.setCollateWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(Token.CollateToken);
                def.collateName = this._tokenList.lookup(0).value;
                this._tokenList.match();
            }
            if (this._tokenList.lookup(0).Equals(Token.NotToken))
            {
                string nullWord = this._tokenList.lookup(0).GetOrgValue() + " " + this._tokenList.lookup(1).GetOrgValue();
                def.setNullWord(nullWord);
                this._tokenList.match();
                this._tokenList.match(Token.NullToken);
                def.allowNull = false;
            }
            else if (this._tokenList.lookup(0).Equals(Token.NullToken))
            {
                def.setNullWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                def.allowNull = true;
            }
            if (this._tokenList.lookup(0).Equals(Token.DefaultToken))
            {
                def.setDefaultWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                bool flag = false;
                if (this._tokenList.lookup(0).Equals(Token.OpenBraceToken))
                {
                    this._tokenList.match(Token.OpenBraceToken);
                    flag = true;
                }
                def.defaultValueExpr = new SqlExprParser(this._tokenList).expr();
                if (flag)
                {
                    this._tokenList.match(Token.CloseBraceToken);
                }
            }
            if (this._tokenList.lookup(0).Equals(Token.NotToken))
            {
                string str5 = this._tokenList.lookup(0).GetOrgValue() + " " + this._tokenList.lookup(1).GetOrgValue();
                def.setNullWord(str5);
                this._tokenList.match();
                this._tokenList.match(Token.NullToken);
                def.allowNull = false;
            }
            else if (this._tokenList.lookup(0).Equals(Token.NullToken))
            {
                def.setNullWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                def.allowNull = true;
            }
            if (this._tokenList.lookup(0).Equals(Token.ConstraintToken))
            {
                def.setConstraintWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                def.containtName = this._tokenList.lookup(0).value;
                if ((def.containtName.Length > 30) && this._tokenList.getSqlParser().isThrowExWhenNameTooLong())
                {
                    throw new ParserException(string.Concat(new object[] { "constraintName name is too long. max length is 30, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                }
                this._tokenList.match(1);
            }
            if (this._tokenList.lookup(0).Equals(Token.NotToken))
            {
                string str6 = this._tokenList.lookup(0).GetOrgValue() + " " + this._tokenList.lookup(1).GetOrgValue();
                def.setNullWord(str6);
                this._tokenList.match();
                this._tokenList.match(Token.NullToken);
                def.allowNull = false;
            }
            else if (this._tokenList.lookup(0).Equals(Token.NullToken))
            {
                def.setNullWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                def.allowNull = true;
            }
            if (this._tokenList.lookup(0).value.EqualsIgnoreCase("AUTO_INCREMENT"))
            {
                def.autoIncrement = true;
                this._tokenList.match();
            }
            if ((def.containtName == null) && this._tokenList.lookup(0).Equals(Token.ConstraintToken))
            {
                def.setConstraintWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                def.containtName = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
            }
            if (this._tokenList.lookup(0).Equals(Token.PrimaryToken))
            {
                string primaryWord = this._tokenList.lookup(0).GetOrgValue() + " " + this._tokenList.lookup(1).GetOrgValue();
                def.setPrimaryWord(primaryWord);
                this._tokenList.match();
                this._tokenList.match(Token.KeyToken);
                def.isPrimaryKey = true;
                def.clustered = true;
                if (this._tokenList.lookup(0).Equals(Token.NonClusteredToken))
                {
                    def.setClusteredWord(this._tokenList.lookup(0).GetOrgValue());
                    def.clustered = false;
                    this._tokenList.match();
                }
            }
            if (this._tokenList.lookup(0).Equals(Token.UniqueToken))
            {
                def.setUniqueWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                def.isUnique = true;
                def.clustered = false;
                if (this._tokenList.lookup(0).Equals(Token.ClusteredToken))
                {
                    def.setClusteredWord(this._tokenList.lookup(0).GetOrgValue());
                    def.clustered = true;
                    this._tokenList.match();
                }
            }
            if (this._tokenList.lookup(0).Equals(Token.ForeignToken))
            {
                def.setForeignWord(this._tokenList.lookup(0).GetOrgValue() + " " + this._tokenList.lookup(1).GetOrgValue());
                this._tokenList.match();
                this._tokenList.match(Token.KeyToken);
                def.setReferencesWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(Token.ReferencesToken);
                def.refTableName = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                this._tokenList.match(Token.OpenBraceToken);
                def.refColumnName = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                this._tokenList.match(Token.CloseBraceToken);
            }
            else if (this._tokenList.lookup(0).Equals(Token.ReferencesToken))
            {
                def.setReferencesWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                def.refTableName = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                this._tokenList.match(Token.OpenBraceToken);
                def.refColumnName = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                this._tokenList.match(Token.CloseBraceToken);
            }
            if (this._tokenList.lookup(0).Equals(Token.CheckToken))
            {
                def.setCheckWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match();
                this._tokenList.match(Token.OpenBraceToken);
                def.checkExpr = new SqlExprParser(this._tokenList).expr();
                this._tokenList.match(Token.CloseBraceToken);
            }
            return def;
        }

        public void parseColumnDefList(IList columnList)
        {
            if (!this._tokenList.lookup(0).Equals(Token.CloseBraceToken))
            {
                SqlColumnDef def = this.parseColumnDef();
                columnList.Add(def);
                while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                {
                    this._tokenList.match();
                    if ((this._tokenList.lookup(0).Equals(Token.ConstraintToken) || this._tokenList.lookup(0).Equals(Token.PrimaryToken)) || ((this._tokenList.lookup(0).Equals(Token.UnionToken) || this._tokenList.lookup(0).Equals(Token.ForeignToken)) || this._tokenList.lookup(0).Equals(Token.CheckToken)))
                    {
                        return;
                    }
                    def = this.parseColumnDef();
                    columnList.Add(def);
                }
            }
        }

        private void parseTableSpace(SqlCreateTableStmt stmt)
        {
            if (Token.OnToken.Equals(this._tokenList.lookup(0)))
            {
                this._tokenList.match(Token.OnToken);
                stmt.tableSpace = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
            }
        }

        public SqlTableConstraint tableConstraint()
        {
            string name = null;
            string constraintWord = null;
            if (this._tokenList.lookup(0).Equals(Token.ConstraintToken))
            {
                constraintWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                name = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                if ((name.Length > 30) && this._tokenList.getSqlParser().isThrowExWhenNameTooLong())
                {
                    throw new ParserException(string.Concat(new object[] { "constraintName name is too long. max length is 30, at line ", this._tokenList.lookup(0).beginLine, ", column ", this._tokenList.lookup(0).beginColumn }));
                }
            }
            if (this._tokenList.lookup(0).Equals(Token.PrimaryToken))
            {
                string orgValue = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                orgValue = orgValue + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.KeyToken);
                SqlTablePrimaryKey key = new SqlTablePrimaryKey(name);
                key.setConstraintWord(constraintWord);
                key.setPrimaryKeyWord(orgValue);
                if (this._tokenList.lookup(0).Equals(Token.NonClusteredToken))
                {
                    key.setClusteredWord(this._tokenList.lookup(0).GetOrgValue());
                    key.clustered = false;
                    this._tokenList.match();
                }
                else if (this._tokenList.lookup(0).Equals(Token.ClusteredToken))
                {
                    key.setClusteredWord(this._tokenList.lookup(0).GetOrgValue());
                    key.clustered = true;
                    this._tokenList.match();
                }
                this._tokenList.match(Token.OpenBraceToken);
                string str4 = this._tokenList.lookup(0).value;
                key.columnList.Add(str4);
                this._tokenList.match(1);
                while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                {
                    this._tokenList.match();
                    str4 = this._tokenList.lookup(0).value;
                    key.columnList.Add(str4);
                    this._tokenList.match(1);
                }
                this._tokenList.match(Token.CloseBraceToken);
                return key;
            }
            if (this._tokenList.lookup(0).Equals(Token.UniqueToken))
            {
                string uniqueWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                SqlTableUnique unique = new SqlTableUnique(name);
                unique.setConstraintWord(constraintWord);
                unique.setUniqueWord(uniqueWord);
                if (this._tokenList.lookup(0).Equals(Token.NonClusteredToken))
                {
                    unique.setClusteredWord(this._tokenList.lookup(0).GetOrgValue());
                    unique.clustered = false;
                    this._tokenList.match();
                }
                else if (this._tokenList.lookup(0).Equals(Token.ClusteredToken))
                {
                    unique.setClusteredWord(this._tokenList.lookup(0).GetOrgValue());
                    unique.clustered = true;
                    this._tokenList.match();
                }
                this._tokenList.match(Token.OpenBraceToken);
                string str6 = this._tokenList.lookup(0).value;
                unique.columnList.Add(str6);
                this._tokenList.match(1);
                while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                {
                    this._tokenList.match();
                    str6 = this._tokenList.lookup(0).value;
                    unique.columnList.Add(str6);
                    this._tokenList.match(1);
                }
                this._tokenList.match(Token.CloseBraceToken);
                return unique;
            }
            if (this._tokenList.lookup(0).Equals(Token.ForeignToken))
            {
                string foreignWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                foreignWord = foreignWord + " " + this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match(Token.KeyToken);
                SqlTableForeignKey key2 = new SqlTableForeignKey(name);
                key2.setConstraintWord(constraintWord);
                key2.setForeignWord(foreignWord);
                this._tokenList.match(Token.OpenBraceToken);
                string str8 = this._tokenList.lookup(0).value;
                key2.columnList.Add(str8);
                this._tokenList.match(1);
                while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                {
                    this._tokenList.match();
                    str8 = this._tokenList.lookup(0).value;
                    key2.columnList.Add(str8);
                    this._tokenList.match(1);
                }
                this._tokenList.match(Token.CloseBraceToken);
                key2.setForeignWord(this._tokenList.lookup(0).GetOrgValue());
                this._tokenList.match(Token.ReferencesToken);
                key2.refTableName = this._tokenList.lookup(0).value;
                this._tokenList.match(1);
                this._tokenList.match(Token.OpenBraceToken);
                string str9 = this._tokenList.lookup(0).value;
                key2.refColumnList.Add(str9);
                this._tokenList.match(1);
                while (this._tokenList.lookup(0).Equals(Token.CommaToken))
                {
                    this._tokenList.match();
                    str9 = this._tokenList.lookup(0).value;
                    key2.refColumnList.Add(str9);
                    this._tokenList.match(1);
                }
                this._tokenList.match(Token.CloseBraceToken);
                return key2;
            }
            if (this._tokenList.lookup(0).Equals(Token.CheckToken))
            {
                string checkWord = this._tokenList.lookup(0).GetOrgValue();
                this._tokenList.match();
                SqlTableCheck check = new SqlTableCheck(name);
                check.setCheckWord(checkWord);
                check.setConstraintWord(constraintWord);
                this._tokenList.match(Token.OpenBraceToken);
                check.expr = new SqlExprParser(this._tokenList).expr();
                this._tokenList.match(Token.CloseBraceToken);
                return check;
            }
            return null;
        }

        private void tableConstraintList(IList constraintList)
        {
            while (this._tokenList.lookup(0).Equals(Token.ConstraintToken))
            {
                SqlTableConstraint constraint = this.tableConstraint();
                constraintList.Add(constraint);
                if (this._tokenList.lookup(0).Equals(Token.CommaToken) && this._tokenList.lookup(1).Equals(Token.ConstraintToken))
                {
                    this._tokenList.match();
                }
            }
        }
    }





}
