using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig
{
    public static class RegistrySaver
    {
        const string registryPath = "SOFTWARE\\Pixela Labs\\vrCluster";
        // conf registry names
        public const string appList = "appList";
        public const string nodeList = "nodeList";
        public const string configList = "configList";
        public const string configName = "configName";
        public const string paramsList = "parameters";
        public const string isStereoName = "isStereo";
        public const string isNoSoundName = "isNoSound";
        public const string isAllCoresName = "isAllCores";
        public const string isFixedSeedName = "isFixedSeed";
        public const string isNoTextureStreamingName = "isNoTextureStreaming";
        public const string openGLName = "openGL";
        public const string additionalParamsName = "additionalParams";

        private static string[] ReadRegistry(string key)
        {
            RegistryKey pixelaKey = Registry.CurrentUser.OpenSubKey(registryPath, true);
            if (pixelaKey == null)
            {
                Registry.CurrentUser.CreateSubKey(registryPath);
            }
            RegistryKey regKey = pixelaKey.OpenSubKey(key, true);
            if (regKey == null)
            {
                regKey = pixelaKey.CreateSubKey(key);
            }
            string[] valueNamesArray = null;
            if (regKey != null)
            {
                valueNamesArray = regKey.GetValueNames();
            }

            return valueNamesArray;
        }

        public static List<ActiveNode> ReadNodesFromRegistry(string key)
        {
            string[] valueNamesArray = ReadRegistry(key);
            RegistryKey pixelaKey = Registry.CurrentUser.OpenSubKey(registryPath, true);
            RegistryKey workKey = pixelaKey.OpenSubKey(key, true);
            List<ActiveNode> regNodes = new List<ActiveNode>();
            foreach (string name in valueNamesArray)
            {
                object keyValue = workKey.GetValue(name);
                bool isSelected = false;

                if (keyValue != null)
                    isSelected = Convert.ToBoolean(keyValue);
                regNodes.Add(new ActiveNode(name, isSelected));
            }

            return regNodes;
        }

        public static List<string> ReadStringsFromRegistry(string key)
        {

            string[] valueNamesArray = ReadRegistry(key);

            List<string> regKeys = null;
            if (valueNamesArray != null)
            {
                regKeys = new List<string>(valueNamesArray);
            }

            return regKeys;
        }

        public static string ReadStringFromRegistry(string key)
        {

            string[] valueNamesArray = ReadRegistry(key);

            string regKey = null;
            if (valueNamesArray != null && valueNamesArray.Length > 0)
            {
                regKey = valueNamesArray.GetValue(0) as string;
            }

            return regKey;
        }

        public static void AddRegistryValue(string key, string value)
        {
            UpdateRegistry(key, value, true);
        }

        public static string ReadStringValue(string key, string name)
        {
            return ReadValue(key, name) as string;
        }

        public static bool ReadBoolValue(string key, string name)
        {

            return Convert.ToBoolean(ReadValue(key, name));
        }

        //public static string ReadStringValue(string key, string name)
        //{
        //    return Convert.ToString(ReadValue(key, name));
        //}

        private static object ReadValue(string key, string name)
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryPath + "\\" + key, true);
                return regKey.GetValue(name);
            }
            catch (Exception exception)
            {
                //AppLogger.Add("Can't read value from registry. EXCEPTION: " + exception.Message);
                return null;
            }
        }

        public static void RemoveRegistryValue(string key, string value)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryPath + "\\" + key, true);
            regKey.DeleteValue(value);
        }

        public static void SetRegValueSelected(string key, string name)
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryPath + "\\" + key, true);
                if (regKey == null)
                {
                    Registry.CurrentUser.CreateSubKey(registryPath + "\\" + key);
                    UpdateRegistry(key, name, true);
                }
                else
                {
                    string[] valueNamesArray = ReadRegistry(key);
                    foreach (string item in valueNamesArray)
                    {
                        if (name == item)
                        {
                            UpdateRegistry(key, item, true);
                        }
                        else
                        {
                            UpdateRegistry(key, name, false);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //AppLogger.Add("Can't set registry value. EXCEPTION: " + exception.Message);
            }
        }

        public static string FindSelectedRegValue(string key)
        {
            string valueName = null;
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryPath + "\\" + key, true);
                if (regKey != null)
                {
                    string[] valueNamesArray = ReadRegistry(key);
                    foreach (string item in valueNamesArray)
                    {
                        if(Convert.ToBoolean(regKey.GetValue(item)))
                        {
                            return item;
                        }
                    }

                }
                            }
            catch (Exception exception)
            {
                //AppLogger.Add("Can't find registry value. EXCEPTION: " + exception.Message);
            }
            return valueName;
        }

        public static void RemoveAllRegistryValues(string key)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryPath + "\\" + key, true);
            string[] values = regKey.GetValueNames();
            foreach (string value in values)
            {
                regKey.DeleteValue(value);
            }
        }

        public static void UpdateRegistry(string key, string name, object value)
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryPath + "\\" + key, true);
                if (regKey == null)
                {
                    Registry.CurrentUser.CreateSubKey(registryPath + "\\" + key);
                    UpdateRegistry(key, name, value);
                }
                regKey.SetValue(name, value);
            }
            catch (Exception exception)
            {
                //AppLogger.Add("Can't update registry value. EXCEPTION: " + exception.Message);
            }
        }
    }
}
