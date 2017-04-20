using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;

namespace vrClusterConfig
{
    public class ClusterNode : ConfigItem, IDataErrorInfo
    {

        public string id { get; set; }
        public bool isMaster { get; set; }
        public string address { get; set; }
        public Screen screen { get; set; }
        public Viewport viewport { get; set; }
        public ClusterNode()
        {
            id = "ClusterNodeId";
            address = "127.0.0.1";
            screen = null;
            viewport = null;
            isMaster = false;
        }

        public ClusterNode(string _id, string _address, Screen _screen, Viewport _viewport, bool _isMaster)
        {
            id = _id;
            address = _address;
            screen = _screen;
            viewport = _viewport;
            isMaster = _isMaster;
        }

        //Implementation IDataErrorInfo methods for validation
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                if (columnName == "id" || columnName == validationName)
                {
                    if (!ValidationRules.IsName(id))
                    {
                        error = "Cluster node ID should contain only letters, numbers and _";
                        AppLogger.Add("ERROR! " + error);
                    }
                }
                if (columnName == "address" || columnName == validationName)
                {
                    if (!ValidationRules.IsIp(address))
                    {
                        error = "Cluster node addres should be IP address";
                        AppLogger.Add("ERROR! " + error);
                    }
                }

                MainWindow.ConfigModifyIndicator();
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Validate()
        {
            bool isValid = ValidationRules.IsName(id) && ValidationRules.IsIp(address);
            if (!isValid)
            {
                AppLogger.Add("ERROR! Errors in Clustr Node [" + id + "]");
                string a = this[validationName];

            }

            return isValid;
        }

        public override string CreateCfg()
        {
            string stringCfg = "[cluster_node] ";
            stringCfg = string.Concat(stringCfg, "id=", id, " addr=", address);
            if (screen != null)
            {
                stringCfg = string.Concat(stringCfg, " screen=", screen.id);
            }
            if (viewport != null)
            {

                stringCfg = string.Concat(stringCfg, " viewport=", viewport.id);
            }

            if (isMaster)
            {
                MainWindow Win = (MainWindow)Application.Current.MainWindow;
                string portCS = Win.currentConfig.portCs;
                string portSS = Win.currentConfig.portSs;
                stringCfg = string.Concat(stringCfg, " port_cs=", portCS, " port_ss=", portSS, " master=true");
            }
            stringCfg = string.Concat(stringCfg, "\n");
            return stringCfg;
        }

    }
}
