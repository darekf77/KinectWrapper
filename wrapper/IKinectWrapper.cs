using Kinect_Wrapper.device;
using Kinect_Wrapper.device.audio.message;
using Kinect_Wrapper.device.stream;
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
        Boolean DeviceNotAvailable { get; }
        /// <summary>
        /// Observable list of devices
        /// </summary>
        ObservableCollection<IDevice> Devices { get; }

        /// <summary>
        /// Current active device
        /// </summary>
        IDevice Device { get; set; }


        IDevice SelectedDevice { get; set; }

        /// <summary>
        /// Current used stream
        /// </summary>
        StreamBase Stream { get; set; }

        /// <summary>
        /// All available streams 
        /// </summary>
        ObservableCollection<StreamBase> Streams { get; set; }

        Boolean UIEnable { get; set; }

        /// <summary>
        /// Whe one device is down pick up next
        /// </summary>
        Boolean AutoPickUpNewDevice { get; set; }       
                
        /// <summary>
        /// Frame from device or frame generate in ball detecor or jugglind detecror
        /// </summary>
        /// <param name="out_frame"></param>
        void commit(IKinectFrame frame);

        void commit(Bitmap frame);


        ObservableCollection<InfoRow> Info { get; }

        /// <summary>
        /// Bytes from device ready
        /// </summary>
        event EventHandler<IKinectFrame> FrameReady;

        /// <summary>
        /// Display image ready 
        /// </summary>
        event EventHandler<ImageSource> DisplayImageReady;

        event EventHandler<StreamBase> StreamChanged;

        event EventHandler<IAudioMessage> NewUserMessageReady;

        List<String> Grammar { get; set; }

    }
}
