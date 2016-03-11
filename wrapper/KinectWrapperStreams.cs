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
        private StreamBase _stream;
        private ObservableCollection<StreamBase> _streams = new ObservableCollection<StreamBase>();

        private void initStreams()
        {
            StreamBase color = new ColorStream();
            _streams.Add(color);
            Stream = color;
            StreamBase depth = new DepthStream();
            _streams.Add(depth);
        }


        public StreamBase Stream
        {
            get
            {
                return _stream;
            }
            set
            {
                var different = (_stream!=null && !_stream.Equals(value));
                _stream = value;
                OnPropertyChanged("Stream");
                if ( different && StreamChanged != null && value != null)
                {
                    StreamChanged(this, value);
                }
            }
        }
               

        public ObservableCollection<StreamBase> Streams
        {
            get
            {
                return _streams;
            }
            set
            {
                _streams = value;
            }
        }

        
        public void commit(IKinectFrame out_frame)
        {
            if (DisplayImageReady != null)
            {
                _statistic.commitFrame(FrameType.NORMAL);
                DisplayImageReady(this, Stream.update(out_frame));
                _infoFramesPerSecond.Value = _statistic.FramesPerSecond(FrameType.NORMAL).ToString() + "/s";
            }
        }
        public void commit(System.Drawing.Bitmap out_frame)
        {
            if (DisplayImageReady != null)
            {
                _statistic.commitFrame(FrameType.NORMAL);
                DisplayImageReady(this, Stream.update(HelpersConverters.ToBitmapSourceSlow(out_frame)));
                _infoFramesPerSecond.Value = _statistic.FramesPerSecond(FrameType.NORMAL).ToString() + "/s";
            }
        }
        



        public event EventHandler<StreamBase> StreamChanged;

    }
}
