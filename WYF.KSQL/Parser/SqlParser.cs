using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Parser
{
    public class SqlParser : SqlParserBase
    {
        
        private TokenList _tokList;
        private bool throwExWhenNameTooLong;

      
        public SqlParser(Lexer lexer)
        {
            this._tokList = new TokenList(lexer);
            this._tokList.setSqlParser(this);
        }

        public SqlParser(string text)
        {
            Lexer l = new Lexer(text);
            this._tokList = new TokenList(l);
            this._tokList.setSqlParser(this);
        }

        public bool isThrowExWhenNameTooLong()
        {
            return this.throwExWhenNameTooLong;
        }

        public IList parseStmtList()
        {
            IList stmtCol = new ArrayList();
            SqlStmtParser parser = new SqlStmtParser(this._tokList);
            if (this._tokList.lookup(0).type != 12)
            {
                parser.stmtList(stmtCol, Token.EOFToken);
            }
            return stmtCol;
        }

        public void setThrowExWhenNameTooLong(bool throwExWhenNameTooLong)
        {
            this.throwExWhenNameTooLong = throwExWhenNameTooLong;
        }
    }






}
