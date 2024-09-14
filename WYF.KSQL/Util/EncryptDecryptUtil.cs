using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace WYF.KSQL.Util
{
    public static class EncryptDecryptUtil
    {

        public static object Decode(object data)
        {
            if (data.ToString().IsNullOrWhiteSpace())
            {
                return "0";
            }
            string s = "WyfK";
            string str3 = "WyfK";
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                byte[] rgbIV = Encoding.ASCII.GetBytes(str3);
                byte[] buffer = Convert.FromBase64String(data.ToString());
                string str4 = "";
                using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
                {
                    using (MemoryStream stream = new MemoryStream(buffer))
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(stream2))
                            {
                                str4 = reader.ReadToEnd();
                            }
                        }
                    }
                }
                return str4;
            }
            catch (System.Exception exception)
            {
                return exception.Message;
            }
        }

        public static object Encode(object data)
        {
            string s = "WyfK";
            string str2 = "WyfK";
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(s);
                byte[] rgbIV = Encoding.ASCII.GetBytes(str2);
                byte[] inArray = null;
                int length = 0;
                using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
                {
                    int keySize = provider.KeySize;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(bytes, rgbIV), CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(stream2))
                            {
                                writer.Write(data);
                                writer.Flush();
                                stream2.FlushFinalBlock();
                                writer.Flush();
                                inArray = stream.GetBuffer();
                                length = (int)stream.Length;
                            }
                        }
                    }
                }
                return Convert.ToBase64String(inArray, 0, length);
            }
            catch (System.Exception exception)
            {
                return exception.Message;
            }
        }
    }






}
