namespace WYF.OrmEngine.Query.Fulltext
{
    public interface IFullTextCustSyncQuery
    {
        bool IsEnable();

        bool IsConfigFullText(string entityName);

        string[] Query(string entityName, FTFilter filter);
    }
}