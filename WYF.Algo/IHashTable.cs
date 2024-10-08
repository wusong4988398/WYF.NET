using static IronPython.SQLite.PythonSQLite;

namespace WYF.Algo
{
    /// <summary>
    /// hash表接口，可以用来与IDataset进行互转。
    /// </summary>
    public interface IHashTable
    {
        RowMeta GetRowMeta();

        Row Lookup(object paramObject);

        void Close();
    }
}