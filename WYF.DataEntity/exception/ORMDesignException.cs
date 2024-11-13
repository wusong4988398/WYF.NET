

namespace WYF.DataEntity
{
    [Serializable]
    public class ORMDesignException: OrmException
    {
        public ORMDesignException(String code, String message):base(code,message)
        {
            
        }

        public ORMDesignException(String code, String message, Exception inner) : base(code, message, inner)
        {
            
        }
    }
}
