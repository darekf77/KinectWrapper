using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.statistic;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
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
        public ObservableCollection<InfoRow> Info { get; private set; }

        private InfoRow _infoDeviceName = new InfoRow("Device Name", "");
        private InfoRow _infoRightHand = new InfoRow("RightHandPos", "");
        private InfoRow _infoLeftHand = new InfoRow("LeftHandPos", "");
        private InfoRow _infoSpine = new InfoRow("SpinePos", "");
        private InfoRow _infoIsSkeletonDetected = new InfoRow("IsSkeletonDetected", "");
        private InfoRow _infoFramesPerSecond = new InfoRow("FPS", "");
        private InfoRow _infoWastedFrames = new InfoRow("Wasted FPS", "");
        private Statistics _statistic;
        public Boolean UIEnable { get; set; }

        private void initStatistics()
        {
            _statistic = new Statistics();
            Info = new TrulyObservableCollection<InfoRow>();
            Info.Add(_infoDeviceName);
            Info.Add(_infoRightHand);
            Info.Add(_infoLeftHand);
            Info.Add(_infoSpine);
            Info.Add(_infoIsSkeletonDetected);
            Info.Add(_infoFramesPerSecond);
            Info.Add(_infoWastedFrames);
        }

        public int fps
        {
            get
            {
                return (_statistic != null) ? _statistic.FramesPerSecond(FrameType.NORMAL) : -1;
            }
        }


    }
}
