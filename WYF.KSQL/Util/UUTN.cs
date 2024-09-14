
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace WYF.KSQL.Util
{
    public class UUTN
    {
       
        public const string CONSTRAINT_PREFIX = "zc";
        public const string GLOBAL_TEMPORARY_TABLE_PREFIX = "##";
        public const string LOCAL_TEMPORARY_TABLE_PREFIX = "#";
        //private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Random mySecureRand = new Random();
        public readonly string prefix;
        public static string S_ID = normalize((long)UUTNSource.getUniqueSource().GetHashCode());
        public const string SEQUENCE_PREFIX = "zs";
        public const string TRIGGER_PREFIX = "zr";
        public readonly string valueAfterMD5;

        // Methods
        public UUTN()
        {
            this.prefix = "#";
            this.valueAfterMD5 = this.getRandomGUID();
        }

        public UUTN(bool isFULL)
        {
            this.prefix = "#";
            this.valueAfterMD5 = this.getRandomGUID(isFULL);
        }

        public UUTN(string prefix)
        {
            this.prefix = prefix;
            this.valueAfterMD5 = this.getRandomGUID();
        }

        public UUTN(bool isFULL, string prefix)
        {
            this.prefix = prefix;
            this.valueAfterMD5 = this.getRandomGUID(isFULL);
        }

        private string getRandomGUID()
        {
            return this.getRandomGUID(false);
        }

        private string getRandomGUID(bool isFULL)
        {
            MD5 md = null;
            StringBuilder builder = new StringBuilder();
            try
            {
                md = new MD5CryptoServiceProvider();
            }
            catch (InvalidOperationException exception)
            {
                //logger.Error(exception);
                throw new UUTNException(exception);
            }
            long ticks = DateTime.Now.Ticks;
            long num2 = 0L;
            num2 = mySecureRand.Next();
            builder.Append(S_ID);
            builder.Append(":");
            builder.Append(ticks.ToString());
            builder.Append(":");
            builder.Append(num2.ToString());
            builder.ToString();
            byte[] buffer = md.ComputeHash(Encoding.UTF8.GetBytes(this.valueAfterMD5));
            long l = (((((((buffer[0] & 0xffL) ^ ((buffer[1] & 0xffL) << 8)) ^ ((buffer[2] & 0xffL) << 0x10)) ^ ((buffer[3] & 0xffL) << 0x18)) ^ ((buffer[4] & 0xffL) << 0x20)) ^ ((buffer[5] & 0xffL) << 40)) ^ ((buffer[6] & 0xffL) << 0x30)) ^ ((buffer[7] & 0xffL) << 0x38);
            long num4 = 0L;
            if (isFULL)
            {
                num4 = (((((((buffer[8] & 0xffL) ^ ((buffer[9] & 0xffL) << 8)) ^ ((buffer[10] & 0xffL) << 0x10)) ^ ((buffer[11] & 0xffL) << 0x18)) ^ ((buffer[12] & 0xffL) << 0x20)) ^ ((buffer[13] & 0xffL) << 40)) ^ ((buffer[14] & 0xffL) << 0x30)) ^ ((buffer[15] & 0xffL) << 0x38);
            }
            else
            {
                num4 = ((buffer[8] & 0xffL) ^ ((buffer[9] & 0xffL) << 8)) ^ ((buffer[10] & 0xffL) << 0x10);
            }
            return (normalize(l) + normalize(num4));
        }

        public static bool isGlobalTempTable(string tablename)
        {
            return ((tablename != null) && tablename.StartsWith("##", StringComparison.OrdinalIgnoreCase));
        }

        public static bool isTempTable(string tablename)
        {
            return ((tablename != null) && tablename.StartsWith("#", StringComparison.OrdinalIgnoreCase));
        }

        private static string normalize(long l)
        {
            return l.ToString().ToUpper().Replace("-", "Z");
        }

        public string toString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.prefix);
            string str = S_ID + this.valueAfterMD5;
            if (this.prefix.EqualsIgnoreCase("zc"))
            {
                builder.Append(str.Substring(str.Length - 0x10));
            }
            else
            {
                builder.Append(S_ID);
                builder.Append(this.valueAfterMD5);
            }
            return builder.ToString();
        }
    }






}
