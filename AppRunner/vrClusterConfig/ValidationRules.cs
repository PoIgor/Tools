using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace vrClusterConfig
{
    public static class ValidationRules
    {
        public static bool IsName(string value)
        {
            return Regex.IsMatch(value, "^[\\w]*$");
        }

        public static bool IsFloat(string value)
        {
            return Regex.IsMatch(value, "^[-??\\d]*(?:\\.[0-9]*)?$");
        }

        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, "^[\\d]*$");
        }

        public static bool IsIp(string value)
        {
            return Regex.IsMatch(value, "^[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}$");
        }

        //is strin value address word@IpAddress
        public static bool IsAddress(string value)
        {
            return Regex.IsMatch(value, "^\\w*@[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}$");
        }

    }
}
