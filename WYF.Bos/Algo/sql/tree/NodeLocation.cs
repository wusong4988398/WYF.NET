using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public class NodeLocation
    {
        private  int line;

        private  int charPositionInLine;

        private  String text;
  
       public NodeLocation(int line, int charPositionInLine):this(line, charPositionInLine, null)
        {
            
        }

        public NodeLocation(int line, int charPositionInLine, String text)
        {
            this.line = line;
            this.charPositionInLine = charPositionInLine;
            this.text = text;
        }
    }
}
