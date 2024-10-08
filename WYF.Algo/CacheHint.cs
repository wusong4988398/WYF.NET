namespace WYF.Algo
{

    public class CacheHint
    {
        public static  int DefaultTimeout = 3600000;

        private long timeout = -1L;

        private int pageSize = 1024;

        private string cacheId;

        private string storageType;
    }
}