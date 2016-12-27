using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Controls;

namespace Kinect_Wrapper.wrapper
{
    public partial class KinectWrapper
    {
        public AudioBase Audio { get; private set; }
        public VideoBase Video { get; private set; }

        private void initAudioVideo()
        {
            Audio = new Audio();
            Video = new Video();
            Video.FrameReady += Video_FramesReady;
            Video.RecordComplete += Video_RecordComplete;
        }
        

        void Video_FramesReady(object sender, IKinectFrame e)
        {
            if (UIEnable && e.IsSkeletonDetected)
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _infoLeftHand.Value = e.UserSkeleton[SkeletonDataType.LEFT_HAND].ToString();
                    _infoRightHand.Value = e.UserSkeleton[SkeletonDataType.RIGHT_HAND].ToString();
                    _infoSpine.Value = e.UserSkeleton[SkeletonDataType.SPINE].ToString();
                    _infoIsSkeletonDetected.Value = e.IsSkeletonDetected.ToString();
                }));
            }
        }

        void Video_RecordComplete(object sender, String path)
        {
            _devices.Add(new Device(Audio, Video, path));
        }
        
    }
}
