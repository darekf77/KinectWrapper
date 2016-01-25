using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.structures
{
    [Serializable]
    public class InfoRow:INotifyPropertyChanged
    {
        private String _name;
        private String _value;
        public InfoRow(String name, String value)
        {
            _name = name;
            _value = value;
        }
        public String Name { get { return _name; } 
            set {
                _name = value;
                NotifyPropertyChanged("Name");
            } 
        }
        public String Value { get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
            }        
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
