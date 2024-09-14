
namespace WYF.SqlParser
{
    public interface ISchema
    {
        string Name { get; }

        ITable[] Tables { get; }

        ITable this[string tableName] { get; }

        IColumn this[string[] columnPath] { get; }

        IFuncFactory FuncFactory { get; }

        bool IsIgnoreCase { get; }
    }
}