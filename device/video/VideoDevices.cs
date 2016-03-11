using Kinect.Replay.Record;
using Kinect.Replay.Replay;
using Kinect_Wrapper.frame;
using Kinect_Wrapper.structures;
using Microsoft.Kinect;
using SharedLibJG.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinect_Wrapper.device.video
{
    public abstract partial class VideoBase
    {
        private IDevice _device;                
        private IKinectFrame _frame;
        

        public IDevice CurrentDevice
        {
            set
            {
                if(value == null)
                {
                    if (_device != null)
                    {
                        if (_device.Type == DeviceType.KINECT_1)
                        {
                            _device.sensor.Stop();
                        }
                        else
                        if (_device.Type == DeviceType.RECORD_FILE_KINECT_1)
                        {
                            _device.replay.AllFramesReady -= ReplayAllFramesReady;
                            _device.replay.ReplayFinished -= _replay_ReplayFinished;
                            _device.replay.Stop();
                        }
                    }
                    _device = null;
                    StreamingFilePath = "";
                    IsRecordingPossible = false;
                    return;
                }

                IsStreaming = false;
                IsRecording = false;
                IsPaused = false;

                _device = value;
                StreamingFilePath = _device.Path;
                if (_device.Type == DeviceType.KINECT_1)
                {
                    _frame = new KinectFrame(_device.sensor);
                    IsRecordingPossible = true;
                }
                else
                if (_device.Type == DeviceType.RECORD_FILE_KINECT_1)
                {
                    _frame = new KinectFrame(_device.replay);
                    _device.replay.AllFramesReady += ReplayAllFramesReady;
                    _device.replay.ReplayFinished += _replay_ReplayFinished;
                    _device.replay.Start();
                }
                else
                if (_device.Type == DeviceType.NO_DEVICE)
                {
                    String exePath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
                    string dir = Path.GetDirectoryName(exePath);
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    bi.UriSource = new Uri(dir + @"\..\..\assets\img\nosignalw640.png", UriKind.Relative);
                    bi.EndInit();
                    ///TODO scale bitmap
                    //var bitmapScale = new TransformedBitmap(bi,
                    //     new ScaleTransform(640 / bi.Width,
                    //                        480 / bi.Height));
                    _frame = new KinectFrame(HelpersConverters.BitmapImage2Bitmap(bi));
                }                

            }
            get { return _device;  }
        }
        
    }
}
