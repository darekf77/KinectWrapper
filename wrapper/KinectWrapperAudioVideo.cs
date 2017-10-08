using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
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
        public Audio Audio { get; private set; }
        public Video Video { get; private set; }

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
                if (App.Current != null) App.Current.Dispatcher.BeginInvoke(new Action(() =>
                   {
                       _infoLeftHand.Value = e.UserSkeleton[SkeletonDataType.HandLeft].ToString();
                       _infoRightHand.Value = e.UserSkeleton[SkeletonDataType.HandRight].ToString();
                       _infoSpine.Value = e.UserSkeleton[SkeletonDataType.Spine].ToString();
                       _infoIsSkeletonDetected.Value = e.IsSkeletonDetected.ToString();
                   }));
            }
        }

        void Video_RecordComplete(object sender, String path)
        {
            Helpers.SetTimeout(() => // TODO QUICK_FIX
            {
                if (App.Current != null) App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _devices.Add(new Device(Audio, Video, path));
                }));
            }, 2000);

        }

    }
}
