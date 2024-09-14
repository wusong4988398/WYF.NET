using System.ComponentModel;

namespace WYF.OrmEngine.Query.Fulltext
{
    public enum FTCompare
    {
        [Description("like")]
        LIKE,
        [Description("in")]
        IN,
        [Description("match")]
        MATCH,
        [Description("eq")]
        EQ,
        [Description("gt")]
        GT,
        [Description("lt")]
        LT,
        [Description("notnull")]
        NOTNULL
    }
}