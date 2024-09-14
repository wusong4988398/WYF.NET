

namespace WYF.SqlParser
{
    public interface ITable
    {
        ISchema Schema { get; }

        string Name { get; }

        IColumn[] Columns { get; }

        IColumn this[string columnName] { get; }
    }
}