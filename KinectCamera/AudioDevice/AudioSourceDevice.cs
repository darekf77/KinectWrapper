using Kinect_Wrapper.device;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using VoiceRecorder.Audio;

namespace Kinect_Wrapper.camera
{

    public class AudioSourceDevice : IAudioSourceDevice
    {
        public IAudioPlayer Player { get; private set; }
        public AudioSourceDevice(WaveInCapabilities c, KinectSensor sensor, IAudioPlayer player, int Id)
        {
            this.Player = player;
            this.Id = Id;
            this.Name = c.ProductName;
            //sensor.AudioSource            
            isRecordingPossible = true;
            if (isKinectMicrophone(Name))
            {
                KinectAudio = sensor.AudioSource;
                Type = AudioSourceType.Kinect;
            }
            else
            {
                Type = AudioSourceType.OtherDevice;
            }

        }

        public static bool isKinectMicrophone(string Name)
        {
            return (Name.Contains("Microphone") && Name.Contains("Kinect"));
        }

        public AudioSourceDevice(string path, int Id)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileNameWithoutExtension(path);
            this.Id = Id;
            Type = AudioSourceType.File;
        }

        public TrimWaveStream Stream
        {
            get
            {
                return (Player != null) ? Player.Stream : null;
            }
        }

        public int Id { get; set; }

        public AudioSourceType Type { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }

        public bool isRecordingPossible { get; set; }

        public KinectAudioSource KinectAudio { get; set; }
    }
}
