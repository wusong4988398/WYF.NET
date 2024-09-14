using System;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public sealed class StringReader
    {
        // Fields
        private string _data;
        private int _length;
        private int _ptr;

        // Methods
        public StringReader(string data)
        {
            this._data = data;
            this._ptr = 0;
            this._length = this._data.Length;
        }

        public bool eos()
        {
            return (this._ptr >= this._length);
        }

        public char lookup(int i)
        {
            return this._data.CharAt((this._ptr + i));
        }

        public char next()
        {
            if (this.eos())
            {
                throw new ParserException("warning : FileReader.GetNext : Read char over eos.", 0, 0);
            }
            return this._data[this._ptr++];
        }

        public char peek()
        {
            if (this.eos())
            {
                return ' ';
            }
            return this._data.CharAt(this._ptr);
        }

        public int ptr()
        {
            return this._ptr;
        }

        public void skip()
        {
            this._ptr++;
        }

        public void unget()
        {
            this._ptr--;
            if (this._ptr < 0)
            {
                throw new ParserException("ungetted first char", 0, 0);
            }
        }
    }






}
