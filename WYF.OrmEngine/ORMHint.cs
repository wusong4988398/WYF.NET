using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WYF.OrmEngine
{

    public class ORMHint
    {
        public interface JoinHinter
        {
            JoinHint Hint(string entityPath);
        }
        private JoinHinter joinHinter;
        private static HashSet<string> innerJoinTables = new HashSet<string>();
        public bool IsSelectNullIfNotExistsProperty { get; set; }

        public JoinHint joinHint(string entityPath)
        {
            return joinHint(entityPath, JoinHint.LEFT);
        }
        public JoinHint joinHint(string entityPath, JoinHint replaceDefaultHint)
        {
            JoinHint hint = new JoinHint();
            if (this.joinHinter != null)
                hint = this.joinHinter.Hint(entityPath);
            if (hint == JoinHint.DEFAULT)
                return (replaceDefaultHint == null) ? JoinHint.LEFT : replaceDefaultHint;
            return hint;
        }

        public static bool IsInnerJoinConfigured(string tableName)
        {
            return (tableName == null) ? false : innerJoinTables.Contains(tableName.ToLower());
        }

        public enum JoinHint
        {

            [Description("LEFT JOIN")]
            DEFAULT,
            [Description("LEFT JOIN")]
            LEFT,
            [Description("INNER JOIN")]
            INNER
        }
    }

}
