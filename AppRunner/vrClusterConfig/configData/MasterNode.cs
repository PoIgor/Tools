using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig
{ 
    class MasterNode
    {
        public ClusterNode masterNode { get; set; }
        public string portCs { get; set; }
        public string portSs { get; set; }
    }
}
