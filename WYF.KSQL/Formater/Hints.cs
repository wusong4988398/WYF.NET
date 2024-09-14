using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WYF.KSQL.Dom;

namespace WYF.KSQL.Formater
{
    public interface Hints
    {
       
        string FormatHints(IList hints, SqlObject sql);
        void FormatHints(IList hints, SqlObject sql, StringBuilder buffer);
    }





}
