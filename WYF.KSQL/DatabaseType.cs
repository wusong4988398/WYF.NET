using System;
using System.Collections.Generic;
using System.Text;

namespace WYF.KSQL
{
    public static class DatabaseType
    {

        public const int AS400 = 9;
        public const int DB2_UDB = 1;
        public const int Derby = 10;
        public const int KSQL = 0;
        public const int MS_SQL_Server = 3;
        public const int MySQL = 6;
        public const int Oracle = 2;
        public const int Oracle10 = 8;
        public const int Oracle9 = 7;
        public const int PostgresSQL = 5;
        public const int Sybase = 4;

   
        public static int[] getAllTypes()
        {
            return new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        public static string getName(int dbType)
        {
            switch (dbType)
            {
                case 0:
                    return "KSQL";

                case 1:
                    return "DB2 UDB";

                case 2:
                    return "Oracle";

                case 3:
                    return "MS SQL Server";

                case 4:
                    return "Sybase";

                case 5:
                    return "PostgresSQL";

                case 6:
                    return "MySQL";

                case 7:
                    return "Oracle9";

                case 8:
                    return "Oracle10";

                case 9:
                    return "AS400";

                case 10:
                    return "Derby";
            }
            throw new ArgumentException("dbType, value is " + dbType);
        }

        public static int getValue(string name)
        {
            if (("KSQL".EqualsIgnoreCase(name) || "KSQL".EqualsIgnoreCase(name)) || "KSQL".EqualsIgnoreCase(name))
            {
                return 0;
            }
            if (("DB2 UDB".EqualsIgnoreCase(name) || "DB2_UDB".EqualsIgnoreCase(name)) || "DB2".EqualsIgnoreCase(name))
            {
                return 1;
            }
            if (("Oracle".EqualsIgnoreCase(name) || "Oracle8".EqualsIgnoreCase(name)) || "Oracle8i".EqualsIgnoreCase(name))
            {
                return 2;
            }
            if ("Oracle9".EqualsIgnoreCase(name) || "Oracle9i".EqualsIgnoreCase(name))
            {
                return 7;
            }
            if ("Sybase".EqualsIgnoreCase(name))
            {
                return 4;
            }
            if ("PostgresSQL".EqualsIgnoreCase(name))
            {
                return 5;
            }
            if ("MySQL".EqualsIgnoreCase(name))
            {
                return 6;
            }
            if (("MS SQL Server".EqualsIgnoreCase(name) || "SQLServer".EqualsIgnoreCase(name)) || "MSSQLServer".EqualsIgnoreCase(name))
            {
                return 3;
            }
            if ("Oracle10".EqualsIgnoreCase(name) || "Oracle10g".EqualsIgnoreCase(name))
            {
                return 8;
            }
            if ("AS400".EqualsIgnoreCase(name))
            {
                return 9;
            }
            if (!"derby".EqualsIgnoreCase(name))
            {
                throw new ArgumentException("name" + name);
            }
            return 10;
        }
    }


 


}
