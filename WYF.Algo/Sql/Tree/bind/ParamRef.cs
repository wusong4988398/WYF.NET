using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Algo.Sql.Tree.calc;

namespace WYF.Algo.Sql.Tree.bind
{
    public class ParamRef : BindRef<string>
    {
        private object _value;

        public ParamRef(Optional<NodeLocation> location, string name, object value)
            : base(location, name)
        {
            _value = value;
        }

        public override DataType CreateDataType()
        {
            return DataType.UnknownType;
        }

        public override string Sql()
        {
            return (string)this.Ref;
        }

        public string GetName()
        {
            return (string)this.Ref;
        }

        public override TR Accept<TR, TC>(IAstVisitor<TR, TC> visitor, TC context)
        {
            return default(TR);
        }

        public override Calc Compile(CompileContext context)
        {
            return new ConstantCalc(this, _value);
        }
    }
}
