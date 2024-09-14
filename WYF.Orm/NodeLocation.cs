using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.SqlParser
{
    public class NodeLocation
    {
        private readonly string text;
        private readonly int begin;
        private readonly int length;
        public NodeLocation(string text, int begin, int length)
        {
            this.text = text;
            this.begin = begin;
            this.length = length;
        }
        public NodeLocation(string text)
        {
            this.text = text;
            this.begin = 0;
            this.length = text.Length;
        }
        public string Text { get { return this.text.Substring(this.begin, this.length); } }

        public override string ToString()
        {
            return Text;
        }
    }
}
