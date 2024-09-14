
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace WYF.KSQL.Util
{
    public class UUTNSource
    {
        
        public const string DEFAULT_PORT = "-11034";
        //private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public const string ORMRPC_CONFIG = "ormrpc.config";
        public const string ORMRPC_PROPERTIES = "ormrpc.properties";
        public const string TCP_CONNECTION_PORT = "tcpConnectionPort";

        // Methods
        public static IPAddress getLocleAddress()
        {
            foreach (NetworkInterface interface2 in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation information in interface2.GetIPProperties().UnicastAddresses)
                {
                    if (information.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return information.Address;
                    }
                }
            }
            return IPAddress.Loopback;
        }

        public static string getPortString()
        {
            string configPath = Environment.GetEnvironmentVariable("ormrpc.config")?.Trim();

            if (string.IsNullOrEmpty(configPath))
            {
                return "-11034";
            }

            try
            {
                using (var stream = new StreamReader(configPath))
                using (var reader = new JsonTextReader(stream))
                {
                    var settings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Error,
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    dynamic config = JsonConvert.DeserializeObject<IDictionary<string, object>>(reader.ReadAsString());

                    if (config.tcpConnectionPort != null)
                    {
                        string portValue = config.tcpConnectionPort.ToNullString().Trim();
                        return portValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Consider logging the exception.
                Console.WriteLine($"ORMRPC cannot load config file from [{configPath}]: ErrorInfo: {ex.Message}");
                return "-11034";
            }

            return "-11034";
        }

        public static string getUniqueSource()
        {
            string str;
            try
            {
                IPAddress address = getLocleAddress();
                //logger.Debug("[TempTableManager] Create UUTN Source: " + address);
                str = address + ":" + getPortString();
            }
            catch (SocketException exception)
            {
                throw new UUTNException(exception);
            }
            return str;
        }
    }





}
