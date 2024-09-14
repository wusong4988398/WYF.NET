using System;
using WYF.KSQL.Exception;

namespace WYF.KSQL.Formater
{
   
    public abstract class FormaterFactory
    {
      
        public static SQLFormater GetFormater(int dbType)
        {
            switch (dbType)
            {
                case 1:
                    return new DB2SQLFormater();
                case 2:
                    return new Oracle9SQLFormater();
                case 3:
                    return new MSTransactSQLFormater();
                case 4:
                    return new SybaseTransactSQLFormater();
                case 5:
                    return new PostgresSQLFormater();
                case 6:
                    return new MySQLFormater();
                case 7:
                    return new Oracle9SQLFormater();
                case 8:
                    return new Oracle10SQLFormater();
                case 9:
                    return new DB2AS400SQLFormater();
                case 10:
                    return new DerbySQLFormater();
                default:
                    throw new SqlTranslateException("DbType is not supported." + DatabaseType.getName(dbType));
            }
        }
    }
}
