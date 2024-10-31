using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WYF.DbEngine.db
{
    public class Seg
    {
        public bool IsIn { get; set; }

        public bool IsInGenned { get; set; }

        public string SqlPart { get; set; }

        public object[] Paramters { get; set; }

        public Seg() { }

        internal void Gen()
        {
            if (!this.IsIn || this.IsInGenned)
                return;
            bool isCreateTemp = false;
            if (!isCreateTemp)
            {
                StringBuilder stringBuilder = new StringBuilder(this.SqlPart);
                stringBuilder.Append(" IN (");
                for (int i = 0; i < this.Paramters.Length; i++) {
                    if (i == 0)
                    {
                        stringBuilder.Append('?');
                    }
                    else
                    {
                        stringBuilder.Append(",?");
                    }
                }
                stringBuilder.Append(')');
                this.SqlPart = stringBuilder.ToNullString();
            }
            this.IsInGenned = true;
        }
    }
}
