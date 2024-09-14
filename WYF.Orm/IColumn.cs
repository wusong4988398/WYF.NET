namespace WYF.SqlParser
{
    public interface IColumn
    {
        ISchema Schema { get; }

        ITable Table { get; }

        int Index { get; }

        string Name { get; }

        string FullName { get; }

        DataType DataType { get; }
    }
}