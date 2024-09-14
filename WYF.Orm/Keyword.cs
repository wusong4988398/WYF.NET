using System.Collections.Generic;

namespace WYF.SqlParser
{
    public class Keyword : LeafExpr
    {
        private static HashSet<string> keywords = new HashSet<string>();
        private string name;

        static Keyword()
        {
            keywords.UnionWith(new string[] { "YEAR", "MONTH", "DAY", "MINUTE", "HOUR", "SECOND", "Y", "M", "D" });
        }

        public static Keyword Of(string name)
        {
            if (keywords.Contains(name.ToUpper()))
            {
                return new Keyword(name);
            }
            return null;
        }

        public Keyword(string name) : base(Optional<NodeLocation>.Empty())
        {
            this.name = name;
        }

        public override DataType CreateDataType()
        {
            return DataType.UnknownType;
        }

        public override string Sql()
        {
            return this.name;
        }

        public override R Accept<R, C>(IAstVisitor<R, C> visitor, C context)
        {
            return default(R);
        }

        public override Calc Compile(CompileContext context)
        {
            return new ConstantCalc(this, this.name);
        }
    }
}