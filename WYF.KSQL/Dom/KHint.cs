using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL.Dom
{
    public class KHint
    {
        // Fields
        private string name;
        private string orgName;
        private ArrayList parameters;

        // Methods
        public KHint(string name)
        {
            this.parameters = new ArrayList();
            this.orgName = name;
            this.name = name.ToUpper();
        }

        public KHint(string name, ICollection _params) : this(name)
        {
            this.parameters.AddRange(_params);
        }

        public void addParameters(ICollection c)
        {
            this.parameters.AddRange(c);
        }

        public bool equals(object obj)
        {
            if (this != obj)
            {
                if (obj == null)
                {
                    return false;
                }
                if (!(obj.GetType() == typeof(KHint)))
                {
                    return false;
                }
                KHint hint = (KHint)obj;
                if (!this.name.Equals(hint.name))
                {
                    return false;
                }
                if (this.parameters.Count != hint.parameters.Count)
                {
                    return false;
                }
                for (int i = 0; i < this.parameters.Count; i++)
                {
                    if ((this.parameters[i] != hint.parameters[i]) && ((this.parameters[i] == null) || !this.parameters[i].Equals(hint.parameters[i])))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string getName()
        {
            return this.name;
        }

        public string getOrgName()
        {
            return this.orgName;
        }

        public ArrayList getParameters()
        {
            return this.parameters;
        }

        public int hashCode()
        {
            int num = this.name.GetHashCode() * 0x25;
            for (int i = 0; i < this.parameters.Count; i++)
            {
                if (this.parameters[i] != null)
                {
                    num += this.parameters[i].GetHashCode() * 0x25;
                }
            }
            return num;
        }

        public string toString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.name);
            if (this.parameters != null)
            {
                builder.Append("(");
                for (int i = 0; i < this.parameters.Count; i++)
                {
                    builder.Append(this.parameters[i] + ", ");
                }
                builder.Remove(builder.Length - 3, builder.Length - 1);
                builder.Append(")");
            }
            return builder.ToString();
        }
    }


 



}
