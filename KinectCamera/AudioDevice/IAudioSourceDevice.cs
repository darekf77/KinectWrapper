using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceRecorder.Audio;

namespace Kinect_Wrapper.camera
{
    public enum AudioSourceType
    {
        Kinect,
        File,
        OtherDevice
    }



    public interface IAudioSourceDevice
    {
        int Id { get; }
        AudioSourceType Type { get; }
        TrimWaveStream Stream { get; }
        string Path { get; }
        string Name { get; }
        bool isRecordingPossible { get; set; }

        KinectAudioSource KinectAudio { get; set; }
    }
}
