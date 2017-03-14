using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig
{
    public class Screen : IConfigItem, IDataErrorInfo
    {

        public string id { get; set; }
        public string locationX { get; set; }
        public string locationY { get; set; }
        public string locationZ { get; set; }
        public string rotationP { get; set; }
        public string rotationY { get; set; }
        public string rotationR { get; set; }
        public string sizeX { get; set; }
        public string sizeY { get; set; }
        public SceneNode parentWall { get; set; }

        public Screen()
        {
            id = "ScreenId";
            locationX = "0";
            locationY = "0";
            locationZ = "0";
            rotationP = "0";
            rotationR = "0";
            rotationY = "0";
            sizeX = "0";
            sizeY = "0";
            parentWall = null;
        }

        public Screen(string _id, string _locationX, string _locationY, string _locationZ, string _rotationP, string _rotationY, string _rotationR, string _sizeX, string _sizeY, SceneNode _parentWall)
        {
            id = _id;
            locationX = _locationX;
            locationY = _locationY;
            locationZ = _locationZ;
            rotationP = _rotationP;
            rotationR = _rotationR;
            rotationY = _rotationY;
            sizeX = _sizeX;
            sizeY = _sizeY;
            parentWall = _parentWall;
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
                            error = "Screen ID should contain only letters, numbers and _";
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
                    case "sizeX":
                        if (!ValidationRules.IsFloat(sizeX.ToString()))
                        {
                            error = "The X size parameter should be a floating point number";
                        }
                        break;
                    case "sizeY":
                        if (!ValidationRules.IsFloat(sizeY.ToString()))
                        {
                            error = "The Y size parameter should be a floating point number";
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

        //Create String of screen parameters for config file
        public string CreateCfg()
        {
            string stringCfg = "[screen] ";
            stringCfg = string.Concat(stringCfg, "id=", id, " loc=\"X=", locationX, ",Y=", locationY, ",Z=", locationZ, 
                "\" rot=\"P=", rotationP, ",Y=", rotationY, ",R=", rotationR, "\" size=\"X=", sizeX, ",Y=", sizeY, "\"");
            if (parentWall != null)
            {
                stringCfg = string.Concat(stringCfg, " parent=", parentWall.id);
            }

            stringCfg = string.Concat(stringCfg, "\n");
            return stringCfg;
        }
    }
}
