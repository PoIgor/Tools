using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig
{
    public class Viewport :IConfigItem, IDataErrorInfo
    {
        public string id { get; set; }
        public string x { get; set; }
        public string y { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public bool horizontalFlip { get; set; }
        public bool verticalFlip { get; set; }

        public Viewport()
        {
            id = "ViewportId";
            x = "0";
            y = "0";
            width = "0";
            height = "0";
            horizontalFlip = false;
            verticalFlip = false;
        }

        public Viewport(string _id, string _x, string _y, string _width, string _height, bool _horizontalFlip, bool _verticalFlip)
        {
            id = _id;
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            horizontalFlip = _horizontalFlip;
            verticalFlip = _verticalFlip;
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
                            error = "Viewport ID should contain only letters, numbers and _";
                        }
                        break;
                    case "x":
                        if (!ValidationRules.IsInt(x.ToString()))
                        {
                            error = "x should be an integer";
                        }
                        break;
                    case "y":
                        if (!ValidationRules.IsInt(y.ToString()))
                        {
                            error = "y should be an integer";
                        }
                        break;
                    case "width":
                        if (!ValidationRules.IsInt(width.ToString()))
                        {
                            error = "Width should be an integer";
                        }
                        break;
                    case "height":
                        if (!ValidationRules.IsInt(height.ToString()))
                        {
                            error = "Height should be an integer";
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
            string stringCfg = "[viewport] ";
            stringCfg = string.Concat(stringCfg, "id=", id, " x=", x, " y=", y, " width=", width, " height=", height, " flip_h=", horizontalFlip.ToString(), "flip_v=", verticalFlip, "\n");

            return stringCfg;
        }
    }
}
