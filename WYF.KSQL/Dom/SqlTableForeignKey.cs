using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public class SqlTableForeignKey : SqlTableConstraint
    {
        // Fields
        public IList columnList;
        private string foreignWord;
        public IList refColumnList;
        private string referencesWord;
        public string refTableName;

        // Methods
        public SqlTableForeignKey()
        {
            this.columnList = new ArrayList();
            this.refColumnList = new ArrayList();
        }

        public SqlTableForeignKey(string name) : base(name)
        {
            this.columnList = new ArrayList();
            this.refColumnList = new ArrayList();
            this.setForeignWord("FOREIGN KEY");
            this.setReferencesWord("REFERENCES");
        }

        public override object Clone()
        {
            SqlTableForeignKey key = new SqlTableForeignKey
            {
                name = base.name
            };
            int num = 0;
            int count = this.refColumnList.Count;
            while (num < count)
            {
                string str = (string)((ArrayList)this.refColumnList)[num];
                this.refColumnList.GetEnumerator().MoveNext();
                ((ArrayList)key.refColumnList).Add(str);
                num++;
            }
            key.setConstraintWord(base.getConstraintWord());
            key.setForeignWord(this.getForeignWord());
            key.setReferencesWord(this.getForeignWord());
            return key;
        }

        public string getForeignWord()
        {
            return this.foreignWord;
        }

        public string getReferencesWord()
        {
            return this.referencesWord;
        }

        public void setForeignWord(string foreignWord)
        {
            this.foreignWord = foreignWord;
        }

        public void setReferencesWord(string referencesWord)
        {
            this.referencesWord = referencesWord;
        }
    }






}
