using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig

{
    public class ActiveNode
    {
        public string key { get; set; }
        private bool _value;
        public bool value {
            get { return _value; }
            set
            {
                _value = value;
                RegistrySaver.UpdateRegistry(RegistrySaver.nodeList, key, value);
            }
        }

        public ActiveNode()
        {
            key = "Active Node";
            value = true;
        }

        public ActiveNode(string _key, bool _value)
        {
            key = _key;
            value = _value;
        }


    }
}
