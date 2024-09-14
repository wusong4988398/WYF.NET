using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Sql.Tree.calc
{
    public interface ICalcCompileable
    {
        Calc Compile(CompileContext context);
    }
}
