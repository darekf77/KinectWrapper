using Kinect.Replay.Record;
using Kinect.Replay.Replay;
using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.device
{
    public partial class Device :DeviceBase
    {

        public Device(AudioBase audio, VideoBase video, KinectSensor sensor_xbox360): base(audio,video,sensor_xbox360)
        {
            
        }
        
        public Device(AudioBase audio, VideoBase video, String filePath):base(audio,video,filePath)
        {
            
        }
                
        public Device(AudioBase audio, VideoBase video):base(audio,video)
        {
            
        }
        
    }
}
