
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree.calc
{
    public interface CalcCompileable
    {
        Calc Compile(CompileContext context);
    }
}
