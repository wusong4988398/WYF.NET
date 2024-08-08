using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql.parser
{
    public class StringReader
    {
        public  string Data { get; private set; }
        public int Ptr { get; private set; }

        public StringReader( string data)
        {
            this.Data = null;
            this.Ptr = 0;
            this.Data = data;
        }

        public  char Next()  
        {
        if (this.Eos()) {
            throw new Exception("warning : FileReader.GetNext : Read char over eos.");
        }
        return this.Data.ElementAt(this.Ptr++);
        }

        public  void Skip() 
        {
           ++this.Ptr;
        }

        public  char Peek() 
        {
        if (this.Eos()) {
            return ' ';
        }
            return this.Data.ElementAt(this.Ptr);

        }

        public  void Unget()
        {
          --this.Ptr;
        if (this.Ptr < 0) {
            throw new Exception("ungetted first char");
        }
        }

        public  bool Eos() 
        {
          return this.Ptr >= this.Data.Length;
    }

    public  char Lookup( int i)  
    {
            
        return this.Data.ElementAt(this.Ptr + i);
    }


}
}
