using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.camera
{
    public interface IAudioRecognition : INotifyPropertyChanged
    {
        bool IsEnable { get; set; }
        void init(IAudioSourceDevice source);

        event EventHandler<IAudioMessage> UserSaying;

        ObservableCollection<String> Grammar { get; set; }
    }
}
