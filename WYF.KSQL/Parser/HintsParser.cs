using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Parser
{
    public class HintsParser
    {
        // Methods
        public static IList parse(string hints)
        {
            IList list = new ArrayList();
            hints = hints.Trim();
            KHint hint = null;
            int index = hints.IndexOf(' ');
            for (int i = hints.IndexOf('('); (index != -1) || (i != -1); i = hints.IndexOf('('))
            {
                if (hints.StartsWith("("))
                {
                    throw new ParserException("Can't parse hints, expect keyword but find token '('");
                }
                int end = (index > i) ? i : index;
                if (end == -1)
                {
                    end = (index > i) ? index : i;
                }
                hint = new KHint(hints.substring(0, end));
                hints = hints.Substring(end).Trim();
                if (hints.StartsWith("("))
                {
                    int num4 = hints.IndexOf(')');
                    if (num4 == -1)
                    {
                        throw new ParserException("Can't parse hints, expect token ')' but can't find one!");
                    }
                    string str = hints.substring(1, num4).Trim();
                    ArrayList list2 = new ArrayList();
                    list2.Add(str);
                    hint.addParameters(list2.ToArray());
                    hints = hints.Substring(num4 + 1).Trim();
                }
                list.Add(hint);
                index = hints.IndexOf(' ');
            }
            if (hints.Length > 0)
            {
                hint = new KHint(hints);
                list.Add(hint);
            }
            return list;
        }
    }






}
