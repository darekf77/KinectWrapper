using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.gestures;
using Kinect_Wrapper.structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.wrapper
{

    public interface IKinectWrapper
    {
        Audio Audio { get; }

        Video Video { get; }

        IGesturesDetector Gestures { get; }

        Boolean IsStopped { get; }

        ObservableCollection<Device> Devices { get; }

        Device Device { get; set; }

        Device SelectedDevice { get; set; }

        StreamBase Stream { get; set; }

        ObservableCollection<StreamBase> Streams { get; set; }

        DeviceType AutopickupDeviceType { get; }

        Boolean UIEnable { get; set; }

        void commit(IKinectFrame frame);

        void commit(Bitmap frame);

        ObservableCollection<InfoRow> Info { get; }

        int fps { get; }


        event EventHandler<ImageSource> DisplayImageReady;

        event EventHandler<StreamBase> StreamChanged;

        event EventHandler DeviceChanged;

        event EventHandler OnAutopickupDeviceChanged;
    }
}
