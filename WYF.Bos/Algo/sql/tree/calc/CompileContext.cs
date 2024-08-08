using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.calc
{
    public class CompileContext
    {
        public RowMeta rowMeta1;

        public RowMeta rowMeta2;

        public CompileContext(RowMeta rowMeta1)
        {
            this.rowMeta1 = rowMeta1;
            this.rowMeta2 = null;
        }

        public CompileContext(RowMeta rowMeta1, RowMeta rowMeta2)
        {
            this.rowMeta1 = rowMeta1;
            this.rowMeta2 = rowMeta2;
        }
    }
}
