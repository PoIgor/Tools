using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrClusterConfig
{
    public class AppLogger : INotifyPropertyChanged
    {
        private AppLogger()
        {

        }

        //Implementation of INotifyPropertyChanged method for TwoWay binding
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //Set property with OnNotifyPropertyChanged call
        protected void Set<T>(ref T field, T newValue, string propertyName)
        {
            field = newValue;
            OnNotifyPropertyChanged(propertyName);
        }

        private static AppLogger _instance;
        public static AppLogger instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppLogger();
                       
                }
                return _instance;
            }
        }

        private string _Log;

        public string Log
        {
            get
            {
                if (_Log == null)
                {
                    _Log = string.Empty;
                }
                return _Log;
            }
            set { Set(ref _Log, value, "Log"); }
        }


        public static void CleanLog()
        {
            instance.Log = DateTime.Now.ToString() + ":  Log Cleaned";
        }

        public static void Add(string text)
        {
            instance.Log = instance.Log + System.Environment.NewLine + DateTime.Now.ToString() + ":  " + text;
        }

    }
}
