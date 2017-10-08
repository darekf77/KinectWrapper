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
        Boolean IsAvailable { get; }

        Boolean IsEnable { get; set; }

        Boolean IsRecordingPossible { get; }

        Boolean IsRecordingEnable { get; set; }

        IDevice CurrentDevice { get; set; }

        ObservableCollection<String> Grammar { get; set; }

        ObservableCollection<string> RecordingDevices { get; }

        SpeechRecognitionEngine SpeechRecognizer { get; }

        event EventHandler<IAudioMessage> UserSaying;
    }
}
