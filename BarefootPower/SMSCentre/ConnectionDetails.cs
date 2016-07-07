using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BarefootPower.SMSCentre
{
    public class ConnectionDetails
    {
        private static string username = ConfigurationManager.AppSettings["SPUserName"];
        private static string password = ConfigurationManager.AppSettings["SPPassword"];
        private static string HostPPPAddress = ConfigurationManager.AppSettings["ServerPPPIPAddress"];
        private static string spID = ConfigurationManager.AppSettings["SPID"];

        public static readonly Dictionary<string, string> SOAPRequestNamespaces = new Dictionary<string, string>()
        {
            { "soapenv",  "http://schemas.xmlsoap.org/soap/envelope/" },
            { "v2",  "http://www.huawei.com.cn/schema/common/v2_1" },
            { "loc", "http://www.csapi.org/schema/parlayx/sms/send/v2_2/local" },
            { "ns1", "http://www.huawei.com.cn/schema/common/v2_1" },
            { "ns2", "http://www.csapi.org/schema/parlayx/sms/notification/v2_2/local" }
        };

        public static string GetUsername()
        {
            return username;
        }

        public static string GetPassword()
        {
            return password;
        }

        public static string GetSpID()
        {
            return spID;
        }

        public static string GetHostPPPAddress()
        {
            return HostPPPAddress;
        }


        public static string HashPassword(string input)
        {
            return string.Join("", MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input)).Select(s => s.ToString("x2")));
        }
        
    }
}