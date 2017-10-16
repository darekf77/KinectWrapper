using Kinect_Wrapper.camera;
using Kinect_Wrapper.device;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.devicemanager;
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
        IKinectCamera Camera { get; }

        IDeviceManager Manager { get; }

        IGesturesDetector Gestures { get; }

        StreamBase Stream { get; set; }

        ObservableCollection<StreamBase> Streams { get; set; }

        Boolean UIEnable { get; set; }

        void commit(IKinectFrame frame);

        void commit(Bitmap frame);

        ObservableCollection<InfoRow> Info { get; }

        int fps { get; }


        event EventHandler<ImageSource> DisplayImageReady;

        event EventHandler<StreamBase> StreamChanged;

    }
}
