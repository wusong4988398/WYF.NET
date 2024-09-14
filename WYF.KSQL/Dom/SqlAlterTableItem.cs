using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    [Serializable]
    public abstract class SqlAlterTableItem : SqlObject
    {
        // Fields
        private bool _isOpenBrace;
        private string itemWord;

        // Methods
        public virtual string getItemWord()
        {
            return this.itemWord;
        }

        public bool isOpenBrace()
        {
            return this._isOpenBrace;
        }

        public void setItemWord(string itemWord)
        {
            this.itemWord = itemWord;
        }

        public void setOpenBrace(bool _isOpenBrace)
        {
            this._isOpenBrace = _isOpenBrace;
        }
    }


   



}
