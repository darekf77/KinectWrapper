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
        IAudioPlayer player;
        public AudioSourceDevice(WaveInCapabilities c, KinectSensor sensor, IAudioPlayer player, int Id)
        {
            this.player = player;
            this.Id = Id;
            this.Name = c.ProductName;
            //sensor.AudioSource            
            isRecordingPossible = true;
            if (Name.StartsWith("Kinect"))
            {
                KinectAudio = sensor.AudioSource;
                Type = AudioSourceType.Kinect;
            }
            else
            {
                Type = AudioSourceType.OtherDevice;
            }

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
                return (player != null) ? player.Stream : null;
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
