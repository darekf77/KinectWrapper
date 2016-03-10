using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.structures;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Wrapper.device.audio
{
    public interface IAudio
    {
        IDevice Device { get; set; }

        ObservableCollection<String> Grammar { get; set; }

        SpeechRecognitionEngine SpeechRecognizer { get; }

        event EventHandler<IAudioMessage> UserSaying;
    }
}
