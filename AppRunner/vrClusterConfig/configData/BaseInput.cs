using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig
{
    public enum InputType
    {
        tracker,
        analog,
        buttons
    }

    public class BaseInput : IConfigItem, IDataErrorInfo
    {

        public string id { get; set; }
        public InputType type { get; set; }
        public string address { get; set; }

        public BaseInput()
        {
            id = "InputId";
            address = "InputName@127.0.0.1";
        }

        public BaseInput(string _id, InputType _type, string _address)
        {
            id = _id;
            type = _type;
            address = _address;
        }

        //Implementation IDataErrorInfo methods for validation
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "id":
                        if (!ValidationRules.IsName(id))
                        {
                            error = "Input ID should contain only letters, numbers and _";
                        }
                        break;
                    case "address":
                        if (!ValidationRules.IsAddress(address))
                        {
                            error = "Input Addres in format InputName@127.0.0.1";
                        }
                        break;

                }
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string CreateCfg()
        {
            string stringCfg = "[input] ";
            stringCfg = string.Concat(stringCfg, "id=", id, " type=", type.ToString(), " addr=", address, "\n");

            return stringCfg;
        }
    }
}