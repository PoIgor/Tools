using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig
{
    public enum LogCatValue
    {
            Off,
            All,
            Verbose,
            VeryVerbose,
            Log,
            Warning,
            Error
    }

    public class LogCategory
    {
        public string id { get; set; }
        private LogCatValue _value;        
        public LogCatValue value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                
            }
        }
        public bool isChecked { get; set; }

        public LogCategory()
        {
            id = string.Empty;
            value = LogCatValue.All;
            isChecked = true;
        }
        public LogCategory(string _id)
        {
            id = _id;
            value = LogCatValue.All;
            isChecked = true;
        }
    }
}
