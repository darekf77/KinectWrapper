using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio;
using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.device.stream;
using Kinect_Wrapper.device.video;
using Kinect_Wrapper.frame;
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
        IAudio Audio { get; }
        
        IVideo Video { get; }

        Boolean IsStopped { get; }
        
        ObservableCollection<IDevice> Devices { get; }
        
        IDevice Device { get; set; }

        IDevice SelectedDevice { get; set; }
        
        StreamBase Stream { get; set; }
        
        ObservableCollection<StreamBase> Streams { get; set; }

        Boolean UIEnable { get; set; }

        void commit(IKinectFrame frame);

        void commit(Bitmap frame);

        ObservableCollection<InfoRow> Info { get; }        
        
        event EventHandler<ImageSource> DisplayImageReady;

        event EventHandler<StreamBase> StreamChanged;
        
    }
}
