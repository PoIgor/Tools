using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig
{
    public abstract class ConfigItem
    {
        public string validationName = "Object validation";

        public abstract string CreateCfg();
        public abstract bool Validate();
    }
}
