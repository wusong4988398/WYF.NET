using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Formater;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public abstract class SqlSelectBase : SqlObject
    {
        // Fields
        private ArrayList hints;
        private Hashtable optionMap;
        private ArrayList optionMapOrgWord;
        public ArrayList orderBy = new ArrayList();
        private string orderByWord;
        private string selectWord = "";
        public ArrayList subQueries = new ArrayList();

        // Methods
        public ArrayList getHints()
        {
            if (this.hints == null)
            {
                lock (this)
                {
                    if (this.hints == null)
                    {
                        this.hints = new ArrayList();
                    }
                }
            }
            return this.hints;
        }

        public Hashtable getOptionMap()
        {
            if (this.optionMap == null)
            {
                lock (this)
                {
                    if (this.optionMap == null)
                    {
                        this.optionMap = new Hashtable();
                    }
                }
            }
            return this.optionMap;
        }

        public Hashtable getOptionMapDirect()
        {
            return this.optionMap;
        }

        public ArrayList getOptionMapOrgWord()
        {
            if (this.optionMapOrgWord == null)
            {
                lock (this)
                {
                    if (this.optionMapOrgWord == null)
                    {
                        this.optionMapOrgWord = new ArrayList();
                    }
                }
            }
            return this.optionMapOrgWord;
        }

        public ArrayList getOptionMapOrgWordDirect()
        {
            return this.optionMapOrgWord;
        }

        public string getOrderByWord()
        {
            if ((this.orderByWord != null) && (this.orderByWord.Trim().Length != 0))
            {
                return this.orderByWord;
            }
            return "ORDER BY";
        }

        public string getSelectWord()
        {
            if ((this.selectWord != null) && (this.selectWord.Trim().Length != 0))
            {
                return this.selectWord;
            }
            return "SELECT";
        }

        public void output(StringBuilder buff)
        {
            this.output(buff, null);
        }

        public void output(StringBuilder buff, FormatOptions options)
        {
            try
            {
                new DrSQLFormater(buff).FormatSelectBase(this);
            }
            catch (FormaterException exception)
            {
                throw exception;
            }
        }

        public void setOrderByWord(string orderByWord)
        {
            this.orderByWord = orderByWord;
        }

        public void setSelectWord(string selectWord)
        {
            this.selectWord = selectWord;
        }

        public string toString()
        {
            StringBuilder buff = new StringBuilder();
            this.output(buff, null);
            return buff.ToString();
        }
    }





}
