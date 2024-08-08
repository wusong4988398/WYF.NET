using WYF.Bos.algo.sql.tree.calc;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.algo.sql.tree
{
    public class Keyword : LeafExpr
    {
        private readonly static HashSet<string> keywords = new HashSet<string>() { "YEAR", "MONTH", "DAY", "MINUTE", "HOUR", "SECOND", "Y", "M", "D" };
        private string _name;
        public Keyword(Optional<NodeLocation> location) : base(location)
        {

        }
        public Keyword(string name):base(new Optional<NodeLocation>())
        {
            
            this._name = name;
        }
        public static Keyword Of(string name)
        {
            if (keywords.Contains(name.ToUpper()))
                return new Keyword(name);
            return null;
        }
        public override R Accept<R, C>(AstVisitor<R, C> visitor, C context)
        {
            return default(R);
        }

        public override Calc Compile(CompileContext context)
        {
            return (Calc)new ConstantCalc(this._name);
        }

        public override DataType GetDataType()
        {
            return (DataType)DataType.UnknownType;
        }

        public override string Sql()
        {
            return this._name;
        }
    }
}
