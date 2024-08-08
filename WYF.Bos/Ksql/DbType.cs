using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.ksql
{
    public class DbType
    {
        public static  int KSQL = 0;
        public static  int DB2_UDB = 1;
        public static  int Oracle = 2;
        public static  int MS_SQL_Server = 3;
        public static  int Sybase = 4;
        public static  int PostgresSQL = 5;
        public static  int MySQL = 6;
        public static  int Oracle9 = 7;
        public static  int Oracle10 = 8;
        public static  int AS400 = 9;
        public static  int Derby = 10;
        public static  int DM = 11;
        public static  int GS = 12;
        public static  int GS100 = 13;

        private DbType()
        {
        }
        public static int[] GetAllTypes()
        {
            return new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        }

        public static string GetName( int dbType)
        {
            switch (dbType)
            {
                case 0:
                    {
                        return "KSQL";
                    }
                case 1:
                    {
                        return "DB2 UDB";
                    }
                case 2:
                    {
                        return "Oracle";
                    }
                case 7:
                    {
                        return "Oracle9";
                    }
                case 4:
                    {
                        return "Sybase";
                    }
                case 5:
                    {
                        return "PostgresSQL";
                    }
                case 6:
                    {
                        return "MySQL";
                    }
                case 3:
                    {
                        return "MS SQL Server";
                    }
                case 8:
                    {
                        return "Oracle10";
                    }
                case 9:
                    {
                        return "AS400";
                    }
                case 10:
                    {
                        return "Derby";
                    }
                case 11:
                    {
                        return "DM";
                    }
                case 12:
                    {
                        return "GS";
                    }
                case 13:
                    {
                        return "GS100";
                    }
                default:
                    {
                        throw new ArgumentException("dbType, value is " + dbType);
                    }
            }
        }

        public static bool IsOracle(int type)
        {
            return type == 8 || type == 7 || type == 2;
        }

        public static int GetValue( string oName)
        {
             string name = oName.ToLower();
            if ("ksql".Equals(name))
            {
                return 0;
            }
            if (name.IndexOf("dm") != -1)
            {
                return 11;
            }
            if (name.IndexOf("gs100") != -1)
            {
                return 13;
            }
            if (name.IndexOf("gs") != -1)
            {
                return 12;
            }
            if (name.IndexOf("mysql") != -1 || name.IndexOf("mariadb") != -1)
            {
                return 6;
            }
            if (name.StartsWith("oracle"))
            {
                if (name.IndexOf("9") != -1)
                {
                    return 7;
                }
                if (name.IndexOf("10") != -1)
                {
                    return 8;
                }
                return 2;
            }
            else
            {
                if (name.IndexOf("postgressql") != -1 || name.IndexOf("postgresql") != -1)
                {
                    return 5;
                }
                if (name.IndexOf("as400") != -1)
                {
                    return 9;
                }
                if (name.StartsWith("db2"))
                {
                    return 1;
                }
                if (name.IndexOf("sybase") != -1)
                {
                    return 4;
                }
                if (name.IndexOf("derby") != -1)
                {
                    return 10;
                }
                if (name.IndexOf("ms") != -1 || name.IndexOf("microsoft") != -1 || name.IndexOf("sqlserver") != -1)
                {
                    return 3;
                }
                throw new ArgumentException("name:" + oName);
            }
        }
    }
}
