using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace vrClusterConfig
{
    public class TrackerInput : BaseInput, IConfigItem, IDataErrorInfo
    {
        public string locationX { get; set; }
        public string locationY { get; set; }
        public string locationZ { get; set; }
        public string rotationP { get; set; }
        public string rotationY { get; set; }
        public string rotationR { get; set; }
        public string front { get; set; }
        public string right { get; set; }
        public string up { get; set; }

        public TrackerInput()
        {
            id = "TrackerInputId";
            address = "TrackerInputName@127.0.0.1";
            type = InputType.tracker;
            locationX = "0";
            locationY = "0";
            locationZ = "0";
            rotationP = "0";
            rotationY = "0";
            rotationR = "0";
            front = "X";
            right = "Y";
            up = "-Z";
        }

        public TrackerInput(string _id, string _address, string _locationX, string _locationY, string _locationZ, string _rotationP, string _rotationY, string _rotationR, string _front, string _right, string _up)
        {
            id = _id;
            address = _address;
            type = InputType.tracker;
            locationX = _locationX;
            locationY = _locationY;
            locationZ = _locationZ;
            rotationP = _rotationP;
            rotationY = _rotationY;
            rotationR = _rotationR;
            front = _front;
            right = _right;
            up = _up;
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
                            error = "Tracker input ID should contain only letters, numbers and _";
                        }
                        break;
                    case "locationX":
                        if (!ValidationRules.IsFloat(locationX.ToString()))
                        {
                            error = "Location X should be a floating point number";
                        }
                        break;
                    case "locationY":
                        if (!ValidationRules.IsFloat(locationY.ToString()))
                        {
                            error = "Location Y should be a floating point number";
                        }
                        break;
                    case "locationZ":
                        if (!ValidationRules.IsFloat(locationZ.ToString()))
                        {
                            error = "Location Z should be a floating point number";
                        }
                        break;
                    case "rotationP":
                        if (!ValidationRules.IsFloat(rotationP.ToString()))
                        {
                            error = "Pitch should be a floating point number";
                        }
                        break;
                    case "rotationY":
                        if (!ValidationRules.IsFloat(rotationY.ToString()))
                        {
                            error = "Yaw should be a floating point number";
                        }
                        break;
                    case "rotationR":
                        if (!ValidationRules.IsFloat(rotationR.ToString()))
                        {
                            error = "Roll should be a floating point number";
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

        public new string CreateCfg()
        {
            string stringCfg = "[input] ";
            stringCfg = string.Concat(stringCfg, "id=", id, " type=", type.ToString(), " addr=", address,
                " loc=\"X=", locationX, ",Y=", locationY, ",Z=", locationZ, "\"",
                " rot=\"P=", rotationP, ",Y=", rotationY, ",R=", rotationR, "\"",
                " front=", front, " right=", right, " up=", up, "\n");

            return stringCfg;
        }
    }
}