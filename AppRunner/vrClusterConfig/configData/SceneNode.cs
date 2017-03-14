using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig
{
    public class SceneNode : IConfigItem, IDataErrorInfo
    {
        public string id { get; set; }
        //public string nodeType { get; set; }
        public string locationX { get; set; }
        public string locationY { get; set; }
        public string locationZ { get; set; }
        public string rotationP { get; set; }
        public string rotationY { get; set; }
        public string rotationR { get; set; }
        public TrackerInput tracker { get; set; }
        public string trackerCh { get; set; }

        public SceneNode parent { get; set; }

        public SceneNode()
        {
            id = string.Empty;
            locationX = string.Empty;
            locationY = string.Empty;
            locationZ = string.Empty;
            rotationP = string.Empty;
            rotationR = string.Empty;
            rotationY = string.Empty;
            trackerCh = string.Empty;
            tracker = new TrackerInput();
            parent = null;
        }

        public SceneNode(string _id, string _locationX, string _locationY, string _locationZ, string _rotationP, string _rotationY, string _rotationR, TrackerInput _tracker, string _trackerCh, SceneNode _parent)
        {
            id = _id;
            locationX = _locationX;
            locationY = _locationY;
            locationZ = _locationZ;
            rotationP = _rotationP;
            rotationY = _rotationY;
            rotationR = _rotationR;
            tracker = _tracker;
            trackerCh = _trackerCh;
            parent = _parent;
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
                            error = "Scene Node ID should contain only letters, numbers and _";
                        }
                        break;
                    case "locationX":
                        if (!ValidationRules.IsFloat(locationX))
                        {
                            error = "Location X should be a floating point number";
                        }
                        break;
                    case "locationY":
                        if (!ValidationRules.IsFloat(locationY))
                        {
                            error = "Location Y should be a floating point number";
                        }
                        break;
                    case "locationZ":
                        if (!ValidationRules.IsFloat(locationZ))
                        {
                            error = "Location Z should be a floating point number";
                        }
                        break;
                    case "rotationP":
                        if (!ValidationRules.IsFloat(rotationP))
                        {
                            error = "Pitch should be a floating point number";
                        }
                        break;
                    case "rotationY":
                        if (!ValidationRules.IsFloat(rotationY))
                        {
                            error = "Yaw should be a floating point number";
                        }
                        break;
                    case "rotationR":
                        if (!ValidationRules.IsFloat(rotationR))
                        {
                            error = "Roll should be a floating point number";
                        }
                        break;
                    case "trackerCh":
                        if (!ValidationRules.IsInt(trackerCh))
                        {
                            error = "Tracker channel should be an integer";
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
            string stringCfg = "[scene_node] ";
            stringCfg = string.Concat(stringCfg, "id=", id);
            if (!string.IsNullOrEmpty(locationX) && !string.IsNullOrEmpty(locationY) && !string.IsNullOrEmpty(locationZ)
                && !string.IsNullOrEmpty(rotationP) && !string.IsNullOrEmpty(rotationY) && !string.IsNullOrEmpty(rotationR))
            {
                stringCfg = string.Concat(stringCfg, " loc=\"X=", locationX, ",Y=", locationY, ",Z=", locationZ,
                "\" rot=\"P=", rotationP, ",Y=", rotationY, ",R=", rotationR, "\"");
            }
            if (!string.IsNullOrEmpty(trackerCh) && tracker != null)
            {
                stringCfg = string.Concat(stringCfg, " tracker_id=", tracker.id, " tracker_ch=", trackerCh);
            }
            if (parent != null)
            {
                stringCfg = string.Concat(stringCfg, " parent=", parent.id);
            }
            stringCfg = string.Concat(stringCfg, "\n");
            return stringCfg;
        }


    }
}
