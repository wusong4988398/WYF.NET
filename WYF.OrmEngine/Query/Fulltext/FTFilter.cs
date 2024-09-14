using System.Text;

namespace WYF.OrmEngine.Query.Fulltext
{
    public class FTFilter
    {
        private string property;

        private FTCompare cp;

        private FTValue value;

        private class FTFilterNest
        {
            public string nestRw;

            public FTFilter filter;

            public FTFilterNest(string nestRw, FTFilter filter)
            {
                this.nestRw = nestRw;
                this.filter = filter;
            }

            public override string ToString()
            {
                return nestRw + filter;
            }
        }

        private List<FTFilterNest> nests = new List<FTFilterNest>();

        public FTFilter(string property, FTCompare cp, FTValue value)
        {
            this.property = property;
            this.cp = cp;
            this.value = value;
        }

        public FTFilter And(FTFilter f)
        {
            return Add("and", f);
        }

        public FTFilter Or(FTFilter f)
        {
            return Add("or", f);
        }

        private FTFilter Add(string nestRw, FTFilter f)
        {
            nests.Add(new FTFilterNest(nestRw, f));
            return this;
        }

        public FTFilterExp ToExp()
        {
            List<FTValue> list = new List<FTValue>();
            StringBuilder sb = new StringBuilder();
            sb.Append(property).Append(' ').Append(cp).Append(' ').Append('?');
            list.Add(value);
            if (!nests.IsEmpty())
                foreach (FTFilterNest nest in nests)
                {
                    sb.Insert(0, '(');
                    FTFilterExp exp = nest.filter.ToExp();
                    sb.Append(' ').Append(nest.nestRw).Append(' ').Append(exp.Exp);
                    list.AddRange(exp.Values);
                    sb.Append(')');
                }
            return new FTFilterExp(sb.ToString(), list.ToArray());
        }
    }


    public class FTFilterExp
    {
        public string Exp { get; private set; }

        public FTValue[] Values { get; private set; }

        public FTFilterExp(string exp, params FTValue[] values)
        {
            Exp = exp;
            Values = values;
        }



        public override string ToString()
        {
            string param = "";
            if (Values != null)
                param = Values.ToList().ToString();
            return Exp + ':' + param;
        }
    }
}